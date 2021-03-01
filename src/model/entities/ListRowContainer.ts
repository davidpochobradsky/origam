import {computed, observable, action, reaction, IReactionDisposer, flow, comparer} from "mobx";
import { IFilterConfiguration } from "./types/IFilterConfiguration";
import { IOrderByDirection, IOrderingConfiguration } from "./types/IOrderingConfiguration";
import { IRowsContainer } from "./types/IRowsContainer";
import {getDataViewPropertyById} from "model/selectors/DataView/getDataViewPropertyById";
import {getDataView} from "model/selectors/DataView/getDataView";
import { getDataTable } from "model/selectors/DataView/getDataTable";
import _ from "lodash";
import { fixRowIdentifier } from "utils/dataRow";
import {IProperty} from "model/entities/types/IProperty";

export class ListRowContainer implements IRowsContainer {
  private orderingConfiguration: IOrderingConfiguration;
  private filterConfiguration: IFilterConfiguration;
  private reactionDisposer: IReactionDisposer | undefined;
  @observable
  private forcedLastRowId: string | undefined;
  constructor(
    orderingConfiguration: IOrderingConfiguration,
    filterConfiguration: IFilterConfiguration,
    rowIdGetter: (row: any[]) => string,
    parent: any
  ) {
    this.orderingConfiguration = orderingConfiguration;
    this.filterConfiguration = filterConfiguration;
    this.rowIdGetter = rowIdGetter;
    this.parent = parent;
  }

  @observable.shallow allRows: any[][] = [];
  rowIdGetter: (row: any[]) => any;

  @observable
  sortedIds: any[] | undefined;

  @computed get idToRow() {
    const entries = this.allRows.map(row => [this.rowIdGetter(row), row] as [any, any[]]);
    return new Map<any, any[]>(entries);
  }

  async start() {
    this.reactionDisposer = reaction(
      () => [
        this.filterConfiguration.activeFilters.map((filter) => [
          filter.propertyId,
          Array.isArray(filter.setting.val1) ? [...filter.setting.val1] : filter.setting.val1,
          Array.isArray(filter.setting.val2) ? [...filter.setting.val2] : filter.setting.val2,
          filter.setting.type,
        ]),
        this.orderingConfiguration.orderings.map((x) => [x.columnId, x.direction])],
      () => this.updateSortAndFilterDebounced(),
      {
        equals: comparer.structural,
      }
    );
    await this.updateSortAndFilter();
  }

  stop(){
    this.reactionDisposer?.();
  }

  *preloadLookups(){
    const dataView = getDataView(this.orderingConfiguration);
    const dataTable = getDataTable(dataView);

    const orderingComboProps = this.orderingConfiguration.userOrderings
      .map((term) => getDataViewPropertyById(this.orderingConfiguration, term.columnId)!)
      .filter((prop) => prop.column === "ComboBox");

    const filterComboProps = this.filterConfiguration.activeFilters
      .map((term) => getDataViewPropertyById(this.filterConfiguration, term.propertyId)!)
      .filter((prop) => prop.column === "ComboBox");
    const allComboProps = Array.from(new Set(filterComboProps.concat(orderingComboProps)));

    yield Promise.all(
      allComboProps.map(async (prop) => {
        return prop.lookupEngine?.lookupResolver.resolveList(
          dataTable.getAllValuesOfProp(prop)
        );
      })
    );
  }

  updateSortAndFilterDebounced = _.debounce(this.updateSortAndFilter, 10);

  @action
  async updateSortAndFilter(data?: {retainPreviousSelection?: boolean}) {
    const self = this;
    await flow(
      function* () {
        yield * self.preloadLookups();
        let rows = self.allRows;
        if (self.filterConfiguration.filteringFunction()) {
          rows = rows.filter((row) => self.filterConfiguration.filteringFunction()(row));
        }
        if (self.orderingConfiguration.orderings.length !== 0) {
          rows = rows.sort((row1: any[], row2: any[]) => self.internalRowOrderingFunc(row1, row2));
        }
        self.sortedIds = rows.map(row => self.rowIdGetter(row));
        const dataView = getDataView(self);
        if(!data?.retainPreviousSelection){
          dataView.reselectOrSelectFirst();
        }
      }
    )();
  }

  getFilteredRows(args:{propertyFilterIdToExclude: string}){
    return this.allRows
      .filter((row) => this.filterConfiguration.filteringFunction(args.propertyFilterIdToExclude)(row));
  }

  @computed get rows() {
    if(!this.sortedIds){
      return this.allRows;
    }
    return this.sortedIds
      .map(id => this.idToRow.get(id))
      .filter(row => row) as any[][];
  }

  internalRowOrderingFunc(row1: any[], row2: any[]) {
    if(this.forcedLastRowId !== undefined){
      const orderings=this.orderingConfiguration.orderings;
      const directionMultiplier = orderings.length === 1 && orderings[0].direction === IOrderByDirection.DESC
        ? -1 
        : 1;
      if (this.forcedLastRowId === this.rowIdGetter(row1)) return 1 * directionMultiplier;
      if (this.forcedLastRowId === this.rowIdGetter(row2)) return -1 * directionMultiplier;
    }
    return this.orderingConfiguration.orderingFunction()(row1, row2);
  }

  unlockAddedRowPosition(): void {
    this.forcedLastRowId = undefined;
  }

  clear(): void {
    this.allRows.length = 0;
  }

  delete(row: any[]): void {
    const rowId = this.rowIdGetter(row);
    const idx = this.allRows.findIndex((r) => this.rowIdGetter(r) === rowId);
    if (idx > -1) {
      this.allRows.splice(idx, 1);
      if(rowId === this.forcedLastRowId){
        this.forcedLastRowId = undefined;
      }
      this.updateSortAndFilter( {retainPreviousSelection: true});
    }
  }

  async insert(index: number, row: any[], shouldLockNewRowPosition?: boolean): Promise<any> {
    const dataTable = getDataTable(this);
    row = fixRowIdentifier(row, dataTable.identifierDataIndex);
    const newRowId = dataTable.getRowId(row);
    const rowExistsAlready = this.allRows.some(row => dataTable.getRowId(row) === newRowId)
    if(rowExistsAlready){
      return;
    }
    this.allRows.splice(index, 0, row);
    if(shouldLockNewRowPosition){
      this.forcedLastRowId = this.rowIdGetter(row);
    }
    await this.updateSortAndFilter();
  }

  async set(rowsIn: any[][]): Promise<any>{
    const dataTable = getDataTable(this);
    const rows: any[][] = [];
    for (let row of rowsIn) {
      rows.push(fixRowIdentifier(row, dataTable.identifierDataIndex));
    }
    this.clear();
    for(let row of rows) this.allRows.push(row);
    await this.updateSortAndFilter();
  }

  appendRecords(rowsIn: any[][]){
    throw new Error("Not implemented");
  }

  substitute(row: any[]): void {
    const dataTable = getDataTable(this);
    row = fixRowIdentifier(row, dataTable.identifierDataIndex);
    const idx = this.allRows.findIndex((r) => this.rowIdGetter(r) === this.rowIdGetter(row));
    if (idx > -1) {
      this.allRows.splice(idx, 1, row);
    }
  }

  registerResetListener(listener: () => void): void {}

  parent: any;

  get addedRowPositionLocked(): boolean {
    return this.forcedLastRowId !== undefined;
  }

  getFirstRow(): any[] | undefined {
    if (this.rows.length === 0) {
      return undefined;
    }
    return this.rows[0];
  }
}

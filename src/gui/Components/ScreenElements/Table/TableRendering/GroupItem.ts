import {computed, observable} from "mobx";
import {IGroupRow, IGroupTreeNode} from "./types";
import {IGrouper} from "../../../../../model/entities/types/IGrouper";
import {IAggregation} from "../../../../../model/entities/types/IAggregation";
import {getOrderingConfiguration} from "../../../../../model/selectors/DataView/getOrderingConfiguration";
import {InfiniteScrollLoader, SCROLL_ROW_CHUNK} from "../../../../Workbench/ScreenArea/TableView/InfiniteScrollLoader";
import {getDataView} from "../../../../../model/selectors/DataView/getDataView";
import {joinWithAND, toFilterItem} from "../../../../../model/entities/OrigamApiHelpers";
import {OpenGroupVisibleRowsMonitor} from "../../../../Workbench/ScreenArea/TableView/VisibleRowsMonitor";
import {getDataTable} from "../../../../../model/selectors/DataView/getDataTable";
import {ScrollRowContainer} from "../../../../../model/entities/ScrollRowContainer";

export interface IGroupItemData{
  childGroups: IGroupTreeNode[];
  childRows: any[][];
  columnId: string;
  columnValue: string ;
  columnDisplayValue: string ;
  groupLabel: string;
  parent: IGroupTreeNode | undefined;
  rowCount: number;
  aggregations: IAggregation[] | undefined;
  grouper: IGrouper;
}

export interface IClientSideGroupItemData extends IGroupItemData {
  expansionListener: (item: ClientSideGroupItem) => void;
}

export class ClientSideGroupItem implements IClientSideGroupItemData, IGroupTreeNode {
  constructor(data: IClientSideGroupItemData) {
    Object.assign(this, data);
  }
  isInfinitelyScrolled = false;
  expansionListener: (item: ClientSideGroupItem) => void = null as any;
  @observable childGroups: IGroupTreeNode[] = null as any;
  @observable _childRows: any[][] = null as any;
  columnId: string = null as any;
  columnValue: string = null as any;
  groupLabel: string = null as any;
  parent: IGroupTreeNode | undefined = null as any;
  rowCount: number = null as any;
  columnDisplayValue: string = null as any;
  aggregations: IAggregation[] | undefined = undefined;
  grouper: IGrouper = null as any;

  get level(){
    return this.allParents.length;
  }

  @observable
  private _isExpanded = false;

  public get isExpanded() {
    return this._isExpanded;
  }
  public set isExpanded(value) {
    this._isExpanded = value;
    this.expansionListener(this);
  }

  get allChildGroups(): IGroupTreeNode[]{
    return allChildGroups(this);
  }
  
  get allParents(): IGroupTreeNode[] {
    return getAllParents(this);
  }

  substituteRecord(row: any[]): void{}
  
  getRowIndex(rowId: string): number | undefined {
    return this._childRows.findIndex(row => getDataTable(this.grouper).getRowId(row) === rowId);
  }

  getRowById(id: string): any[] | undefined{
    return this._childRows.find(row => getDataTable(this.grouper).getRowId(row) === id);
  }

  @computed get childRows(){
    const orderingConfiguration = getOrderingConfiguration(this.grouper);
    
    if(orderingConfiguration.userOrderings.length === 0){
      return this._childRows;
    }else{
      return this._childRows.slice().sort(orderingConfiguration.orderingFunction());
    }
  }
  set childRows(rows: any[][]){
    this._childRows = rows;
  }

  composeGroupingFilter(): string {
    throw new Error("Method not implemented.");
  }
}

export class ServerSideGroupItem implements IGroupTreeNode {
  constructor(data: IGroupItemData) {
    const dataTable = getDataTable(data.grouper);
    this._childRows = new ScrollRowContainer(
      (row: any[]) => dataTable.getRowId(row),
      dataTable);
    Object.assign(this, data);
    
    const dataView = getDataView(this.grouper);
    this.scrollLoader = new InfiniteScrollLoader({
      ctx: this.grouper,
      gridDimensions: dataView.gridDimensions,
      scrollState: dataView.scrollState,
      rowsContainer: this._childRows,
      groupFilter: this.composeGroupingFilter(),
      visibleRowsMonitor: new OpenGroupVisibleRowsMonitor(this.grouper, dataView.gridDimensions, dataView.scrollState)
    })
    this.scrollLoader.registerAppendListener(data => dataTable.appendRecords(data))
    this.scrollLoader.registerPrependListener(data => dataTable.appendRecords(data))
  }
  @observable childGroups: IGroupTreeNode[] = null as any;
  columnId: string = null as any;
  columnValue: string = null as any;
  groupLabel: string = null as any;
  parent: IGroupTreeNode | undefined = null as any;
  rowCount: number = null as any;
  columnDisplayValue: string = null as any;
  aggregations: IAggregation[] | undefined = undefined;
  grouper: IGrouper = null as any;
  scrollLoader: InfiniteScrollLoader;
  _childRows: ScrollRowContainer;
  

  get level(){
    return this.allParents.length;
  }
  
  get isInfinitelyScrolled(){
    return this.rowCount >= SCROLL_ROW_CHUNK && this.isExpanded && this.childRows.length > 0
  }
  
  get allChildGroups(): IGroupTreeNode[]{
    return allChildGroups(this);
  }
  
  get allParents(): IGroupTreeNode[] {
    return getAllParents(this);
  }

  substituteRecord(row: any[]): any{
    this._childRows.substitute(row);
  }

  getRowIndex(rowId: string): number | undefined {
    return this.childRows.findIndex(row => getDataTable(this.grouper).getRowId(row) === rowId);
  }

  getRowById(id: string): any[] | undefined{
    return this.childRows.find(row => getDataTable(this.grouper).getRowId(row) === id);
  }

  @computed get childRows(){
    return this._childRows.rows;
  }
  set childRows(rows: any[][]){
    if(rows.length > 0){
      this.scrollLoader.start();
      getDataTable(this.grouper).appendRecords(rows);
    }
    this._childRows.set(rows);
  }

  composeGroupingFilter(): string {
    const parents = getAllParents(this);
    if(parents.length === 0){
      return toFilterItem(this.columnId, null, "eq" ,this.columnValue)
    }else{
      const andOperands = parents
        .concat([this])
        .map(row => toFilterItem(row.columnId, null, "eq", row.columnValue))
      return joinWithAND(andOperands);
    }
  }
  
  @observable private _isExpanded = false;

  get isExpanded(): boolean {
    return this._isExpanded;
  }

  set isExpanded(value: boolean) {
    if(!value){
      this.grouper.notifyGroupClosed(this);
    }
    this._isExpanded = value;
  }
}

function getAllParents(group: IGroupTreeNode){
  const parents: IGroupTreeNode[] = [];
  let parent = group.parent;
  while(parent){
    parents.push(parent);
    parent = parent.parent;
  }
  return parents;
}

function allChildGroups(group: IGroupTreeNode): IGroupTreeNode[]{
  const allChildGroups = group.childGroups.flatMap(childGroup => childGroup.allChildGroups)
  return [...group.childGroups, ... allChildGroups];
}

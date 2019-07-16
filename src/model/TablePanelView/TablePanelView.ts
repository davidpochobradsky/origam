import { observable, computed, action } from "mobx";
import { ITablePanelView, ITablePanelViewData } from "./types/ITablePanelView";
import { getDataView } from "../selectors/DataView/getDataView";
import { getDataTable } from "../selectors/DataView/getDataTable";
import { IProperty } from "../types/IProperty";
import { getDataViewPropertyById } from "../selectors/DataView/getDataViewPropertyById";
import { getSelectedRow } from "../selectors/DataView/getSelectedRow";
import { getDataViewLifecycle } from "../selectors/DataView/getDataViewLifecycle";

export class TablePanelView implements ITablePanelView {

  $type_ITablePanelView: 1 = 1;

  constructor(data: ITablePanelViewData) {
    Object.assign(this, data);
  }

  @observable isColumnConfigurationDialogVisible = false;

  @observable isEditing: boolean = false;

  @observable tablePropertyIds: string[] = [];

  @observable columnOrderChangingTargetId: string | undefined;
  @observable columnOrderChangingSourceId: string | undefined;

  @computed get tableProperties() {
    return this.tablePropertyIds.map(id =>
      getDataTable(this).getPropertyById(id)
    ) as IProperty[];
  }
  @observable selectedColumnId: string | undefined;
  @computed get selectedRowId(): string | undefined {
    return getDataView(this).selectedRowId;
  }

  @computed get selectedColumnIndex(): number | undefined {
    const idx = this.tableProperties.findIndex(
      prop => prop.id === this.selectedColumnId
    );
    return idx > -1 ? idx : undefined;
  }
  @computed get selectedRowIndex(): number | undefined {
    return getDataView(this).selectedRowIndex;
  }
  @computed get selectedProperty(): IProperty | undefined {
    return this.selectedColumnId
      ? getDataViewPropertyById(this, this.selectedColumnId)
      : undefined;
  }

  getCellValueByIdx(rowIdx: number, columnIdx: number) {
    const property = this.tableProperties[columnIdx]!;
    const row = this.dataTable.getRowByExistingIdx(rowIdx);
    return this.dataTable.getCellValue(row, property);
  }

  @action.bound
  onCellClick(rowIndex: number, columnIndex: number): void {
    // console.log("CellClicked:", rowIndex, columnIndex);
    const row = this.dataTable.getRowByExistingIdx(rowIndex);
    const property = this.tableProperties[columnIndex];
    if (
      this.dataTable.getRowId(row) === this.selectedRowId &&
      property.id === this.selectedColumnId
    ) {
      this.setEditing(true);
    } else {
      const { isEditing } = this;
      if (isEditing) {
        this.editingWillFinish();
        this.setEditing(false);
      }
      this.selectCell(row[0] as string, property.id);
      if (isEditing) {
        this.setEditing(true);
      }
    }
  }

  @action.bound
  onNoCellClick(): void {
    if (this.isEditing) {
      this.editingWillFinish();
      this.setEditing(false);
    }
  }

  @action.bound
  onOutsideTableClick(): void {
    if (this.isEditing) {
      this.editingWillFinish();
      this.setEditing(false);
    }
  }

  @action.bound
  onColumnConfClick(event: any): void {
    this.isColumnConfigurationDialogVisible = true;
  }

  @action.bound editingWillFinish() {
    this.dataTable.flushFormToTable(getSelectedRow(this)!);
    getDataViewLifecycle(this).requestFlushData(
      getSelectedRow(this)!,
      this.selectedProperty!
    );
  }

  @action.bound selectCell(
    rowId: string | undefined,
    columnId: string | undefined
  ) {
    this.selectedColumnId = columnId;
    getDataView(this).setSelectedRowId(rowId);
  }

  @action.bound
  setSelectedColumnId(id: string | undefined): void {
    this.selectedColumnId = id;
  }

  @action.bound
  setEditing(state: boolean): void {
    this.isEditing = state;
  }

  @action.bound
  swapColumns(id1: string, id2: string): void {
    const idx1 = this.tablePropertyIds.findIndex(id => id === id1);
    const idx2 = this.tablePropertyIds.findIndex(id => id === id2);
    const tmp = this.tablePropertyIds[idx1];
    this.tablePropertyIds[idx1] = this.tablePropertyIds[idx2];
    this.tablePropertyIds[idx2] = tmp;
  }

  @action.bound
  setColumnOrderChangeAttendants(
    idSource: string | undefined,
    idTarget: string | undefined
  ): void {
    console.log(idSource, idTarget)
    this.columnOrderChangingTargetId = idTarget;
    this.columnOrderChangingSourceId = idSource;
  }

  @computed get dataTable() {
    return getDataTable(this);
  }

  parent?: any;
}

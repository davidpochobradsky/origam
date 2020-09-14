import { computed } from "mobx";
import { getSelectedRow } from "model/selectors/DataView/getSelectedRow";
import { getRowStateIsDisableAction } from "model/selectors/RowState/getRowStateIsDisabledAction";
import { getSelectedRowId } from "model/selectors/TablePanelView/getSelectedRowId";
import {
  IAction,
  IActionData,
  IActionMode,
  IActionPlacement,
  IActionType,
} from "./types/IAction";
import { IActionParameter } from "./types/IActionParameter";
import {getIsAnySelected} from "model/selectors-tree/selectionCheckboxes";
import { getDataTable } from "model/selectors/DataView/getDataTable";

export class Action implements IAction {
  $type_IAction: 1 = 1;

  constructor(data: IActionData) {
    Object.assign(this, data);
    this.parameters.forEach((o) => (o.parent = this));
  }

  type: IActionType = null as any;
  id: string = "";
  groupId: string = "";
  caption: string = "";
  placement: IActionPlacement = null as any;
  iconUrl: string = "";
  mode: IActionMode = null as any;
  isDefault: boolean = false;
  parameters: IActionParameter[] = [];

  @computed get isEnabled() {
    if (this.mode === IActionMode.Always) {
      return true;
    }
    const dataTable = getDataTable(this);
    if(dataTable.loadedRowsCount === 0) {
      return false
    }
    const selRowId = getSelectedRowId(this);
    const isDisabledOverride = selRowId
      ? getRowStateIsDisableAction(this, selRowId, this.id)
      : false;
    if (isDisabledOverride) {
      return false;
    }
    switch (this.mode) {
      case IActionMode.ActiveRecord: {
        const selectedRow = getSelectedRow(this);
        return !!selectedRow;
      }
      case IActionMode.MultipleCheckboxes: {
        return getIsAnySelected(this);
      }
    }
  }

  parent?: any;
}

import {getDataTable} from "../DataView/getDataTable";
import {getTablePanelView} from "./getTablePanelView";
import {IProperty} from "../../entities/types/IProperty";

export function getCellValueByIdx(ctx: any) {
  const tablePanelView = getTablePanelView(ctx);
  return function getCellValueByIdx(rowIndex: number, columnIndex: number) {
    return tablePanelView.getCellValueByIdx(rowIndex, columnIndex);
  };
}

export function getCellValue(ctx: any, row: any[], property: IProperty) {
  return getDataTable(ctx).getCellValue(row, property);
}
import { selectPrevRow } from "model/actions/DataView/selectPrevRow";
import { selectNextRow } from "model/actions/DataView/selectNextRow";
import { selectPrevColumn } from "model/actions/DataView/TableView/selectPrevColumn";
import { selectNextColumn } from "model/actions/DataView/TableView/selectNextColumn";
import { getTablePanelView } from "model/selectors/TablePanelView/getTablePanelView";
import { flow } from "mobx";
import { handleError } from "model/actions/handleError";
import { getDataView } from "model/selectors/DataView/getDataView";
import { shouldProceedToChangeRow } from "./shouldProceedToChangeRow";
import uiActions from "../../../actions-ui-tree";

export function onTableKeyDown(ctx: any) {
  return flow(function* onTableKeyDown(event: any) {
    try {
      const dataView = getDataView(ctx);
      switch (event.key) {
        case "ArrowUp":
          event.preventDefault();
          if (!(yield shouldProceedToChangeRow(dataView))) {
            break;
          }
          yield* selectPrevRow(ctx)();
          getTablePanelView(ctx).scrollToCurrentCell();
          break;
        case "ArrowDown":
          event.preventDefault();
          if (!(yield shouldProceedToChangeRow(dataView))) {
            break;
          }
          yield* selectNextRow(ctx)();
          getTablePanelView(ctx).scrollToCurrentCell();
          break;
        case "ArrowLeft":
          selectPrevColumn(ctx)();
          event.preventDefault();
          getTablePanelView(ctx).scrollToCurrentCell();
          break;
        case "ArrowRight":
          selectNextColumn(ctx)();
          event.preventDefault();
          getTablePanelView(ctx).scrollToCurrentCell();
          break;
        case "F2":
          getTablePanelView(ctx).setEditing(true);
          event.preventDefault();
          getTablePanelView(ctx).scrollToCurrentCell();
          break;
        case "Tab":
          if (event.shiftKey) {
            selectPrevColumn(ctx)(true);
          } else {
            selectNextColumn(ctx)(true);
          }
          event.preventDefault();
          getTablePanelView(ctx).scrollToCurrentCell();
          break;
        case "Enter":
          if (dataView.firstEnabledDefaultAction) {
            uiActions.actions.onActionClick(dataView.firstEnabledDefaultAction)(
                event,
                dataView.firstEnabledDefaultAction
            );
          }
          break;
        case "Escape": {
          getTablePanelView(ctx).setEditing(false);
          getTablePanelView(ctx).clearCurrentCellEditData();
          getTablePanelView(ctx).triggerOnFocusTable();
          break;
        }
      }
    } catch (e) {
      yield* handleError(ctx)(e);
      throw e;
    }
  });
}
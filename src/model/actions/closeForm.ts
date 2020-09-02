import {getWorkbenchLifecycle} from "model/selectors/getWorkbenchLifecycle";
import {getOpenedScreen} from "model/selectors/getOpenedScreen";
import {IRefreshOnReturnType} from "../entities/WorkbenchLifecycle/WorkbenchLifecycle";
import {getApi} from "../selectors/getApi";
import {ICRUDResult, processCRUDResult} from "./DataLoading/processCRUDResult";
import {getDontRequestData} from "model/selectors/getDontRequestData";
import {getDataViewLifecycle} from "model/selectors/DataView/getDataViewLifecycle";
import {getFormScreen} from "model/selectors/FormScreen/getFormScreen";

export function closeForm(ctx: any) {
  return function* closeForm(): Generator {
    const lifecycle = getWorkbenchLifecycle(ctx);
    const openedScreen = getOpenedScreen(ctx);
    const parentScreen = getOpenedScreen(openedScreen.parentContext);

    yield* lifecycle.closeForm(openedScreen);
    if(openedScreen.content){
      const refreshOnReturnType = openedScreen.content.refreshOnReturnType;
      switch(refreshOnReturnType){
        case IRefreshOnReturnType.ReloadActualRecord:
          if (getDontRequestData(ctx)) {
            break;
          }
          const formScreen = getFormScreen(openedScreen.parentContext);
          for (let dataView of formScreen.dataViews) {
            const dataViewLifecycle = getDataViewLifecycle(dataView);
            yield dataViewLifecycle.runRecordChangedReaction();
          }
          break;
        case IRefreshOnReturnType.RefreshCompleteForm:
          break;
        case IRefreshOnReturnType.MergeModalDialogChanges:
          const api = getApi(ctx);
          const parentScreenSessionId = parentScreen!.content!.formScreen!.sessionId;
          const changes = (yield api.pendingChanges({ sessionFormIdentifier: parentScreenSessionId})) as ICRUDResult[];
          for (let change of changes) {
            yield* processCRUDResult(openedScreen.parentContext, change);
          }
          break;
      }
    }
  };
}

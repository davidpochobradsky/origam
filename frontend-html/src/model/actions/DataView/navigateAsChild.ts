import {getDataViewLifecycle} from "model/selectors/DataView/getDataViewLifecycle"

export function navigateAsChild(ctx: any, rows?: any[]) {
  return function* navigateAsChild() {
    yield* getDataViewLifecycle(ctx).navigateAsChild(rows);
  }
}
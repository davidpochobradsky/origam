import _ from "lodash";
import {action, computed, createAtom, flow, IAtom, observable} from "mobx";
import {handleError} from "model/actions/handleError";
import {getEntity} from "model/selectors/DataView/getEntity";
import {getApi} from "model/selectors/getApi";
import {getSessionId} from "model/selectors/getSessionId";
import {flashColor2htmlColor} from "utils/flashColorFormat";
import {IRowState, IRowStateColumnItem, IRowStateData, IRowStateItem} from "./types/IRowState";
import {FlowBusyMonitor} from "../../utils/flow";

export enum IIdState {
  LOADING = "LOADING",
  ERROR = "ERROR"
}

export class RowState implements IRowState {
  $type_IRowState: 1 = 1;
  suppressWorkingStatus: boolean = false;

  constructor(data: IRowStateData) {
    Object.assign(this, data);
  }

  monitor: FlowBusyMonitor =  new FlowBusyMonitor();

  get isWorking() {
    return this.monitor.isWorkingDelayed;
  }

  @observable firstLoadingPerformed = false;
  @computed get mayCauseFlicker() {
    return  !this.firstLoadingPerformed;
  }
  containers: Map<string, RowStateContainer> = new Map<string, RowStateContainer>();

  @observable
  isSomethingLoading = false;

  triggerLoadImm = flow(
    function*(this: RowState) {
      if (this.isSomethingLoading) {
        return;
      }
      let containersToLoad: Map<string, RowStateContainer> = new Map();
      let reportBusyStatus = true;
      while (true) {
        try {
          for (let container of this.containers.values()) {
            if(container.rowId && !container.isValid && !container.processingSate){
              containersToLoad.set(container.rowId, container);
            }
          }
          reportBusyStatus = Array.from(containersToLoad.values()).every(container => !container.suppressWorkingStatus);
          if(reportBusyStatus){
            this.monitor.inFlow++;
          }
          if (containersToLoad.size === 0) {
            break;
          }
          this.isSomethingLoading = true;
          const api = getApi(this);
          const states = yield api.getRowStates({
            SessionFormIdentifier: getSessionId(this),
            Entity: getEntity(this),
            Ids: Array.from(containersToLoad.values()).map(container => container.rowId)
          });
          this.isSomethingLoading = false;
          this.firstLoadingPerformed = true;
          for (let state of states) {
            this.putValue(state);
            this.containers.get(state.id)!.processingSate = IIdState.LOADING;
            containersToLoad.delete(state.id);
          }
        } catch (error) {
          this.isSomethingLoading = false;
          this.firstLoadingPerformed = true;
          console.error(error);
          for (let container of containersToLoad.values()) {
            container.processingSate = IIdState.LOADING;
          }
          yield* handleError(this)(error);
        } finally {
          if(reportBusyStatus){
            this.monitor.inFlow--;
          }
          containersToLoad.forEach(container => container.suppressWorkingStatus = false);
        }
      }
    }.bind(this)
  );

  triggerLoad = _.debounce(this.triggerLoadImm, 666);

  getValue(rowId: string) {
    if(!this.containers.has(rowId)){
      this.containers.set(rowId, new RowStateContainer(rowId,) );
    }
    let container = this.containers.get(rowId)!;
    if(!container.atom){
      container.suppressWorkingStatus = this.suppressWorkingStatus;
      container.atom = createAtom(
          `RowState atom [${rowId}]`,
          () =>
              requestAnimationFrame(() => {
                this.triggerLoad();
              }),
          () => {}
      )
    }
    container.atom.reportObserved?.();
    return this.containers.get(rowId)?.rowStateItem;
  }

  async loadValues(rowIds: string[]) {
    for (const rowId of rowIds) {
      if (!this.containers.has(rowId)) {
        this.containers.set(rowId, new RowStateContainer(rowId));
      }
    }
    await this.triggerLoadImm();
  }

  hasValue(rowId: string): boolean {
    return this.containers.has(rowId);
  }

  @action.bound
  putValue(state: any) {
    let rowStateItem = new RowStateItem(
        state.id,
        state.allowCreate,
        state.allowDelete,
        flashColor2htmlColor(state.foregroundColor),
        flashColor2htmlColor(state.backgroundColor),
        new Map(
            state.columns.map((column: any) => {
              const rs = new RowStateColumnItem(
                  column.name,
                  column.dynamicLabel,
                  flashColor2htmlColor(column.foregroundColor),
                  flashColor2htmlColor(column.backgroundColor),
                  column.allowRead,
                  column.allowUpdate
              );
              return [column.name, rs];
            })
        ),
        new Set(state.disabledActions),
        state.relations
    );
    this.containers.get(state.id)!.rowStateItem = rowStateItem;
    this.containers.get(state.id)!.isValid = true;
    this.firstLoadingPerformed = true;
  }

  @action.bound clearAll() {

    for (let rowStateContainer of this.containers.values()) {
      rowStateContainer.atom?.onBecomeUnobservedListeners?.clear();
      rowStateContainer.atom?.onBecomeObservedListeners?.clear();
      rowStateContainer.atom = undefined;
      rowStateContainer.isValid = false;
      rowStateContainer.processingSate = undefined;
    }
    // TODO: Wait when something is currently loading.
  }

  parent?: any;
}

export class RowStateItem implements IRowStateItem {
  constructor(
    public id: string,
    public allowCreate: boolean,
    public allowDelete: boolean,
    public foregroundColor: string | undefined,
    public backgroundColor: string | undefined,
    public columns: Map<string, IRowStateColumnItem>,
    public disabledActions: Set<string>,
    public relations: any[]
  ) {}
}

export class RowStateColumnItem implements IRowStateColumnItem {
  constructor(
    public name: string,
    public dynamicLabel: string | undefined | null,
    public foregroundColor: string | undefined,
    public backgroundColor: string | undefined,
    public allowRead: boolean,
    public allowUpdate: boolean
  ) {}
}


class RowStateContainer {
  public rowId: string;

  @observable
  public rowStateItem: IRowStateItem | undefined;

  @observable
  public isValid: boolean = false;

  public processingSate: IIdState | undefined;
  public suppressWorkingStatus: boolean = false;
  constructor(
      rowId: string,
      public atom? : IAtom
  ) {
    this.rowId = rowId;
  }
}


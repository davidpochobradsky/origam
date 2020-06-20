import {
  IOpenedScreen,
  IOpenedScreenData,
  IDialogInfo
} from "./types/IOpenedScreen";
import { observable, action, computed } from "mobx";
import { IFormScreen, IFormScreenEnvelope } from "./types/IFormScreen";
import { IMainMenuItemType } from "./types/IMainMenu";

export class DialogInfo implements IDialogInfo {
  constructor(public width: number, public height: number) {}
}

export class OpenedScreen implements IOpenedScreen {
  dialogInfo?: IDialogInfo | undefined;
  $type_IOpenedScreen: 1 = 1;

  constructor(data: IOpenedScreenData) {
    Object.assign(this, data);
    this.content.parent = this;
  }

  @observable stackPosition: number = 0;
  @observable isActive = false;
  dontRequestData: boolean = false;
  menuItemId: string = "";
  menuItemType: IMainMenuItemType = null as any;
  order: number = 0;
  _title: string = "";
  @observable isSleeping?: boolean = false;
  @observable isSleepingDirty?: boolean = false;
  @observable content: IFormScreenEnvelope = null as any;
  parameters: { [key: string]: any } = {};

  get title(){
      return this.content.formScreen?.dynamicTitle ?? this._title;
  }

  set title(value: string){
    this._title = value;
  }

  @computed get isDialog() {
    return this.dialogInfo !== undefined;
  }

  @action.bound
  setActive(state: boolean): void {
    this.isActive = state;
  }

  @action.bound
  setContent(screen: IFormScreenEnvelope): void {
    this.content = screen;
    screen.parent = this;
  }

  parent?: any;
}

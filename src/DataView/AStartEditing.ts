import { action } from "mobx";
import { L, ML } from "../utils/types";
import { IAStartEditing } from "./types/IAStartEditing";
import { IEditing } from "./types/IEditing";
import { IAInitForm } from "./types/IAInitForm";
import { IDataViewMediator } from "./types/IDataViewMediator";
import { unpack } from "../utils/objects";
import * as DataViewAction from "./DataViewActions";
import { isType } from "ts-action";

export class AStartEditing implements IAStartEditing {
  constructor(
    public P: {
      editing: IEditing;
      aInitForm: IAInitForm;
      listen(cb: (action: any) => void): void;
    }
  ) {
    this.subscribeMediator();
  }

  subscribeMediator() {
    this.P.listen((action: any) => {
      if (isType(action, DataViewAction.startEditing)) {
        this.do();
      }
    });
  }

  @action.bound
  public do() {
    console.log("StartEditing");
    const editing = this.P.editing;
    // --------------------------------------------------------
    editing.setEditing(true);
    this.P.aInitForm.do();
  }
}

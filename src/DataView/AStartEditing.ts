import { action } from "mobx";
import * as DataViewAction from "./DataViewActions";
import { IAInitForm } from "./types/IAInitForm";
import { IAStartEditing } from "./types/IAStartEditing";
import { IEditing } from "./types/IEditing";

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
      switch(action.type) {
        case DataViewAction.START_EDITING:
          this.do();
          break;
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

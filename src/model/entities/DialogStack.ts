/*
Copyright 2005 - 2021 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/

import { action, computed, observable } from "mobx";
import { IDialogStack } from "./types/IDialogStack";
import { IDialogInfo } from "./types/IDialogInfo";
import { IDialogDimensions } from "../../gui/Components/Dialog/types";

let nextId = 0;

export class DialogStack implements IDialogStack {
  parent?: any;
  @observable.shallow stackedDialogs: Array<IDialogInfo> = [];

  @computed get isAnyDialogShown() {
    return this.stackedDialogs.length > 0;
  }

  @action.bound pushDialog(
    key: string,
    component: React.ReactElement,
    dialogDimensions?: IDialogDimensions,
    closeOnClickOutside?: boolean) {
    const useKey = key ? key : `DEFAULT_DIALOG_KEY_${nextId++}`;
    this.stackedDialogs.push({
      key: useKey,
      closeOnClickOutside: closeOnClickOutside,
      component: component,
      dimensions: dialogDimensions
    });
    return () => {
      this.closeDialog(useKey);
    };
  }

  @action.bound closeDialog(key: string) {
    const index = this.stackedDialogs.findIndex(dlg => dlg.key === key);
    if (index > -1) {
      this.stackedDialogs.splice(index, 1);
    }
  }

  isOpen(key: string) {
    return this.stackedDialogs.findIndex(info => info.key === key) !== -1;
  }
}

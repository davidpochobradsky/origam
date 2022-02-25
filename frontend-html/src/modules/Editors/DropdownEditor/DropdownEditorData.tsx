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

import { bind } from "@decorize/bind";
import { action, computed } from "mobx";
import { DataViewData } from "modules/DataView/DataViewData";
import { RowCursor } from "modules/DataView/TableCursor";
import { DropdownEditorSetup } from "modules/Editors/DropdownEditor/DropdownEditorSetup";

export interface IDropdownEditorData {
  idsInEditor: string[];
  value: string | string[] | null;
  text: string;
  isResolving: boolean;

  chooseNewValue(value: any): void;

  remove(value: any): void;

  setValue(value: string[]): void;
}

@bind
export class DropdownEditorData implements IDropdownEditorData {
  constructor(
    private dataTable: DataViewData,
    private rowCursor: RowCursor,
    private setup: () => DropdownEditorSetup
  ) {
  }

  setValue(value: string[]) {
  }

  @computed get value(): string | string[] | null {
    if (this.rowCursor.selectedId) {
      return this.dataTable.getCellValue(this.rowCursor.selectedId, this.setup().propertyId);
    } else return null;
  }

  @computed get text(): string {
    if (!this.isResolving && this.value) {
      return this.dataTable.getCellText(this.setup().propertyId, this.value);
    } else return "";
  }

  get isResolving() {
    return this.dataTable.getIsCellTextLoading(this.setup().propertyId, this.value);
  }

  @action.bound chooseNewValue(value: any) {
    if (this.rowCursor.selectedId) {
      this.dataTable.setNewValue(this.rowCursor.selectedId, this.setup().propertyId, value);
    }
  }

  idsInEditor: string[] = [];

  remove(value: any): void {
    // not needed in DropdownEditorData
  }
}

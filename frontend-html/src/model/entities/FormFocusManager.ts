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

import { isGlobalAutoFocusDisabled } from "model/actions-ui/ScreenToolbar/openSearchWindow";
import { compareTabIndexOwners, ITabIndexOwner } from "model/entities/TabIndexOwner";
import { requestFocus } from "utils/focus";

export class FormFocusManager {
  autoFocusDisabled = false;

  stopAutoFocus() {
    this.autoFocusDisabled = true;
  }

  objectMap: Map<string, IFocusable> = new Map<string, IFocusable>();
  focusAbleContainers: IFocusAbleObjectContainer[] = [];
  private lastFocused: IFocusable | undefined;

  setLastFocused(focusable: IFocusable) {
    this.lastFocused = focusable;
  }

  constructor(public parent: any) {
  }

  subscribe(focusAbleObject: IFocusable, name: string | undefined, tabIndex: string | undefined) {
    if (!focusAbleObject) {
      return;
    }
    const focusAbleContainer = new FocusAbleObjectContainer(focusAbleObject, name, tabIndex);
    const existingContainer = this.focusAbleContainers
      .find(container => container.name && container.name === name ||
        container.focusable === focusAbleObject);
    if (existingContainer) {
      this.focusAbleContainers.remove(existingContainer);
    }
    this.focusAbleContainers.push(focusAbleContainer);
    this.focusAbleContainers = this.focusAbleContainers.sort(compareTabIndexOwners);
  }

  focus(name: string) {
    let focusable = this.focusAbleContainers.find((container) => container.name === name)?.focusable;
    this.focusAndRemember(focusable);
  }

  private focusAndRemember(focusable: IFocusable | undefined) {
    if (!focusable) {
      return;
    }
    this.lastFocused = focusable;
    requestFocus(focusable as any);
  }

  refocusLast() {
    requestFocus(this.lastFocused as any);
  }

  forceAutoFocus() {
    const focusable = this.focusAbleContainers[0].focusable;
    if (focusable.disabled) {
      //  (focusable as any).readOnly returns always false => readonly fields cannot be skipped
      this.focusNext(focusable);
      return;
    }
    setTimeout(() => {
      this.focusAndRemember(focusable);
    }, 0);
  }

  autoFocus() {
    if (this.focusAbleContainers.length === 0 || this.autoFocusDisabled || isGlobalAutoFocusDisabled(this.parent)) {
      return;
    }
    this.forceAutoFocus();
  }

  focusNext(activeElement: any) {
    this.focusNextInternal(activeElement, 0);
  }

  focusNextInternal(activeElement: any, callNumber: number) {
    if (callNumber > 20) {
      return;
    }
    const currentContainerIndex = this.focusAbleContainers.findIndex(
      (container) => container.focusable === activeElement
    );
    const nextIndex =
      this.focusAbleContainers.length - 1 > currentContainerIndex ? currentContainerIndex + 1 : 0;
    const focusable = this.focusAbleContainers[nextIndex].focusable;
    if (focusable !== activeElement && focusable.disabled) {
      this.focusNextInternal(focusable, callNumber + 1);
    } else {
      setTimeout(() => {
        this.focusAndRemember(focusable);
      });
    }
  }

  focusPrevious(activeElement: any) {
    const currentContainerIndex = this.focusAbleContainers.findIndex(
      (container) => container.focusable === activeElement
    );
    const previousIndex =
      currentContainerIndex === 0 ? this.focusAbleContainers.length - 1 : currentContainerIndex - 1;
    const focusable = this.focusAbleContainers[previousIndex].focusable;
    if (focusable.disabled) {
      this.focusPrevious(focusable);
    } else {
      setTimeout(() => {
        this.focusAndRemember(focusable);
      });
    }
  }
}

export interface IFocusAbleObjectContainer extends ITabIndexOwner{
  name: string | undefined;
  focusable: IFocusable;
  tabIndex: string | undefined;
}

export class FocusAbleObjectContainer implements IFocusAbleObjectContainer {
  constructor(
    public focusable: IFocusable,
    public name: string | undefined,
    public tabIndex: string | undefined
  ) {
  }
}

export interface IFocusable {
  focus(): void;

  disabled: boolean;
}

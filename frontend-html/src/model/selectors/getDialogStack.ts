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

import { getApplication } from './getApplication';
import { isMobileLayoutActive } from "model/selectors/isMobileLayoutActive";
import React from "react";
import { getMobileState } from "model/selectors/getMobileState";
import { runInFlowWithHandler } from "utils/runInFlowWithHandler";

export function getDialogStack(ctx: any) {
  return getApplication(ctx).dialogStack;
}

export function showDialog(ctx: any, key: string, component: React.ReactElement) {
  if(isMobileLayoutActive(ctx)){
    const closeFunction = () => {
      runInFlowWithHandler({ctx: ctx, action: async ()=> {
          getMobileState(ctx).dialogComponent = null;
        }
      })
    };
    getMobileState(ctx).dialogComponent = component;
    return closeFunction;
  }
  return getDialogStack(ctx).pushDialog(key, component);
}


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

import {IUrlUpenMethod} from "model/entities/types/IUrlOpenMethod";
import {getWorkbenchLifecycle} from "model/selectors/getWorkbenchLifecycle";

export function openNewUrl(ctx: any) {
  return function* openNewUrl(
    url: string,
    urlOpenMethod: IUrlUpenMethod,
    title: string
  ) {
    switch (urlOpenMethod) {
      case IUrlUpenMethod.OrigamTab:
        yield* getWorkbenchLifecycle(ctx).openNewUrl(url, title);
        break;
      default:
        // TODO: Transform url to be absolute to urlroot?
        const win = window.open(`${url}`, "_blank");
        win && win.focus();
        break;
    }
  };
}

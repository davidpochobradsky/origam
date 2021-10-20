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

import React, { useContext } from "react";
import S from "gui/Components/TabbedView/TabbedViewPanel.module.scss";
import cx from "classnames";
import { CtxPanelVisibility } from "gui/contexts/GUIContexts";

export const TabbedViewPanel: React.FC<{ isActive: boolean }> = props => {
  const ctxPanelVisibility = useContext(CtxPanelVisibility);
  return (
    <CtxPanelVisibility.Provider
      value={{isVisible: props.isActive && ctxPanelVisibility.isVisible}}
    >
      <div className={cx(S.root, {isActive: props.isActive})}>
        {props.children}
      </div>
    </CtxPanelVisibility.Provider>
  );
};

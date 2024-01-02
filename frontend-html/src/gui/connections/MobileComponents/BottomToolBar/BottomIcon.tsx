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

import React from "react";
import S from "gui/connections/MobileComponents/BottomToolBar/BottomIcon.module.scss";
import { Icon } from "gui/Components/Icon/Icon";
import cx from "classnames";

export const BottomIcon: React.FC<{
  iconPath?: string;
  onClick: ()=>void;
  hidden?: boolean;
  className?: string;
}> = (props) => {
  return (
    <div
      className={cx(S.root, "bottomBarItem")}
      onClick={props.onClick}
    >
      {props.iconPath && <div className={S.icon}>
        <Icon
          src={props.iconPath}
          className={props.className + " " + (props.hidden ? S.hidden : "")}
        />
      </div>}
    </div>
  );
}

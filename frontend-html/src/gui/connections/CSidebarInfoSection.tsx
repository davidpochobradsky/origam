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
import { IInfoSubsection } from "gui/connections/types";
import { SidebarRecordInfo } from "gui/Components/SidebarInfoSection/SidebarRecordInfo";
import { SidebarRecordAudit } from "gui/Components/SidebarInfoSection/SidebarRecordAudit";
import { MobXProviderContext, observer } from "mobx-react";
import { getRecordInfo } from "model/selectors/RecordInfo/getRecordInfo";

export const CSidebarInfoSection: React.FC<{
  activeSubsection: IInfoSubsection;
}> = observer(props => {
  const workbench = useContext(MobXProviderContext).workbench;
  const recordInfo = getRecordInfo(workbench);

  return (
    <>
      {props.activeSubsection === IInfoSubsection.Info &&
      recordInfo.info.length > 0 && (
        <SidebarRecordInfo lines={recordInfo.info}/>
      )}
      {props.activeSubsection === IInfoSubsection.Audit && recordInfo.audit && (
        <SidebarRecordAudit/>
      )}
    </>
  );
});

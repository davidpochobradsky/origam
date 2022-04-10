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
import S from "./MobileMain.module.scss";
import { TopToolBar } from "gui/connections/MobileComponents/TopToolBar/TopToolBar";
import { CSidebar } from "gui/connections/CSidebar";
import { MobXProviderContext, observer } from "mobx-react";
import { MobileState, } from "model/entities/MobileState/MobileState";
import { About } from "model/entities/AboutInfo";
import { getAbout } from "model/selectors/getAbout";
import { Search } from "gui/connections/MobileComponents/Search";
import { BottomToolBar } from "gui/connections/MobileComponents/BottomToolBar/BottomToolBar";
import { MobileAboutView } from "gui/connections/MobileComponents/MobileAboutView";
import { IWorkbench } from "model/entities/types/IWorkbench";
import { CDialogContent } from "gui/connections/CDialogContent";
import { BreadCrumbs } from "gui/connections/MobileComponents/Navigation/BreadCrumbs";
import { CScreenContent } from "gui/connections/CScreenContent";
import { ScreenHeader } from "gui/connections/MobileComponents/ScreenHeader";
import {
  AboutLayoutState,
  EditLayoutState,
  MenuLayoutState,
  ScreenLayoutState,
  SearchLayoutState
} from "model/entities/MobileState/MobileLayoutState";
import { getActiveScreen } from "model/selectors/getActiveScreen";

@observer
export class MobileMain extends React.Component<{}> {

  static contextType = MobXProviderContext;

  get mobileState(): MobileState {
    return this.context.application.mobileState;
  }

  get workbench(): IWorkbench {
    return this.context.workbench;
  }

  get about(): About {
    return getAbout(this.context.application);
  }

  componentDidMount() {
    if (!getActiveScreen(this.workbench)) {
      this.mobileState.layoutState = new MenuLayoutState();
    }
    this.about.update();
  }

  renderMainPageContents() {
    return (
      <>
        {this.renderStateComponent()}
        <div className={S.dialog + " " + (this.mobileState.dialogComponent === null ? S.hidden : "")}>
          {this.mobileState.dialogComponent}
        </div>
      </>
    );
  }

  private renderStateComponent() {
    if (this.mobileState.layoutState instanceof MenuLayoutState) {
      return <CSidebar/>;
    }
    if (this.mobileState.layoutState instanceof ScreenLayoutState) {
      return (
        <>
          <BreadCrumbs/>
          <ScreenHeader/>
          <CScreenContent/>
        </>
      );
    }
    if (this.mobileState.layoutState instanceof SearchLayoutState) {
      return <Search/>
    }
    if (this.mobileState.layoutState instanceof EditLayoutState) {
      return (this.mobileState.layoutState as EditLayoutState).component
    }
    if (this.mobileState.layoutState instanceof AboutLayoutState) {
      return <MobileAboutView
        aboutInfo={this.about.info}
        mobileState={this.mobileState}
      />
    }
    throw new Error(this.mobileState.layoutState + " not implemented");
  }

  render() {
    return (
      <div className={S.root}>
        <TopToolBar mobileState={this.mobileState}/>
        {this.renderMainPageContents()}
        <CDialogContent/>
        <BottomToolBar
          mobileState={this.mobileState}
          ctx={this.context.application}
        />
      </div>
    );
  }
}



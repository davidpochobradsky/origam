import React from "react";
import { LoginPage } from "./Login/LoginPage";
import { observer, inject } from "mobx-react";
import { getApplicationLifecycle } from "../model/selectors/getApplicationLifecycle";
import { IApplicationPage } from "../model/types/IApplicationLifecycle";
import { WorkbenchPage } from "./Workbench/WorkbenchPage";
import { getShownPage } from "../model/selectors/Application/getShownPage";

@inject(({ application }) => ({
  page: getShownPage(application)
}))
@observer
export class Main extends React.Component<{ page?: IApplicationPage }> {
  render() {
    switch (this.props.page!) {
      case IApplicationPage.Login:
        return <LoginPage />;
      case IApplicationPage.Workbench:
        return <WorkbenchPage />;
    }
  }
}

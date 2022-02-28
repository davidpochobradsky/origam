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

import { Icon } from "gui/Components/Icon/Icon";
import { ScreenToolbar } from "gui/Components/ScreenToolbar/ScreenToolbar";
import { ScreenToolbarAction } from "gui/Components/ScreenToolbar/ScreenToolbarAction";
import { ScreenToolbarPusher } from "gui/Components/ScreenToolbar/ScreenToolbarPusher";
import { MobXProviderContext, Observer, observer } from "mobx-react";
import { IApplication } from "model/entities/types/IApplication";
import React, { Fragment } from "react";
import { action, observable } from "mobx";
import { onScreenToolbarLogoutClick } from "model/actions-ui/ScreenToolbar/onScreenToolbarLogoutClick";
import { openSearchWindow } from "model/actions-ui/ScreenToolbar/openSearchWindow";
import { ScreenToolbarActionGroup } from "gui/Components/ScreenToolbar/ScreenToolbarActionGroup";
import { getActiveScreen } from "model/selectors/getActiveScreen";
import { onSaveSessionClick } from "model/actions-ui/ScreenToolbar/onSaveSessionClick";
import { onRefreshSessionClick } from "model/actions-ui/ScreenToolbar/onRefreshSessionClick";
import { getActiveScreenActions } from "model/selectors/getActiveScreenActions";
import { getIsEnabledAction } from "model/selectors/Actions/getIsEnabledAction";

import uiActions from "model/actions-ui-tree";
import { Dropdowner } from "gui/Components/Dropdowner/Dropdowner";
import { UserMenuDropdown } from "gui/Components/UserMenuDropdown/UserMenuDropdown";
import { getLoggedUserName } from "model/selectors/User/getLoggedUserName";
import { onReloadWebScreenClick } from "model/actions-ui/ScreenToolbar/onReloadWebScreen";
import { isIFormScreenEnvelope } from "model/entities/types/IFormScreen";
import { isIWebScreen } from "model/entities/types/IWebScreen";
import { getIsSuppressSave } from "model/selectors/FormScreen/getIsSuppressSave";
import { Dropdown } from "gui/Components/Dropdown/Dropdown";
import { IAction, IActionType } from "model/entities/types/IAction";

import { T } from "utils/translation";
import { getUserAvatarLink } from "model/selectors/User/getUserAvatarLink";
import { getCustomAssetsRoute } from "model/selectors/User/getCustomAssetsRoute";
import { DropdownItem } from "gui/Components/Dropdown/DropdownItem";
import { IAboutInfo } from "model/entities/types/IAboutInfo";
import { runInFlowWithHandler } from "utils/runInFlowWithHandler";
import { getApi } from "model/selectors/getApi";
import { getIsSuppressRefresh } from "model/selectors/FormScreen/getIsSuppressRefresh";
import { getHelpUrl } from "model/selectors/User/getHelpUrl";

function isSaveShortcut(event: any) {
  return event.key === "s" && (event.ctrlKey || event.metaKey);
}

function isRefreshShortcut(event: any) {
  return event.key === "r" && (event.ctrlKey || event.metaKey);
}

@observer
export class CScreenToolbar extends React.Component<{}> {
  static contextType = MobXProviderContext;

  state = {
    hiddenActionIds: new Set<string>(),
  };

  get application(): IApplication {
    return this.context.application;
  }

  @observable
  aboutInfo: IAboutInfo = {
    serverVersion: "",
  };

  componentDidMount() {
    runInFlowWithHandler({
      ctx: this.application,
      action: async () => {
        const api = getApi(this.application);
        this.aboutInfo = await api.getAboutInfo();
      },
    });
  }

  @action.bound
  handleLogoutClick(event: any) {
    onScreenToolbarLogoutClick(this.application)(event);
  }

  getOverfullActionsDropdownContent(
    toolbarActions: Array<{
      section: string;
      actions: IAction[];
    }>,
    actionFilter: ((action: IAction) => boolean) | undefined,
    setDropped: (state: boolean) => void
  ) {
    const customAssetsRoute = getCustomAssetsRoute(this.application);

    const iconsWillBeShown = toolbarActions
      .flatMap((toolbar) => toolbar.actions)
      .some((action) => action.iconUrl);

    function getIcon(action: IAction) {
      if (action.iconUrl) {
        return <Icon src={customAssetsRoute + "/" + action.iconUrl}/>;
      }
      return iconsWillBeShown ? <div/> : null;
    }

    return toolbarActions
      .filter((actionGroup) => actionGroup.actions.length > 0)
      .map((actionGroup) => (
        <Fragment key={actionGroup.section}>
          {/*{this.renderActions(actionGroup.actions)}*/}
          {actionGroup.actions
            .filter(
              (action) => (actionFilter ? actionFilter(action) : true) && getIsEnabledAction(action)
            )
            .map((action, idx) => (
              <DropdownItem>
                <ScreenToolbarAction
                  icon={getIcon(action)}
                  label={action.caption}
                  onClick={(event) =>
                  {
                    uiActions.actions.onActionClick(action)(event, action);
                    setDropped(false)
                  }
                }
                />
              </DropdownItem>
            ))}
        </Fragment>
      ));
  }

  renderActions(actions: IAction[]) {
    const actionsToRender = actions.filter((action) => getIsEnabledAction(action));
    return actionsToRender
      .filter((action) => !action.groupId)
      .map((action, idx) => this.renderAction(action, actionsToRender, idx));
  }

  renderAction(action: IAction, actionsToRender: IAction[], order: number) {
    const customAssetsRoute = getCustomAssetsRoute(this.application);
    if (action.type === IActionType.Dropdown) {
      const childActions = actionsToRender.filter(
        (otherAction) => otherAction.groupId === action.id
      );
      return (
        <Dropdowner
          style={{width: "auto"}}
          trigger={({refTrigger, setDropped, isDropped}) => (
            <Observer key={action.id}>
              {() => (
                <ScreenToolbarAction
                  rootRef={refTrigger}
                  onMouseDown={() => setDropped(true)}
                  className={isDropped ? "isActiveDropDownAction" : ""}
                  icon={
                    action.iconUrl ? (
                      <Icon src={customAssetsRoute + "/" + action.iconUrl}/>
                    ) : undefined
                  }
                  label={action.caption}
                />
              )}
            </Observer>
          )}
          content={args => (
            <Dropdown>
              {this.getOverfullActionsDropdownContent(
                [{section: "", actions: childActions}],
                undefined,
                args.setDropped
              )}
            </Dropdown>
          )}
        />
      );
    }
    return (
      <Observer key={action.id}>
        {() => (
          <ScreenToolbarAction
            icon={
              action.iconUrl ? <Icon src={customAssetsRoute + "/" + action.iconUrl}/> : undefined
            }
            label={action.caption}
            onClick={(event) => {
              uiActions.actions.onActionClick(action)(event, action);
            }
          }
          />
        )}
      </Observer>
    );
  }

  renderForFormScreen() {
    const activeScreen = getActiveScreen(this.application);
    if (activeScreen && !activeScreen.content) return null;
    const formScreen =
      activeScreen && !activeScreen.content.isLoading ? activeScreen.content.formScreen : undefined;
    const isDirty = formScreen && formScreen.isDirty;
    const toolbarActions = getActiveScreenActions(this.application);
    const userName = getLoggedUserName(this.application);
    const avatarLink = getUserAvatarLink(this.application);
    return (
      <ScreenToolbar>
        {formScreen ? (
          <>
            <ScreenToolbarActionGroup>
              {!getIsSuppressSave(formScreen) && (
                <ScreenToolbarAction
                  className={isDirty ? "isRed isHoverGreen" : ""}
                  onClick={onSaveSessionClick(formScreen)}
                  onShortcut={onSaveSessionClick(formScreen)}
                  id={"saveButton"}
                  shortcutPredicate={isSaveShortcut}
                  icon={
                    <Icon
                      src="./icons/save.svg"
                      className={isDirty ? "isRed isHoverGreen" : ""}
                      tooltip={T("Save", "save_tool_tip")}
                    />
                  }
                  label={T("Save", "save_tool_tip")}
                />
              )}
              {!getIsSuppressRefresh(formScreen) && (
                <ScreenToolbarAction
                  onClick={onRefreshSessionClick(formScreen)}
                  onShortcut={onRefreshSessionClick(formScreen)}
                  id={"refreshButton"}
                  shortcutPredicate={isRefreshShortcut}
                  icon={
                    <Icon src="./icons/refresh.svg" tooltip={T("Refresh", "refresh_tool_tip")}/>
                  }
                  label={T("Refresh", "refresh_tool_tip")}
                />
              )}
            </ScreenToolbarActionGroup>
            <Observer>
              {() => (
                <ScreenToolbarActionGroup grovable={true}>
                  {toolbarActions
                    .filter((actionGroup) => actionGroup.actions.length > 0)
                    .map((actionGroup) => (
                      <ScreenToolbarActionGroup key={actionGroup.section}>
                        {this.renderActions(actionGroup.actions)}
                      </ScreenToolbarActionGroup>
                    ))}
                </ScreenToolbarActionGroup>
              )}
            </Observer>
          </>
        ) : null}
        {this.state.hiddenActionIds.size > 0 && (
          <Dropdowner
            style={{width: "auto"}}
            trigger={({refTrigger, setDropped}) => (
              <ScreenToolbarAction
                rootRef={refTrigger}
                onMouseDown={() => setDropped(true)}
                icon={<Icon src="./icons/dot-menu.svg" tooltip={""}/>}
              />
            )}
            content={(args) => (
              <Dropdown>
                {this.getOverfullActionsDropdownContent(
                  toolbarActions,
                  action => this.state.hiddenActionIds.has(action.id),
                  args.setDropped
                )}
              </Dropdown>
            )}
          />
        )}
        <ScreenToolbarAction
          onClick={() => openSearchWindow(this.application)}
          icon={<Icon src="./icons/search.svg"/>}
        />
        <UserMenuDropdown
          avatarLink={avatarLink}
          userName={userName}
          handleLogoutClick={(event) => this.handleLogoutClick(event)}
          ctx={this.application}
          aboutInfo={this.aboutInfo}
          helpUrl={getHelpUrl(this.application)}
        />
      </ScreenToolbar>
    );
  }

  renderForWebScreen() {
    const activeScreen = getActiveScreen(this.application);
    const userName = getLoggedUserName(this.application);
    const avatarLink = getUserAvatarLink(this.application);
    return (
      <ScreenToolbar>
        <>
          <ScreenToolbarActionGroup>
            <ScreenToolbarAction
              onMouseDown={onReloadWebScreenClick(activeScreen)}
              icon={<Icon src="./icons/refresh.svg" tooltip={T("Refresh", "refresh_tool_tip")}/>}
              label={T("Refresh", "refresh_tool_tip")}
            />
          </ScreenToolbarActionGroup>
        </>
        <ScreenToolbarPusher/>
        <ScreenToolbarAction
          onClick={() => openSearchWindow(this.application)}
          icon={<Icon src="./icons/search.svg"/>}
        />
        <UserMenuDropdown
          avatarLink={avatarLink}
          userName={userName}
          handleLogoutClick={(event) => this.handleLogoutClick(event)}
          ctx={this.application}
          aboutInfo={this.aboutInfo}
          helpUrl={getHelpUrl(this.application)}
        />
      </ScreenToolbar>
    );
  }

  renderDefault() {
    const userName = getLoggedUserName(this.application);
    const avatarLink = getUserAvatarLink(this.application);
    return (
      <ScreenToolbar>
        <ScreenToolbarPusher/>
        {/*<ScreenToolbarAction
          icon={<Icon src="./icons/search.svg" />}
          label="Search"
        />*/}
        <ScreenToolbarAction
          onClick={() => openSearchWindow(this.application)}
          icon={<Icon src="./icons/search.svg"/>}
        />
        <UserMenuDropdown
          avatarLink={avatarLink}
          userName={userName}
          handleLogoutClick={(event) => this.handleLogoutClick(event)}
          ctx={this.application}
          aboutInfo={this.aboutInfo}
          helpUrl={getHelpUrl(this.application)}
        />
      </ScreenToolbar>
    );
  }

  render() {
    const activeScreen = getActiveScreen(this.application);
    if (!activeScreen) {
      return this.renderDefault();
    }
    if (activeScreen.content && isIFormScreenEnvelope(activeScreen.content)) {
      return this.renderForFormScreen();
    }
    if (isIWebScreen(activeScreen)) {
      return this.renderForWebScreen();
    }
    return null;
  }
}

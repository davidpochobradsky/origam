
export interface IApplicationLifecycleData {}

export interface IApplicationLifecycle extends IApplicationLifecycleData {
  $type_IApplicationLifecycle: 1;

  parent?: any;
  isWorking: boolean;

  loginPageMessage?: string;

  onLoginFormSubmit(args: {
    event: any;
    userName: string;
    password: string;
  }): Generator;
  onSignOutClick(args: { event: any }): Generator;

  performLogout(): Generator;

  /*
  onMainMenuItemClick(args: { event: any; item: any }): void;
  onScreenTabHandleClick(event: any, openedScreen: IOpenedScreen): void;
  onScreenTabCloseClick(event: any, openedScreen: IOpenedScreen): void;
  */

  run(): Generator;

  setLoginPageMessage(msg: string): void;
  resetLoginPageMessage(): void;
}

export const isIApplicationLifecycle = (o: any): o is IApplicationLifecycle =>
  o.$type_IApplicationLifecycle;
import bind from "bind-decorator";
import { ModalWindow } from "gui/Components/Dialog/Dialog";
import _ from "lodash";
import { action, computed, observable } from "mobx";
import { observer, Observer } from "mobx-react";
import { getDialogStack } from "model/selectors/getDialogStack";
import React from "react";
import CS from "./ErrorDialog.module.scss";
import moment, { Moment } from "moment";
import { T } from "utils/translation";
import { IErrorDialogController } from "./types/IErrorDialog";
import { Icon } from "gui/Components/Icon/Icon";

function NewExternalPromise<T>() {
  let resolveFn: any;
  let rejectFn: any;
  const p = new Promise((resolve, reject) => {
    resolveFn = resolve;
    rejectFn = reject;
  });
  (p as any).resolve = resolveFn;
  (p as any).reject = rejectFn;
  return p as Promise<T> & {
    resolve(value: T): void;
    reject(error: any): void;
  };
}

export class ErrorDialogController implements IErrorDialogController {
  @observable errorStack: Array<{
    id: number;
    error: any;
    promise: any;
    timestamp: Moment;
  }> = [];

  @observable isDialogDisplayed = false;

  @computed get errorMessages() {
    return this.errorStack.map((errItem) => {
      console.error(errItem.error);

      const handlePlainText = () =>
        _.get(errItem.error, "response.headers.content-type", "").startsWith("text/plain") &&
        errItem.error.response.data;

      const handleMessageField = () => {
        if (!errItem.error.response?.data) {
          return "";
        }
        let exception = errItem.error.response.data;
        let message = "";
        do {
          const exMessage = _.get(exception, "message") || _.get(exception, "Message");
          if (exMessage) {
            message += exMessage;
            message += "\n";
          }
          exception = exception.innerException || exception.InnerException ;
        } while (exception);
        return message;
      };

      const handleRuntimeException = () => "" + errItem.error;

      const handleLoginValidation = () =>
        [
          ..._.get(errItem.error, "response.data.UserName", []),
          ..._.get(errItem.error, "response.data.Password", []),
        ].join(" ");

      let errorMessage =
        handlePlainText() ||
        handleMessageField() ||
        handleLoginValidation() ||
        handleRuntimeException();

      if (errItem.error?.request?.status === 500 || errItem.error?.request?.status === 409) {
        errorMessage =
          "Server error occurred. Please check server log for more details:\n" + errorMessage;
      }

      return {
        message: errorMessage,
        id: errItem.id,
        timestamp: errItem.timestamp.format("YYYY-MM-DD HH:mm:ss"),
      };
    });
  }

  idGen = 0;
  @bind
  *pushError(error: any) {
    const myId = this.idGen++;
    const promise = NewExternalPromise();
    this.errorStack.push({ id: myId, error, promise, timestamp: moment() });
    this.displayDialog();
    console.log(this.errorMessages.length);
    yield promise;
  }

  @action.bound displayDialog() {
    if (!this.isDialogDisplayed) {
      this.isDialogDisplayed = true;
      const previouslyFocusedElement = document.activeElement as HTMLElement;
      const closeDialog = getDialogStack(this).pushDialog(
        "",
        <Observer>
          {() => (
            <ErrorDialogComponent
              errorMessages={this.errorMessages}
              onOkClick={action(() => {
                console.log("close dialog...");
                closeDialog();
                this.isDialogDisplayed = false;
                this.dismissErrors();
                setTimeout(() => previouslyFocusedElement?.focus(), 100);
              })}
            />
          )}
        </Observer>
      );
    }
  }

  @action.bound
  dismissError(id: number) {
    const errItemIndex = this.errorStack.findIndex((item) => item.id === id);
    if (errItemIndex > -1) {
      const errItem = this.errorStack[errItemIndex];
      errItem.promise.resolve(null);
      this.errorStack.splice(errItemIndex, 1);
    }
  }

  @action.bound dismissErrors() {
    for (let errItem of [...this.errorStack]) {
      this.dismissError(errItem.id);
    }
  }
}

@observer
export class ErrorDialogComponent extends React.Component<{
  errorMessages: Array<{ id: number; message: string; timestamp: string }>;
  onOkClick?: (event: any) => void;
}> {
  render() {
    return (
      <ModalWindow
        title={T("Error", "error_window_title")}
        titleButtons={null}
        buttonsCenter={
          <>
            <button tabIndex={0} autoFocus={true} onClick={this.props.onOkClick}>
              {T("Ok", "button_ok")}
            </button>
          </>
        }
        buttonsLeft={null}
        buttonsRight={null}
      >
        <div className={CS.dialogContent}>
          <div className={CS.dialogBigIconColumn}>
            {/* SVG as data url here because we might not be able to determine customAssets path. 
            This has fill color embedded :-( */}
            <Icon
              src={`data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4KPCEtLSBHZW5lcmF0b3I6IEFkb2JlIElsbHVzdHJhdG9yIDIzLjEuMSwgU1ZHIEV4cG9ydCBQbHVnLUluIC4gU1ZHIFZlcnNpb246IDYuMDAgQnVpbGQgMCkgIC0tPgo8c3ZnIHZlcnNpb249IjEuMSIgaWQ9IkViZW5lXzEiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgeD0iMHB4IiB5PSIwcHgiCiAgICAgdmlld0JveD0iMCAwIDIwIDIwIiBzdHlsZT0iZW5hYmxlLWJhY2tncm91bmQ6bmV3IDAgMCAyMCAyMDsiIHhtbDpzcGFjZT0icHJlc2VydmUiPgo8dGl0bGU+WmVpY2hlbmZsw6RjaGUgMTwvdGl0bGU+CjxnPgoJPHBhdGggZmlsbD0iI2ZmNDM1NCIgZD0iTTkuMywwLjlsLTkuMiwxN2MtMC4zLDAuNSwwLjEsMS4yLDAuNywxLjJoMTguNGMwLjYsMCwxLTAuNywwLjctMS4ybC05LjItMTdDMTAuNCwwLjQsOS42LDAuNCw5LjMsMC45eiBNMTAsMTUuOUwxMCwxNS45CgkJYy0wLjQsMC0wLjgtMC40LTAuOC0wLjh2MGMwLTAuNCwwLjQtMC44LDAuOC0wLjhoMGMwLjQsMCwwLjgsMC40LDAuOCwwLjh2MEMxMC44LDE1LjUsMTAuNCwxNS45LDEwLDE1Ljl6IE05LjIsMTEuOFY2LjkKCQljMC0wLjQsMC40LTAuOCwwLjgtMC44aDBjMC40LDAsMC44LDAuNCwwLjgsMC44djQuOWMwLDAuNC0wLjQsMC44LTAuOCwwLjhoMEM5LjYsMTIuNiw5LjIsMTIuMiw5LjIsMTEuOHoiLz4KPC9nPgo8L3N2Zz4K`}
            />
          </div>
          <div className={CS.dialogMessageColumn}>
            {this.props.errorMessages.length === 1 ? (
              this.props.errorMessages[0].message
            ) : (
              <div className={CS.errorMessageList}>
                {this.props.errorMessages.map((errMessage) => (
                  <div key={errMessage.id} className={CS.errorMessageListItem}>
                    <span className={CS.errorMessageDatetime}>{errMessage.timestamp}</span>
                    {errMessage.message}
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </ModalWindow>
    );
  }
}

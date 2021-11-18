import S from "./Dialog.module.scss";
import React from "react";

import * as ReactDOM from "react-dom";
import { observer, Observer } from "mobx-react";
import { action, observable } from "mobx";
import Measure, { BoundingRect } from "react-measure";
import { Icon } from "gui/Components/Icon/Icon";

export class ModalWindowOverlay extends React.Component {
  render() {
    return ReactDOM.createPortal(
      <div className={S.modalWindowOverlay}>{this.props.children}</div>,
      document.getElementById("modal-window-portal")!
    );
  }
}

export class ModalWindowNoOverlay extends React.Component {
  render() {
    return ReactDOM.createPortal(
      this.props.children,
      document.getElementById("modal-window-portal")!
    );
  }
}

@observer
export class ModalWindow extends React.Component<{
  title: React.ReactNode;
  titleButtons: React.ReactNode;
  titleIsWorking?: boolean;
  buttonsLeft: React.ReactNode;
  buttonsRight: React.ReactNode;
  buttonsCenter: React.ReactNode;
  width?: number;
  height?: number;
  topPosiotionProc?: number;
  onKeyDown?: (event: any) => void;
}> {
  @observable top: number = window.screen.height + 50;
  @observable left: number = window.screen.width + 50;
  @observable isDragging = false;

  dragStartMouseX = 0;
  dragStartMouseY = 0;
  dragStartPosX = 0;
  dragStartPosY = 0;

  isInitialized = false;

  @action.bound handleResize(contentRect: { bounds: BoundingRect }) {
    if (!(!this.isInitialized && contentRect.bounds!.height && contentRect.bounds!.width)) {
      return;
    }
    if(this.props.topPosiotionProc){
      this.top = window.innerHeight * this.props.topPosiotionProc / 100;
    }else{
      this.top = window.innerHeight / 2 - contentRect.bounds!.height / 2;
    }
    this.left = window.innerWidth / 2 - contentRect.bounds!.width / 2;
    this.isInitialized = true;
  }

  @action.bound handleTitleMouseDown(event: any) {
    window.addEventListener("mousemove", this.handleWindowMouseMove);
    window.addEventListener("mouseup", this.handleWindowMouseUp);
    this.isDragging = true;
    this.dragStartMouseX = event.screenX;
    this.dragStartMouseY = event.screenY;
    this.dragStartPosX = this.left;
    this.dragStartPosY = this.top;
  }

  @action.bound handleWindowMouseMove(event: any) {
    this.top = this.dragStartPosY + event.screenY - this.dragStartMouseY;
    this.left = this.dragStartPosX + event.screenX - this.dragStartMouseX;
    event.preventDefault();
    event.stopPropagation();
  }

  @action.bound handleWindowMouseUp(event: any) {
    window.removeEventListener("mousemove", this.handleWindowMouseMove);
    window.removeEventListener("mouseup", this.handleWindowMouseUp);
    this.isDragging = false;
  }

  onKeyDown(event: any) {
    this.props.onKeyDown?.(event);
  }

  _focusHookIsOn = false;

  footerFocusHookEnsureOn() {
    if (this.elmFooter && !this._focusHookIsOn) {
      this.elmFooter.addEventListener(
        "keydown",
        (evt: any) => {
          if (evt.key === "Tab") {
            evt.preventDefault();
            if (evt.shiftKey) {
              if (evt.target.previousSibling) {
                evt.target.previousSibling.focus();
              } else {
                this.elmFooter?.lastChild?.focus();
              }
            } else {
              if (evt.target.nextSibling) {
                evt.target.nextSibling.focus();
              } else {
                this.elmFooter?.firstChild?.focus();
              }
            }
          }
        },
        true
      );
      console.log("handler added");
      this._focusHookIsOn = true;
    }
  }

  componentDidMount() {
    this.footerFocusHookEnsureOn();
  }

  componentWillUnmount() {}

  refFooter = (elm: any) => {
    this.elmFooter = elm;
    if (elm) {
      this.footerFocusHookEnsureOn();
    }
  };
  elmFooter: any;

  renderFooter() {
    if (this.props.buttonsLeft || this.props.buttonsCenter || this.props.buttonsRight) {
      return (
        <div ref={this.refFooter} className={S.footer}>
          {this.props.buttonsLeft}
          {this.props.buttonsCenter ? this.props.buttonsCenter : <div className={S.pusher} />}
          {this.props.buttonsRight}
        </div>
      );
    } else {
      return null;
    }
  }

  render() {
    return (
      <Measure bounds={true} onResize={this.handleResize}>
        {({ measureRef }) => (
          <Observer>
            {() => (
              <div
                ref={measureRef}
                className={S.modalWindow}
                style={{
                  top: this.top,
                  left: this.left,
                  minWidth: this.props.width,
                  minHeight: this.props.height,
                }}
                tabIndex={0}
                onKeyDown={(event: any) => this.onKeyDown(event)}
              >
                {this.props.title && (
                  <div className={S.title} onMouseDown={this.handleTitleMouseDown}>
                    <div className={S.label}>
                      <div className={S.labelText}>{this.props.title}</div>
                      {this.props.titleIsWorking && (
                        <div className={S.progressIndicator}>
                          <div className={S.indefinite} />
                        </div>
                      )}
                    </div>

                    <div className={S.buttons}>{this.props.titleButtons}</div>
                  </div>
                )}
                <div className={S.body}>{this.props.children}</div>
                {this.renderFooter()}
              </div>
            )}
          </Observer>
        )}
      </Measure>
    );
  }
}

export const CloseButton = (props: { onClick?: (event: any) => void }) => (
  <button className={S.btnClose} onClick={props.onClick}>
    <div className={S.btnIconContainer}>
      <Icon src="./icons/close.svg" tooltip={""} />
    </div>
  </button>
);

import React from "react";
import S from "gui/Components/TabbedView/TabbedViewHandle.module.scss";
import cx from "classnames";
import { Icon } from "gui/Components/Icon/Icon";

export class TabbedViewHandle extends React.Component<{
  title?: string;
  isActive?: boolean;
  hasCloseBtn?: boolean;
  isDirty?: boolean;
  onClick?(event: any): void;
  onCloseClick?(event: any): void;
  onCloseMouseDown?(event: any): void;
}> {
  render() {
    return (
      <div
        onClick={this.props.onClick}
        className={cx(S.root, { isActive: this.props.isActive, isDirty: this.props.isDirty })}
        title={this.props.title}
      >
        <div className={S.label}>{this.props.children}</div>
        {this.props.hasCloseBtn && (
          <a
            className={S.closeBtn}
            onClick={this.props.onCloseClick}
            onMouseDown={this.props.onCloseMouseDown}
          >
            <Icon src="./icons/close.svg" tooltip={""} />
          </a>
        )}
      </div>
    );
  }
}
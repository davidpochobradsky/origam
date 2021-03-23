import S from "gui/Components/Form/FormField.module.scss";
import { inject, observer } from "mobx-react";
import { IDockType } from "model/entities/types/IProperty";
import { getRowStateDynamicLabel } from "model/selectors/RowState/getRowStateNameOverride";
import { getSelectedRowId } from "model/selectors/TablePanelView/getSelectedRowId";
import React from "react";
import { formatTooltipText, formatTooltipPlaintext } from "../ToolTip/FormatTooltipText";
export enum ICaptionPosition {
  Left = "Left",
  Right = "Right",
  Top = "Top",
  None = "None",
}

@inject(({ property }, { caption }) => {
  const rowId = getSelectedRowId(property);

  const ovrCaption = getRowStateDynamicLabel(property, rowId || "", property.id);

  return {
    caption: !!ovrCaption ? ovrCaption : caption,
  };
})
@observer
export class FormField extends React.Component<{
  caption: React.ReactNode;
  captionPosition?: ICaptionPosition;
  captionLength: number;
  dock?: IDockType;
  editor: React.ReactNode;
  left: number;
  top: number;
  width: number;
  height: number;
  isCheckbox?: boolean;
  isHidden?: boolean;
  hideCaption?: boolean;
  captionColor?: string;
  toolTip?: string;
}> {
  get captionStyle() {
    if (this.props.isHidden) {
      return {
        display: "none",
      };
    }
    switch (this.props.captionPosition) {
      default:
      case ICaptionPosition.Left:
        return {
          top: this.props.top,
          left: this.props.left - this.props.captionLength,
          color: this.props.captionColor,
        };
      case ICaptionPosition.Right:
        // 20 is expected checkbox width, might be needed to be set dynamically
        // if there is some difference in chekbox sizes between various platforms.
        return {
          top: this.props.top,
          left: this.props.isCheckbox ? this.props.left + 20 : this.props.left + this.props.width,
          color: this.props.captionColor,
        };
      case ICaptionPosition.Top:
        return {
          top: this.props.top - 20, // TODO: Move this constant somewhere else...
          left: this.props.left,
          color: this.props.captionColor,
        };
    }
  }

  get formFieldStyle() {
    if (this.props.isHidden) {
      return {
        display: "none",
      };
    }
    if (this.props.dock === IDockType.Fill) {
      return {
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
      };
    }
    return {
      left: this.props.left,
      top: this.props.top,
      width: this.props.width,
      height: this.props.height,
    };
  }

  renderEditorWithToolTip() {
   return (
      <label
        className={S.caption}
        style={this.captionStyle}
        title={formatTooltipPlaintext(this.props.toolTip)}
      >
        {this.props.caption}
      </label>
    );
  }

  render() {
    const { props } = this;
    return (
      <>
        {this.props.captionPosition !== ICaptionPosition.None &&
          !this.props.hideCaption &&
          (this.props.toolTip ? (
            this.renderEditorWithToolTip()
          ) : (
            <label className={S.caption} style={this.captionStyle}>
              {props.caption}
            </label>
          ))}
        <div className={S.editor} style={this.formFieldStyle}>
          {props.editor}
        </div>
      </>
    );
  }
}

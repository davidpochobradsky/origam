import React, { useState } from "react";
import {IDockType, IProperty} from "../../../model/entities/types/IProperty";
import { BoolEditor } from "../../../gui/Components/ScreenElements/Editors/BoolEditor";
import S from "./CheckBox.module.scss";
import { inject } from "mobx-react";
import { getSelectedRow } from "../../../model/selectors/DataView/getSelectedRow";
import { onFieldBlur } from "../../../model/actions-ui/DataView/TableView/onFieldBlur";
import { onFieldChange } from "../../../model/actions-ui/DataView/TableView/onFieldChange";

export const CheckBox: React.FC<{
  checked: boolean;
  tabIndex?: number;
  readOnly: boolean;
  isHidden?: boolean;
  onChange?: (event: any, value: any) => void;
  property?: IProperty;
}> = inject(({ property, formPanelView }) => {
  const row = getSelectedRow(formPanelView)!;
  return {
    property,
    onEditorBlur: (event: any) => onFieldBlur(formPanelView)(event),
    onChange: (event: any, value: any) => onFieldChange(formPanelView)(event, row, property, value),
  };
})((props) => {
  const [isFocused, setIsFocused] = useState<boolean>(false);
  const [isChecked, setIsChecked] = useState<boolean>(props.checked);
  const [refInput, setRefInput] = useState<HTMLInputElement>();

  // const {name, captionLength, dock, height, width, x, y} = props.property!;

  const label = props.property!.name;
  const captionLength = props.property!.captionLength;
  const dock = props.property!.dock;
  const height = props.property!.height;
  const width = props.property!.width;
  const left = props.property!.x;
  const top = props.property!.y;

  function captionStyle() {
    if (props.isHidden) {
      return {
        display: "none",
      };
    }
    // switch (props.captionPosition) {
    //   default:
    //   case ICaptionPosition.Left:
    //     return {
    //       top: props.top,
    //       left: props.left - props.captionLength,
    //       width: props.captionLength
    //       //  height: this.props.height
    //     };
    //   case ICaptionPosition.Right:
    return {
      top: top,
      left: left + height,
      width: captionLength,
      //  height: this.props.height
    };
    //   case ICaptionPosition.Top:
    //     return {
    //       top: props.top - 20, // TODO: Move this constant somewhere else...
    //       left: props.left,
    //       width: props.captionLength
    //     };
    // }
  }

  function formFieldStyle() {
    if (props.isHidden) {
      return {
        display: "none",
      };
    }
    if (dock === IDockType.Fill) {
      return {
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
      };
    }
    return {
      left: left,
      top: top,
      width: width,
      height: height,
    };
  }

  function onLabelClick(event: any) {
    refInput?.focus();
    onChange(event);
  }

  function onChange(event: any){
    setIsChecked(!isChecked);
    props.onChange && !props.readOnly && props.onChange(event, isChecked);
  }

  function onInputFocus() {
    setIsFocused(true);
  }

  function onInputBlur() {
    setIsFocused(false);
  }

  return (
    <div>
      <div className={S.editor} style={formFieldStyle()}>
        <BoolEditor
          value={isChecked}
          isReadOnly={props.readOnly}
          tabIndex={props.tabIndex}
          inputSetter={(refInput) => setRefInput(refInput)}
          onBlur={onInputBlur}
          onFocus={onInputFocus}
          onChange={onChange}
        />
      </div>
      <label
        className={S.caption + " " + (isFocused ? S.focusedLabel : S.unFocusedLabel)}
        onClick={onLabelClick}
        style={captionStyle()}
      >
        {label}
      </label>
    </div>
  );
});

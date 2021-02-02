import React from "react";
import S from "./FormSection.module.scss";
import cx from "classnames";
import {FormSectionHeader} from "./FormSectionHeader";

export const FormSection: React.FC<{
  top: number;
  left: number;
  width: number;
  height: number;
  title?: React.ReactNode;
  backgroundColor: string | undefined;
  foreGroundColor: string | undefined;
}> = props => {
  const hasTitle = !!props.title;
  return (
    <div
      className={cx(S.root, { hasTitle })}
      style={{
        top: props.top,
        left: props.left,
        width: props.width,
        height: props.height,
        backgroundColor: props.backgroundColor,
      }}
    >
      {hasTitle && 
        <FormSectionHeader foreGroundColor={props.foreGroundColor}>
            {props.title}
        </FormSectionHeader>}
      {props.children}
    </div>
  );
};

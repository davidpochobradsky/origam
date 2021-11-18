import React from "react";
import S from "gui/Components/Sidebar/SidebarSectionBody.module.scss";
import cx from "classnames";

export const SidebarSectionBody: React.FC<{ isActive?: boolean }> = props => (
  <div className={cx(S.root, { isActive: props.isActive })}>
    {props.children}
  </div>
);

import React from "react";
import {
  FilterSettingsComboBox,
  FilterSettingsComboBoxItem,
} from "gui/Components/ScreenElements/Table/FilterSettings/FilterSettingsComboBox";

import CS from "./FilterSettingsCommon.module.scss";
import { action, observable } from "mobx";
import { observer } from "mobx-react";
import produce from "immer";
import { FilterSetting } from "./FilterSetting";
import { T } from "utils/translation";

const OPERATORS = () =>
  [
    {
      human: <>{T("begins with", "filter_operator_begins_with")}</>,
      type: "starts",
    },
    {
      human: <>{T("not begins with", "filter_operator_not_begins_with")}</>,
      type: "nstarts",
    },
    { human: <>{T("ends with", "filter_operator_ends_with")}</>, type: "ends" },
    {
      human: <>{T("not ends with", "filter_operator_not_ends_with")}</>,
      type: "nends",
    },
    { human: <>{T("contains", "filter_operator_contains")}</>, type: "contains" },
    {
      human: <>{T("not contains", "filter_operator_not_contains")}</>,
      type: "ncontains",
    },
    { human: <>=</>, type: "eq" },
    { human: <>&ne;</>, type: "neq" },
    { human: <>{T("is null", "filter_operator_is_null")}</>, type: "null" },
    {
      human: <>{T("is not null", "filter_operator_not_is_null")}</>,
      type: "nnull",
    },
  ] as any[];

const OpCombo: React.FC<{
  setting: any;
  onChange: (newSetting: any) => void;
}> = (props) => {
  return (
    <FilterSettingsComboBox
      trigger={<>{(OPERATORS().find((op) => op.type === props.setting.type) || {}).human}</>}
    >
      {OPERATORS().map((op) => (
        <FilterSettingsComboBoxItem
          key={op.type}
          onClick={() =>
            props.onChange(
              produce(props.setting, (draft: any) => {
                draft.type = op.type;
              })
            )
          }
        >
          {op.human}
        </FilterSettingsComboBoxItem>
      ))}
    </FilterSettingsComboBox>
  );
};

const OpEditors: React.FC<{
  setting: any;
  onChange?: (newSetting: any) => void;
  onBlur?: (event: any) => void;
}> = (props) => {
  const { setting } = props;
  switch (setting.type) {
    case "eq":
    case "neq":
    case "starts":
    case "nstarts":
    case "ends":
    case "nends":
    case "contains":
    case "ncontains":
      return (
        <input
          className={CS.input}
          value={setting.val1}
          onChange={(event: any) =>
            props.onChange &&
            props.onChange(
              produce(setting, (draft: any) => {
                draft.val1 = event.target.value === "" ? undefined : event.target.value;
              })
            )
          }
          onBlur={props.onBlur}
        />
      );
    case "null":
    case "nnull":
    default:
      return null;
  }
};

@observer
export class FilterSettingsString extends React.Component<{
  setting?: any;
  onTriggerApplySetting?: (setting: any) => void;
}> {
  @observable.ref setting: FilterSetting = new FilterSetting(
    OPERATORS()[0].type,
    OPERATORS()[0].human
  );

  componentDidMount() {
    this.takeSettingFromProps();
  }

  componentDidUpdate() {
    this.takeSettingFromProps();
  }

  @action.bound takeSettingFromProps() {
    if (this.props.setting) {
      this.setting = this.props.setting;
    }
  }

  @action.bound
  handleBlur() {
    this.handleSettingChange();
  }

  @action.bound
  handleChange(newSetting: any) {
    this.setting = newSetting;
    this.handleSettingChange();
  }

  @action.bound
  handleSettingChange() {
    this.setting = produce(this.setting, (draft) => {
      draft.isComplete =
        draft.type === "null" || draft.type === "nnull" || draft.val1 !== undefined;
      draft.val2 = undefined;
    });
    this.props.onTriggerApplySetting && this.props.onTriggerApplySetting(this.setting);
  }

  render() {
    return (
      <>
        <OpCombo setting={this.setting} onChange={this.handleChange} />
        <OpEditors setting={this.setting} onChange={this.handleChange} onBlur={this.handleBlur} />
      </>
    );
  }
}

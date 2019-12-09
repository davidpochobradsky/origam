import React from "react";
import { FilterSettingsBoolean } from "./HeaderControls/FilterSettingsBoolean";
import { IProperty } from "../../../../../model/entities/types/IProperty";
import { FilterSettingsString } from "./HeaderControls/FilterSettingsString";
import { FilterSettingsDate } from "./HeaderControls/FilterSettingsDate";
import { observer } from "mobx-react-lite";
import { FilterSettingsNumber } from "./HeaderControls/FilterSettingsNumber";
import { FilterSettingsLookup } from "./HeaderControls/FilterSettingsLookup";
import { toJS, flow } from "mobx";
import { useContext } from "react";
import { MobXProviderContext } from "mobx-react";
import { onApplyFilterSetting } from "../../../../../model/actions-ui/DataView/TableView/onApplyFilterSetting";
import { getFilterSettingByProperty } from "model/selectors/DataView/getFilterSettingByProperty";
import { getDataTable } from "model/selectors/DataView/getDataTable";

export const FilterSettings: React.FC = observer(props => {
  const property = useContext(MobXProviderContext).property as IProperty;
  const dataTable = getDataTable(property);
  const setting = getFilterSettingByProperty(property, property.id);
  const handleApplyFilterSetting = onApplyFilterSetting(property);
  console.log(setting);

  switch (property.column) {
    case "Text":
      return (
        <FilterSettingsString
          onTriggerApplySetting={handleApplyFilterSetting}
          setting={setting as any}
        />
      );
    case "CheckBox":
      return (
        <FilterSettingsBoolean
          onTriggerApplySetting={handleApplyFilterSetting}
          setting={setting as any}
        />
      );
    case "Date":
      return (
        <FilterSettingsDate
          onTriggerApplySetting={handleApplyFilterSetting}
          setting={setting as any}
        />
      );
    case "Number":
      return (
        <FilterSettingsNumber
          onTriggerApplySetting={handleApplyFilterSetting}
          setting={setting as any}
        />
      );
    case "ComboBox":
      return (
        <FilterSettingsLookup
          setting={setting as any}
          onTriggerApplySetting={handleApplyFilterSetting}
          getOptions={flow(function*(searchTerm: string) {
            const allIds = new Set(dataTable.getAllValuesOfProp(property));
            yield property.lookup!.resolveList(allIds);
            console.log(dataTable.getAllValuesOfProp(property), allIds);
            return Array.from(allIds.values())
              .map(item => ({
                content: property.lookup!.getValue(item),
                value: item
              }))
              .filter(item =>
                item.content.toLowerCase().includes(searchTerm.toLowerCase())
              );
          })}
        />
      );
    default:
      return <>{property.column}</>;
  }
});

import { action, computed, observable, toJS } from "mobx";
import { MobXProviderContext, observer } from "mobx-react";
import { CancellablePromise } from "mobx/lib/api/flow";
import React, { useContext, useState } from "react";
import { Grid, GridCellProps } from "react-virtualized";
import Highlighter from "react-highlight-words";
import {
  FilterSettingsComboBox,
  FilterSettingsComboBoxItem,
} from "gui/Components/ScreenElements/Table/FilterSettings/FilterSettingsComboBox";
import S from "./FilterSettingsLookup.module.scss";
import CS from "./FilterSettingsCommon.module.scss";
import { IFilterSetting } from "model/entities/types/IFilterSetting";
import { rowHeight } from "gui/Components/ScreenElements/Table/TableRendering/cells/cellsCommon";
import {
  CtxDropdownEditor,
  DropdownEditor,
  DropdownEditorSetup,
  IDropdownEditorContext,
} from "modules/Editors/DropdownEditor/DropdownEditor";
import { TagInputEditor } from "gui/Components/ScreenElements/Editors/TagInputEditor";
import { IDropdownEditorApi } from "modules/Editors/DropdownEditor/DropdownEditorApi";
import { IDropdownEditorData } from "modules/Editors/DropdownEditor/DropdownEditorData";
import {
  DropdownColumnDrivers,
  DropdownDataTable,
} from "modules/Editors/DropdownEditor/DropdownTableModel";
import { DropdownEditorLookupListCache } from "modules/Editors/DropdownEditor/DropdownEditorLookupListCache";
import { DropdownEditorBehavior } from "modules/Editors/DropdownEditor/DropdownEditorBehavior";
import { TextCellDriver } from "modules/Editors/DropdownEditor/Cells/TextCellDriver";
import { DefaultHeaderCellDriver } from "modules/Editors/DropdownEditor/Cells/HeaderCell";
import { ILookup } from "model/entities/types/ILookup";
import { Operator } from "gui/Components/ScreenElements/Table/FilterSettings/HeaderControls/Operator";
import { getGroupingConfiguration } from "model/selectors/TablePanelView/getGroupingConfiguration";
import { IProperty } from "model/entities/types/IProperty";

const OPERATORS = [
    Operator.in,
    Operator.notIn,
    Operator.startsWith,
    Operator.notStartsWith,
    Operator.contains,
    Operator.notContains,
    Operator.isNull,
    Operator.isNotNull
  ];

const OpCombo: React.FC<{
  setting: any;
}> = observer((props) => {
  return (
    <FilterSettingsComboBox
      trigger={<>{(OPERATORS.find((op) => op.type === props.setting.type) || {}).caption}</>}
    >
      {OPERATORS.map((op) => (
        <FilterSettingsComboBoxItem
          key={op.type}
          onClick={() => {
            props.setting.type = op.type;
            props.setting.isComplete = op.type === "null" || op.type === "nnull";
            props.setting.val1 = undefined;
            props.setting.val2 = undefined;
          }}
        >
          {op.caption}
        </FilterSettingsComboBoxItem>
      ))}
    </FilterSettingsComboBox>
  );
});

export interface ITagEditorItem {
  text: string;
  value: string;
}

@observer
class OpEditors extends React.Component<{
  setting: IFilterSetting;
  getOptions: (searchTerm: string) => CancellablePromise<Array<any>>;
  lookup: ILookup;
  property: IProperty;
}> {

  @action.bound handleSelectedItemsChange(items: Array<any>) {
    this.props.setting.val1 = [...items];
    this.props.setting.val2 = undefined;
    this.props.setting.isComplete = this.props.setting.val1 !== undefined && this.props.setting.val1.length > 0;
  }

  @action.bound handleTermChange(event: any) {
    this.props.setting.val1 = undefined;
    this.props.setting.val2 = event.target.value;
    this.props.setting.isComplete = !!this.props.setting.val2;
  }

  render() {
    const { setting } = this.props;
    switch (setting?.type) {
      case "in":
      case "nin":
        return (
          <FilterBuildDropdownEditor
            lookup={this.props.lookup}
            property={this.props.property}
            getOptions={this.props.getOptions}
            onChange={this.handleSelectedItemsChange}
            values={this.props.setting.val1 ?? []}
          />
        );
      case "starts":
      case "nstarts":
      case "contains":
      case "ncontains":
         return (
          <input
            value={this.props.setting.val2 ?? ""}
            className={CS.input}
            onChange={this.handleTermChange} 
          />
        );
      case "null":
      case "nnull":
      default:
        return null;
    }
  }
}

@observer
export class FilterSettingsLookup extends React.Component<{
  getOptions: (searchTerm: string) => CancellablePromise<Array<any>>;
  lookup: ILookup;
  property: IProperty;
  setting: IFilterSetting;
}> {
  static get defaultSettings(){
    return new LookupFilterSetting(OPERATORS[0].type)
  }

  render() {
    const setting = this.props.setting;
    return (
      <>
        <OpCombo setting={setting} />
        <OpEditors
          setting={setting}
          getOptions={this.props.getOptions}
          lookup={this.props.lookup}
          property={this.props.property}
        />
      </>
    );
  }
}

export class LookupFilterSetting implements IFilterSetting {
  @observable type: string;
  @observable val1?: any; // used for "in" operator ... string[]
  @observable val2?: any; // used for "contains" operator ... string
  isComplete: boolean;

  private _lookupId: string | undefined;

  operatorsRequiringLookup = [ "contains", "ncontains", "starts", "nstarts", "ends", "nends"]

  public get lookupId(): string | undefined {
    if(this.operatorsRequiringLookup.includes(this.type)){
      return  this._lookupId;
    }
    return undefined;
  }
  public set lookupId(value: string | undefined) {
    this._lookupId = value;
  }

  get filterValue1() {
    if (!this.val1) {
      return this.val1;
    }
    switch (this.type) {
      case "contain":
      case "ncontain":
        return this.val2;
      case "in":
      case "nin":
        return this.val1;
      default:
        return undefined;
    }
  }

  get filterValue2() {
    return this.val2;
  }


  get val1ServerForm(){
    return this.val1 ? this.val1.join(",") : this.val1;
  }

  get val2ServerForm(){
    return this.type === "between" || this.type === "nbetween" 
      ? this.val2 
      : undefined;
  }

  constructor(type: string, isComplete=false, val1?:string, val2?: any) {
    this.type = type;
    this.isComplete = isComplete;
    if(Array.isArray(val1)){
      this.val1 = [... new Set(val1)];
    }
    else if(val1 !== undefined && val1 !== null){
      this.val1 = [... new Set(val1.split(","))];
    }
    this.val2 = val2 ?? undefined;
  }
}

export function FilterBuildDropdownEditor(props: {
  lookup: ILookup;
  property: IProperty;
  getOptions: (searchTerm: string) => CancellablePromise<Array<any>>;
  onChange(selectedItems: Array<any>): void;
  values: Array<any>;
}) {
  const mobxContext = useContext(MobXProviderContext);


  const workbench = mobxContext.workbench;
  const { lookupListCache } = workbench;

  const [dropdownEditorInfrastructure] = useState<IDropdownEditorContext>(() => {
    const dropdownEditorApi: IDropdownEditorApi = new DropDownApi(props.getOptions);
    const dropdownEditorData: IDropdownEditorData = new FilterEditorData(props.onChange);

    const dropdownEditorDataTable = new DropdownDataTable(
      () => dropdownEditorSetup,
      dropdownEditorData
    );
    const dropdownEditorLookupListCache = new DropdownEditorLookupListCache(
      () => dropdownEditorSetup,
      lookupListCache
    );
    const dropdownEditorBehavior = new DropdownEditorBehavior(
      dropdownEditorApi,
      dropdownEditorData,
      dropdownEditorDataTable,
      () => dropdownEditorSetup,
      dropdownEditorLookupListCache,
      false
    );


    const drivers = new DropdownColumnDrivers();

    let identifierIndex = 0;
    const columnNameToIndex = new Map<string, number>([
      [props.property.identifier!, identifierIndex],
    ]);
    const visibleColumnNames: string[] = [];

    columnNameToIndex.set(props.property.name, 1);
    visibleColumnNames.push(props.property.name);

    const bodyCellDriver = new TextCellDriver(1, dropdownEditorDataTable, dropdownEditorBehavior);

    drivers.drivers.push({
      headerCellDriver: new DefaultHeaderCellDriver(props.property.name),
      bodyCellDriver,
    });

    const showUniqueValues = true;

    const cached =  getGroupingConfiguration(props.property).isGrouping 
      ? false 
      : props.property.lookup?.cached!

    const dropdownEditorSetup = new DropdownEditorSetup(
      props.property.id,
      props.lookup.lookupId,
      [],
      visibleColumnNames,
      columnNameToIndex,
      showUniqueValues,
      props.property.identifier!,
      identifierIndex,
      props.property.parameters,
      props.property.lookup?.dropDownType!,
      cached,
      !props.property.lookup?.searchByFirstColumnOnly
    );

    return {
      behavior: dropdownEditorBehavior,
      editorData: dropdownEditorData,
      columnDrivers: drivers,
      editorDataTable: dropdownEditorDataTable,
      setup: dropdownEditorSetup
    };
  });

  function onItemRemoved(event: any, item: any) {
    props.onChange(props.values);
  }

  const value = props.values;
  return (
    <CtxDropdownEditor.Provider value={dropdownEditorInfrastructure}>
      <DropdownEditor
        editor={
          <TagInputEditor
            customInputClass={S.tagInput}
            value={value}
            isReadOnly={false}
            isInvalid={false}
            isFocused={false}
            refocuser={undefined}
            onChange={onItemRemoved}
            onClick={undefined}
          />
        }
      />
    </CtxDropdownEditor.Provider>
  );
}

export class FilterEditorData implements IDropdownEditorData {
  constructor(private onChange: (selectedItems: Array<any>) => void) {}

  @computed get value(): string | string[] | null {
    return this._value;
  }

  @observable
  _value: any[] = [];

  @computed get text(): string {
    return "";
  }

  get isResolving() {
    return false;
  }

  @action.bound chooseNewValue(value: any) {
    if (value !== null && !this._value.includes(value)) {
      this._value = [...this._value, value];
      this.onChange(this._value);
    }
  }

  get idsInEditor() {
    return this._value as string[];
  }

  remove(valueToRemove: any): void {
    const index = this._value.indexOf(valueToRemove)
    if(index > -1){
      this._value.splice(index, 1);
    }
  }
}

class DropDownApi implements IDropdownEditorApi {
  constructor(private getOptions: (searchTerm: string) => CancellablePromise<Array<any>>) {}

  *getLookupList(searchTerm: string): Generator {
    return yield this.getOptions("");
  }
}

import { inject, observer, Observer, Provider } from "mobx-react";
import React, { useContext } from "react";
import { IDataView } from "../../../../model/entities/types/IDataView";
import { getDataTable } from "../../../../model/selectors/DataView/getDataTable";
import { getDataViewPropertyById } from "../../../../model/selectors/DataView/getDataViewPropertyById";
import { getSelectedRow } from "../../../../model/selectors/DataView/getSelectedRow";
import { findStrings } from "../../../../xmlInterpreters/screenXml";

import { FormRoot } from "./FormRoot";
import { getSelectedRowId } from "model/selectors/TablePanelView/getSelectedRowId";
import { getRowStateRowBgColor } from "model/selectors/RowState/getRowStateRowBgColor";
import { FormField } from "gui/Components/Form/FormField";
import { FormSection } from "gui/Components/Form/FormSection";
import { FormLabel } from "gui/Components/Form/FormLabel";
import { RadioButton } from "gui/Components/Form/RadioButton";
import { getDataSourceFieldByName } from "../../../../model/selectors/DataSources/getDataSourceFieldByName";
import { getFormScreenLifecycle } from "../../../../model/selectors/FormScreen/getFormScreenLifecycle";
import { flow } from "mobx";
import { CheckBox } from "../../../Components/Form/CheckBox";
import { isReadOnly } from "../../../../model/selectors/RowState/isReadOnly";
import { DomEvent } from "leaflet";
import { getRowStateAllowRead } from "model/selectors/RowState/getRowStateAllowRead";
import { getRowStateMayCauseFlicker } from "model/selectors/RowState/getRowStateMayCauseFlicker";
import { CtxPanelVisibility } from "gui/contexts/GUIContexts";
import { getRowStateForegroundColor } from "model/selectors/RowState/getRowStateForegroundColor";


@inject(({ dataView }) => {
  return { dataView, xmlFormRootObject: dataView.formViewUI };
})
@observer
export class FormBuilder extends React.Component<{
  xmlFormRootObject?: any;
  dataView?: IDataView;
}> {
  static contextType = CtxPanelVisibility

  onKeyDown(event: any) {
    if (event.key === "Tab") {
      DomEvent.preventDefault(event);
      if (event.shiftKey) {
        this.props.dataView!.focusManager.focusPrevious(document.activeElement);
      } else {
        this.props.dataView!.focusManager.focusNext(document.activeElement);
      }
      return;
    }
  }

  buildForm() {
    const self = this;
    const row = getSelectedRow(this.props.dataView);
    const rowId = getSelectedRowId(this.props.dataView);
    const dataTable = getDataTable(this.props.dataView);
    let backgroundColor: string | undefined;
    let foreGroundColor: string | undefined;
    if (row && rowId) {
      backgroundColor = getRowStateRowBgColor(self.props.dataView, rowId);
      foreGroundColor = getRowStateForegroundColor(
        self.props.dataView,
        rowId || ""
      );
    }
    const focusManager = self.props.dataView!.focusManager;

    function recursive(xfo: any) {
      if (xfo.name === "FormRoot") {
        return (
          <FormRoot key={xfo.$iid} style={{ backgroundColor }}>
            {xfo.elements.map((child: any) => recursive(child))}
          </FormRoot>
        );
      } else if (xfo.name === "FormElement" && xfo.attributes.Type === "FormSection") {
        return (
          <FormSection
            key={xfo.$iid}
            width={parseInt(xfo.attributes.Width, 10)}
            height={parseInt(xfo.attributes.Height, 10)}
            left={parseInt(xfo.attributes.X, 10)}
            top={parseInt(xfo.attributes.Y, 10)}
            title={xfo.attributes.Title}
            backgroundColor={backgroundColor}
            foreGroundColor={foreGroundColor}
          >
            {xfo.elements.map((child: any) => recursive(child))}
          </FormSection>
        );
      } else if (xfo.name === "FormElement" && xfo.attributes.Type === "Label") {
        return (
          <FormLabel
            key={xfo.$iid}
            title={xfo.attributes.Title}
            left={+xfo.attributes.X}
            top={+xfo.attributes.Y}
            width={+xfo.attributes.Width}
            height={+xfo.attributes.Height}
            foregroundColor={foreGroundColor}
          />
        );
      } else if (xfo.name === "Control" && xfo.attributes.Column === "RadioButton") {
        const sourceField = getDataSourceFieldByName(self.props.dataView, xfo.attributes.Id);

        const checked = row
          ? String(dataTable.getCellValueByDataSourceField(row, sourceField!)) === xfo.attributes.Value
          : false;
          
        return (
          <RadioButton
            key={xfo.$iid}
            caption={xfo.attributes.Name}
            left={+xfo.attributes.X}
            top={+xfo.attributes.Y}
            width={+xfo.attributes.Width}
            height={+xfo.attributes.Height}
            name={xfo.attributes.Id}
            value={xfo.attributes.Value}
            checked={checked}
            onKeyDown={(event) => self.onKeyDown(event)}
            subscribeToFocusManager={(radioInput) =>
              focusManager.subscribe(radioInput, xfo.attributes.Id, xfo.attributes.TabIndex)
            }
            labelColor={foreGroundColor}
            onClick={() => self?.props?.dataView?.focusManager.stopAutoFocus()}
            onSelected={(value) => {
              const formScreenLifecycle = getFormScreenLifecycle(self.props.dataView);
              flow(function* () {
                yield* formScreenLifecycle.updateRadioButtonValue(
                  self.props.dataView!,
                  row,
                  xfo.attributes.Id,
                  value
                );
              })();
            }}
          />
        );
      } else if (xfo.name === "PropertyNames") {
        const propertyNames = findStrings(xfo);
        return propertyNames.map((propertyId) => {
          return (
            <Observer key={propertyId}>
              {() => {
                let property = getDataViewPropertyById(self.props.dataView, propertyId);
                if (row && property?.column === "Polymorph") {
                  property = property.getPolymophicProperty(row);
                }
                let value;
                let textualValue = value;
                if (row && property) {
                  value = dataTable.getCellValue(row, property);
                  if (property.isLookup) {
                    textualValue = dataTable.getCellText(row, property);
                  }
                }
                if (!property) {
                  return <></>;
                }

                const isHidden =
                  (!getRowStateAllowRead(property, rowId || "", property.id) ||
                  getRowStateMayCauseFlicker(property)) && !!row;

                if (property.column === "CheckBox") {
                  return (
                    <Provider property={property}>
                      <CheckBox
                        isHidden={isHidden}
                        checked={value}
                        readOnly={!row || isReadOnly(property, rowId)}
                        onKeyDown={(event) => self.onKeyDown(event)}
                        subscribeToFocusManager={(radioInput) =>
                          focusManager.subscribe(radioInput, property!.id, property!.tabIndex)
                        }
                        onClick={() => self?.props?.dataView?.focusManager.stopAutoFocus()}
                        labelColor={foreGroundColor}
                      />
                    </Provider>
                  );
                }

                return (
                  <Provider property={property} key={property.id}>
                    <FormField
                      isHidden={isHidden}
                      caption={property.name}
                      hideCaption={property.column === "Image"}
                      captionLength={property.captionLength}
                      captionPosition={property.captionPosition}
                      captionColor={foreGroundColor}
                      dock={property.dock}
                      height={property.height}
                      width={property.width}
                      left={property.x}
                      top={property.y}
                      toolTip={property.toolTip}
                      value={value}
                      isRichText={property.isRichText}
                      textualValue={textualValue}
                      xmlNode={property.xmlNode}
                      backgroundColor={backgroundColor}
                    />
                  </Provider>
                );
              }}
            </Observer>
          );
        });
      } else {
        return xfo.elements.map((child: any) => recursive(child));
      }
    }
    const form = recursive(this.props.xmlFormRootObject);
    if (this.props.dataView?.isFirst && this.context.isVisible) {
      focusManager.autoFocus();
    }
    return form;
  }

  render() {
    return this.buildForm();
  }
}
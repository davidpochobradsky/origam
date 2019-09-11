import { inject, observer, Observer, Provider } from "mobx-react";
import React from "react";
import { IDataView } from "../../../../model/entities/types/IDataView";
import { getDataTable } from "../../../../model/selectors/DataView/getDataTable";
import { getDataViewPropertyById } from "../../../../model/selectors/DataView/getDataViewPropertyById";
import { getSelectedRow } from "../../../../model/selectors/DataView/getSelectedRow";
import { findStrings } from "../../../../xmlInterpreters/screenXml";
import { FormSection } from "../../../Components/ScreenElements/FormSection";
import { FormField } from "./FormField";
import { FormRoot } from "./FormRoot";
import { FormViewEditor } from "./FormViewEditor";

let keyGen = 1;

@inject(({ dataView }) => {
  return { dataView, xmlFormRootObject: dataView.formViewUI };
})
@observer
export class FormBuilder extends React.Component<{
  xmlFormRootObject?: any;
  dataView?: IDataView;
}> {
  buildForm() {
    const self = this;
    const row = getSelectedRow(this.props.dataView);
    const dataTable = getDataTable(this.props.dataView);

    function recursive(xfo: any) {
      if (xfo.name === "FormRoot") {
        return (
          <FormRoot key={keyGen++}>
            {xfo.elements.map((child: any) => recursive(child))}
          </FormRoot>
        );
      } else if (
        xfo.name === "FormElement" &&
        xfo.attributes.Type === "FormSection"
      ) {
        return (
          <FormSection
            key={keyGen++}
            width={parseInt(xfo.attributes.Width, 10)}
            height={parseInt(xfo.attributes.Height, 10)}
            x={parseInt(xfo.attributes.X, 10)}
            y={parseInt(xfo.attributes.Y, 10)}
            title={xfo.attributes.Title}
          >
            {xfo.elements.map((child: any) => recursive(child))}
          </FormSection>
        );
      } else if (xfo.name === "PropertyNames") {
        const propertyNames = findStrings(xfo);
        return propertyNames.map(propertyId => {
          return (
            <Observer key={keyGen++}>
              {() => {
                const property = getDataViewPropertyById(
                  self.props.dataView,
                  propertyId
                );
                let value;
                let textualValue = value;
                if (row && property) {
                  value = dataTable.getCellValue(row, property);
                  if (property.isLookup) {
                    textualValue = dataTable.getCellText(row, property);
                  }
                }

                return property ? (
                  <Provider property={property}>
                    <FormField
                      Id={property.id}
                      Name={property.name}
                      CaptionLength={property.captionLength}
                      CaptionPosition={property.captionPosition}
                      Column={property.column}
                      Entity={property.entity}
                      Height={property.height}
                      Width={property.width}
                      X={property.x}
                      Y={property.y}
                    >
                      <FormViewEditor
                        value={value}
                        textualValue={textualValue}
                      />
                    </FormField>
                  </Provider>
                ) : (
                  <></>
                );
              }}
            </Observer>
          );
        });
      } else {
        return xfo.elements.map((child: any) => recursive(child));
      }
    }
    return recursive(this.props.xmlFormRootObject);
  }

  render() {
    // debugger
    return this.buildForm();
  }
}

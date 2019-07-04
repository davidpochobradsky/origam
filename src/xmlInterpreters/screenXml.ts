import { findStopping } from "./xmlUtils";
import { FormScreen } from "../model/FormScreen";
import { DataSource } from "../model/DataSource";
import { DataSourceField } from "../model/DataSourceField";
import { DataView } from "../model/DataView";
import { IPanelViewType } from "../model/types/IPanelViewType";
import { Property } from "../model/Property";
import { DropDownColumn } from "../model/DropDownColumn";
import { IComponentBinding } from "../model/types/IComponentBinding";
import {
  ComponentBindingPair,
  ComponentBinding
} from "../model/ComponentBinding";
import { IFormScreenLifecycle } from "../model/types/IFormScreenLifecycle";

export const findUIRoot = (node: any) =>
  findStopping(node, n => n.name === "UIRoot")[0];

export const findUIChildren = (node: any) =>
  findStopping(node, n => n.parent.name === "UIChildren");

export const findBoxes = (node: any) =>
  findStopping(node, n => n.attributes && n.attributes.Type === "Box");

export function interpretScreenXml(
  screenDoc: any,
  formScreenLifecycle: IFormScreenLifecycle
) {
  console.log(screenDoc);

  const dataSourcesXml = findStopping(
    screenDoc,
    n => n.name === "DataSources"
  )[0];

  const windowXml = findStopping(screenDoc, n => n.name === "Window")[0];

  const dataViews = findStopping(
    screenDoc,
    n =>
      (n.name === "UIElement" || n.name === "UIRoot") &&
      n.attributes.Type === "Grid"
  );

  function panelViewFromNumber(pvn: number) {
    switch (pvn) {
      case 0:
      default:
        return IPanelViewType.Table;
      case 1:
        return IPanelViewType.Form;
    }
  }

  const xmlComponentBindings = findStopping(
    screenDoc,
    n => n.name === "Binding" && n.parent.name === "ComponentBindings"
  );

  const componentBindings: IComponentBinding[] = [];

  for (let xmlBinding of xmlComponentBindings) {
    let existingBinding = componentBindings.find(
      item =>
        item.parentId === xmlBinding.attributes.ParentId &&
        item.childId === xmlBinding.attributes.ChildId
    );
    const componentBindingPair = new ComponentBindingPair({
      parentPropertyId: xmlBinding.attributes.ParentProperty,
      childPropertyId: xmlBinding.attributes.ChildProperty
    });
    if (existingBinding) {
      existingBinding.bindingPairs.push(componentBindingPair);
    } else {
      componentBindings.push(
        new ComponentBinding({
          parentId: xmlBinding.attributes.ParentId,
          childId: xmlBinding.attributes.ChildId,
          parentEntity: xmlBinding.attributes.ParentEntity,
          childEntity: xmlBinding.attributes.ChildEntity,
          bindingPairs: [componentBindingPair],
          childPropertyType: xmlBinding.attributes.ChildPropertyType
        })
      );
    }
  }

  const scr = new FormScreen({
    title: windowXml.attributes.Title,
    menuId: windowXml.attributes.MenuId,
    openingOrder: 0,
    showInfoPanel: windowXml.attributes.ShowInfoPanel === "true",
    autoRefreshInterval: parseInt(windowXml.attributes.AutoRefreshInterval, 10),
    cacheOnClient: windowXml.attributes.CacheOnClient === "true",
    autoSaveOnListRecordChange:
      windowXml.attributes.AutoSaveOnListRecordChange === "true",
    requestSaveAfterUpdate:
      windowXml.attributes.RequestSaveAfterUpdate === "true",
    screenUI: screenDoc,
    formScreenLifecycle,
    dataSources: dataSourcesXml.elements.map((dataSource: any) => {
      return new DataSource({
        entity: dataSource.attributes.Entity,
        identifier: dataSource.attributes.Identifier,
        lookupCacheKey: dataSource.attributes.LookupCacheKey,
        fields: findStopping(dataSource, n => n.name === "Field").map(field => {
          return new DataSourceField({
            index: parseInt(field.attributes.Index, 10),
            name: field.attributes.Name
          });
        })
      });
    }),

    dataViews: dataViews.map(dataView => {
      return new DataView({
        id: dataView.attributes.Id,
        modelInstanceId: dataView.attributes.ModelInstanceId,
        name: dataView.attributes.Name,
        modelId: dataView.attributes.ModelId,
        defaultPanelView: panelViewFromNumber(
          parseInt(dataView.attributes.DefaultPanelView)
        ),
        isHeadless: dataView.attributes.IsHeadless === "true",
        disableActionButtons:
          dataView.attributes.DisableActionButtons === "true",
        showAddButton: dataView.attributes.ShowAddButton === "true",
        showDeleteButton: dataView.attributes.ShowDeleteButton === "true",
        showSelectionCheckboxes:
          dataView.attributes.ShowSelectionCheckboxes === "true",
        isGridHeightDynamic: dataView.attributes.IsGridHeightDynamic === "true",
        selectionMember: dataView.attributes.SelectionMember,
        orderMember: dataView.attributes.OrderMember,
        isDraggingEnabled: dataView.attributes.IsDraggingEnabled === "true",
        entity: dataView.attributes.Entity,
        dataMember: dataView.attributes.DataMember,
        isRootGrid: dataView.attributes.IsRootGrid === "true",
        isRootEntity: dataView.attributes.IsRootEntity === "true",
        isPreloaded: dataView.attributes.IsPreloaded === "true",
        requestDataAfterSelectionChange:
          dataView.attributes.RequestDataAfterSelectionChange === "true",
        confirmSelectionChange:
          dataView.attributes.ConfirmSelectionChange === "true",

        properties: findStopping(dataView, n => n.name === "Property").map(
          property => {
            return new Property({
              id: property.attributes.Id,
              modelInstanceId: property.attributes.ModelInstanceId || "",
              name: property.attributes.Name,
              readOnly: property.attributes.ReadOnly === "true",
              x: parseInt(property.attributes.X, 10),
              y: parseInt(property.attributes.Y, 10),
              width: parseInt(property.attributes.Width, 10),
              height: parseInt(property.attributes.Height, 10),
              captionLength: parseInt(property.attributes.CaptionLength, 10),
              captionPosition: property.attributes.CaptionPosition,
              entity: property.attributes.Entity,
              column: property.attributes.Column,
              dock: property.attributes.Dock,
              multiline: property.attributes.Multiline === "true",
              isPassword: property.attributes.IsPassword === "true",
              isRichText: property.attributes.IsRichText === "true",
              maxLength: parseInt(property.attributes.MaxLength, 10),

              dropDownShowUniqueValues:
                property.attributes.DropDownShowUniqueValues === "true",
              lookupId: property.attributes.LookupId,
              identifier: property.attributes.Identifier,
              identifierIndex: parseInt(
                property.attributes.IdentifierIndex,
                10
              ),
              dropDownType: property.attributes.DropDownType,
              cached: property.attributes.Cached === "true",
              searchByFirstColumnOnly:
                property.attributes.SearchByFirstColumnOnly === "true",
              allowReturnToForm:
                property.attributes.AllowReturnToForm === "true",
              isTree: property.attributes.IsTree === "true",

              dropDownColumns: findStopping(
                property,
                n => n.name === "Property"
              ).map(ddProperty => {
                return new DropDownColumn({
                  id: ddProperty.attributes.Id,
                  name: ddProperty.attributes.Name,
                  column: ddProperty.attributes.Column,
                  entity: ddProperty.attributes.Entity,
                  index: parseInt(ddProperty.attributes.Index, 10)
                });
              })
            });
          }
        )
      });
    }),
    componentBindings
  });
  return scr;
}

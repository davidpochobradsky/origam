import {IProperty} from "model/entities/types/IProperty";
import {IColumnConfiguration} from "model/entities/TablePanelView/types/IConfigurationManager";
import {ConfigurationManager} from "model/entities/TablePanelView/configurationManager";
import {findStopping} from "xmlInterpreters/xmlUtils";
import {parseAggregationType, tryParseAggregationType} from "model/entities/types/AggregationType";
import {fixColumnWidth} from "xmlInterpreters/screenXml";
import {TableConfiguration} from "model/entities/TablePanelView/tableConfiguration";
import {TableColumnConfiguration} from "model/entities/TablePanelView/tableColumnConfiguration";

export function createConfigurationManager(configurationNodes: any, properties: IProperty[]) {

  if (configurationNodes.length === 0) {
    return new ConfigurationManager(
      [], TableConfiguration.createEmpty(properties));
  } else if (configurationNodes.length > 1) {
    throw new Error("Can not process more than one configuration node")
  }

  const tableConfigurationNodes = findStopping(configurationNodes[0], (n) => n.name === "tableConfigurations")?.[0]?.elements;
  if(!tableConfigurationNodes){
    return new ConfigurationManager(
      [], TableConfiguration.createEmpty(properties));
  }
  const tableConfigurations = tableConfigurationNodes.map((tableConfigNode: any) =>
    TableConfiguration.create(
      {
        name: tableConfigNode.attributes.name,
        isActive: tableConfigNode.attributes.isActive === "true",
        fixedColumnCount: parseIntOrZero(tableConfigNode.attributes.fixedColumnCount),
        columnConfigurations: tableConfigNode.elements
          .map((columnConfigNode: any) => parseColumnConfigurationNode(columnConfigNode, properties))
          .filter((columnConfiguration: IColumnConfiguration | undefined) => columnConfiguration),
      }));

  const defaultTableConfiguration = tableConfigurations.find((tableConfig: TableConfiguration) => tableConfig.name === "")
          ?? TableConfiguration.createEmpty(properties);

  return new ConfigurationManager(
    tableConfigurations
      .filter((tableConfig: TableConfiguration) => tableConfig !== defaultTableConfiguration),
    defaultTableConfiguration
  );
}

function parseColumnConfigurationNode(columnConfigNode: any, properties: IProperty[]){
  const property = properties.find((prop) => prop.id === columnConfigNode.attributes.propertyId);
  if (!property) {
    return undefined;
  }
  const tableConfiguration = new TableColumnConfiguration(property.id)
  tableConfiguration.width = fixColumnWidth(parseInt(columnConfigNode.attributes.width));

  if (columnConfigNode.attributes.isVisible === "false" || tableConfiguration.width < 0) {
    tableConfiguration.isVisible = false;
  }
  tableConfiguration.aggregationType = tryParseAggregationType(columnConfigNode.attributes.aggregationType);

  if (!property?.isLookupColumn) {
    tableConfiguration.groupingIndex = parseIntOrZero(columnConfigNode.attributes.groupingIndex);
    tableConfiguration.timeGroupingUnit = isNaN(parseInt(columnConfigNode.attributes.groupingUnit))
      ? undefined
      : parseInt(columnConfigNode.attributes.groupingUnit)
  }
  return tableConfiguration;
}

function parseIntOrZero(value: string): number {
  const intValue = parseInt(value, 10);
  return isNaN(intValue) ? 0 : intValue;
}
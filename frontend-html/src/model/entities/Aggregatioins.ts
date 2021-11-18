import {AggregationType, parseAggregationType} from "model/entities/types/AggregationType";
import {IDataView} from "model/entities/types/IDataView";
import {IAggregationInfo} from "model/entities/types/IAggregationInfo";
import {getProperties} from "model/selectors/DataView/getProperties";
import {IAggregation} from "model/entities/types/IAggregation";
import { IProperty } from "./types/IProperty";
import { formatNumber } from "./NumberFormating";

export function parseAggregations(objectArray: any[] | undefined): IAggregation[] | undefined{
  if(!objectArray) return undefined;
  return objectArray.map(object =>
  {
    return {
      columnId: object["column"],
      type: parseAggregationType(object["type"]),
      value: object["value"]
    }
  });
}

export function aggregationToString(aggregation: IAggregation, property: IProperty){
  function round(value: number){
    return Math.round(value * 100)/100
  }
  const formattedValue = formatNumber(
    property.customNumericFormat,
    property.entity ?? '',
    round(aggregation.value));
  
  if(aggregation.type === AggregationType.SUM){
    return "Σ " + formattedValue
  }
  return aggregation.type + ": " + formattedValue
}

export function calcAggregations(dataView: IDataView, aggregationInfos: IAggregationInfo[]): IAggregation[] {
  return aggregationInfos.map(aggregationInfo => {
    return {
      columnId: aggregationInfo.ColumnName,
      type: aggregationInfo.AggregationType,
      value: calculateAggregationValue(dataView, aggregationInfo),
    }
  })
}

function calculateAggregationValue(dataView: IDataView, aggregationInfo: IAggregationInfo){
  const properties = getProperties(dataView);
  const property = properties.find(prop => prop.id === aggregationInfo.ColumnName)!
  const values = dataView.dataTable.rows
    .map(row => dataView.dataTable.getCellValue(row, property) as number);

  if(values.length === 0){
    return 0;
  }
  switch (aggregationInfo.AggregationType) {
    case AggregationType.SUM:
      return values.reduce((a, b) => a + b, 0);
    case AggregationType.AVG:
      return values.reduce((a, b) => a + b, 0) / values.length;
    case AggregationType.MAX:
      return Math.max(...values);
    case AggregationType.MIN:
      return Math.min(...values);
    case AggregationType.COUNT:
      return values.length;
  }
}

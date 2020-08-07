import {getFilterConfiguration} from "./getFilterConfiguration";
import {filterToFilterItem, joinWithAND, toFilterItem} from "../../entities/OrigamApiHelpers";
import {getDataView} from "./getDataView";

export function getUserFilters(ctx: any){
  const dataView = getDataView(ctx);
  const filterConfiguration = getFilterConfiguration(dataView);
  const filterList = filterConfiguration.filters
    .filter(filter => filter.setting.isComplete)
    .map(filter => filterToFilterItem(filter));
  return joinWithAND(filterList);
}
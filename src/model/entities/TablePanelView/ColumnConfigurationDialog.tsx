import React from "react";
import {action, computed} from "mobx";
import {getDialogStack} from "../../selectors/DialogStack/getDialogStack";
import {IColumnConfigurationDialog} from "./types/IColumnConfigurationDialog";
import {ColumnsDialog,} from "gui/Components/Dialogs/ColumnsDialog";
import {onColumnConfigurationSubmit} from "model/actions-ui/ColumnConfigurationDialog/onColumnConfigurationSubmit";
import {getGroupingConfiguration} from "model/selectors/TablePanelView/getGroupingConfiguration";
import {isLazyLoading} from "model/selectors/isLazyLoading";
import {ITableConfiguration} from "./types/IConfigurationManager";
import {runInFlowWithHandler} from "utils/runInFlowWithHandler";
import {getConfigurationManager} from "model/selectors/TablePanelView/getConfigurationManager";
import {NewConfigurationDialog} from "gui/Components/Dialogs/NewConfigurationDialog";
import {getFormScreenLifecycle} from "model/selectors/FormScreen/getFormScreenLifecycle";
import {getTablePanelView} from "model/selectors/TablePanelView/getTablePanelView";

export interface IColumnOptions {
  canGroup: boolean;
  canAggregate: boolean;
  entity: string;
  name: string;
}

export class ColumnConfigurationDialog implements IColumnConfigurationDialog {
  getColumnOptions(){
    const groupingConf = getGroupingConfiguration(this);
    const groupingOnClient = !isLazyLoading(this);
    const activeTableConfiguration = getConfigurationManager(this).activeTableConfiguration;
    const optionsMap = new Map<string, IColumnOptions>()

    for (let columnConfiguration of activeTableConfiguration.columnConfigurations) {
      const property = this.tablePanelView.allTableProperties
        .find(prop => prop.id === columnConfiguration.propertyId)!;
      optionsMap.set(
        property.id,
        {
          canGroup: groupingOnClient ||
            (!property.isAggregatedColumn && !property.isLookupColumn && property.column !== "TagInput"),
          canAggregate: groupingOnClient ||
            (!property.isAggregatedColumn && !property.isLookupColumn && property.column !== "TagInput"),
          entity: property.entity,
          name: property.name,
        })
    }
    return optionsMap;
  }

  @computed get columnsConfiguration() {
    return getConfigurationManager(this).activeTableConfiguration;
  }

  dialogKey = "";
  dialogId = 0;

  @action.bound
  onColumnConfClick(event: any): void {
    this.dialogKey = `ColumnConfigurationDialog@${this.dialogId++}`;
    getDialogStack(this).pushDialog(
      this.dialogKey,
      <ColumnsDialog
        columnOptions={this.getColumnOptions()}
        configuration={this.columnsConfiguration}
        onCancelClick={this.onColumnConfCancel}
        onSaveAsClick={this.onSaveAsClick}
        onCloseClick={this.onColumnConfCancel}
        onOkClick={onColumnConfigurationSubmit(this.tablePanelView)}
      />
    );
  }

  @action.bound onColumnConfCancel(event: any): void {
    getDialogStack(this).closeDialog(this.dialogKey);
  }

  @action.bound onSaveAsClick(event: any, configuration: ITableConfiguration): void {
    this.ApplyConfiguration(configuration);

    const closeDialog = getDialogStack(this).pushDialog(
      "",
      <NewConfigurationDialog
        onOkClick={(name) => {
          runInFlowWithHandler({
            ctx: this,
            action: () => {
              const newConfig = configuration.cloneAs(name);
              getConfigurationManager(this).activeTableConfiguration = newConfig
            }
          });
          closeDialog();
        }}
        onCancelClick={() => closeDialog()}
      />
    );

    getDialogStack(this).closeDialog(this.dialogKey);
  }

  @action.bound onColumnConfSubmit(event: any, configuration: ITableConfiguration): void {
    this.ApplyConfiguration(configuration);
    getDialogStack(this).closeDialog(this.dialogKey);
  }

  private ApplyConfiguration(configuration: ITableConfiguration) {
    this.tablePanelView.fixedColumnCount = configuration.fixedColumnCount;
    this.tablePanelView.hiddenPropertyIds.clear();
    const groupingConf = getGroupingConfiguration(this);
    groupingConf.clearGrouping();
    for (let column of configuration.columnConfigurations) {
      this.tablePanelView.hiddenPropertyIds.set(column.propertyId, !column.isVisible);
      if (column.groupingIndex) {
        groupingConf.setGrouping(column.propertyId, column.timeGroupingUnit, column.groupingIndex);
      }
      this.tablePanelView.aggregations.setType(column.propertyId, column.aggregationType);
    }
    getFormScreenLifecycle(this).loadInitialData();
  }

  @computed get tablePanelView() {
    return getTablePanelView(this);
  }

  parent?: any;
}

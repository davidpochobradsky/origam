import * as React from "react";
import { AutoSizer } from "react-virtualized";
import { GridEditorMounter } from "./cells/GridEditorMounter";
import { StringGridEditor } from "./cells/string/GridEditor";
import { LookupResolverProvider } from "./DataLoadingStrategy/LookupResolverProvider";
import { DataTableSelectors } from "./DataTable/DataTableSelectors";
import { DataTableState, DataTableField } from "./DataTable/DataTableState";
import { CellScrolling } from "./Grid/CellScrolling";
import { createColumnHeaderRenderer } from "./Grid/ColumnHeaderRenderer";
import { GridActions } from "./Grid/GridActions";
import { createGridCellRenderer } from "./Grid/GridCellRenderer";
import { ColumnHeaders, GridComponent } from "./Grid/GridComponent";
import { GridCursorComponent } from "./Grid/GridCursorComponent";
import { GridCursorView } from "./Grid/GridCursorView";
import { GridInteractionActions } from "./Grid/GridInteractionActions";
import { GridInteractionSelectors } from "./Grid/GridInteractionSelectors";
import { GridInteractionState } from "./Grid/GridInteractionState";
import { GridSelectors } from "./Grid/GridSelectors";
import { GridState } from "./Grid/GridState";
import { GridView } from "./Grid/GridView";
import { GridSetup } from "./GridPanel/adapters/GridSetup";
import { EventObserver } from "./utils/events";
import { DataLoadingStrategyActions } from "./DataLoadingStrategy/DataLoadingStrategyActions";
import { DataLoadingStrategySelectors } from "./DataLoadingStrategy/DataLoadingStrategySelectors";
import { DataLoadingStrategyState } from "./DataLoadingStrategy/DataLoadingStrategyState";
import { DataTableActions } from "./DataTable/DataTableActions";
import { DataLoader } from "./DataLoadingStrategy/DataLoader";
import { GridOrderingSelectors } from "./GridOrdering/GridOrderingSelectors";
import { GridOrderingState } from "./GridOrdering/GridOrderingState";
import { GridOrderingActions } from "./GridOrdering/GridOrderingActions";
import { GridOutlineState } from "./GridOutline/GridOutlineState";
import { GridOutlineSelectors } from "./GridOutline/GridOutlineSelectors";
import { GridOutlineActions } from "./GridOutline/GridOutlineActions";
import { GridTopology } from "./GridPanel/adapters/GridTopology";
import { Observer, observer } from "mobx-react";
import { GridToolbarView } from "./GridPanel/GridToolbarView";
import {
  IFieldType,
  IDataTableFieldStruct,
  IDataTableSelectors,
  ICellValue
} from "./DataTable/types";
import { Splitter } from "./uiParts/Splitter/SplitterComponent";
import {
  IGridTopology,
  IGridSetup,
  IFormSetup,
  GridViewType,
  IFormTopology,
  IGridInteractionSelectors,
  IGridInteractionActions
} from "./Grid/types";
import { action, computed } from "mobx";
import { IGridPanelBacking } from "./GridPanel/types";
import {
  FormComponent,
  FormFieldPositioner,
  FormFieldLabel
} from "./Grid/FormComponent";
import { FormView } from "./Grid/FormView";
import { FormTopology } from "./GridPanel/adapters/FormTopology";
import { FormState } from "./Grid/FormState";
import { FormActions } from "./Grid/FormActions";
import { DataSavingStrategy } from "./DataLoadingStrategy/DataSavingStrategy";
import { DataSaver } from "./DataLoadingStrategy/DataSaver";

const personFields = [
  new DataTableField({
    id: "name",
    label: "Name",
    type: IFieldType.string,
    dataIndex: 0,
    isLookedUp: false
  }),
  new DataTableField({
    id: "birth_date",
    label: "Birth date",
    type: IFieldType.date,
    dataIndex: 1,
    isLookedUp: false
  }),
  new DataTableField({
    id: "likes_platypuses",
    label: "Likes platypuses?",
    type: IFieldType.boolean,
    dataIndex: 2,
    isLookedUp: false
  }),
  new DataTableField({
    id: "city_id",
    label: "Lives in",
    type: IFieldType.string,
    dataIndex: 3,
    isLookedUp: true,
    lookupResultFieldId: "name",
    lookupResultTableId: "city"
  }),
  new DataTableField({
    id: "favorite_color",
    label: "Favorite color",
    type: IFieldType.color,
    dataIndex: 4,
    isLookedUp: false
  })
];

const cityFields = [
  new DataTableField({
    id: "name",
    label: "Name",
    type: IFieldType.string,
    dataIndex: 0,
    isLookedUp: false
  }),
  new DataTableField({
    id: "inhabitants",
    label: "Inhabitants",
    type: IFieldType.integer,
    dataIndex: 1,
    isLookedUp: false
  })
];

class GridConfiguration {
  public gridSetup: IGridSetup;
  public gridTopology: IGridTopology;
  public formSetup: IFormSetup;
  public formTopology: IFormTopology;

  @action.bound
  public set(
    gridSetup: IGridSetup,
    gridTopology: IGridTopology,
    formSetup: IFormSetup,
    formTopology: IFormTopology
  ) {
    this.gridSetup = gridSetup;
    this.gridTopology = gridTopology;
    this.formSetup = formSetup;
    this.formTopology = formTopology;
  }
}

function createGridPaneBacking(
  dataTableName: string,
  dataTableFields: IDataTableFieldStruct[]
) : IGridPanelBacking {
  const configuration = new GridConfiguration();

  const lookupResolverProvider = new LookupResolverProvider({
    get dataLoader() {
      return dataLoader;
    }
  });

  const gridOrderingState = new GridOrderingState();
  const gridOrderingSelectors = new GridOrderingSelectors(gridOrderingState);
  const gridOrderingActions = new GridOrderingActions(
    gridOrderingState,
    gridOrderingSelectors
  );

  const gridOutlineState = new GridOutlineState();
  const gridOutlineSelectors = new GridOutlineSelectors(gridOutlineState);
  const gridOutlineActions = new GridOutlineActions(
    gridOutlineState,
    gridOutlineSelectors
  );

  const dataLoader = new DataLoader(dataTableName);

  const dataTableState = new DataTableState();

  dataTableState.fields = dataTableFields;

  const dataTableSelectors = new DataTableSelectors(
    dataTableState,
    lookupResolverProvider,
    dataTableName
  );
  const dataTableActions = new DataTableActions(
    dataTableState,
    dataTableSelectors
  );

  const onStartGrid = EventObserver();
  const onStopGrid = EventObserver();

  const gridState = new GridState();
  const gridSelectors = new GridSelectors(
    gridState,
    configuration,
    configuration
  );
  const gridActions = new GridActions(gridState, gridSelectors, configuration);

  const gridView = new GridView(gridSelectors, gridActions);

  const gridInteractionState = new GridInteractionState();
  const gridInteractionSelectors = new GridInteractionSelectors(
    gridInteractionState,
    configuration,
    configuration
  );

  const formState = new FormState();
  const formActions = new FormActions(formState);

  const gridInteractionActions = new GridInteractionActions(
    gridInteractionState,
    gridInteractionSelectors,
    gridActions,
    gridSelectors,
    formActions,
    configuration
  );
  onStartGrid(() => gridInteractionActions.start());
  onStopGrid(() => gridInteractionActions.stop());

  const gridCursorView = new GridCursorView(
    gridInteractionSelectors,
    gridSelectors,
    dataTableSelectors,
    dataTableActions,
    configuration,
    configuration
  );

  const cellScrolling = new CellScrolling(
    gridSelectors,
    gridActions,
    gridInteractionSelectors,
    configuration,
    configuration
  );
  onStartGrid(() => cellScrolling.start());
  onStopGrid(() => cellScrolling.stop());

  const dataLoadingStrategyState = new DataLoadingStrategyState();
  const dataLoadingStrategySelectors = new DataLoadingStrategySelectors(
    dataLoadingStrategyState,
    gridSelectors,
    dataTableSelectors
  );
  const dataLoadingStrategyActions = new DataLoadingStrategyActions(
    dataLoadingStrategyState,
    dataLoadingStrategySelectors,
    dataTableActions,
    dataTableSelectors,
    dataLoader,
    gridOrderingSelectors,
    gridOrderingActions,
    gridOutlineSelectors,
    gridInteractionActions,
    gridSelectors,
    gridActions
  );
  onStartGrid(() => dataLoadingStrategyActions.start());
  onStopGrid(() => dataLoadingStrategyActions.stop());

  const dataSaver = new DataSaver(
    dataTableName,
    dataTableActions,
    dataTableSelectors
  );
  const dataSavingStrategy = new DataSavingStrategy(
    dataTableSelectors,
    dataTableActions,
    dataSaver
  );

  const gridToolbarView = new GridToolbarView(
    gridInteractionSelectors,
    gridSelectors,
    dataTableSelectors,
    dataTableActions,
    gridInteractionActions,
    configuration
  );

  const gridSetup = new GridSetup(gridInteractionSelectors, dataTableSelectors);
  const gridTopology = new GridTopology(dataTableSelectors);

  /*
  onStartGrid.trigger();

  // gridOrderingActions.setOrdering('name', 'asc');

  dataLoadingStrategyActions.requestLoadFresh();*/

  const formSetup = new FormSetup(dataTableSelectors);
  const formView = new FormView(
    dataTableSelectors,
    gridInteractionSelectors,
    formSetup
  );

  const formTopology = new FormTopology(gridTopology);

  configuration.set(gridSetup, gridTopology, formSetup, formTopology);

  return {
    gridToolbarView,
    gridView,
    gridSetup,
    gridTopology,
    gridCursorView,
    gridInteractionActions,
    gridInteractionSelectors,
    onStartGrid,
    onStopGrid,
    dataLoadingStrategyActions,
    dataTableSelectors,
    gridOrderingActions,
    gridOrderingSelectors,

    formView,
    formSetup,
    formTopology,
    formActions
  };
}

class FormSetup implements IFormSetup {
  constructor(public dataTableSelectors: IDataTableSelectors) {}

  public get dimensions() {
    return [
      [200, 30, 100, 20],
      [200, 60, 100, 20],
      [200, 90, 100, 20],
      [200, 120, 100, 20],
      [200, 150, 100, 20],
      [450, 30, 100, 20],
      [450, 60, 100, 20],
      [450, 90, 100, 20],
      [450, 120, 100, 20]
    ].slice(0, this.fieldCount);
  }

  @computed
  public get fieldCount(): number {
    return this.dataTableSelectors.fieldCount;
  }

  public isScrollingEnabled: boolean = true;

  public getCellTop(fieldIndex: number): number {
    return this.dimensions[fieldIndex][1];
  }

  public getCellBottom(fieldIndex: number): number {
    return this.getCellTop(fieldIndex) + this.getCellHeight(fieldIndex);
  }

  public getCellLeft(fieldIndex: number): number {
    return this.dimensions[fieldIndex][0];
  }

  public getCellRight(fieldIndex: number): number {
    return this.getCellLeft(fieldIndex) + this.getCellWidth(fieldIndex);
  }

  public getCellHeight(fieldIndex: number): number {
    return this.dimensions[fieldIndex][3];
  }

  public getCellWidth(fieldIndex: number): number {
    return this.dimensions[fieldIndex][2];
  }

  public getCellValue(
    recordIndex: number,
    fieldIndex: number
  ): ICellValue | undefined{
    const record = this.dataTableSelectors.getRecordByRecordIndex(recordIndex);
    const field = this.dataTableSelectors.getFieldByFieldIndex(fieldIndex);
    if (record && field) {
      return this.dataTableSelectors.getValue(record, field);
    } else {
      return;
    }
  }

  public getFieldLabel(fieldIndex: number): string {
    return `Field label ${fieldIndex}`;
  }

  public getLabelOffset(fieldIndex: number): number {
    return 100;
  }
}

@observer
class GridPane extends React.Component<{
  initialDataTableName: string;
  initialFields: IDataTableFieldStruct[];
}> {
  constructor(props: any) {
    super(props);
    this.gridPanelBacking = createGridPaneBacking(
      this.props.initialDataTableName,
      this.props.initialFields
    );
  }

  private gridPanelBacking: IGridPanelBacking;

  public componentDidMount() {
    this.gridPanelBacking.onStartGrid.trigger();
    this.gridPanelBacking.dataLoadingStrategyActions
      .requestLoadFresh()
      .then(() => {
        this.gridPanelBacking.gridInteractionActions.selectFirst();
      });
  }

  public render() {
    const {
      gridToolbarView,
      gridView,
      gridSetup,
      gridTopology,
      gridCursorView,
      gridInteractionActions,
      gridInteractionSelectors,
      formView,
      formSetup,
      formTopology,
      formActions,
      gridOrderingActions,
      gridOrderingSelectors
    } = this.gridPanelBacking;
    return (
      <AutoSizer>
        {({ width: paneWidth, height: paneHeight }) => (
          <Observer>
            {() => (
              <div
                style={{
                  display: "flex",
                  flexDirection: "column",
                  width: paneWidth,
                  height: paneHeight,
                  overflow: "hidden"
                }}
              >
                <div
                  style={{
                    display: "flex",
                    flexDirection: "row"
                  }}
                >
                  <button onClick={gridToolbarView.handleAddRecordClick}>
                    Add
                  </button>
                  <button onClick={gridToolbarView.handleRemoveRecordClick}>
                    Remove
                  </button>
                  <button onClick={gridToolbarView.handleSetGridViewClick}>
                    Grid
                  </button>
                  <button onClick={gridToolbarView.handleSetFormViewClick}>
                    Form
                  </button>
                  <button onClick={gridToolbarView.handlePrevRecordClick}>
                    Prev
                  </button>
                  <button onClick={gridToolbarView.handleNextRecordClick}>
                    Next
                  </button>
                </div>
                {gridInteractionSelectors.activeView === GridViewType.Grid && (
                  <div
                    style={{
                      display: "flex",
                      flexDirection: "column"
                    }}
                  >
                    <ColumnHeaders
                      view={gridView}
                      columnHeaderRenderer={createColumnHeaderRenderer({
                        gridSetup,
                        gridOrderingActions,
                        gridOrderingSelectors,
                        gridTopology
                      })}
                    />
                  </div>
                )}
                <div
                  style={{
                    flexDirection: "column",
                    height: "100%",
                    flex: "1 1",
                    display:
                      gridInteractionSelectors.activeView === GridViewType.Grid
                        ? "flex"
                        : "none"
                  }}
                >
                  <AutoSizer>
                    {({ width, height }) => (
                      <Observer>
                        {() => (
                          <GridComponent
                            view={gridView}
                            gridSetup={gridSetup}
                            gridTopology={gridTopology}
                            width={width}
                            height={height}
                            overlayElements={
                              <GridCursorComponent
                                view={gridCursorView}
                                cursorContent={
                                  gridInteractionSelectors.activeView ===
                                    GridViewType.Grid && (
                                    <GridEditorMounter
                                      cursorView={gridCursorView}
                                    >
                                      {gridCursorView.isCellEditing && (
                                        <StringGridEditor
                                          editingRecordId={
                                            gridCursorView.editingRowId!
                                          }
                                          editingFieldId={
                                            gridCursorView.editingColumnId!
                                          }
                                          value={
                                            gridCursorView.editingOriginalCellValue
                                          }
                                          onKeyDown={
                                            gridInteractionActions.handleDumbEditorKeyDown
                                          }
                                          onDataCommit={
                                            gridCursorView.handleDataCommit
                                          }
                                        />
                                      )}
                                    </GridEditorMounter>
                                  )
                                }
                              />
                            }
                            cellRenderer={createGridCellRenderer({
                              gridSetup,
                              onClick(event, cellRect, cellInfo) {
                                gridInteractionActions.handleGridCellClick(
                                  event,
                                  {
                                    rowId: gridTopology.getRowIdByIndex(
                                      cellInfo.rowIndex
                                    )!,
                                    columnId: gridTopology.getColumnIdByIndex(
                                      cellInfo.columnIndex
                                    )!
                                  }
                                );
                              }
                            })}
                            onKeyDown={gridInteractionActions.handleGridKeyDown}
                            onOutsideClick={
                              gridInteractionActions.handleGridOutsideClick
                            }
                            onNoCellClick={
                              gridInteractionActions.handleGridNoCellClick
                            }
                          />
                        )}
                      </Observer>
                    )}
                  </AutoSizer>
                </div>
                {gridInteractionSelectors.activeView === GridViewType.Form && (
                  <div
                    style={{
                      flexDirection: "column",
                      height: "100%",
                      flex: "1 1",
                      display: "flex"
                    }}
                  >
                    <AutoSizer>
                      {({ width, height }) => (
                        <Observer>
                          {() => (
                            <div
                              style={{
                                display: "flex",
                                flexDirection: "column",
                                width,
                                height,
                                overflow: "hidden"
                              }}
                            >
                              <FormComponent
                                fieldCount={formSetup.fieldCount}
                                onKeyDown={
                                  gridInteractionActions.handleFormKeyDown
                                }
                                formActions={formActions}
                                overlayElements={
                                  <FormCursorComponent
                                    formSetup={formSetup}
                                    formTopology={formTopology}
                                    gridInteractionSelectors={
                                      gridInteractionSelectors
                                    }
                                    gridInteractionActions={
                                      gridInteractionActions
                                    }
                                  >
                                    {gridInteractionSelectors.activeView ===
                                      GridViewType.Form && (
                                      <GridEditorMounter
                                        cursorView={gridCursorView}
                                      >
                                        {gridCursorView.isCellEditing && (
                                          <StringGridEditor
                                            editingRecordId={
                                              gridCursorView.editingRowId!
                                            }
                                            editingFieldId={
                                              gridCursorView.editingColumnId!
                                            }
                                            value={
                                              gridCursorView.editingOriginalCellValue
                                            }
                                            onKeyDown={
                                              gridInteractionActions.handleDumbEditorKeyDown
                                            }
                                            onDataCommit={
                                              gridCursorView.handleDataCommit
                                            }
                                          />
                                        )}
                                      </GridEditorMounter>
                                    )}
                                  </FormCursorComponent>
                                }
                                cellRenderer={({ fieldIndex }) => (
                                  <Observer>
                                    {() => (
                                      <>
                                        <FormFieldLabel
                                          fieldIndex={fieldIndex}
                                          formSetup={formSetup}
                                          formView={formView}
                                        />
                                        <FormFieldPositioner
                                          fieldIndex={fieldIndex}
                                          formSetup={formSetup}
                                          formTopology={formTopology}
                                          onClick={
                                            gridInteractionActions.handleFormFieldClick
                                          }
                                        >
                                          <FormTextRenderer
                                            value={formView.getCellValue(
                                              fieldIndex
                                            )}
                                          />
                                        </FormFieldPositioner>
                                      </>
                                    )}
                                  </Observer>
                                )}
                              />
                            </div>
                          )}
                        </Observer>
                      )}
                    </AutoSizer>
                  </div>
                )}
              </div>
            )}
          </Observer>
        )}
      </AutoSizer>
    );
  }
}

@observer
class FormTextRenderer extends React.Component<{
  value: ICellValue | undefined;
}> {
  public render() {
    return (
      <input
        style={{
          width: "100%",
          height: "100%"
        }}
        value={''+this.props.value}
        readOnly={true}
      />
    );
  }
}

@observer
class FormCursorComponent extends React.Component<{
  formTopology: IFormTopology;
  formSetup: IFormSetup;
  gridInteractionSelectors: IGridInteractionSelectors;
  gridInteractionActions: IGridInteractionActions;
}> {
  public render() {
    const {
      formTopology,
      gridInteractionSelectors,
      formSetup,
      gridInteractionActions
    } = this.props;
    if (!gridInteractionSelectors.isCellSelected) {
      return null;
    }
    const fieldIndex = formTopology.getFieldIndexById(
      gridInteractionSelectors.selectedColumnId!
    );
    return (
      <FormFieldPositioner
        fieldIndex={fieldIndex}
        formSetup={formSetup}
        formTopology={formTopology}
        onClick={gridInteractionActions.handleFormFieldClick}
      >
        <div
          style={{ width: "100%", height: "100%", border: "1px solid #4444ff" }}
        >
          {this.props.children}
        </div>
      </FormFieldPositioner>
    );
  }
}

@observer
class App extends React.Component {
  public render() {
    return (
      <AutoSizer>
        {({ width, height }) => (
          <Observer>
            {() => (
              <Splitter width={width} height={height} vertical={false}>
                <GridPane
                  initialDataTableName="city"
                  initialFields={cityFields}
                />
                <GridPane
                  initialDataTableName="person"
                  initialFields={personFields}
                />
              </Splitter>
            )}
          </Observer>
        )}
      </AutoSizer>
    );
  }
}

export default App;
/*
class Splitter extends React.Component<{ width: number; height: number; vertical: boolean; }> {
  public render() {
    return (
      <div
        style={{
          display: "flex",
          width: this.props.width,
          height: this.props.height,
          flexDirection: "row"
        }}
      >
        {React.Children.map(this.props.children, (child, index) => (
          <div key={index} style={{flexGrow: 1}}>{child}</div>
        ))}
      </div>
    );
  }
}

*/

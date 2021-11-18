/*
Copyright 2005 - 2021 Advantage Solutions, s. r. o.

This file is part of ORIGAM (http://www.origam.org).

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ORIGAM. If not, see <http://www.gnu.org/licenses/>.
*/

import { observer, Observer } from "mobx-react";
import React, { useContext, useEffect, useMemo, createRef } from "react";
import { GridCellProps, MultiGrid } from "react-virtualized";
import { CtxCell } from "./Cells/CellsCommon";
import S from "./Dropdown/Dropdown.module.scss";
import { CtxDropdownCtrlRect, CtxDropdownRefBody } from "./Dropdown/DropdownCommon";
import { CtxDropdownEditor } from "./DropdownEditor";
import SE from "./DropdownEditor.module.scss";
import { rowHeight } from "gui/Components/ScreenElements/Table/TableRendering/cells/cellsCommon";
import cx from "classnames";
import { getCanvasFontSize, getTextWidth } from "utils/textMeasurement";
import { DropdownColumnDrivers, DropdownDataTable } from "modules/Editors/DropdownEditor/DropdownTableModel";
import { BoundingRect } from "react-measure";
import { DropdownEditorBehavior } from "modules/Editors/DropdownEditor/DropdownEditorBehavior";
import { observable } from "mobx";

export function DropdownEditorBody() {
  const refCtxBody = useContext(CtxDropdownRefBody);
  const beh = useContext(CtxDropdownEditor).behavior;

  const ref = useMemo(
    () => (elm: any) => {
      refCtxBody(elm);
      beh.refDropdownBody(elm);
    },
    []
  );

  useEffect(() => {
    window.addEventListener("mousedown", beh.handleWindowMouseDown);
    return () => window.removeEventListener("mousedown", beh.handleWindowMouseDown);
  }, [beh]);

  const drivers = useContext(CtxDropdownEditor).columnDrivers;
  const dataTable = useContext(CtxDropdownEditor).editorDataTable;
  const rectCtrl = useContext(CtxDropdownCtrlRect);

  return (
    <Observer>
      {() => (
        <div ref={ref} className={S.body} onMouseDown={beh.handleBodyMouseDown}>
          <DropdownEditorTable
            drivers={drivers}
            dataTable={dataTable}
            rectCtrl={rectCtrl}
            beh={beh}/>
        </div>
      )}
    </Observer>
  );
}

@observer
export class DropdownEditorTable extends  React.Component<{
  drivers: DropdownColumnDrivers,
  dataTable: DropdownDataTable,
  rectCtrl: BoundingRect,
  beh: DropdownEditorBehavior
}> {
  refMultiGrid = createRef<MultiGrid>();
  @observable
  scrollbarSize = { horiz: 0, vert: 0 };
  hasHeader: boolean;
  hoveredRowIndex= - 1;
  width = 0;
  columnCount = 0;

  get rowCount(){
    return this.props.dataTable.rowCount + (this.hasHeader ? 1 : 0);
  }

  get height(){
    let height = 0;
    for (let i = 0; i < this.rowCount; i++) {
      height = height + rowHeight;
    }
    return Math.min(height, 300) + this.scrollbarSize.horiz;
  }

  @observable
  widths: Array<number> = [];

  constructor(props: any) {
    super(props);
    this.columnCount = this.props.drivers.driverCount;
    this.hasHeader = this.columnCount > 1;
  }

  handleScrollbarPresenceChange(args: {
    horizontal: boolean;
    size: number;
    vertical: boolean;
  }) {
    this.scrollbarSize = {
      horiz: args.horizontal ? args.size : 0,
      vert: args.vertical ? args.size : 0,
    };
  }

  componentDidMount() {
    let columnWidthSum = 0;
    for (let columnIndex = 0; columnIndex < this.columnCount; columnIndex++) {
      let cellWidth = 100;
      for (let rowIndex = 0; rowIndex < this.rowCount - 1; rowIndex++) {
        const cellText = this.props.drivers.getDriver(columnIndex).bodyCellDriver.formattedText(rowIndex);
        const currentCellWidth = Math.round(getTextWidth(cellText, getCanvasFontSize()));
        if(currentCellWidth > cellWidth){
          cellWidth = currentCellWidth;
        }
      }

      this.width = this.width + cellWidth;
      this.widths.push(cellWidth);
      columnWidthSum = columnWidthSum + cellWidth;
      if (this.width >= window.innerWidth - 100) {
        this.width = window.innerWidth - 100;
        break;
      }
    }

    this.width = Math.max(this.width + this.scrollbarSize.vert, this.props.rectCtrl.width!);
    let columnGrowFactor = 1;
    if (columnWidthSum > 0 && columnWidthSum < this.props.rectCtrl.width!) {
      columnGrowFactor = (this.width - this.scrollbarSize.vert) / columnWidthSum;
    }
    this.widths = this.widths.map((w) => w * columnGrowFactor);
  }

  renderTableCell({columnIndex, key, parent, rowIndex, style}: GridCellProps) {
    const Prov = CtxCell.Provider as any;
    return (
      <CtxCell.Provider
        key={key}
        value={{ visibleColumnIndex: columnIndex, visibleRowIndex: rowIndex }}
      >
        {(this.hasHeader && rowIndex > 0) || !this.hasHeader ? (
          <div
            style={style}
            className={cx({ isHoveredRow: rowIndex === this.hoveredRowIndex })}
            onMouseOver={(evt) => {
              this.hoveredRowIndex = rowIndex;
            }}
            onMouseOut={(evt) => {
              this.hoveredRowIndex= -1;
            }}
          >
            <Observer>
              {() => (
                <>
                  {this.props.drivers
                    .getDriver(columnIndex)
                    .bodyCellDriver.render(rowIndex - (this.hasHeader ? 1 : 0))}
                </>
              )}
            </Observer>
          </div>
        ) : (
          <div style={style}>
            <Observer>
              {() => <>{this.props.drivers.getDriver(columnIndex).headerCellDriver.render()}</>}
            </Observer>
          </div>
        )}
      </CtxCell.Provider>
    );
  }

  render(){
    if(this.width === 0){
      return null;
    }

    return (
      <MultiGrid
        ref={this.refMultiGrid}
        scrollToRow={this.props.beh.scrollToRowIndex}
        scrollToAlignment="center"
        onScrollbarPresenceChange={args => this.handleScrollbarPresenceChange(args)}
        classNameTopRightGrid={SE.table}
        classNameBottomRightGrid={SE.table}
        columnCount={this.columnCount}
        rowCount={this.rowCount}
        columnWidth={({ index }) => this.widths[index]}
        rowHeight={rowHeight}
        fixedRowCount={this.hasHeader ? 1 : 0}
        height={this.height}
        width={this.width}
        cellRenderer={args => this.renderTableCell(args)}
        onScroll={this.props.beh.handleScroll}
      />
    );
  }
}
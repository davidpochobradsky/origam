export interface IRowsContainer {
  clear(): void;

  delete(row: any[]): void;

  insert(index: number, row: any[]): Promise<any>;

  set(rows: any[][]): void;

  appendRecords(rowsIn: any[][]): void;

  substitute(row: any[]): void;

  registerResetListener(listener: () => void): void;

  unlockAddedRowPosition(): void;

  addedRowPositionLocked: boolean;

  maxRowCountSeen: number;

  rows: any[];

  allRows: any[];

  updateSortAndFilter(data?: {retainPreviousSelection?: true}): Promise<any>;

  start(): void;

  stop():void;

  getFirstRow(): any[] | undefined;

  parent?: any;
}

export interface IDataViewLifecycleData {}

export interface IDataViewLifecycle extends IDataViewLifecycleData {
  $type_IDataViewLifecycle: 1;
  isWorking: boolean;
  changeMasterRow(): Generator;
  navigateChildren(): Generator;
  navigateAsChild(rows?: any[]): Generator;
  start(): void;

  startSelectedRowReaction(fireImmediatelly?: boolean): void;
  stopSelectedRowReaction(): void;
  runRecordChangedReaction(action?: ()=>Generator): Promise<void>;

  parent?: any;
}

export const isIDataViewLifecycle = (o: any): o is IDataViewLifecycle =>
  o.$type_IDataViewLifecycle;
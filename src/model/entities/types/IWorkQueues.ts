export interface IWorkQueuesData {}

export interface IWorkQueues extends IWorkQueuesData {
  $type_IWorkQueues: 1;
  parent?: any;

  isTimerRunning: boolean;
  items: any[];
  totalItemCount: number;

  getWorkQueueList(): Generator<any>;
  startTimer(refreshIntervalMs: number): Generator<any>;
  stopTimer(): Generator<any>;
}

export const isIWorkQueues = (obj: any): obj is IWorkQueues =>
  obj.$type_IWorkQueues;

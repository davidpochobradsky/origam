import React from "react";
import {IPlugin} from "./IPlugin";
import {IDataView} from "../../model/entities/types/IDataView";


export interface IFormPlugin extends IPlugin{
  initialize(): void;
  requestSessionRefresh:  (() => Promise<any>) | undefined;
  setFormParameters:  ((parameters: { [key: string]: string }) => void) | undefined;
}

export const isIFormPlugin = (o: any): o is IFormPlugin => o?.$type_IFormPlugin;

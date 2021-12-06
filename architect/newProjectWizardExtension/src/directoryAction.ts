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
import fs = require('fs');
import { pathFormat } from './function';

 export let rootProjectDirectory = "";
 export let modelDirectory = "";
 export let customAssetsDirectory = "";
 export let dockerScripsDirectory = "";
 export let rootDirectory = "";

 export function createDirectory (path: string)
{
    fs.mkdirSync(path);
    console.log(path + " successfully created.");
}
export async function createTopDirectorySructure(state:string[])
{
    rootDirectory = state[3];
    if(rootDirectory === "")
    {
		console.log("Destination Model Directory not set. End.");
		return false;
    }
    else
    {
    //create root model directory 
      rootProjectDirectory = pathFormat(rootDirectory,state[0]);
      modelDirectory = pathFormat(rootProjectDirectory,"model");
      customAssetsDirectory = pathFormat(rootProjectDirectory,"customAssets");
      dockerScripsDirectory = pathFormat(rootProjectDirectory,"NewProject");
      createDirectory(rootProjectDirectory);
      createDirectory(customAssetsDirectory);
      createDirectory(dockerScripsDirectory);
      return true;
    }
}
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
import * as vscode from 'vscode';
import { multiStepInput } from './multiStepInput';
import { downloadsync, unzipfile } from './fileaction';
import { existsSync } from 'fs';
import { createTopDirectorySructure, rootProjectDirectory } from './directoryAction';
import { pathFormat } from './function';
import { createPackageDirectoryPackage } from './origamPackage';
import { createDatabaseDocker } from './databaseBuilder';
import { createComposeFile, createDockerEnvFile, createDockerHttpFiles, createDockerOrigamFile } from './dockerFiles';
import {v4 as uuidv4} from 'uuid';

export function activate(context: vscode.ExtensionContext) 
{
	console.log('Congratulations, your extension "neworigamproject" is now active!');
	context.subscriptions.push
	(
		vscode.commands.registerCommand
		('neworigamproject.NewProjectWizard', async () => 
			{
				multiStepInput(context);
				vscode.window.showInformationMessage('New Project Wizard start from NewOrigamProject. Please wait for finish.');
				// Start Wizard
				const state = await multiStepInput(context);
				if(await createTopDirectorySructure(state))
				{
					console.log({state});
					const projectName = state[0];
					const databaseType = state[1];
					const origamVersion = state[2];
					const fullPathZipFile = pathFormat(rootProjectDirectory,"model.zip");
					await downloadsync(getOption(origamVersion), fullPathZipFile);
					isExistsFile(fullPathZipFile);
					if (isExistsFile(fullPathZipFile))
					{
						console.log("Donwloaded: " + fullPathZipFile);
						try
						{
						console.log("Start unzip: " + fullPathZipFile);
						await unzipfile(fullPathZipFile,rootProjectDirectory);
						const packageId = createPackageDirectoryPackage(projectName);
						createDatabaseDocker(databaseType,rootProjectDirectory);
						const userPassword = generatePassword();
						createDockerEnvFile(rootProjectDirectory,projectName,
							databaseType,packageId,userPassword,origamVersion);
						createDockerOrigamFile(rootProjectDirectory,origamVersion);	
						createComposeFile(rootProjectDirectory,userPassword,databaseType);
						createDockerHttpFiles(rootProjectDirectory);
						} catch (e)
						{
							console.error(e);
						}
					}
					vscode.window.showInformationMessage('New Project is created.');
				}
				else
				{
					vscode.window.showInformationMessage('New Project is canceled.');
				}
			}
		)
	);
}
// this method is called when your extension is deactivated
export function deactivate() {}

function isExistsFile(fullPathFileName: string):boolean {
	try 
	{
		if (!existsSync(fullPathFileName)) { return false; }
		return true;
	} 
	catch(err) 
	{
		console.error("File not Exists");return false;
	}
}
function getOption(origamVersion:string)
{
	return {
		hostname: 'github.com',
		port: 443,
		path: '/origam/origam-source/releases/download/'+origamVersion+'-latest/origam-model.zip',
		method: 'GET',
		makeSynchronousRequest: true,
		
	  };
}

function generatePassword() {
	return uuidv4().substr(0,12).replace(/-/,"");
}
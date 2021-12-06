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
import { QuickPickItem, window, Disposable, CancellationToken, 
	QuickInputButton, QuickInput, ExtensionContext, QuickInputButtons, Uri } from 'vscode';
import { State } from './interfaces';

export async function multiStepInput(context: ExtensionContext) {
	let states: string[] = [];
	class MyButton implements QuickInputButton {
		constructor(public iconPath: { light: Uri; dark: Uri; }, public tooltip: string) { }
	}

	const createResourceGroupButton = new MyButton({
		dark: Uri.file(context.asAbsolutePath('resources/dark/add.svg')),
		light: Uri.file(context.asAbsolutePath('resources/light/add.svg')),
	}, 'Create Resource Group');

	//step 1
	async function collectInputs() {
		//const state = {} as Partial<State>;
		await MultiStepInput.run(input => inputProjectName(input, states));
		return states;
	}

	//step 2
	async function inputProjectName(input: MultiStepInput, states: string[]) {
		const state = {} as Partial<State>;
		state.resourceGroup = await input.showInputBox({
			title,
			step: 1,
			totalSteps: 4,
			value: typeof state.resourceGroup === 'string' ? state.resourceGroup : '',
			prompt: 'Project Name',
			validate: validateNameIsUnique,
			shouldResume: shouldResume
		});
		states.push(state.resourceGroup);
		return (input: MultiStepInput) => pickRuntime(input, states);
	}

	const title = 'Create New Origam Project';
	//step 3
	async function pickRuntime(input: MultiStepInput, states: string[]) {
		const state = {} as Partial<State>;
		const runtimes = await getAvailableRuntimes(state.resourceGroup!, undefined /* TODO: token */);
		// TODO: Remember currently active item when navigating back.
		state.runtime = await input.showQuickPick({
			title,
			step: 2,
			totalSteps: 4,
			placeholder: 'Chose Database',
			items: runtimes,
			activeItem: state.runtime,
			shouldResume: shouldResume
		});
		states.push(state.runtime.label);
		return (input: MultiStepInput) => versionInput(input, states);
	}
	//step 4
	async function versionInput(input: MultiStepInput, states: string[]) {
		const state = {} as Partial<State>;
		const runtimes = await getAvailableVersions(state.resourceGroup!, undefined /* TODO: token */);
		// TODO: Remember currently active item when navigating back.
		state.runtime = await input.showQuickPick({
			title,
			step: 3,
			totalSteps: 4,
			placeholder: 'Chose Version',
			items: runtimes,
			activeItem: state.runtime,
			shouldResume: shouldResume
		});
		states.push(state.runtime.label);
		return (input: MultiStepInput) => modelDirectory(input, states);
	}
	//step 5
	async function modelDirectory(input: MultiStepInput, states: string[]) {
		const options: vscode.OpenDialogOptions = {
			canSelectMany: false,
			openLabel: 'Project directory',
			canSelectFolders: true,
			title: 'Select Project directory'
	   };
	   await vscode.window.showInformationMessage("Select Project Directory",{modal:true});
	   let directory = "";
	   let count = 0;
	   while (directory === "")
	   {
		await vscode.window.showOpenDialog(options).then(dir => 
			{
				if(dir && dir[0])
				{
					directory =  dir[0].fsPath;
				}
			});
			if(directory === "" && count=== 0)
			{
				await window.showWarningMessage("Project Directory is mandatory.", { modal: true });
			}
			if(count > 0)
			{
				break;
			}
			count++;
		}
		states.push(directory);
	}

	function shouldResume() {
		return new Promise<boolean>(() => {
			// noop
		});
	}

	async function validateNameIsUnique(name: string) {
		return  name.length===0 ?"Please fil name":undefined;
	}

	//chose node
	async function getAvailableRuntimes(resourceGroup: QuickPickItem | string, token?: CancellationToken): Promise<QuickPickItem[]> {
		// ...retrieve...
		return ['PostgreSQL', 'MsSQL']
			.map(label => ({ label }));
	}
	async function getAvailableVersions(resourceGroup: QuickPickItem | string, token?: CancellationToken): Promise<QuickPickItem[]> {
		return ['2021.1', 'master']
			.map(label => ({ label }));
	}

	// Start Wizard
	const state = await collectInputs();
	return state;
}

// -------------------------------------------------------
// Helper code that wraps the API for the multi-step case.
// -------------------------------------------------------


class InputFlowAction {
	static back = new InputFlowAction();
	static cancel = new InputFlowAction();
	static resume = new InputFlowAction();
}

type InputStep = (input: MultiStepInput) => Thenable<InputStep | void>;

interface QuickPickParameters<T extends QuickPickItem> {
	title: string;
	step: number;
	totalSteps: number;
	items: T[];
	activeItem?: T;
	placeholder: string;
	buttons?: QuickInputButton[];
	shouldResume: () => Thenable<boolean>;
}

interface InputBoxParameters {
	title: string;
	step: number;
	totalSteps: number;
	value: string;
	prompt: string;
	validate: (value: string) => Promise<string | undefined>;
	buttons?: QuickInputButton[];
	shouldResume: () => Thenable<boolean>;
}

class MultiStepInput {

	static async run<T>(start: InputStep) {
		const input = new MultiStepInput();
		return input.stepThrough(start);
	}

	private current?: QuickInput;
	private steps: InputStep[] = [];

	private async stepThrough<T>(start: InputStep) {
		let step: InputStep | void = start;
		while (step) {
			this.steps.push(step);
			if (this.current) {
				this.current.enabled = false;
				this.current.busy = true;
			}
			try {
				step = await step(this);
			} catch (err) {
				if (err === InputFlowAction.back) {
					this.steps.pop();
					step = this.steps.pop();
				} else if (err === InputFlowAction.resume) {
					step = this.steps.pop();
				} else if (err === InputFlowAction.cancel) {
					step = undefined;
				} else {
					throw err;
				}
			}
		}
		if (this.current) {
			this.current.dispose();
		}
	}

	async showQuickPick<T extends QuickPickItem, P extends QuickPickParameters<T>>({ title, step, totalSteps, items, activeItem, placeholder, buttons, shouldResume }: P) {
		const disposables: Disposable[] = [];
		try {
			return await new Promise<T | (P extends { buttons: (infer I)[] } ? I : never)>((resolve, reject) => {
				const input = window.createQuickPick<T>();
				input.title = title;
				input.step = step;
				input.totalSteps = totalSteps;
				input.placeholder = placeholder;
				input.items = items;
				if (activeItem) {
					input.activeItems = [activeItem];
				}
				input.buttons = [
					...(this.steps.length > 1 ? [QuickInputButtons.Back] : []),
					...(buttons || [])
				];
				disposables.push(
					input.onDidTriggerButton(item => {
						if (item === QuickInputButtons.Back) {
							reject(InputFlowAction.back);
						} else {
							resolve(<any>item);
						}
					}),
					input.onDidChangeSelection(items => resolve(items[0])),
					input.onDidHide(() => {
						(async () => {
							reject(shouldResume && await shouldResume() ? InputFlowAction.resume : InputFlowAction.cancel);
						})()
							.catch(reject);
					})
				);
				if (this.current) {
					this.current.dispose();
				}
				this.current = input;
				this.current.show();
			});
		} finally {
			disposables.forEach(d => d.dispose());
		}
	}

	async showInputBox<P extends InputBoxParameters>({ title, step, totalSteps, value, prompt, validate, buttons, shouldResume }: P) {
		const disposables: Disposable[] = [];
		try {
			return await new Promise<string | (P extends { buttons: (infer I)[] } ? I : never)>((resolve, reject) => {
				const input = window.createInputBox();
				input.title = title;
				input.step = step;
				input.totalSteps = totalSteps;
				input.value = value || '';
				input.prompt = prompt;
				input.buttons = [
					...(this.steps.length > 1 ? [QuickInputButtons.Back] : []),
					...(buttons || [])
				];
				let validating = validate('');
				disposables.push(
					input.onDidTriggerButton(item => {
						if (item === QuickInputButtons.Back) {
							reject(InputFlowAction.back);
						} else {
							resolve(<any>item);
						}
					}),
					input.onDidAccept(async () => {
						const value = input.value;
						input.enabled = false;
						input.busy = true;
						if (!(await validate(value))) {
							resolve(value);
						}
						input.enabled = true;
						input.busy = false;
					}),
					input.onDidChangeValue(async text => {
						const current = validate(text);
						validating = current;
						const validationMessage = await current;
						if (current === validating) {
							input.validationMessage = validationMessage;
						}
					}),
					input.onDidHide(() => {
						(async () => {
							reject(shouldResume && await shouldResume() ? InputFlowAction.resume : InputFlowAction.cancel);
						})()
							.catch(reject);
					})
				);
				if (this.current) {
					this.current.dispose();
				}
				this.current = input;
				this.current.show();
			});
		} finally {
			disposables.forEach(d => d.dispose());
		}
	}
}

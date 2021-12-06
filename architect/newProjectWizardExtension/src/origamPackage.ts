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
import { createDirectory, modelDirectory } from "./directoryAction";
import { pathFormat } from "./function";
import {v4 as uuidv4} from 'uuid';
import fs = require('fs');
const stringBuilder = require("node-stringbuilder");

export function createPackageDirectoryPackage(projectName:string)
{
    const packageDirectory = pathFormat(modelDirectory,projectName);
    createDirectory(packageDirectory);
    createBasicPackageDirectoryStructure(packageDirectory,projectName);
    return createOrigamPackakageFile(packageDirectory,projectName);
}

function createBasicPackageDirectoryStructure(packageDirectory:string,projectName:string) {
    createPartPackageDirectory(packageDirectory,"DatabaseDataType",projectName);
    createPartPackageDirectory(packageDirectory,"DataConstant",projectName);
    createPartPackageDirectory(packageDirectory,"DataEntity",projectName);
    createPartPackageDirectory(packageDirectory,"DataLookup",projectName);
    createPartPackageDirectory(packageDirectory,"DataStructure",projectName);
    createPartPackageDirectory(packageDirectory,"DeploymentVersion",projectName);
    createPartPackageDirectory(packageDirectory,"FormControlSet",projectName);
    createPartPackageDirectory(packageDirectory,"PanelControlSet",projectName);
    createPartPackageDirectory(packageDirectory,"Rule",projectName);
    createPartPackageDirectory(packageDirectory,"Transformation",projectName);
    createPartPackageDirectory(packageDirectory,"Workflow",projectName);
    createPartPackageDirectory(packageDirectory,"WorkflowStateMachine",projectName);
}

function createPartPackageDirectory(packageDirectory: string, part: string,projectName:string) {
    const partDir = pathFormat(packageDirectory,part);
    const modelPartDir = pathFormat(partDir,projectName);
    createDirectory(partDir);
    createDirectory(modelPartDir);
    const filedata = createDataFile(".origamGroup",projectName,uuidv4());
    let absoluteFilePath = pathFormat(modelPartDir,".origamGroup");
    fs.writeFileSync(absoluteFilePath,filedata);
    if(part === "DeploymentVersion")
    {
        const filedata = createDataFile("1.0.0.origam","",uuidv4());
        absoluteFilePath = pathFormat(modelPartDir,"1.0.0.origam");
        fs.writeFileSync(absoluteFilePath,filedata);
    }
}

function createOrigamPackakageFile(packageDirectory:string,projectName: string) {
    const guidId = uuidv4();
    const filedata = createDataFile(".origamPackage",projectName,guidId);
    const absoluteFilePath = pathFormat(packageDirectory,".origamPackage");
    fs.writeFileSync(absoluteFilePath,filedata);
    return guidId;
}

function createDataFile(datatype: string,replacement:string,guidId:string) {
    const sb = new stringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    if(datatype === ".origamGroup")
    {
        sb.appendLine("<x:file xmlns:x=\"http://schemas.origam.com/model-persistence/1.0.0\" xmlns:g=\"http://schemas.origam.com/Origam.Schema.SchemaItemGroup/6.0.0\">");
        sb.appendLine("<g:group");
        sb.appendLine("    x:id=\""+guidId+"\"");
        sb.appendLine("    x:isFolder=\"true\"");
        sb.appendLine("    g:name=\""+replacement+"\"");
        sb.appendLine("    g:rootItemType=\"DeploymentVersion\" />");
        sb.appendLine("</x:file>");
    }
    if(datatype ===".origamPackage")
    {
        sb.appendLine("<x:file xmlns:x=\"http://schemas.origam.com/model-persistence/1.0.0\" xmlns:p=\"http://schemas.origam.com/Origam.Schema.Package/6.1.0\" xmlns:pr=\"http://schemas.origam.com/Origam.Schema.PackageReference/6.0.0\">");
        sb.appendLine("  <p:package");
        sb.appendLine("    x:id=\""+guidId+"\"");
        sb.appendLine("    x:isFolder=\"true\"");
        sb.appendLine("    p:name=\""+replacement+"\"");
        sb.appendLine("    p:version=\"1.0.0\">");
        sb.appendLine("   <pr:packageReference");
        sb.appendLine("       x:id=\"dbc2319d-4146-4de7-bdbb-bddc1afbe889\"");
        sb.appendLine("       pr:referencedPackage=\"Root Menu/.origamPackage#b9ab12fe-7f7d-43f7-bedc-93747647d6e4\" />");
        sb.appendLine("  </p:package>");
        sb.appendLine("</x:file>");
    }
    if(datatype === "1.0.0.origam")
    {
        sb.appendLine("<x:file xmlns:x=\"http://schemas.origam.com/model-persistence/1.0.0\" xmlns:asi=\"http://schemas.origam.com/Origam.Schema.AbstractSchemaItem/6.0.0\" xmlns:dv=\"http://schemas.origam.com/Origam.Schema.DeploymentModel.DeploymentVersion/6.0.0\">");
        sb.appendLine("  <dv:DeploymentVersion");
        sb.appendLine("    asi:abstract=\"false\"");
        sb.appendLine("    dv:deploymentDependenciesCsv=\"&#xD;&#xA;147fa70d-6519-4393-b5d0-87931f9fd609, 5.1.4\"");
        sb.appendLine("    x:id=\""+guidId+"\"");
        sb.appendLine("    asi:name=\"1.0.0\"");
        sb.appendLine("    dv:version=\"1.0.0\" />");
        sb.appendLine("</x:file>");
    }
    return sb.toString();
}
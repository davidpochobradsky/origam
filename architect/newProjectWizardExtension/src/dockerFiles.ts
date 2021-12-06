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
const stringBuilder = require("node-stringbuilder");
import { pathFormat } from './function';
import fs = require('fs');
import { modelDirectory } from './directoryAction';
import { getBase64Data } from './base64';
import { unzipfile } from './fileaction';

export function createDockerEnvFile(rootProjectDirectory:string,projectName:string,databaseType:string,
    packageId:string,userPassword:string,origamVersion:string)
{
    const newProjectDir = pathFormat(rootProjectDirectory,"NewProject");
    const dockerData = new stringBuilder("");
    dockerData.appendLine("SA_PASSWORD=1Secure*Password1");
    dockerData.appendLine("DOCKER_TAG_SERVER="+origamVersion+"-latest");
    dockerData.appendLine("gitPullOnStart=false");
    dockerData.appendLine("OrigamSettings_SetOnStart=true");
    dockerData.appendLine("OrigamSettings_SchemaExtensionGuid=" + packageId);
    dockerData.appendLine("OrigamSettings_DbHost=host.docker.internal");
    dockerData.appendLine("OrigamSettings_DbPort=5544");
    dockerData.appendLine("OrigamSettings_DbUsername=origam");
    dockerData.appendLine("OrigamSettings_DbPassword="+userPassword);
    dockerData.appendLine("DatabaseName=origam");
    dockerData.appendLine("OrigamSettings_TitleName=" + projectName);
    dockerData.appendLine("DatabaseType=" + getDatabaseType(databaseType));
    dockerData.appendLine("ExternalDomain_SetOnStart=https://localhost:3000");
    
    let absoluteFilePath = pathFormat(newProjectDir,"envFile.env");
    fs.writeFileSync(absoluteFilePath,dockerData.toString());
}

export function createDockerOrigamFile(rootProjectDirectory:string,origamVersion:string)
{
    const newProjectDir = pathFormat(rootProjectDirectory,"NewProject");
    const dockerData = new stringBuilder("");
    dockerData.appendLine("FROM origam/server:"+origamVersion+" AS base");
    dockerData.appendLine("USER origam");
    dockerData.appendLine("WORKDIR /home/origam/HTML5");
    let absoluteFilePath = pathFormat(newProjectDir,"DockerFile.origamServer");
    fs.writeFileSync(absoluteFilePath,dockerData.toString());
}

function getDatabaseType(databaseType: string) {
    if(databaseType === "PostgreSQL")
    {
        return "postgresql";
    }
    if(databaseType === "MsSQL")
    {
        return "mssql";
    }
    return "";
}

export function createComposeFile(rootProjectDirectory: string, userPassword: string,databaseType: string) {
	const newProjectDir = pathFormat(rootProjectDirectory,"NewProject");
    const dockerData = new stringBuilder("");
    dockerData.appendLine("services:");
    dockerData.appendLine(createDatabasePart(databaseType));
    dockerData.appendLine(createOrigamPart());
    dockerData.appendLine(createHTTPSPart());
    dockerData.appendLine("volumes:");
    dockerData.appendLine("  myshare:");
    dockerData.appendLine("  https-proxy_node_modules:");
    dockerData.appendLine("  executor_node_modules:");
    dockerData.appendLine("  yarn_cache:");
    dockerData.appendLine("  npm_cache:");
    let absoluteFilePath = pathFormat(newProjectDir,"docker-compose.yml");
    fs.writeFileSync(absoluteFilePath,dockerData.toString());
}

function createDatabasePart(databaseType: string): any {
    if(databaseType === "MsSQL")
    {
        return createComposePartMssql();
    }
    return createComposePartPostgres();
}
function createOrigamPart(): any {
    const dockerData = new stringBuilder("");
    dockerData.appendLine("  server:");
    dockerData.appendLine("    build:");
    dockerData.appendLine("      dockerfile: ./Dockerfile.origamServer");
    dockerData.appendLine("      args:");
    dockerData.appendLine("        - DOCKER_TAG_SERVER=${DOCKER_TAG_SERVER}");
    dockerData.appendLine("    environment:");
    dockerData.appendLine("        - gitPullOnStart=${gitPullOnStart}");
    dockerData.appendLine("        - OrigamSettings_SetOnStart=${OrigamSettings_SetOnStart}");
    dockerData.appendLine("        - OrigamSettings_SchemaExtensionGuid=${OrigamSettings_SchemaExtensionGuid}");
    dockerData.appendLine("        - OrigamSettings_DbHost=${OrigamSettings_DbHost}");
    dockerData.appendLine("        - OrigamSettings_DbPort=${OrigamSettings_DbPort}");
    dockerData.appendLine("        - OrigamSettings_DbUsername=${OrigamSettings_DbUsername}");
    dockerData.appendLine("        - OrigamSettings_DbPassword=${OrigamSettings_DbPassword}");
    dockerData.appendLine("        - DatabaseName=${DatabaseName}");
    dockerData.appendLine("        - OrigamSettings_TitleName=${OrigamSettings_TitleName}");
    dockerData.appendLine("        - DatabaseType=${DatabaseType}");
    dockerData.appendLine("        - ExternalDomain_SetOnStart=${ExternalDomain_SetOnStart}");
    dockerData.appendLine("        - OrigamSettings_ReportDefinitionsPath=${OrigamSettings_ReportDefinitionsPath}");
    dockerData.appendLine("        - EnableChat=${EnableChat}");
    dockerData.appendLine("    ports:");
    dockerData.appendLine("      - \"3030:8080\"");
    dockerData.appendLine("    volumes:");
    dockerData.appendLine("      - "+modelDirectory+":/home/origam/HTML5/data/origam");
    dockerData.appendLine("    depends_on:");
    dockerData.appendLine("      - databasesql");
    dockerData.appendLine("    tty: true");
    return dockerData;
}

function createHTTPSPart(): any {
    const dockerData = new stringBuilder("");
    dockerData.appendLine("  server-https:");
    dockerData.appendLine("    build:");
    
    dockerData.appendLine("      dockerfile: ./server-https/Dockerfile");
    dockerData.appendLine("    ports:");
    dockerData.appendLine("      - \"3000:3000\"");
    dockerData.appendLine("    volumes:");
    dockerData.appendLine("      - https-proxy_node_modules:/sources/https-proxy/node_modules");
    dockerData.appendLine("      - yarn_cache:/usr/local/share/.cache/yarn/v6");
    dockerData.appendLine("      - npm_cache:/root/.npm");
    return dockerData;
}

function createComposePartMssql(): any {
    const dockerData = new stringBuilder("");
    dockerData.appendLine("  databasesql:");
    dockerData.appendLine("    build:");
    dockerData.appendLine("      dockerfile: .Dockerfile.mssql");
    dockerData.appendLine("    environment:");
    dockerData.appendLine("      - SA_PASSWORD=${SA_PASSWORD}");
    dockerData.appendLine("      - USER_PASSWORD=${OrigamSettings_DbPassword}");
    dockerData.appendLine("      - ACCEPT_EULA=Y");
    dockerData.appendLine("    ports:");
    dockerData.appendLine("      - \"5544:1433\"");
    dockerData.appendLine("    tty: true");
    return dockerData;
}

function createComposePartPostgres(): any {
    const dockerData = new stringBuilder("");
    dockerData.appendLine("  databasesql:");
    dockerData.appendLine("    build:");
    dockerData.appendLine("      dockerfile: ./Dockerfile.postgresql");
    dockerData.appendLine("    environment:");
    dockerData.appendLine("      - POSTGRES_PASSWORD=${SA_PASSWORD}");
    dockerData.appendLine("      - USER_PASSWORD=${OrigamSettings_DbPassword}");
    dockerData.appendLine("    ports:");
    dockerData.appendLine("      - \"5544:5432\"");
    dockerData.appendLine("    tty: true");
    return dockerData;
}

export async function createDockerHttpFiles(rootProjectDirectory:string)
{
    const newProjectDir = pathFormat(rootProjectDirectory,"NewProject");
    const uintArray: Uint8Array = new Uint8Array(Buffer.from(getBase64Data(), 'base64'));
    let absoluteFilePath = pathFormat(newProjectDir,"https.zip");

    fs.writeFileSync(absoluteFilePath,uintArray);
    await unzipfile(absoluteFilePath,newProjectDir);
}

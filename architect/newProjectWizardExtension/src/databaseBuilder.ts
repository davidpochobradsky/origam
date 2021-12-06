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
const stringBuilder = require("node-stringbuilder");
let newProjectDir = "";

export function createDatabaseDocker(databaseType:string,rootProjectDirectory:string)
{
    newProjectDir = pathFormat(rootProjectDirectory,"NewProject");
    if(databaseType === "PostgreSQL")
    {
        postgresFiles();
    }
    if(databaseType === "MsSQL")
    {
        mssqlFiles();
    }
}

function postgresFiles() {
    const dockerData = new stringBuilder("");
    dockerData.appendLine("from postgres:13 as base");
    dockerData.appendLine("WORKDIR /docker-entrypoint-initdb.d");
    dockerData.appendLine("COPY 01-pgscript.sh /docker-entrypoint-initdb.d/");
    let absoluteFilePath = pathFormat(newProjectDir,"DockerFile.postgresql");
    fs.writeFileSync(absoluteFilePath,dockerData.toString());
    const startScript = new stringBuilder("");
    startScript.appendLine("#!/bin/bash");
    startScript.appendLine("psql -c \"CREATE DATABASE origam  ENCODING ='UTF8'\"");
    startScript.appendLine("psql -d origam -c \"CREATE SCHEMA origam\" ");
    startScript.appendLine("psql -d origam -c \"CREATE EXTENSION pgcrypto SCHEMA origam\"");
    startScript.appendLine("psql -c \"CREATE USER origam WITH LOGIN NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION PASSWORD '$USER_PASSWORD'\"");
    startScript.appendLine("psql -c \"GRANT CONNECT ON DATABASE origam TO origam\"");
    startScript.appendLine("psql -c \"GRANT ALL PRIVILEGES ON DATABASE origam TO origam\"");
    startScript.appendLine("psql -d origam -c \"GRANT ALL ON SCHEMA origam TO origam WITH GRANT OPTION\"");
    absoluteFilePath = pathFormat(newProjectDir,"01-pgscript.sh");
    fs.writeFileSync(absoluteFilePath,startScript.toString());
}
function mssqlFiles() {
    const dockerData = new stringBuilder("");
    dockerData.appendLine("from mcr.microsoft.com/mssql/server:2019-latest as basefrom postgres:13 as base");
    dockerData.appendLine("RUN mkdir /tmp/mssql");
    dockerData.appendLine("WORKDIR /tmp/mssql");
    dockerData.appendLine("RUN touch /tmp/mssql/first");
    dockerData.appendLine("COPY --chown=mssql:root mssqlscript.sh /tmp/mssql");
    dockerData.appendLine("RUN chmod +x mssqlscript.sh");
    dockerData.appendLine("CMD /usr/bin/bash /tmp/mssql/mssqlscript.sh & /opt/mssql/bin/sqlservr");
    let absoluteFilePath = pathFormat(newProjectDir,"DockerFile.mssql");
    fs.writeFileSync(absoluteFilePath,dockerData.toString());
    const startScript = new stringBuilder("");
    startScript.appendLine("#!/bin/bash");
    startScript.appendLine("if [[ -f \"first\" ]]; then");
    startScript.appendLine("until /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P $SA_PASSWORD \\ ");
    startScript.appendLine("         -Q \"SELECT 1\"");
    startScript.appendLine("do ");
    startScript.appendLine("  sleep 1");
    startScript.appendLine("done ");
    startScript.appendLine("/opt/mssql-tools/bin/sqlcmd \\ ");
    startScript.appendLine("-S localhost -U SA -P $SA_PASSWORD \\ ");
    startScript.appendLine("-Q \"CREATE DATABASE origam\" ");
    startScript.appendLine("/opt/mssql-tools/bin/sqlcmd \\ ");
    startScript.appendLine("-S localhost -U SA -P $SA_PASSWORD \\ ");
    startScript.appendLine(" -Q \"CREATE LOGIN origam WITH PASSWORD = '$USER_PASSWORD'; USE origam;CREATE USER origam FOR LOGIN origam; EXEC sp_addrolemember N'db_owner', N'origam'\"");
    startScript.appendLine(" rm first");
    startScript.appendLine("fi");
    absoluteFilePath = pathFormat(newProjectDir,"mssqlscript.sh");
    fs.writeFileSync(absoluteFilePath,startScript.toString());
}


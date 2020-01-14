#region license
/*
Copyright 2005 - 2020 Advantage Solutions, s. r. o.

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
#endregion

using System;
using System.Collections;
using System.Data;
using Origam.DA.ObjectPersistence;
using Origam.Schema;

namespace Origam.Workbench.Services
{
    public class NullPersistenceService : IPersistenceService
    {
        public void InitializeService()
        {
        }

        public void UnloadService()
        {
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public IPersistenceProvider SchemaProvider => new NullPersistenceProvider();
        public IPersistenceProvider SchemaListProvider => new NullPersistenceProvider();

        public void LoadSchema(ArrayList extensions, bool append, bool loadDocumentation, bool loadDeploymentScripts,
            string transactionId)
        {
        }

        public SchemaExtension LoadSchema(Guid schemaExtension, bool loadDocumentation, bool loadDeploymentScripts,
            string transactionId)
        {
            throw new NotImplementedException();
        }

        public SchemaExtension LoadSchema(Guid schemaExtensionId, Guid extraExtensionId, bool loadDocumentation,
            bool loadDeploymentScripts, string transactionId)
        {
            throw new NotImplementedException();
        }

        public void LoadSchemaList()
        {
        }

        public void UpdateRepository()
        {
        }

        public bool IsRepositoryVersionCompatible()
        {
            throw new NotImplementedException();
        }

        public bool CanUpdateRepository()
        {
            throw new NotImplementedException();
        }

        public void ExportPackage(Guid extensionId, string fileName)
        {
        }

        public void MergePackage(Guid extensionId, DataSet data, string transcationId)
        {
        }

        public void MergeSchema(DataSet schema, Key activePackage)
        {
        }

        public void InitializeRepository()
        {
        }

        public void Dispose()
        {
        }
    }
}
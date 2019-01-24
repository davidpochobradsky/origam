﻿#region license
/*
Copyright 2005 - 2018 Advantage Solutions, s. r. o.

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
using Origam.DA;
using Origam.DA.ObjectPersistence;
using Origam.DA.Service;
using Origam.Workbench.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Origam.Rule
{
    public class ModelRules
    {
        public static List<Dictionary<IFilePersistent, string>> GetErrors(FilePersistenceService independentPersistenceService, CancellationToken cancellationToken)
        {
            List<Dictionary<IFilePersistent, string>> errorFragments = independentPersistenceService
                    .SchemaProvider
                    .RetrieveList<IFilePersistent>()
                    .Select(retrievedObj =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var errorMessages = RuleTools.GetExceptions(retrievedObj)
                            .Select(exception => exception.Message)
                            .ToList();
                        if (errorMessages.Count == 0) return null;

                        return new Dictionary<IFilePersistent, string>
                        {
                            { retrievedObj, string.Join("\n", errorMessages) }
                        };
                    })
                    .Where(x => x != null)
                    .ToList();
            return errorFragments;
        }
    }
}

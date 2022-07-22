﻿#region license

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

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Origam.DA;
using Origam.Schema.EntityModel;
using Origam.Workbench.Services;

namespace Origam.Rule;

public static class XsltFunctionContainerFactory
{
    public static IEnumerable<IXsltFunctionContainer> Create()
    {
        return Create(
            ServiceManager.Services.GetService<IBusinessServicesService>(),
            ServiceManager.Services.GetService<SchemaService>()
                .GetProvider<XsltFunctionSchemaItemProvider>(),
            ServiceManager.Services.GetService<IPersistenceService>(),
            ServiceManager.Services.GetService<IDataLookupService>(),
            ServiceManager.Services.GetService<IParameterService>(),
            ServiceManager.Services.GetService<IStateMachineService>(),
            ServiceManager.Services.GetService<ITracingService>(),
            ServiceManager.Services.GetService<IDocumentationService>(),
            SecurityManager.GetAuthorizationProvider(),
            SecurityManager.CurrentUserProfile);
    }

    public static IEnumerable<IXsltFunctionContainer> Create (
        IBusinessServicesService businessService,
        IXsltFunctionSchemaItemProvider xsltFunctionSchemaItemProvider,
        IPersistenceService persistence, IDataLookupService lookupService,
        IParameterService parameterService, IStateMachineService stateMachineService ,
        ITracingService tracingService, IDocumentationService documentationService,
        IOrigamAuthorizationProvider authorizationProvider, Func<UserProfile> userProfileGetter)
    {
        return xsltFunctionSchemaItemProvider
            .ChildItemsByType(XsltFunctionCollection.CategoryConst)
            .Cast<XsltFunctionCollection>()
            .Select(collection =>
            {
                object instantiatedObject = Reflector.InvokeObject(
                    collection.FullClassName,
                    collection.AssemblyName);
                if (!(instantiatedObject is IXsltFunctionContainer container))
                {
                    throw new Exception(
                        $"Referenced class {collection.FullClassName} from {collection.AssemblyName} does not implement interface {nameof(IXsltFunctionContainer)}");
                }

                if (instantiatedObject is IOrigamDependentXsltFunctionContainer
                    origamContainer)
                {
                    origamContainer.Persistence = persistence;
                    origamContainer.LookupService = lookupService;
                    origamContainer.ParameterService = parameterService;
                    origamContainer.StateMachineService = stateMachineService;
                    origamContainer.TracingService = tracingService;
                    origamContainer.DocumentationService = documentationService;
                    origamContainer.AuthorizationProvider = authorizationProvider;
                    origamContainer.UserProfileGetter = userProfileGetter;
                    origamContainer.BusinessService = businessService;
                }

                if (!string.IsNullOrWhiteSpace(collection.XslNameSpacePrefix))
                {
                    container.XslNameSpacePrefix = collection.XslNameSpacePrefix;
                }
                if (!string.IsNullOrWhiteSpace(collection.XslNameSpaceUri))
                {
                    container.XslNameSpaceUri = collection.XslNameSpaceUri;
                }
                return container;
            });
    }
}
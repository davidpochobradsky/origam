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
using Origam.DA;
using Origam.Workbench.Services;

namespace Origam.Rule;

public abstract class AbstractOrigamDependentXsltFunctionContainer:
    AbstractXsltFunctionContainer, IOrigamDependentXsltFunctionContainer
{
    public IPersistenceService Persistence { get; set; }
    public IDataLookupService LookupService { get; set; }
    public IParameterService ParameterService { get; set; }
    public IBusinessServicesService BusinessService { get; set; }
    public IStateMachineService StateMachineService { get; set; }
    public ITracingService TracingService { get; set; }
    public IDocumentationService DocumentationService { get; set; }
    public IOrigamAuthorizationProvider AuthorizationProvider { get; set; }
    public Func<UserProfile> UserProfileGetter { get; set; }
    public string XslNameSpacePrefix { get; set; }
    public string XslNameSpaceUri { get; set; }
}
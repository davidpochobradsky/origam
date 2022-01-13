#region license
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
using System.ComponentModel.DataAnnotations;
using Origam.ServerCore.Attributes;

namespace Origam.ServerCore.Model.UIService
{
    public class LookupListInput 
    {
        public Guid SessionFormIdentifier { get; set; } = Guid.Empty;
        public Guid DataStructureEntityId { get; set; }
        public string Entity { get; set; }
        [Required]
        public string[] ColumnNames { get; set; }
        [Required]
        public string Property { get; set; }
        public Guid Id { get; set; } = Guid.Empty;
        [RequiredNonDefault]
        public Guid LookupId { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
        public bool ShowUniqueValues { get; set; }
        public string SearchText { get; set; }
        [Range(-1, 10_000)]
        public int PageSize { get; set; } = -1;
        [Range(1, 10_000)]
        public int PageNumber { get; set; } = -1;
        [RequiredNonDefault]
        public Guid MenuId { get; set; }
    }
}

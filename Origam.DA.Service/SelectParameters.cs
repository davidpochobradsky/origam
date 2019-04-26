#region license
/*
Copyright 2005 - 2019 Advantage Solutions, s. r. o.

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
using System.Collections.Generic;
using Origam.Schema.EntityModel;

namespace Origam.DA.Service
{
    public class SelectParameters
    {
        public DataStructure DataStructure { get; set; }
        public DataStructureEntity Entity { get; set; }
        public DataStructureFilterSet Filter { get; set; }
        public DataStructureSortSet SortSet { get; set; }
        public Hashtable Parameters { get; set; }
        public bool Paging { get; set; }
        public string ColumnName { get; set; }
        public string CustomFilters { get; set; } = "";
        public int RowLimit { get; set; }
        public List<Tuple<string, string>> CustomOrdering { get; set; }
        public bool ForceDatabaseCalculation { get; set; }
    }
}
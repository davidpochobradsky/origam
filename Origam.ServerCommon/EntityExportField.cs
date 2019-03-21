#region license
/*
Copyright 2005 - 2019 Advantage Solutions, s. r. o.

This file is part of ORIGAM.

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with ORIGAM.  If not, see<http://www.gnu.org/licenses/>.
*/
#endregion

namespace Origam.Server
{
    public class EntityExportField
    {
        private string caption;
        private string fieldName;
        private string lookupId;
        private string format;
        private EntityExportPolymorphRules polymorphRules;

        public string Caption { get => caption; set => caption = value; }
        public string FieldName { get => fieldName; set => fieldName = value; }
        public string LookupId { get => lookupId; set => lookupId = value; }
        public string Format { get => format; set => format = value; }
        public EntityExportPolymorphRules PolymorphRules
        {
            get => polymorphRules; set => polymorphRules = value;
        }
    }
}

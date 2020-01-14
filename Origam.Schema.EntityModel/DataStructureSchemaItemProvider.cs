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

namespace Origam.Schema.EntityModel
{
	/// <summary>
	/// Summary description for EntityModelSchemaItemProvider.
	/// </summary>
	public class DataStructureSchemaItemProvider : AbstractSchemaItemProvider, ISchemaItemFactory
	{
		public DataStructureSchemaItemProvider()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region ISchemaItemProvider Members
		public override string RootItemType
		{
			get
			{
				return AbstractDataStructure.ItemTypeConst;
			}
		}
		public override bool AutoCreateFolder
		{
			get
			{
				return true;
			}
		}
		public override string Group
		{
			get
			{
				return "DATA";
			}
		}
		#endregion

		#region IBrowserNode Members

		public override string Icon
		{
			get
			{
				// TODO:  Add EntityModelSchemaItemProvider.ImageIndex getter implementation
				return "icon_07_data-structures.png";
			}
		}

		public override string NodeText
		{
			get
			{
				return "Data Structures";
			}
			set
			{
				base.NodeText = value;
			}
		}

		public override string NodeToolTipText
		{
			get
			{
				// TODO:  Add EntityModelSchemaItemProvider.NodeToolTipText getter implementation
				return null;
			}
		}

		#endregion

		#region ISchemaItemFactory Members

		public override Type[] NewItemTypes
		{
			get
			{
				return new Type[] {typeof(DataStructure), typeof(XsdDataStructure)};
			}
		}

		public override AbstractSchemaItem NewItem(Type type, Guid schemaExtensionId, SchemaItemGroup group)
		{
			AbstractDataStructure item;

			if(type == typeof(DataStructure))
			{
				item = new DataStructure(schemaExtensionId);
				item.Name = "NewDataStructure";

			}
			else if(type == typeof(XsdDataStructure))
			{
				item = new XsdDataStructure(schemaExtensionId);
				item.Name = "NewXsdDataStructure";

			}
			else
				throw new ArgumentOutOfRangeException("type", type, ResourceUtils.GetString("ErrorDataStructureUnknownType"));

			item.Group = group;
			item.RootProvider = this;
			item.PersistenceProvider = this.PersistenceProvider;
			this.ChildItems.Add(item);

			return item;
		}

		#endregion
	}
}

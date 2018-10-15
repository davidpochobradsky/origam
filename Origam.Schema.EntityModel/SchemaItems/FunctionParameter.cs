#region license
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

using System;
using Origam.DA.ObjectPersistence;
using System.Xml.Serialization;

namespace Origam.Schema.EntityModel
{
	/// <summary>
	/// Summary description for FunctionParameter.
	/// </summary>
	[SchemaItemDescription("Parameter", 15)]
    [HelpTopic("Function+Parameter")]
	[XmlModelRoot(ItemTypeConst)]
	public class FunctionParameter : AbstractSchemaItem
	{
		public const string ItemTypeConst = "FunctionParameter";

		public FunctionParameter() : base() {}

		public FunctionParameter(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public FunctionParameter(Key primaryKey) : base(primaryKey)	{}
	
		#region Overriden AbstractDataEntityColumn Members
		
		[EntityColumn("ItemType")]
		public override string ItemType
		{
			get
			{
				return ItemTypeConst;
			}
		}

		public override string Icon
		{
			get
			{
				return "15";
			}
		}

		public override SchemaItemCollection ChildItems
		{
			get
			{
				return new SchemaItemCollection();
			}
		}
		#endregion

		private int _ordinalPosition;
		[EntityColumn("I01")]
        [XmlAttribute("ordinalPosition")]
        public int OrdinalPosition
		{
			get
			{
				return _ordinalPosition;
			}
			set
			{
				_ordinalPosition = value;
			}
		}

	}
}

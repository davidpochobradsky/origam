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

namespace Origam.Schema
{
	/// <summary>
	/// Parameter that can be used to parametrize any kind of schema item.
	/// </summary>
	[SchemaItemDescription("Parameter", "Parameters", "icon_parameter.png")]
    [XmlModelRoot(ItemTypeConst)]
    public class SchemaItemParameter : AbstractSchemaItem
	{
		public const string ItemTypeConst = "Parameter";

		public SchemaItemParameter() : base() {}

		public SchemaItemParameter(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public SchemaItemParameter(Key primaryKey) : base(primaryKey)	{}

		#region Properties
		protected OrigamDataType _dataType;
		[EntityColumn("I01")]
        [XmlAttribute("dataType")]
		public virtual OrigamDataType DataType
		{
			get
			{
				return _dataType;
			}
			set
			{
				_dataType = value;
			}
		}
		private int _dataLength = 0;
		[EntityColumn("I02")]
        [XmlAttribute("dataLength")]
        public int DataLength
		{
			get
			{
				if(this.DataType == OrigamDataType.Date)
				{
					return 8;
				}
				else
				{
					return _dataLength;
				}
			}
			set
			{
				_dataLength = value;
			}
		}

		private bool _allowNulls = true;
		[EntityColumn("B01")]
        [XmlAttribute("allowNulls")]
        public bool AllowNulls
		{
			get
			{
				return _allowNulls;
			}
			set
			{
				_allowNulls = value;
			}
		}
		#endregion

		#region Overriden AbstractDataEntityColumn Members
		[EntityColumn("ItemType")]
		public override string ItemType
		{
			get
			{
				return ItemTypeConst;
			}
		}

		public override bool CanMove(Origam.UI.IBrowserNode2 newNode)
		{
			AbstractSchemaItem newItem = newNode as AbstractSchemaItem;
			if(newItem != null)
			{
				return newItem.ItemType.Equals(this.ParentItem.ItemType);
			}
			else
			{
				return newNode.GetType().Equals(this.ParentItem.GetType());
			}
		}

		#endregion
	}
}

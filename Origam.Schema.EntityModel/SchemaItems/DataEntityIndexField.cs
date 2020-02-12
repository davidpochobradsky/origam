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

using Origam.DA.Common;
using System;
using System.ComponentModel;

using Origam.DA.ObjectPersistence;
using System.Xml.Serialization;

namespace Origam.Schema.EntityModel
{
	/// <summary>
	/// Summary description for DataEntityIndexField.
	/// </summary>
	[SchemaItemDescription("Index Field", "Fields", "icon_index-field.png")]
    [HelpTopic("Index+Field")]
	[XmlModelRoot(ItemTypeConst)]
	[DefaultProperty("Field")]
    [ClassMetaVersion("1.0.0")]
    public class DataEntityIndexField : AbstractSchemaItem
	{
		public DataEntityIndexField() : base(){}
		
		public DataEntityIndexField(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public DataEntityIndexField(Key primaryKey) : base(primaryKey)	{}

		public const string ItemTypeConst = "DataEntityIndexField";

		#region Properties
		private DataEntityIndexSortOrder _sortOrder = DataEntityIndexSortOrder.Ascending;
		
		[EntityColumn("I01")]
		[NotNullModelElementRule()]
        [XmlAttribute("sortOrder")]
        public DataEntityIndexSortOrder SortOrder
		{
			get
			{
				return _sortOrder;
			}
			set
			{
				_sortOrder = value;
			}
		}

		private int _ordinalPosition;
		[EntityColumn("I02")] 
		[NotNullModelElementRule()]
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
				UpdateName();
			}
		}

		[EntityColumn("G01")]  
		public Guid ColumnId;

		[TypeConverter(typeof(EntityColumnReferenceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotNullModelElementRule()]
        [XmlReference("field", "ColumnId")]
        public IDataEntityColumn Field
		{
			get
			{
				ModelElementKey key = new ModelElementKey();
				key.Id = this.ColumnId;

				return (IDataEntityColumn)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), key);
			}
			set
			{
				this.ColumnId = (Guid)value.PrimaryKey["Id"];

				UpdateName();
			}
		}
		#endregion

		#region Overriden AbstractSchemaItem Members
		[Browsable(false)]
		public override bool UseFolders
		{
			get
			{
				return false;
			}
		}

		[EntityColumn("ItemType")]
		public override string ItemType
		{
			get
			{
				return ItemTypeConst;
			}
		}

		public override SchemaItemCollection ChildItems
		{
			get
			{
				return new SchemaItemCollection();
			}
		}

		public override void GetExtraDependencies(System.Collections.ArrayList dependencies)
		{
			dependencies.Add(this.Field);

			base.GetExtraDependencies (dependencies);
		}

		public override void UpdateReferences()
		{
			foreach(ISchemaItem item in this.RootItem.ChildItemsRecursive)
			{
				if(item.OldPrimaryKey != null)
				{
					if(item.OldPrimaryKey.Equals(this.Field.PrimaryKey))
					{
						this.Field = item as IDataEntityColumn;
						break;
					}
				}
			}

			base.UpdateReferences ();
		}
		#endregion

		private void UpdateName()
		{
			this.Name = this.OrdinalPosition.ToString() + "_" + (this.ColumnId == Guid.Empty ? "?" : this.Field.Name);
		}

		#region IComparable Members

		public override int CompareTo(object obj)
		{
			if(obj is DataEntityIndexField)
			{
				DataEntityIndexField f = obj as DataEntityIndexField;
				
				return this.OrdinalPosition.CompareTo(f.OrdinalPosition);
			}
			else
			{
				return base.CompareTo(obj);
			}
		}

		#endregion
	}
}

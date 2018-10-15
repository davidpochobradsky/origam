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
using System.Collections;
using System.ComponentModel;
using Origam.DA.ObjectPersistence;
using System.Xml.Serialization;

namespace Origam.Schema.EntityModel
{
	/// <summary>
	/// Summary description for EntityFilter.
	/// </summary>
	[SchemaItemDescription("Dependency", "Dependencies", 3)]
    [HelpTopic("Field+Dependencies")]
	[XmlModelRoot(ItemTypeConst)]
	[DefaultProperty("Field")]
    public class EntityFieldDependency : AbstractSchemaItem, ISchemaItemFactory
	{
		public const string ItemTypeConst = "EntityFieldDependency";

		public EntityFieldDependency() : base() {}

		public EntityFieldDependency(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public EntityFieldDependency(Key primaryKey) : base(primaryKey)	{}

		#region Overriden AbstractSchemaItem Members
		public override bool CanMove(Origam.UI.IBrowserNode2 newNode)
		{
			return newNode is IDataEntity;
		}

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
				return "3";
			}
		}

		public override bool UseFolders
		{
			get
			{
				return false;
			}
		}

        public override void GetExtraDependencies(ArrayList dependencies)
        {
            dependencies.Add(this.Field);
            base.GetExtraDependencies(dependencies);
        }
		#endregion

		#region Properties
		[EntityColumn("G01")]  
		public Guid FieldId;

		[Category("Reference")]
		[TypeConverter(typeof(EntityColumnReferenceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotNullModelElementRule()]
        [XmlReference("field", "FieldId")]
        public IDataEntityColumn Field
		{
			get
			{
				ModelElementKey key = new ModelElementKey();
				key.Id = this.FieldId;

				return (AbstractSchemaItem)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), key) as IDataEntityColumn;
			}
			set
			{
				this.FieldId = (Guid)value.PrimaryKey["Id"];

				this.Name = this.Field.Name;
			}
		}
		#endregion
	}
}

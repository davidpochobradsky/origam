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

using Origam.DA;
using Origam.DA.ObjectPersistence;
using System.Xml.Serialization;

namespace Origam.Schema.EntityModel
{
	/// <summary>
	/// Implementation of LookupField.
	/// </summary>
	[SchemaItemDescription("Lookup Field", "Fields", 50)]
    [HelpTopic("Lookup+Field")]
	[XmlModelRoot(ItemTypeConst)]
	public class LookupField : AbstractSchemaItem, IDataEntityColumn
	{

		public const string ItemTypeConst = "DataEntityColumn";
		public LookupField() : base() {}

		public LookupField(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public LookupField(Key primaryKey) : base(primaryKey)	{}

		#region IDataEntityColumn Members
		
		[Browsable(false)]
		public OrigamDataType DataType
		{
			get
			{
				if(this.Lookup == null)
				{
					return OrigamDataType.Boolean;
				}
				else if (this.Lookup.ValueDisplayMember.Contains(";"))
				{
					// concatenated lookup field
					return OrigamDataType.String;
				}
				else if (this.Lookup.ValueDisplayColumn == null)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format("ValueDisplayMember {0} not found in lookup {1}.", 
                        Lookup.ValueDisplayMember, Lookup.NodeId));
                }
				else
                {
					return this.Lookup.ValueDisplayColumn.DataType;
				}
			}
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public int DataLength
		{
			get
			{
				if(this.Lookup == null)
				{
					return 0;
				}
                else if(this.Lookup.ValueColumn == null)
                {
                    throw new ArgumentOutOfRangeException(
                        string.Format("ValueValueMember {0} not found in lookup {1}.",
                        Lookup.ValueValueMember, Lookup.NodeId));
                }
                else
				{
					return this.Lookup.ValueColumn.FinalColumn.Field.DataLength;
				}
			}
			set => throw new NotSupportedException();
		}

		[Category("Entity Column"), DefaultValue(true)]
		[EntityColumn("B01")]
		[XmlAttribute ("allowNulls")]
		[Description("Indicates if the field allows empty values or not. If set to False, also the database column will be generated so that it does not allow nulls. In the user interface the user will have to enter a value before saving the record.")]
		public bool AllowNulls { get; set; } = true;

		[Category("Entity Column"), DefaultValue(false)]
		[EntityColumn("B02")]
		[XmlAttribute ("isPrimaryKey")]
		[Description("Indicates if the field is a primary key. If set to True, also a database primary key is generated. IMPORTANT: Every entity should have a primary key specified, otherwise data merges will not be able to correlate existing records. NOTE: Multi-column primary keys are possible but GUI expects always only single-column primary keys.")]
		public bool IsPrimaryKey { get; set; } = false;

		[Category("Entity Column")]
		[EntityColumn("SS01")]
		[XmlAttribute ("caption")]
		[Localizable(true)]
		[Description("Default label for the field in a GUI. Audit log viewer also gets the field names from here.")]
		public string Caption { get; set; } = "";

		[Category("Entity Column"), DefaultValue(false)]
		[EntityColumn("B04")]
		[XmlAttribute ("excludeFromAllFields")]
		[Description("If set to True, the field will not be included in the list of fields in a Data Structure if 'AllFields=True' is set in a Data Structure Entity. This is useful e.g. for database function calls that are expensive and used only for lookups that would otherwise slow down the system if loaded e.g. to forms.")]
		public bool ExcludeFromAllFields { get; set; } = false;

		[Browsable(false)]
		public bool AutoIncrement
		{
			get => false;
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public long AutoIncrementSeed
		{
			get => 0;
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public long AutoIncrementStep
		{
			get => 1;
			set => throw new NotSupportedException();
		}

		[EntityColumn("G01")]  
		public Guid DefaultLookupId;

		[Browsable(false)]
		[XmlReference("defaultLookup", "DefaultLookupId")]
		public IDataLookup DefaultLookup
		{
			get
			{
				ModelElementKey key = new ModelElementKey();
				key.Id = this.DefaultLookupId;

				try
				{
					return (IDataLookup)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), key);
				}
				catch
				{
					return null;
				}
			}
			set
			{
				if(value == null)
				{
					this.DefaultLookupId = Guid.Empty;
				}
				else
				{
					this.DefaultLookupId = (Guid)value.PrimaryKey["Id"];
				}
			}
		}

		[TypeConverter(typeof(DataLookupConverter))]
		[Category("Lookup")]
		[Description("Lookup to be used by the data service to lookup value by the provided Field.")]
		[NotNullModelElementRule()]
		public IDataLookup Lookup
		{
			get => this.DefaultLookup;
			set => this.DefaultLookup = value;
		}

		[Browsable(false)]
		public IDataEntity ForeignKeyEntity
		{
			get => null;
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public IDataEntityColumn ForeignKeyField
		{
			get => null;
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public DataConstant DefaultValue
		{
			get => null;
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public SchemaItemParameter DefaultValueParameter
		{
			get => null;
			set => throw new NotSupportedException();
		}


		[Category("Entity Column"), DefaultValue(EntityColumnXmlMapping.Attribute)]
		[EntityColumn("I03")]
		[XmlAttribute ("xmlMappingType")]
		public EntityColumnXmlMapping XmlMappingType { get; set; } = EntityColumnXmlMapping.Attribute;

		[Browsable(false)]
		public OnCopyActionType OnCopyAction
		{
			get => OnCopyActionType.Copy;
			set => throw new NotSupportedException();
		}

		[Browsable(false)]
		public ArrayList RowLevelSecurityRules => this.ChildItemsByType(AbstractEntitySecurityRule.ItemTypeConst);

		[Browsable(false)]
		public ArrayList ConditionalFormattingRules => this.ChildItemsByType(EntityConditionalFormatting.ItemTypeConst);

		[Browsable(false)]
		public ArrayList DynamicLabels => this.ChildItemsByType(EntityFieldDynamicLabel.ItemTypeConst);
		#endregion

		#region Properties
		[EntityColumn("G05")]  
		public Guid FieldId;

		[TypeConverter(typeof(EntityColumnReferenceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotNullModelElementRule()]
		[Category("Lookup")]
        [XmlReference("field", "FieldId")]
        public IDataEntityColumn Field
		{
			get => (IDataEntityColumn)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), new ModelElementKey(this.FieldId));
			set => this.FieldId = (Guid)value.PrimaryKey["Id"];
		}

        [Browsable(false)]
        public DataEntityConstraint ForeignKeyConstraint => null;
		#endregion

        #region Overriden AbstractSchemaItem Methods
        [Browsable(false)]
		public bool ReadOnly => false;

		public override string Icon => "50";

		public override bool CanMove(Origam.UI.IBrowserNode2 newNode) => newNode is IDataEntity;

		[EntityColumn("ItemType")]
		public override string ItemType => AbstractDataEntityColumn.ItemTypeConst;

		public override void GetExtraDependencies(ArrayList dependencies)
		{
			if(this.DefaultLookup != null) dependencies.Add(this.DefaultLookup);
			if(this.DefaultValue != null) dependencies.Add(this.DefaultValue);
			if(this.DefaultValueParameter != null) dependencies.Add(this.DefaultValueParameter);
			if(this.Field != null) dependencies.Add(this.Field);

			base.GetExtraDependencies (dependencies);
		}

		public override void GetParameterReferences(AbstractSchemaItem parentItem, System.Collections.Hashtable list)
		{
			return;
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

		#region ISchemaItemFactory Members
		[Browsable(false)]
		public override Type[] NewItemTypes =>
			new Type[] {
				typeof(EntityFieldSecurityRule),
				typeof(EntityFieldDependency),
				typeof(EntityConditionalFormatting),
				typeof(EntityFieldDynamicLabel)
			};

		public override AbstractSchemaItem NewItem(Type type, Guid schemaExtensionId, SchemaItemGroup group)
		{
			AbstractSchemaItem item;

			if(type == typeof(EntityFieldSecurityRule))
			{
				item = new EntityFieldSecurityRule(schemaExtensionId);
				item.Name = "NewRowLevelSecurityRule";
			}
			else if(type == typeof(EntityFieldDependency))
			{
				item = new EntityFieldDependency(schemaExtensionId);
				item.Name = "NewEntityFieldDependency";
			}
			else if(type == typeof(EntityConditionalFormatting))
			{
				item = new EntityConditionalFormatting(schemaExtensionId);
				item.Name = "NewFormatting";
			}
			else if(type == typeof(EntityFieldDynamicLabel))
			{
				item = new EntityFieldDynamicLabel(schemaExtensionId);
				item.Name = "NewDynamicLabel";
			}
			else
				throw new ArgumentOutOfRangeException("type", type, ResourceUtils.GetString("ErrorDataEntityUnknownType"));

			item.Group = group;
			item.PersistenceProvider = this.PersistenceProvider;
			this.ChildItems.Add(item);
			return item;
		}
		#endregion
	}
}

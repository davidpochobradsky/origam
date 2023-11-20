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

using Origam.DA.Common;
using System;
using System.Collections;
using System.ComponentModel;

using Origam.DA.ObjectPersistence;
using System.Xml.Serialization;

namespace Origam.Schema.EntityModel
{
	[SchemaItemDescription("Rule", "icon_rule.png")]
    [HelpTopic("Rule+Set+Rule")]
	[XmlModelRoot(CategoryConst)]
    [ClassMetaVersion("6.0.0")]
	public class DataStructureRule : AbstractSchemaItem
	{
		public const string CategoryConst = "DataStructureRule";

		public DataStructureRule() {}
		
		public DataStructureRule(Guid schemaExtensionId) 
			: base(schemaExtensionId) {}

		public DataStructureRule(Key primaryKey) : base(primaryKey)	{}

		#region Properties
		public ArrayList RuleDependencies => ChildItemsByType(
			DataStructureRuleDependency.CategoryConst);

		private int _priority = 100;
		
		[DefaultValue(100)]
        [XmlAttribute("priority")]
		public int Priority
		{
			get => _priority;
			set => _priority = value;
		}
		
		public Guid DataStructureEntityId;

		private string entityName;

		public String EntityName
		{
			get
			{
				if (entityName == null)
				{
					entityName = Entity.Name;
				}
				return entityName;
			}
		}

		[TypeConverter(typeof(DataQueryEntityConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
        [XmlReference("entity", "DataStructureEntityId")]
        public DataStructureEntity Entity
		{
			get => (DataStructureEntity)PersistenceProvider.RetrieveInstance(
				typeof(DataStructureEntity), 
				new ModelElementKey(DataStructureEntityId));
			set
			{
				DataStructureEntityId = (value == null) 
					? Guid.Empty : (Guid)value.PrimaryKey["Id"];
				TargetField = null;
			}
		}
        
		public Guid TargetFieldId;

		[TypeConverter(typeof(DataStructureEntityFieldConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
        [XmlReference("targetField", "TargetFieldId")]
		public IDataEntityColumn TargetField
		{
			get => (IDataEntityColumn)PersistenceProvider.RetrieveInstance(
				typeof(AbstractSchemaItem), new ModelElementKey(TargetFieldId));
			set => TargetFieldId = (value == null) 
				? Guid.Empty : (Guid)value.PrimaryKey["Id"];
		}
		
		public Guid ValueRuleId;

		[TypeConverter(typeof(DataRuleConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
        [XmlReference("valueRule", "ValueRuleId")]
		[NotNullModelElementRule]
		public IDataRule ValueRule
		{
			get => (IDataRule)PersistenceProvider.RetrieveInstance(
				typeof(AbstractSchemaItem), new ModelElementKey(ValueRuleId));
			set => ValueRuleId = (value == null) 
				? Guid.Empty : (Guid)value.PrimaryKey["Id"];
		}
		
		public Guid CheckRuleId;

		[TypeConverter(typeof(StartRuleConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
        [XmlReference("conditionRule", "CheckRuleId")]
		public IStartRule ConditionRule
		{
			get => (IStartRule)PersistenceProvider.RetrieveInstance(
				typeof(AbstractSchemaItem), new ModelElementKey(CheckRuleId));
			set => CheckRuleId = (value == null) 
				? Guid.Empty : (Guid)value.PrimaryKey["Id"];
		}
		#endregion

		#region Overriden AbstractSchemaItem Members
		
		public override string ItemType => CategoryConst;

		public override void GetExtraDependencies(ArrayList dependencies)
		{
			dependencies.Add(Entity);
			if(TargetField != null)
			{
				dependencies.Add(TargetField);
			}
			if(ValueRule != null)
			{
				dependencies.Add(ValueRule);
			}
			if(ConditionRule != null)
			{
				dependencies.Add(ConditionRule);
			}
			base.GetExtraDependencies (dependencies);
		}

		public override void UpdateReferences()
		{
			foreach(ISchemaItem item in RootItem.ChildItemsRecursive)
			{
				if(item.OldPrimaryKey?.Equals(Entity.PrimaryKey) == true)
				{
					var targetFieldBck = TargetField;
					// setting the Entity normally resets TargetField as well
					Entity = item as DataStructureEntity;
					TargetField = targetFieldBck;
					break;
				}
			}

			base.UpdateReferences ();
		}

		public override bool CanMove(UI.IBrowserNode2 newNode)
		{
			return newNode.Equals(ParentItem);
		}

		#endregion

		#region ISchemaItemFactory Members

		public override Type[] NewItemTypes => new[]
		{
			typeof(DataStructureRuleDependency)
		};

		public override T NewItem<T>(
			Guid schemaExtensionId, SchemaItemGroup group)
		{
			return base.NewItem<T>(schemaExtensionId, group,
				typeof(T) == typeof(DataStructureRuleDependency)
					? "NewDependency" : null);
		}
	#endregion
	}
}

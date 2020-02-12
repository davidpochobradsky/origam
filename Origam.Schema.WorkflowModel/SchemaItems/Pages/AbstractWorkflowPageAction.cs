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
using System.Xml.Serialization;
using Origam.DA.ObjectPersistence;

using Origam.Schema.EntityModel;

namespace Origam.Schema.WorkflowModel
{
	/// <summary>
	/// Summary description for ServiceMethod.
	/// </summary>
	[SchemaItemDescription("Action", "Actions", 18)]
	[XmlModelRoot(ItemTypeConst)]
    [ClassMetaVersion("1.0.0")]
    public class AbstractWorkflowPageAction : AbstractSchemaItem
	{
		public const string ItemTypeConst = "WorkflowPageAction";

		public AbstractWorkflowPageAction() : base() {Init();}
		public AbstractWorkflowPageAction(Guid schemaExtensionId) : base(schemaExtensionId) {Init();}
		public AbstractWorkflowPageAction(Key primaryKey) : base(primaryKey) {Init();}

		private void Init()
		{
			this.ChildItemTypes.Add(typeof(WorkflowPageActionParameter));
		}

		#region Properties
		[EntityColumn("G01")]  
		public Guid ConditionRuleId;

		[Category("Conditions")]
		[TypeConverter(typeof(StartRuleConverter))]
        [XmlReference("conditionRule", "ConditionRuleId")]
		public IStartRule ConditionRule
		{
			get
			{
                return (IStartRule)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), new ModelElementKey(this.ConditionRuleId));
			}
			set
			{
				this.ConditionRuleId = (value == null ? Guid.Empty : (Guid)value.PrimaryKey["Id"]);
			}
		}

		private int _sortOrder = 100;
		[EntityColumn("I01")]
		[DefaultValue(100)]
        [XmlAttribute("sortOrder")]
		public int SortOrder
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

		private string _roles = "*";
		[Category("Conditions")]
		[EntityColumn("SS01")]
        [NotNullModelElementRule()]
        [DefaultValue("*")]
        [XmlAttribute("roles")]
        public string Roles
		{
			get
			{
				return _roles;
			}
			set
			{
				_roles = value;
			}
		}

		private string _features;
		[Category("Conditions")]
		[EntityColumn("SS02")]
        [XmlAttribute("features")]
		public string Features
		{
			get
			{
				return _features;
			}
			set
			{
				_features = value;
			}
		}		
		#endregion

		#region Overriden AbstractSchemaItem Members
		
		[EntityColumn("ItemType")]
		public override string ItemType
		{
			get
			{
				return ItemTypeConst;
			}
		}

		public override bool UseFolders
		{
			get
			{
				return false;
			}
		}

		public override void GetExtraDependencies(System.Collections.ArrayList dependencies)
		{
			base.GetExtraDependencies (dependencies);

			if(this.ConditionRule != null) dependencies.Add(this.ConditionRule);
		}

		#endregion

		#region IComparable Members
		public override int CompareTo(object obj)
		{
			AbstractWorkflowPageAction compareItem = obj as AbstractWorkflowPageAction;

			if(compareItem == null) throw new InvalidCastException(ResourceUtils.GetString("ErrorCompareAbstractWorkflowPageAction"));

			return this.SortOrder.CompareTo(compareItem.SortOrder);
		}
		#endregion
	}
}

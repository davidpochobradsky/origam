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
using System.ComponentModel;
using System.Xml.Serialization;
using Origam.DA.ObjectPersistence;
using Origam.Schema.EntityModel;

namespace Origam.Schema.WorkflowModel
{
	/// <summary>
	/// Summary description for ContextStore.
	/// </summary>
	[SchemaItemDescription("Input Mapping", "Input Mappings", 17)]
	[XmlModelRoot(ItemTypeConst)]
	public class WorkQueueClassEntityMapping : AbstractSchemaItem, IComparable
	{
		public const string ItemTypeConst = "WorkQueueClassEntityMapping";

		public WorkQueueClassEntityMapping() : base() {}
		public WorkQueueClassEntityMapping(Guid schemaExtensionId) : base(schemaExtensionId) {}
		public WorkQueueClassEntityMapping(Key primaryKey) : base(primaryKey)	{}

		#region Overriden AbstractSchemaItem Members
		
		[EntityColumn("ItemType")]
		public override string ItemType => ItemTypeConst;

		public override string Icon => "17";

		public override void GetExtraDependencies(System.Collections.ArrayList dependencies)
		{
			XsltDependencyHelper.GetDependencies(this, dependencies, this.XPath);

			base.GetExtraDependencies (dependencies);
		}

		public override SchemaItemCollection ChildItems => new SchemaItemCollection();

		public override bool CanMove(Origam.UI.IBrowserNode2 newNode) => newNode.GetType().Equals(this.ParentItem.GetType());
		#endregion

		#region Properties
//		[EntityColumn("G01")]  
//		public Guid FieldId;
//
//		[TypeConverter(typeof(WorkQueueClassEntityMappingFieldConverter))]
//		[RefreshProperties(RefreshProperties.Repaint)]
//		public IDataEntityColumn Field
//		{
//			get
//			{
//				return (IDataEntityColumn)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), new ModelElementKey(this.FieldId));
//			}
//			set
//			{
//				this.FieldId = value == null ? Guid.Empty : (Guid)value.PrimaryKey["Id"];
//			}
//		}

		[EntityColumn("LS01")] 
		[XmlAttribute ("xPath")]
		public string XPath { get; set; }

		[EntityColumn("SS01")] 
		[XmlAttribute ("formatPattern")]
		public string FormatPattern { get; set; }

		[Category("GUI")]
		[EntityColumn("I01")]
		[XmlAttribute ("sortOrder")]
		public int SortOrder { get; set; }
		#endregion

		#region IComparable Members
		public override int CompareTo(object obj)
		{
			WorkQueueClassEntityMapping compareItem = obj as WorkQueueClassEntityMapping;
			if(compareItem == null)
            {
                return base.CompareTo(obj);
            }
            else
            {
                return this.SortOrder.CompareTo(compareItem.SortOrder);
            }
		}
		#endregion
	}
}

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

namespace Origam.Schema.GuiModel
{
	/// <summary>
	/// Summary description for RuleReference.
	/// </summary>
	[SchemaItemDescription("Report Reference", 29)]
    [HelpTopic("Report+Reference")]
    [DefaultProperty("Report")]
	[XmlModelRoot(ItemTypeConst)]
    public class ReportReference : AbstractSchemaItem
	{
		public const string ItemTypeConst = "ReportReference";

		public ReportReference() : base() {}

		public ReportReference(Guid schemaExtensionId) : base(schemaExtensionId) {}

		public ReportReference(Key primaryKey) : base(primaryKey)	{}
	
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
				return "29";
			}
		}

		public override void GetParameterReferences(AbstractSchemaItem parentItem, System.Collections.Hashtable list)
		{
			if(this.Report != null)
				base.GetParameterReferences(this.Report as AbstractSchemaItem, list);
		}

		public override void GetExtraDependencies(System.Collections.ArrayList dependencies)
		{
			dependencies.Add(this.Report);

			base.GetExtraDependencies (dependencies);
		}

		public override SchemaItemCollection ChildItems
		{
			get
			{
				return new SchemaItemCollection();
			}
		}
		#endregion

		#region Properties
		[EntityColumn("G01")]  
		public Guid ReportId;

		[Category("Reference")]
		[TypeConverter(typeof(ReportConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
        [XmlReference("report", "ReportId")]
		public AbstractReport Report
		{
			get
			{
				ModelElementKey key = new ModelElementKey();
				key.Id = this.ReportId;

				return (AbstractSchemaItem)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), key) as AbstractReport;
			}
			set
			{
				this.ReportId = (Guid)value.PrimaryKey["Id"];

				//this.Name = this.Report.Name;
			}
		}
		#endregion
	}
}

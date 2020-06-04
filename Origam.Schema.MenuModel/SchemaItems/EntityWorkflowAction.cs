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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using Origam.DA.ObjectPersistence;
using Origam.Schema.GuiModel;
using Origam.Schema.WorkflowModel;

using Origam.Workbench.Services;

namespace Origam.Schema.MenuModel
{
	/// <summary>
	/// Summary description for EntitySecurityRule.
	/// </summary>
	[SchemaItemDescription("Sequential Workflow Action", "UI Actions",
        "icon_sequential-workflow-action.png")]
    [HelpTopic("Sequential+Workflow+Action")]
	public class EntityWorkflowAction : EntityUIAction
	{
		public EntityWorkflowAction() : base() { Init();}

		public EntityWorkflowAction(Guid schemaExtensionId) : base(schemaExtensionId) { Init();}

		public EntityWorkflowAction(Key primaryKey) : base(primaryKey) { Init();}

		private void Init()
		{
			ChildItemTypes.Add(typeof(EntityWorkflowActionScriptCall));
		}
	
		#region Overriden AbstractDataEntityColumn Members
		
		public override void GetExtraDependencies(System.Collections.ArrayList dependencies)
		{
			dependencies.Add(this.Workflow);
			base.GetExtraDependencies (dependencies);
		}

		public override IList<string> NewTypeNames
		{
			get
			{
				try
				{
					IBusinessServicesService agents = ServiceManager.Services.GetService(typeof(IBusinessServicesService)) as IBusinessServicesService;
					IServiceAgent agent = agents.GetAgent("WorkflowService", null, null);
					return agent.ExpectedParameterNames(this.Workflow as AbstractSchemaItem, "ExecuteWorkflow", "Parameters");
				}
				catch
				{
					return new string[] {};
				}
			}
		}
		#endregion

		#region Properties
		private ServiceOutputMethod _mergeType = ServiceOutputMethod.AppendMergeExisting;

		[DefaultValue(ServiceOutputMethod.AppendMergeExisting)]
		[EntityColumn("I08")]
        [XmlAttribute("mergeType")]
		public ServiceOutputMethod MergeType
		{
			get
			{
				return _mergeType;
			}
			set
			{
				_mergeType = value;
			}
		}

		private bool _saveAfterWorkflow = false;

		[DefaultValue(false)]
		[EntityColumn("B03")]
        [XmlAttribute("saveAfterWorkflow")]
		public bool SaveAfterWorkflow
		{
			get
			{
				return _saveAfterWorkflow;
			}
			set
			{
				_saveAfterWorkflow = value;
			}
		}

		private bool _requestSaveBeforeWorkflow = false;

		[DefaultValue(false)]
		[EntityColumn("B04")]
        [XmlAttribute("requestSaveBeforeWorkflow")]
        public bool RequestSaveBeforeWorkflow
		{
			get
			{
				return _requestSaveBeforeWorkflow;
			}
			set
			{
				_requestSaveBeforeWorkflow = value;
			}
		}

		private bool _commitChangesAfterMerge = false;

		[DefaultValue(false)]
		[EntityColumn("B05")]
        [XmlAttribute("commitChangesAfterMerge")]
        public bool CommitChangesAfterMerge
		{
			get
			{
				return _commitChangesAfterMerge;
			}
			set
			{
				_commitChangesAfterMerge = value;
			}
		}

		private bool _cleanDataBeforeMerge = false;

		[DefaultValue(false)]
		[EntityColumn("B06")]
        [XmlAttribute("cleanDataBeforeMerge")]
        public bool CleanDataBeforeMerge
		{
			get
			{
				return _cleanDataBeforeMerge;
			}
			set
			{
				_cleanDataBeforeMerge = value;
			}
		}

        private bool _refreshPortalAfterFinish = false;
        [DefaultValue(false)]
        [EntityColumn("B07")]
        [Description("If true, the client will refresh its menu after finishing the action.")]
        [XmlAttribute("refreshPortalAfterFinish")]
        public bool RefreshPortalAfterFinish
        {
            get
            {
                return _refreshPortalAfterFinish;
            }
            set
            {
                _refreshPortalAfterFinish = value;
            }
        }

        private ModalDialogCloseType _closeType = ModalDialogCloseType.None;

		[DefaultValue(ModalDialogCloseType.None)]
		[EntityColumn("I09")]
        [XmlAttribute("closeType")]
        public ModalDialogCloseType CloseType
		{
			get
			{
				return _closeType;
			}
			set
			{
				_closeType = value;
			}
		}

		private SaveRefreshType _refreshAfterWorkflow = SaveRefreshType.RefreshChangedRecords;

		[DefaultValue(SaveRefreshType.RefreshChangedRecords)]
		[EntityColumn("I07")]
        [XmlAttribute("refreshAfterWorkflow")]
        public SaveRefreshType RefreshAfterWorkflow
		{
			get
			{
				return _refreshAfterWorkflow;
			}
			set
			{
				_refreshAfterWorkflow = value;
			}
		}

		[EntityColumn("G05")]  
		public Guid WorkflowId;

		[Category("References")]
		[TypeConverter(typeof(WorkflowConverter))]
		[NotNullModelElementRule()]
        [XmlReference("workflow", "WorkflowId")]
		public IWorkflow Workflow
		{
			get
			{
				return (IWorkflow)this.PersistenceProvider.RetrieveInstance(typeof(AbstractSchemaItem), new ModelElementKey(this.WorkflowId));
			}
			set
			{
				this.WorkflowId = (value == null ? Guid.Empty : (Guid)value.PrimaryKey["Id"]);
			}
		}
		#endregion
	}
}

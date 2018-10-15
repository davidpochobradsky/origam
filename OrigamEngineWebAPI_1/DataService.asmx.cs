using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

using Origam.Workbench.Services;
using Origam.Schema.EntityModel;
using Origam.DA;
using Origam.DA.Service;
using core = Origam.Workbench.Services.CoreServices;
using System.Xml;
using Origam;
using System.Threading;
using Origam.Schema;

namespace OrigamEngineWebAPI
{
	/// <summary>
	/// Summary description for Service1.
	/// </summary>
	[WebService(Namespace="http://origamenginewebapi.advantages.cz/", Description="Service used to communicate with ORIGAM data sources. Using data service you can load, store or quickly lookup data.")]
	public class DataService : System.Web.Services.WebService
	{
		public DataService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}


		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		#region Web Methods

		[WebMethod(Description = "Loads data with optionally passing any number of parameters.")]
		public DataSet LoadData(string dataStructureId, string filterId, string defaultSetId, string sortSetId, QueryParameterCollection parameters)
		{
			Guid dsId = new Guid(dataStructureId);
			Guid fId = (filterId == String.Empty ? Guid.Empty : new Guid(filterId));
			Guid dId = (defaultSetId == String.Empty ? Guid.Empty : new Guid(defaultSetId));
			Guid sId = (sortSetId == String.Empty ? Guid.Empty : new Guid(sortSetId));

			return core.DataService.LoadData(dsId, fId, dId, sId, null, parameters);
		}

		[WebMethod(MessageName = "LoadData0", Description = "Loads data without passing any parameters.")]
		public DataSet LoadData(string dataStructureId, string filterId, string sortSetId, string defaultSetId)
		{
			Guid dsId = new Guid(dataStructureId);
			Guid fId = (filterId == String.Empty ? Guid.Empty : new Guid(filterId));
			Guid dId = (defaultSetId == String.Empty ? Guid.Empty : new Guid(defaultSetId));
			Guid sId = (sortSetId == String.Empty ? Guid.Empty : new Guid(sortSetId));

			return core.DataService.LoadData(dsId, fId, dId, sId, null);
		}

		[WebMethod(MessageName = "LoadData1", Description = "Loads data by passing 1 parameter.")]
		public DataSet LoadData(string dataStructureId, string filterId, string defaultSetId, string sortSetId, string paramName1, string paramValue1)
		{
			Guid dsId = new Guid(dataStructureId);
			Guid fId = (filterId == String.Empty ? Guid.Empty : new Guid(filterId));
			Guid dId = (defaultSetId == String.Empty ? Guid.Empty : new Guid(defaultSetId));
			Guid sId = (sortSetId == String.Empty ? Guid.Empty : new Guid(sortSetId));

			return core.DataService.LoadData(dsId, fId, dId, sId, null, paramName1, paramValue1);
		}

		[WebMethod(MessageName = "LoadData2", Description = "Loads data by passing 2 parameters.")]
		public DataSet LoadData(string dataStructureId, string filterId, string defaultSetId, string sortSetId, string paramName1, string paramValue1, string paramName2, string paramValue2)
		{
			Guid dsId = new Guid(dataStructureId);
			Guid fId = (filterId == String.Empty ? Guid.Empty : new Guid(filterId));
			Guid dId = (defaultSetId == String.Empty ? Guid.Empty : new Guid(defaultSetId));
			Guid sId = (sortSetId == String.Empty ? Guid.Empty : new Guid(sortSetId));

			return core.DataService.LoadData(dsId, fId, dId, sId, null, paramName1, paramValue1, paramName2, paramValue2);
		}

		[WebMethod(Description = "Executes procedure on the data store.")]
		public DataSet ExecuteProcedure(string procedureName, QueryParameterCollection parameters)
		{
			return core.DataService.ExecuteProcedure(procedureName, parameters, null);
		}

        [WebMethod(Description="Stores data to the data store, optionally returning actual values after storing is completed.")]
        public DataSet StoreData(string dataStructureId, DataSet data, bool loadActualValuesAfterUpdate)
        {
            Guid guid = new Guid(dataStructureId);
            IPersistenceService service = ServiceManager.Services.GetService(typeof(IPersistenceService)) as IPersistenceService;
            DataStructure structure = service.SchemaProvider.RetrieveInstance(typeof(DataStructure), new ModelElementKey(guid)) as DataStructure;
            DataSet set = new DatasetGenerator(true).CreateDataSet(structure);
            //object obj2 = (SecurityManager.GetProfileProvider().GetProfile(Thread.CurrentPrincipal.Identity) as UserProfile).Id;
            //DatasetTools.Merge(set, data, true, true, false, true, obj2);
            foreach (DataTable table in data.Tables)
            {
                if (set.Tables.Contains(table.TableName))
                {
                    DataTable setTable = set.Tables[table.TableName];
                    table.ExtendedProperties.Clear();
                    foreach (DictionaryEntry entry in setTable.ExtendedProperties)
                    {
                        table.ExtendedProperties.Add(entry.Key, entry.Value);
                    }

                    foreach (DataColumn col in table.Columns)
                    {
                        if (setTable.Columns.Contains(col.ColumnName))
                        {
                            col.ExtendedProperties.Clear();
                            foreach (DictionaryEntry entry in setTable.Columns[col.ColumnName].ExtendedProperties)
                            {
                                col.ExtendedProperties.Add(entry.Key, entry.Value);
                            }
                        }
                    }
                }
            }
            return core.DataService.StoreData(guid, data, loadActualValuesAfterUpdate, null);
        }

        [WebMethod(Description="Stores data to the data store, optionally returning actual values after storing is completed.")]
        public DataSet StoreXml(string dataStructureId, XmlDocument xml, bool loadActualValuesAfterUpdate)
        {
            Guid guid = new Guid(dataStructureId);
            IPersistenceService service = ServiceManager.Services.GetService(typeof(IPersistenceService)) as IPersistenceService;
            DataStructure structure = service.SchemaProvider.RetrieveInstance(typeof(DataStructure), new ModelElementKey(guid)) as DataStructure;
            DataSet set = new DatasetGenerator(true).CreateDataSet(structure);
            set.EnforceConstraints = false;
            set.ReadXml(new XmlNodeReader(xml));
            DataSet set2 = new DatasetGenerator(true).CreateDataSet(structure);
            object obj2 = (SecurityManager.GetProfileProvider().GetProfile(Thread.CurrentPrincipal.Identity) as UserProfile).Id;
            DatasetTools.Merge(set2, set, false, true, false, true, obj2);
            return core.DataService.StoreData(guid, set2, loadActualValuesAfterUpdate, null);
        }

		//[WebMethod]
        //public string HelloWorld()
        //{
        //    return new Origam.Workflow.DebugInfo().GetInfo();
        //}
		#endregion
	}
}

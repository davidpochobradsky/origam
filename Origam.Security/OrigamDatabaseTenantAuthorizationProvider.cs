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

using System;
using System.Collections;
using System.Data;
using System.Security.Principal;

using Origam.DA;
using Origam.Workbench.Services;

namespace Origam.Security
{
	/// <summary>
	/// Summary description for OrigamDatabaseTenantAuthorizationProvider.
	/// </summary>
	public class OrigamDatabaseTenantAuthorizationProvider : IOrigamAuthorizationProvider
	{
		private class Credential
		{
			public string RoleName;
			public bool IsReadOnly;

			public Credential(string roleName, bool isReadOnly)
			{
				RoleName = roleName;
				IsReadOnly = isReadOnly;
			}
		}

		public OrigamDatabaseTenantAuthorizationProvider()
		{
		}

		private Hashtable GetCache()
		{
			string cacheName = "DatabaseAuthorizationProviderCache";
			Hashtable context = OrigamUserContext.Context;

			if(! context.Contains(cacheName))
			{
				context.Add(cacheName, new Hashtable());
			}

			return (Hashtable)OrigamUserContext.Context[cacheName];
		}

		#region IAuthorizationProvider Members

		public bool Authorize(IPrincipal principal, string context)
		{
			if(context == null) return false;
			if(context.Trim() == "") return false;

			if(context == "*") return true;
			string[] roles = context.Split(";".ToCharArray());

			foreach(string roleTest in roles)
			{
				string[] rolePart = roleTest.Split(SecurityManager.ROLE_SUFFIX_DIVIDER.ToCharArray());
				string appRole = rolePart[0];
				bool negation = false;
				
				if(appRole.StartsWith("!"))
				{
					appRole = appRole.Substring(1);
					negation = true;
				}

				if(appRole == "*") return false;

				foreach(Credential c in RoleList(principal))
				{
					bool process = true;

					if(rolePart.Length == 2 && rolePart[1] == SecurityManager.READ_ONLY_ROLE_SUFFIX)
					{
						process = c.IsReadOnly;
					}

					bool result = false;
					if(process)
					{
						if(appRole.Equals(c.RoleName)) result = true;
					}

					if(result)
					{
						if(negation)
						{
							return false;
						}

						return result;
					}
				}

				// we must not force readonly if there is a negation in role list
				if(negation && !(rolePart.Length == 2 && rolePart[1] == SecurityManager.READ_ONLY_ROLE_SUFFIX))
				{
					return true;
				}
			}

			return false;
		}
		#endregion

		#region private methods
		Credential[] RoleList(IPrincipal principal)
		{
			Hashtable cache = GetCache();
			string name = principal.Identity.Name;
			if(! principal.Identity.IsAuthenticated)
			{
				name = "guest";
			}

			if(cache.Contains(name))
			{
				return (Credential[])cache[name];
			}

			IServiceAgent dataServiceAgent;
			try
			{
				dataServiceAgent = (ServiceManager.Services.GetService(typeof(IBusinessServicesService)) as IBusinessServicesService).GetAgent("DataService", null, null);
			}
			catch
			{
				throw new Exception(ResourceUtils.GetString("ErrorNoDataServiceAgent"));
			}

			UserProfile profile = SecurityManager.GetProfileProvider().GetProfile(principal.Identity) as UserProfile;

			DataStructureQuery query = new DataStructureQuery(new Guid("24ec7286-942e-4ed8-9934-d7619f6216d7"), new Guid("603938fd-a9dd-4807-aca2-a161bc6fadc5"));
			query.Parameters.Add(new QueryParameter("BusinessPartnerOrigamRole_parBusinessPartnerId", profile.Id));
				
			dataServiceAgent.MethodName = "LoadDataByQuery";
			dataServiceAgent.Parameters.Clear();
			dataServiceAgent.Parameters.Add("Query", query);

			try
			{
				dataServiceAgent.Run();
			}
			catch(Exception ex)
			{
				throw new Exception(ResourceUtils.GetString("ErrorWhenLoadingRoleList"), ex);
			}

			DataSet result = (DataSet)dataServiceAgent.Result;
			DataTable table = result.Tables["OrigamRoleOrigamApplicationRole"];

			if(table == null) throw new NullReferenceException(ResourceUtils.GetString("ErrorRoleListNotLoaded"));

			// retrieve application roles for organization
			DataStructureQuery query2 = new DataStructureQuery(new Guid("c68dca4b-5690-40f5-8b11-4aa6efdc1b04"), new Guid("d4452a51-54d8-46ba-81cf-8af26a733534"));
			query2.Parameters.Add(new QueryParameter("OrganizationBusinessPartner_parUserBusinessPartnerId", profile.Id));
			query2.Parameters.Add(new QueryParameter("OrganizationBusinessPartner_parOrganizationId", profile.OrganizationId));
				
			dataServiceAgent.MethodName = "LoadDataByQuery";
			dataServiceAgent.Parameters.Clear();
			dataServiceAgent.Parameters.Add("Query", query2);

			try
			{
				dataServiceAgent.Run();
			}
			catch(Exception ex)
			{
				throw new Exception(ResourceUtils.GetString("ErrorWhenLoadingRoleList"), ex);
			}

			DataSet result2 = (DataSet)dataServiceAgent.Result;
			DataTable table2 = result2.Tables["OrganizationOrigamRoleOrigamApplicationRole"];

			if(table2 == null) throw new NullReferenceException(ResourceUtils.GetString("ErrorOrganizationRoleListNotLoaded"));

			Credential[] array = new Credential[table.Rows.Count + table2.Rows.Count];
			
			for(int i = 0; i < table.Rows.Count; i++)
			{
				DataRow row = table.Rows[i];
				
				bool isReadOnly = false;
				if(table.Columns.Contains("IsFormReadOnly"))
				{
					isReadOnly = (bool)row["IsFormReadOnly"];
				}

				array[i] = new Credential((string)row["OrigamApplicationRole_Name"], isReadOnly);
			}

			for(int i = table.Rows.Count; i < array.Length; i++)
			{
				DataRow row = table2.Rows[i-table.Rows.Count];
				
				bool isReadOnly = false;
				if(table.Columns.Contains("IsFormReadOnly"))
				{
					isReadOnly = (bool)row["IsFormReadOnly"];
				}

				array[i] = new Credential((string)row["OrigamApplicationRole_Name"], isReadOnly);
			}

			
			


			// -------------------------



			cache[name] = array;

			return array;
		}
		#endregion
	}
}

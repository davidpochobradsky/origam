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
using System.Collections;

namespace Origam.Workbench.Services
{
	/// <summary>
	/// This class does basic service handling for you.
	/// </summary>
	public class ServiceManager
	{
		ArrayList serviceList       = new ArrayList();
		Hashtable servicesHashtable = new Hashtable();
		
		private static readonly ServiceManager defaultServiceManager = new ServiceManager();
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		/// <summary>
		/// Gets the default ServiceManager
		/// </summary>
		public static ServiceManager Services 
		{
			get 
			{
				return defaultServiceManager;
			}
		}		
		
		/// <summary>
		/// Don't create ServiceManager objects, only have ONE per application.
		/// </summary>
		private ServiceManager()
		{
//			// add 'core' services
//			AddService(new PersistenceService());
//			AddService(new SchemaService());
		}
		
		/// <remarks>
		/// This method initializes the service system to a path inside the add-in tree.
		/// This method must be called ONCE.
		/// </remarks>
		public void InitializeServicesSubsystem(string servicesPath)
		{
//			// add add-in tree services
//			AddServices((IService[])AddInTreeSingleton.AddInTree.GetTreeNode(servicesPath).BuildChildItems(this).ToArray(typeof(IService)));
//			
//			// initialize all services
//			foreach (IService service in serviceList) 
//			{
//				DateTime now = DateTime.Now;
//				service.InitializeService();
//			}
		}
		
		/// <remarks>
		/// Calls UnloadService on all services. This method must be called ONCE.
		/// </remarks>
		public void UnloadAllServices()
		{
			ArrayList copy = new ArrayList(serviceList);

			foreach (IWorkbenchService service in copy) 
			{
				UnloadService(service);
			}
		}

		public void UnloadService(IWorkbenchService service)
		{
            if (service == null)
            {
                return;
            }
			if(log.IsInfoEnabled) log.Info("Unloading workbench service: " + service.GetType());

			service.UnloadService();
			serviceList.Remove(service);

			ArrayList hashTypes = new ArrayList();
			foreach(DictionaryEntry entry in servicesHashtable)
			{
				if(entry.Value.Equals(service))
				{
					hashTypes.Add(entry.Key);
				}
			}

			foreach(Type hashType in hashTypes)
			{
				servicesHashtable.Remove(hashType);
			}
		}
		
		public void AddService(IWorkbenchService service)
		{	
			if(log.IsInfoEnabled) log.Info("Adding workbench service: " + service.GetType());

			service.InitializeService();
			serviceList.Add(service);
		}
		
		public void AddServices(IWorkbenchService[] services)
		{
			foreach (IWorkbenchService service in services) 
			{
				AddService(service);
			}
		}
		
		// HACK: MONO BUGFIX
		// this doesn't work on mono:serviceType.IsInstanceOfType(service)
		bool IsInstanceOfType(Type type, IWorkbenchService service)
		{
			Type serviceType = service.GetType();

			foreach (Type iface in serviceType.GetInterfaces()) 
			{
				if (iface == type) 
				{
					return true;
				}
			}
			
			while (serviceType != typeof(System.Object)) 
			{
				if (type == serviceType) 
				{
					return true;
				}
				serviceType = serviceType.BaseType;
			}
			return false;
		}
		
		/// <remarks>
		/// Requestes a specific service, may return null if this service is not found.
		/// </remarks>
		public IWorkbenchService GetService(Type serviceType)
		{
			IWorkbenchService s = (IWorkbenchService)servicesHashtable[serviceType];
			if (s != null) 
			{
				return s;
			}
			
			foreach (IWorkbenchService service in serviceList) 
			{
				if (IsInstanceOfType(serviceType, service)) 
				{
					servicesHashtable[serviceType] = service;
					return service;
				}
			}
			
			return null;
		}
		
		public T GetService<T>() =>  (T)GetService((typeof(T)));
	}
}

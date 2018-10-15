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
using System.Xml;
using System.Data;
using OrigamArchitect;
using Origam.DA.Service;
using Origam.Workbench;
using Origam.Workbench.Pads;
using Origam.UI;
using Origam.Schema.WorkflowModel;

using Origam.Schema.GuiModel;


namespace Origam.Schema.Wizards
{
    /// <summary>
    /// Summary description for SetInheritanceOff.
    /// </summary>
    public class ShowModelElementXmlCommand : AbstractMenuCommand
	{
		public override bool IsEnabled
		{
			get
			{
				return Owner is FormControlSet 
					|| Owner is MenuModel.Menu
					|| Owner is WorkQueueClass
					|| Owner is MenuModel.FormReferenceMenuItem
					|| (Owner is AbstractDashboardWidget && ! (Owner is DashboardWidgetFolder));
			}
			set
			{
				throw new ArgumentException("Cannot set this property", "IsEnabled");
			}
		}

		public override void Run()
		{
            Origam.Workbench.Commands.ViewOutputPad outputPad =
                new Origam.Workbench.Commands.ViewOutputPad();
            outputPad.Run();
            OutputPad o = WorkbenchSingleton.Workbench.GetPad(typeof(OutputPad)) as OutputPad;
			XmlDocument doc = null;
			if(Owner is FormControlSet)
			{
				FormControlSet item = Owner as FormControlSet;
				doc = Origam.OrigamEngine.ModelXmlBuilders.FormXmlBuilder.GetXml(item, item.Name, true, Guid.Empty, item.DataStructure, false, "");
			}
			else if(Owner is MenuModel.FormReferenceMenuItem)
			{
				MenuModel.FormReferenceMenuItem formMenu = Owner as MenuModel.FormReferenceMenuItem;
				doc = Origam.OrigamEngine.ModelXmlBuilders.FormXmlBuilder.GetXml(formMenu.Screen, formMenu.DisplayName, formMenu.ListDataStructure == null, formMenu.Id, formMenu.Screen.DataStructure, formMenu.ReadOnlyAccess, formMenu.SelectionChangeEntity);
			}
			else if(Owner is MenuModel.Menu)
			{
				MenuModel.Menu item = Owner as MenuModel.Menu;
				doc = Origam.OrigamEngine.ModelXmlBuilders.MenuXmlBuilder.GetXml(item);
			}
			else if(Owner is WorkQueueClass)
			{
				WorkQueueClass item = Owner as WorkQueueClass;
				DataSet dataset = new DatasetGenerator(true).CreateDataSet(item.WorkQueueStructure);
				doc = Origam.OrigamEngine.ModelXmlBuilders.FormXmlBuilder.GetXml(item, dataset, strings.Massages_XmlItem, Guid.Empty);
			}
			else if(Owner is AbstractDashboardWidget)
			{
				AbstractDashboardWidget item = Owner as AbstractDashboardWidget;
				
				string config = "<configuration><item id=\"" + Guid.NewGuid().ToString() + "\" componentId=\"" + item.Id.ToString() + "\" left=\"0\" top=\"0\" colSpan=\"1\" rowSpan=\"1\"/></configuration>";
				doc = Origam.OrigamEngine.ModelXmlBuilders.FormXmlBuilder.GetXml(config, "test", item.Id, null);
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.StringWriter sw = new System.IO.StringWriter(sb);
			XmlTextWriter xw = new XmlTextWriter(sw);
			xw.Formatting = Formatting.Indented;
			doc.WriteTo(xw);
			o.SetOutputText(sb.ToString());
		}
	}
}

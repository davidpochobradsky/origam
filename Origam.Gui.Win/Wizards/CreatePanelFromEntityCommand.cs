#region license
/*
Copyright 2005 - 2019 Advantage Solutions, s. r. o.

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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Origam.Schema;
using Origam.Schema.EntityModel;
using Origam.Schema.EntityModel.Wizards;
using Origam.Schema.GuiModel;
using Origam.Services;
using Origam.UI;
using Origam.UI.WizardForm;
using Origam.Workbench;
using Origam.Workbench.Commands;
using Origam.Workbench.Services;

namespace Origam.Gui.Win.Wizards
{
	/// <summary>
	/// Summary description for CreatePanelFromEntityCommand.
	/// </summary>
	public class CreatePanelFromEntityCommand : AbstractMenuCommand
	{
        SchemaBrowser _schemaBrowser = WorkbenchSingleton.Workbench.GetPad(typeof(SchemaBrowser)) as SchemaBrowser;
        ISchemaService schema = ServiceManager.Services.GetService(typeof(ISchemaService)) as ISchemaService;
        ScreenWizardForm wizardForm;
        PanelControlSet panel;

        public override bool IsEnabled
		{
			get
			{
				return Owner is IDataEntity;
			}
			set
			{
				throw new ArgumentException("Cannot set this property", "IsEnabled");
			}
		}

		public override void Run()
		{
            PanelSchemaItemProvider dsprovider = schema.GetProvider(typeof(PanelSchemaItemProvider)) as PanelSchemaItemProvider;
            List<string> listdsName = dsprovider.ChildItemsByType(PanelControlSet.ItemTypeConst)
                            .ToArray()
                            .Select(x => { return ((AbstractSchemaItem)x).Name; })
                            .ToList();

            ArrayList list = new ArrayList();
            PanelControlSet pp = new PanelControlSet();
            list.Add(new ListViewItem(pp.ItemType, pp.Icon));
            
            Stack stackPage = new Stack();
            stackPage.Push(PagesList.finish);
            stackPage.Push(PagesList.ScreenForm);
            if (listdsName.Any(name => name == (Owner as IDataEntity).Name))
            {
                stackPage.Push(PagesList.StructureNamePage);
            }
            stackPage.Push(PagesList.startPage);

            wizardForm = new ScreenWizardForm
            {
                ItemTypeList = list,
                Title = "Create Screen Section Wizard",
                PageTitle = "",
                Description = "This will create Field for looku and bla bla bla bla bla bla bla bla bla bla bla bla ",
                Pages = stackPage,
                StructureList = listdsName,
                Entity = Owner as IDataEntity,
                NameOfEntity = (Owner as IDataEntity).Name,
                IsRoleVisible = false,
                textColumnsOnly = false,
                ImageList = _schemaBrowser.EbrSchemaBrowser.imgList,
                Command = this
            };

            Wizard wiz = new Wizard(wizardForm);
            if (wiz.ShowDialog() == DialogResult.OK)
            {
                EditSchemaItem edit = new EditSchemaItem
                {
                    Owner = panel
                };
                edit.Run();
                _schemaBrowser.EbrSchemaBrowser.SelectItem(panel);
            }
        }

        public override void Execute()
        {
            string groupName = null;
            if (wizardForm.Entity.Group != null) groupName = wizardForm.Entity.Group.Name;

            panel = GuiHelper.CreatePanel(groupName, wizardForm.Entity, wizardForm.SelectedFieldNames,wizardForm.NameOfEntity);
        }
    }
}

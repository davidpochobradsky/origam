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
using System.Windows.Forms;
using Origam.Schema.EntityModel;
using Origam.UI;
using Origam.UI.WizardForm;
using Origam.Workbench;

namespace Origam.Schema.LookupModel.Wizards
{
    /// <summary>
    /// Summary description for CreateFieldWithLookupRelationshipEntityCommand.
    /// </summary>
    public class CreateFieldWithRelationshipEntityCommand : AbstractMenuCommand
	{
        CreateFieldWithRelationshipEntityWizardForm wizardForm;
        SchemaBrowser _schemaBrowser = WorkbenchSingleton.Workbench.GetPad(typeof(SchemaBrowser)) as SchemaBrowser;
        public override bool IsEnabled
		{
			get
			{
                return Owner is IDataEntity
                    || Owner is IDataEntityColumn;
			}
			set
			{
				throw new ArgumentException("Cannot set this property", "IsEnabled");
			}
		}

		public override void Run()
		{
            FieldMappingItem baseField = Owner as FieldMappingItem;
            IDataEntity baseEntity = Owner as IDataEntity;
            if (baseField != null)
            {
                baseEntity = baseField.ParentItem as IDataEntity;
            }
            //CreateFieldWithRelationshipEntityWizard wiz = new CreateFieldWithRelationshipEntityWizard
            //{
            //    Entity = baseEntity
            //};

            ArrayList list = new ArrayList();
            TableMappingItem table1 = new TableMappingItem();
            EntityRelationItem entityRelation = new EntityRelationItem();
            list.Add(new ListViewItem(table1.ItemType, table1.Icon));
            list.Add(new ListViewItem(entityRelation.ItemType, entityRelation.Icon));
            

            Stack stackPage = new Stack();
            stackPage.Push(PagesList.Finish);
            stackPage.Push(PagesList.FieldEntity);
            stackPage.Push(PagesList.StartPage);


            wizardForm = new CreateFieldWithRelationshipEntityWizardForm
            {
                Title = "Create Field With Relationship Entity.",
                PageTitle = "",
                Description = "Create Some Description.",
                Entity = baseEntity,
                ItemTypeList = list,
                Pages = stackPage,
                ImageList = _schemaBrowser.EbrSchemaBrowser.imgList,
                Command = this,
                EnterAllInfo = ResourceUtils.GetString("EnterAllInfo"),
                LookupWiz = ResourceUtils.GetString("LookupWiz")
            };

            Wizard wiz = new Wizard(wizardForm);
            wiz.Show();
        }

        public override void Execute()
        {
            FieldMappingItem baseField = Owner as FieldMappingItem;
            IDataEntity baseEntity = Owner as IDataEntity;
            if (baseField != null)
            {
                baseEntity = baseField.ParentItem as IDataEntity;
            }
            // 1. entity
            TableMappingItem table = (TableMappingItem)baseEntity;
            EntityRelationItem relation = EntityHelper.CreateRelation(table, (IDataEntity)wizardForm.RelatedEntity, wizardForm.ParentChildCheckbox, true);
            EntityHelper.CreateRelationKey(relation,
                (AbstractDataEntityColumn)wizardForm.BaseEntityFieldSelect,
                (AbstractDataEntityColumn)wizardForm.RelatedEntityFieldSelect, true);
        }
    }
}

﻿using Origam.UI.Commands;
using Origam.UI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Origam.UI.WizardForm
{
    public class AbstractWizardForm : IWizardForm
    {
        public ArrayList ItemTypeList { get ; set ; }
        public  Stack Pages { get; set; }
        public  string Description { get ; set ; }
        public  List<string> DatastructureList { get ; set ; }
        public  string NameOfEntity { get ; set ; }
        public ImageList ImageList { get; set ; }
        public IRunCommand Command { get; set; }

        public bool IsExistsNameInDataStructure(string name)
        {
            return DatastructureList.Contains(name);
        }

        public void ListView(ListView listView)
        {
            listView.Items.Clear();
            foreach (ListViewItem item in ItemTypeList)
            {
                item.ImageIndex = ImageList.ImageIndex(item.ImageKey);
                listView.Items.Add(item);
            }
        }
    }
    public enum PagesList
    {
        startPage,
        StructureNamePage,
        ScreenForm,
        LookupForm,
        FieldLookup,
        FieldEntity,
        MenuPage,
        finish
    }
}

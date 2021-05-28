﻿#region license
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Origam.Workbench.Services;
using Origam.Schema;
using Origam.Schema.GuiModel;
namespace Origam.Gui.Win
{
    public class FormLevelPlugin : Label,
        IOrigamMetadataConsumer, ISupportInitialize
    {
        public AbstractSchemaItem OrigamMetadata { get; set; }
        public void BeginInit()
        {
        }
	    

        public void EndInit()
        {
        }
    }   
    
    
    
    public class SectionLevelPlugin : Label,
        IOrigamMetadataConsumer, ISupportInitialize, IAsDataConsumer
    {
        public AbstractSchemaItem OrigamMetadata { get; set; }
        public void BeginInit()
        {
        }
	    

        public void EndInit()
        {
        }
        
        private string dataMember;
        private object dataSource;
        
        [
            DefaultValue((string) null),
            TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
            RefreshProperties(RefreshProperties.Repaint),
            Category("Data"),
            Description("Data source of the tree.")
        ]
        public object DataSource
        {
            get => dataSource;
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                }
            }
        }
        
        [
            DefaultValue(""),
            Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            RefreshProperties(RefreshProperties.Repaint),
            Category("Data"),
            Description("Data member of the tree.")
        ]
        public string DataMember
        {
            get
            {
                return this.dataMember;
            }
            set
            {
                if (this.dataMember != value)
                {
                    this.dataMember = value;
                }
            }
        }
    }
}
﻿#region license
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Mvp.Xml.Common.Xsl;
using Mvp.Xml.Exslt;
using Origam.DA.Service;
using Origam.Gui.Win;
using Origam.Schema;
using Origam.Schema.EntityModel;
using Origam.Schema.GuiModel;
using Origam.Schema.MenuModel;
using Origam.Workbench.Services;

namespace Origam.DocGenerator
{
    public class DocCreate
    {
        private XmlWriter XmlwriterSource { get; set; }
        private XmlWriter Xmlwriter { get; set; }
        private IDocumentationService documentation;
        private MenuSchemaItemProvider menuprovider;
        private readonly IPersistenceService ps;
        private readonly string XsltPath, RootFile,xmlsourcefile, DirectoryPath;
        private MemoryStream mstream;
        public DocCreate(string path, string xslt, string rootfile, FileStorageDocumentationService documentation, MenuSchemaItemProvider menuprovider, FilePersistenceService persprovider, string xmlfile)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("Path for Export is not set!");
            }
            if (string.IsNullOrEmpty(xslt))
            {
                throw new Exception("Xslt template is not set!");
            }
            if (string.IsNullOrEmpty(rootfile))
            {
                throw new Exception("RootFileName is not set!");
            }
            xmlsourcefile = xmlfile;
            RootFile = rootfile;
            XsltPath = xslt;
            ps = persprovider ?? throw new Exception("Persprovider  is not set!"); 
            DirectoryPath = string.Join("", path.Split(Path.GetInvalidPathChars())); ;
            this.documentation = documentation ?? throw new Exception("Documentation  is not set!");
            this.menuprovider = menuprovider ?? throw new Exception("MenuSchemaItemProvider is not set!");
            CreateWriter();
        }
        public DocCreate(XmlWriter writer, IDocumentationService documentation, IPersistenceService ps)
        {
            XmlwriterSource = writer;
            this.documentation = documentation;
            this.ps = ps;
            CreateWriter();
        }
        private void CreateWriter()
        {
            mstream = new MemoryStream();
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };
            Xmlwriter = XmlWriter.Create(mstream, settings);
            Xmlwriter.WriteStartDocument();
            Xmlwriter.WriteComment("This file is generated by the program.(" + DateTime.Now + ")");
        }
        private void MakeXml(ControlSetItem control, FormControlSet formItem, DataSet dataset, string dataMember)
        {
            string caption, gridCaption, bindingMember, panelTitle, section;
            caption = gridCaption = bindingMember = panelTitle = section = "";
            int tabIndex = 0;
            foreach (PropertyValueItem property in control.ChildItemsByType(PropertyValueItem.ItemTypeConst))
            {
                if (property.ControlPropertyItem.Name == "TabIndex")
                {
                    tabIndex = property.IntValue;
                }
                if (property.ControlPropertyItem.Name == "Caption")
                {
                    caption = property.Value;
                }
                if (property.ControlPropertyItem.Name == "GridColumnCaption")
                {
                    gridCaption = property.Value;
                }
                if (property.ControlPropertyItem.Name == "PanelTitle")
                {
                    panelTitle = property.Value;
                }
                if (control.ControlItem.IsComplexType && property.ControlPropertyItem.Name == "DataMember")
                {
                    dataMember = property.Value;
                }
            }
            caption = (gridCaption == "" | gridCaption == null) ? caption : gridCaption;
            foreach (PropertyBindingInfo bindItem in control.ChildItemsByType(PropertyBindingInfo.ItemTypeConst))
            {
                bindingMember = bindItem.Value;
            }
            if (bindingMember != "")
            {
                DataTable table = dataset.Tables[FormGenerator.FindTableByDataMember(dataset, dataMember)];
                if (!table.Columns.Contains(bindingMember))
                {
                    throw new Exception("Field '" + bindingMember +
                        "' not found in a data structure for the form '" + control.RootItem.Path + "'");
                }
                if (string.IsNullOrEmpty(caption))
                {
                    caption = table.Columns[bindingMember].Caption;
                }
                Guid id = (Guid)table.Columns[bindingMember].ExtendedProperties["Id"];
                WriteStartElement("Field", caption, control.ControlItem.Id.ToString(), control.ControlItem.GetType().Name);
                WriteElement("description",
                     documentation.GetDocumentation(id, DocumentationType.USER_LONG_HELP));
            }
            ArrayList sortedControls;
            if (control.ControlItem.IsComplexType)
            {
                if (panelTitle != "")
                {
                    section = panelTitle;
                }
                else
                {
                    section = "Panel";
                }
                if (string.IsNullOrEmpty(section))
                {
                    section = "Panel";
                }
                AbstractDataEntity entity = GetEntity(ps, dataMember, dataset);
                WriteStartElement("Section", section, control.ControlItem.Id.ToString(), control.ControlItem.GetType().Name);
                WriteStartElement("entity");
                WriteElement("entityid", entity.PrimaryKey["Id"].ToString());
                WriteElement("entityname", entity.NodeText);
                WriteEndElement();
                WriteElement("description",
                    documentation.GetDocumentation(control.ControlItem.PanelControlSet.Id, DocumentationType.USER_LONG_HELP));
                sortedControls = control.ControlItem.PanelControlSet.ChildItems[0].ChildItemsByType(ControlSetItem.ItemTypeConst);
            }
            else
            {
                sortedControls = control.ChildItemsByType(ControlSetItem.ItemTypeConst);
            }
            sortedControls.Sort(new ControlSetItemComparer());
            foreach (ControlSetItem subControl in sortedControls)
            {
                MakeXml(subControl, formItem, dataset, dataMember);
            }
            if (bindingMember != "")
            {
                WriteEndElement();
            }
            if (section != "")
            {
                WriteEndElement();
            }
        }
        private AbstractDataEntity GetEntity(IPersistenceService ps, string dataMember, DataSet dataset)
        {
            DataTable table = dataset.Tables[FormGenerator.FindTableByDataMember(dataset, dataMember)];
            Guid entityId = (Guid)table.ExtendedProperties["EntityId"];
            AbstractDataEntity entity = ps.SchemaProvider.RetrieveInstance(typeof(AbstractDataEntity), new ModelElementKey(entityId)) as AbstractDataEntity;
            return entity;
        }
        public void Screen(FormControlSet form, string xsltdoc)
        {
            WriteStartElement("Menu");
            MakeXmlSections(form);
            WriteEndElement();
            CloseXml();
            MvpXslTransform xsltTransform = new MvpXslTransform();
            xsltTransform.Load(new XmlTextReader(new StringReader(xsltdoc)));
            SaveXslt(xsltTransform, new XmlOutput(XmlwriterSource));
        }
        public void Run()
        {
            if (!string.IsNullOrEmpty(XsltPath))
            {
                MvpXslTransform processor = new MvpXslTransform(false);
                processor.Load(XsltPath);
            }
            List<AbstractSchemaItem> menulist = menuprovider.ChildItems.ToList();
            menulist.Sort();
            WriteStartElement("Menu");
            CreateXml(menulist[0]);
            WriteEndElement();
            CloseXml();
            if (!string.IsNullOrEmpty(xmlsourcefile))
            {
                SaveSchemaXml();
            }
            else
            {
                MvpXslTransform xslttransform = new MvpXslTransform();
                xslttransform.Load(XsltPath);
                MultiXmlTextWriter multiWriter = new MultiXmlTextWriter(Path.Combine(DirectoryPath, RootFile), new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented
                };
                SaveXslt(xslttransform,new XmlOutput(multiWriter));
            }
        }
        private void SaveSchemaXml()
        {
            FileStream file = new FileStream(Path.Combine(DirectoryPath, xmlsourcefile), FileMode.Create, FileAccess.Write);
            mstream.WriteTo(file);
            file.Close();
            mstream.Close();
        }
        private void SaveXslt( MvpXslTransform xslTransform, XmlOutput xmlOutput)
        {
            mstream.Seek(0, SeekOrigin.Begin);
            XPathDocument doc = new XPathDocument(mstream);
            xslTransform.Transform(new XmlInput(doc), null, xmlOutput);
        }
        private void CreateXml(AbstractSchemaItem menuSublist)
        {
            foreach (AbstractSchemaItem menuitem in menuSublist.ChildItems)
            {
                WriteStartElement("Menuitem", menuitem.NodeText, menuitem.Id.ToString(), menuitem.GetType().Name);
                WriteElement("documentation",
                    documentation.GetDocumentation(menuitem.Id, DocumentationType.USER_LONG_HELP));
                if (menuitem is FormReferenceMenuItem formItem)
                {
                    MakeXmlSections(formItem.Screen);
                }
                CreateXml(menuitem);
                WriteEndElement();
            }
        }
        private void MakeXmlSections(FormControlSet form)
        {
            DataSet dataset = new DatasetGenerator(false).CreateDataSet(form.DataStructure);
            MakeXml(form.ChildItems[0] as ControlSetItem, form, dataset, null);
        }
        private void WriteStartElement(string element)
        {
            Xmlwriter.WriteStartElement(element);
        }
        private void WriteStartElement(string element, string displayName, string id, string typeitem)
        {
            WriteStartElement(element);
            Xmlwriter.WriteAttributeString("DisplayName", displayName);
            Xmlwriter.WriteAttributeString("Id", id);
            Xmlwriter.WriteAttributeString("Type", typeitem);
        }
        private void WriteElement(string caption, string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                Xmlwriter.WriteElementString(caption, description);
            }
        }
        private void WriteEndElement()
        {
            Xmlwriter.WriteEndElement();
        }
        private void CloseXml()
        {
            Xmlwriter.WriteEndDocument();
            Xmlwriter.Flush();
        }
    }
}
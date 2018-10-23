﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using Origam.DA.Service;
using Origam.Gui.Win;
using Origam.Schema;
using Origam.Schema.EntityModel;
using Origam.Schema.GuiModel;
using Origam.Schema.MenuModel;
using Origam.Workbench.Services;

namespace Origam.MdExporter
{
    class XmlCreate
    {
        public string Xmlpath { get; set; }
        public XmlTextWriter Xmlwriter { get; set; }
        IDocumentationService documentation;
        MenuSchemaItemProvider menuprovider = new MenuSchemaItemProvider();
        private FilePersistenceProvider persprovider;
        private string mezerka = " ";
        private  int pocetmezer = 0;
        public XmlCreate(string xmlpath,string filename, FileStorageDocumentationService documentation1,FilePersistenceProvider persprovider)
        {
            this.Xmlpath = xmlpath;
            documentation = documentation1;
            menuprovider.PersistenceProvider = persprovider;
            this.persprovider = persprovider;
            Xmlwriter = new XmlTextWriter(xmlpath + "\\" + filename, Encoding.UTF8)
            {
                Formatting = Formatting.Indented
            };
            Xmlwriter.WriteStartDocument();
            Xmlwriter.WriteComment("This file is generated by the program.("+ DateTime.Now + ")");
        }

        private void MakeXml( ControlSetItem control, FormControlSet formItem, DataSet dataset,string dataMember)
        {
            string caption = "";
            string gridCaption = "";
            string bindingMember = "";
            string panelTitle = "";
            int tabIndex = 0;
            //ControlSetItem control = (ControlSetItem)ff.ChildItems[0];
           
            //var fstructura = formItem.DataStructure.ChildItems.ToList();
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

                    if (!table.Columns.Contains(bindingMember)) throw new Exception("Field '" + bindingMember + "' not found in a data structure for the form '" + control.RootItem.Path + "'");

                    if (caption == "") caption = table.Columns[bindingMember].Caption;
                    Guid id = (Guid)table.Columns[bindingMember].ExtendedProperties["Id"];

                WriteStartElement("screenField");
                WriteStartElement("title",caption);
                WriteEndElement();
                 DocumentationComplete docData = documentation.LoadDocumentation(id);
                if (docData != null)
                {
                    if ( docData.Documentation.Count > 0)
                    {
                        WriteStartElement("Element");
                        WriteElement("Id", control.Id.ToString());
                        WriteElement("Path", control.Path);

                        foreach (DocumentationComplete.DocumentationRow docRow in docData.Documentation)
                        {
                            WriteStartElement("Documentation");
                            WriteElement("Category", docRow.Category);
                            WriteElement("OriginalText", docRow.Data.ToString());
                            WriteEndElement();
                        }

                        WriteEndElement();
                    }
                }
                WriteEndElement();
                }

            ArrayList sortedControls;

                if (control.ControlItem.IsComplexType)
                {
                    if (panelTitle != "")
                    {
                        WriteStartElement("Section ",panelTitle );
                    }
                    
                    AbstractDataEntity entity = GetEntity(persprovider, dataMember, dataset);
                    WriteStartElement(entity.Name, entity.NodeText);
                    
                    //writer.WriteString("Entity ");
                    //DocTools.WriteElementLink(writer, entity, new DocEntity(null).FilterName);
                    WriteEndElement();

                    sortedControls = control.ControlItem.PanelControlSet.ChildItems[0].ChildItemsByType(ControlSetItem.ItemTypeConst);
                    if (panelTitle != "")
                    {
                        WriteEndElement();
                    }
                }
                else
                {
                    sortedControls = control.ChildItemsByType(ControlSetItem.ItemTypeConst);
                }

                sortedControls.Sort(new ControlSetItemComparer());

            
            foreach (ControlSetItem subControl in sortedControls)
            {
                    WriteStartElement(subControl.Name, subControl.NodeText);
                    //WriteElement("subControl", subControl.NodeText);
                    MakeXml(subControl,formItem,dataset, dataMember);
                    WriteEndElement();
            }

        }

        private AbstractDataEntity GetEntity(FilePersistenceProvider persprovider, string dataMember, DataSet dataset)
        {
            DataTable table = dataset.Tables[FormGenerator.FindTableByDataMember(dataset, dataMember)];
            Guid entityId = (Guid)table.ExtendedProperties["EntityId"];
            AbstractDataEntity entity = persprovider.RetrieveInstance(typeof(AbstractDataEntity), new ModelElementKey(entityId)) as AbstractDataEntity;
            return entity;
        }

        internal void Run()
        {
            List<AbstractSchemaItem> menulist = menuprovider.ChildItems.ToList();
            menulist.Sort();
            WriteStartElement("Menu");
            CreateXml(menulist[0]);
            WriteEndElement();
            CloseXml();
            //Console.ReadKey();
        }

        internal void CreateXml(AbstractSchemaItem menuSublist)
        {
            foreach (AbstractSchemaItem menuitem in menuSublist.ChildItems)
            {
                WriteStartElement(menuitem.Name, menuitem.NodeText);
                if (menuitem is FormReferenceMenuItem formItem)
                {
                    FormControlSet form = formItem.Screen;
                    DataSet dataset = new DatasetGenerator(false).CreateDataSet(form.DataStructure);
                    MakeXml(form.ChildItems[0] as ControlSetItem, form, dataset,null);
                }
                CreateXml(menuitem);
                WriteEndElement();
             }
        }

        internal void WriteStartElement(string v)
        {
            pocetmezer = pocetmezer + 3;
            Xmlwriter.WriteStartElement(v);
            Console.WriteLine(mezerka.PadLeft(pocetmezer) + "WriteStartElement" +v);
        }

        internal void WriteStartElement(string v,string title)
        {
            pocetmezer = pocetmezer + 3;
            Xmlwriter.WriteStartElement(v);
            Xmlwriter.WriteAttributeString("DisplayName", title);
            Console.WriteLine(mezerka.PadLeft(pocetmezer) + "WriteStartAttrElement" + v);
        }

        public void WriteElement(string caption,string description)
        {
            pocetmezer= pocetmezer+3;
            //Xmlwriter.WriteStartElement(caption);
            Xmlwriter.WriteElementString(caption, description);
            Console.WriteLine(mezerka.PadLeft(pocetmezer) + "  WriteElement");
        }

        public void WriteEndElement()
        {
            pocetmezer = pocetmezer - 3;
            Xmlwriter.WriteEndElement();
            Console.WriteLine(mezerka.PadLeft(pocetmezer) + "WriteEndElement");
        }

        public void CloseXml()
        {
            Xmlwriter.WriteEndDocument();
            Xmlwriter.Flush();
            Xmlwriter.Close();
        }
    }
}

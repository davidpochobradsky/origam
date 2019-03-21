#region license
/*
Copyright 2005 - 2019 Advantage Solutions, s. r. o.

This file is part of ORIGAM.

ORIGAM is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ORIGAM is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with ORIGAM.  If not, see<http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Origam.Workbench.Services;
using System.Web;
using System.Web.SessionState;
using Origam.DA;
using System.Collections;

using NPOI.HSSF.UserModel;

using log4net;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Origam.Excel;

namespace Origam.Server
{
    public class EntityExportDownloadHandler : IHttpHandler, IRequiresSessionState
    {
        private static readonly ILog perfLog = LogManager.GetLogger(
            "Performance");
        private static readonly ILog log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ICellStyle dateCellStyle;
        IDataLookupService lookupService = ServiceManager.Services.GetService(
            typeof(IDataLookupService)) as IDataLookupService;
        private IDictionary<string, IDictionary<object, object>> lookupCache 
            = new Dictionary<string, IDictionary<object, object>>();
        OrigamSettings settings 
            = ConfigurationManager.GetActiveConfiguration() as OrigamSettings;
        bool isExportUnlimited = SecurityManager.GetAuthorizationProvider()
            .Authorize(SecurityManager.CurrentPrincipal, 
            "SYS_ExcelExport_Unlimited");
        ExcelFormat exportFormat; 

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if(perfLog.IsInfoEnabled)
            {
                perfLog.Info("ExcelExport");
            }
            string requestId = context.Request.Params.Get("id");
            try
            {
                EntityExportInfo info 
                    = (EntityExportInfo)context.Application[requestId];
                if(info == null)
                {
                    context.Response.Write(
                        Properties.Resources.ErrorExportNotAvailable);
                    return;
                }
                SetExportFormat();
                SetupResponseHeader(context);
                IWorkbook workbook = CreateWorkbook();
                SetupDateCellStyle(workbook);
                ISheet sheet = workbook.CreateSheet("Data");
                SetupSheetHeader(sheet, info);
                bool isPkGuid 
                    = info.Table.PrimaryKey[0].DataType == typeof(Guid);
                for(int rowNumber = 1; rowNumber <= info.RowIds.Count; rowNumber++)
                {
                    if(!isExportUnlimited && (settings.ExportRecordsLimit > -1) 
                    && (rowNumber > settings.ExportRecordsLimit))
                    {
                        FillExportLimitExceeded(workbook, sheet, rowNumber);
                        break;
                    }
                    DataRow row = GetDataRow(info, rowNumber, isPkGuid);
                    if (row != null)
                    {
                        AddRowToSheet(info, workbook, sheet, rowNumber, row);
                    }
                }
                workbook.Write(context.Response.OutputStream);
                context.Response.End();
            }
            catch (Exception ex)
            {
                if(log.IsErrorEnabled)
                {
                    log.Error(ex.Message, ex);
                }
                throw;
            }
            finally
            {
                context.Application.Remove(requestId);
            }
        }

        private void SetExportFormat()
        {
            string value = settings.GUIExcelExportFormat;
            if (value == "XLSX")
            {
                exportFormat = ExcelFormat.XLSX;
            }
            else
            {
                exportFormat = ExcelFormat.XLS;
            }
        }

        private IWorkbook CreateWorkbook()
        {
            if (exportFormat == ExcelFormat.XLS)
            {
                return new HSSFWorkbook();
            }
            else
            {
                return new XSSFWorkbook();
            }
        }

        private void AddRowToSheet(
            EntityExportInfo info, IWorkbook workbook, ISheet sheet, 
            int rowNumber, DataRow row)
        {
            IRow excelRow = sheet.CreateRow(rowNumber);
            for (int i = 0; i < info.Fields.Count; i++)
            {
                AddCellToRow(info, workbook, excelRow, i, row);
            }
        }

        private void AddCellToRow(
            EntityExportInfo info, IWorkbook workbook, IRow excelRow, 
            int columnIndex, DataRow row)
        {
            EntityExportField field = info.Fields[columnIndex];
            ICell cell = excelRow.CreateCell(columnIndex);
            object val;
            if (SessionStore.IsColumnArray(info.Table.Columns[field.FieldName]))
            {
                val = GetArrayColumnValue(info, field, row);
            }
            else
            {
                val = GetNonArrayColumnValue(field, row);
            }
            SetCellValue(workbook, val, cell);
        }

        private object GetNonArrayColumnValue(EntityExportField field, DataRow row)
        {
            // normal (non-array) column
            if((field.LookupId != null) && (field.LookupId != ""))
            {
                return GetLookupValue(row[field.FieldName], field.LookupId);
            }
            else if(field.PolymorphRules != null)
            {
                var controlFieldValue = row[
                    field.PolymorphRules.ControlField];
                if((controlFieldValue == null) 
                || !field.PolymorphRules.Rules.Contains(
                    controlFieldValue.ToString()))
                {
                    return null;
                }
                else
                {
                    return row[field.PolymorphRules.Rules[
                        controlFieldValue.ToString()]
                        .ToString()];
                }
            }
            else
            {
                return row[field.FieldName];
            }
        }

        private object GetArrayColumnValue(
            EntityExportInfo info, EntityExportField field, DataRow row)
        {
            // returns list of array elements
            ArrayList arrayElements = 
                    SessionStore.GetRowColumnArrayValue(row,
                    info.Table.Columns[field.FieldName]);
            // try to use default lookup
            if((field.LookupId == null) || (field.LookupId == ""))
            {
                field.LookupId = info.Table.Columns[field.FieldName]
                        .ExtendedProperties[Const.DefaultLookupIdAttribute].ToString();
            }
            if((field.LookupId != null) && (field.LookupId != ""))
            {
                // lookup array elements
                ArrayList lookupedArrayElements = new ArrayList(arrayElements.Count);
                foreach (object arrayElement in arrayElements)
                {
                    // get lookup value
                    lookupedArrayElements.Add(
                        GetLookupValue(arrayElement, field.LookupId));
                }
                // store lookuped array elements
                return lookupedArrayElements;
            }
            else
            {
                // store array elements
                return arrayElements;
            }
        }

        private void FillExportLimitExceeded(
            IWorkbook workbook, ISheet sheet, int rowNumber)
        {
            IRow excelRow = sheet.CreateRow(rowNumber);
            ICell cell = excelRow.CreateCell(0);
            SetCellValue(
                workbook, Properties.Resources.ExportLimitExceeded, cell);
        }

        private void SetupSheetHeader(ISheet sheet, EntityExportInfo info)
        {
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < info.Fields.Count; i++)
            {
                EntityExportField field = info.Fields[i];
                headerRow.CreateCell(i).SetCellValue(field.Caption);
            }
        }
        private void SetupDateCellStyle(IWorkbook workbook)
        {
            dateCellStyle = workbook.CreateCellStyle();
            dateCellStyle.DataFormat 
                = workbook.CreateDataFormat().GetFormat("m/d/yy h:mm");
        }

        private void SetupResponseHeader(HttpContext context)
        {
            if (exportFormat == ExcelFormat.XLS)
            {
                context.Response.ContentType = "application/vnd.ms-excel";
                context.Response.AppendHeader(
                    "content-disposition", "attachment; filename=export.xls");
            }
            else
            {
                context.Response.ContentType 
                    = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                context.Response.AppendHeader(
                    "content-disposition", "attachment; filename=export.xlsx");
            }
        }

        private DataRow GetDataRow(
            EntityExportInfo info, int rowNumber, bool isPkGuid)
        {
            object pk = info.RowIds[rowNumber - 1];
            if(isPkGuid && (pk is string))
            {
                pk = new Guid((string)pk);
            }
            DataRow row = info.Table.Rows.Find(pk);
            // make sure lazy loaded list gets filled
            if(info.Store.IsLazyLoadedRow(row))
            {
                info.Store.LazyLoadListRowData(pk, row);
            }
            return row;
        }

        private Object GetLookupValue(object key, string lookupId)
        {
            if(!lookupCache.ContainsKey(lookupId))
            {
                lookupCache.Add(lookupId, new Dictionary<object, object>());
            }
            IDictionary<object, object> cache = lookupCache[lookupId];
            if (!cache.ContainsKey(key))
            {
                cache.Add(key, lookupService.GetDisplayText(
                    new Guid(lookupId), key, false, false, null));
            }
            return cache[key];
        }

        private void SetCellValue(IWorkbook workbook, object val, ICell cell)
        {
            if((val != null) && (val.GetType() == typeof(ArrayList)))
            {
                String delimiter = ",";
                String escapeDelimiter = "\\,";
                StringBuilder sb = new StringBuilder();
                foreach(object arrayItem in (ArrayList)val)
                {
                    // add array item to stream
                    if (sb.Length > 0) sb.Append(delimiter);
                    string inc = arrayItem.ToString();
                    // escape quote chars
                    sb.Append(inc.Replace(delimiter, escapeDelimiter));
                }
                cell.SetCellValue(sb.ToString());
            }
            else
            {
                SetScalarCellValue(workbook, val, cell);
            }
        }

        private void SetScalarCellValue(
            IWorkbook workbook, object val, ICell cell)
        {
            if(val == null)
            {
                return;
            }
            if(val is DateTime)
            {
                cell.SetCellValue((DateTime)val);
                cell.CellStyle = dateCellStyle;
            }
            else if((val is int) || (val is double) 
            || (val is float) || (val is decimal))
            {
                cell.SetCellValue(Convert.ToDouble(val));
            }
            else
            {
                string fieldValue = val.ToString();
                if(fieldValue.Contains("\r"))
                {
                    fieldValue = fieldValue.Replace("\n", "");
                    fieldValue = fieldValue.Replace("\r", Environment.NewLine);
                    fieldValue = fieldValue.Replace("\t", " ");
                    cell.SetCellValue(fieldValue);
                }
                else
                {
                    cell.SetCellValue(fieldValue);
                }
            }
        }

        #endregion
    }
}

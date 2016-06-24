using aZaaS.KStar;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Utilities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public class ExportController : BaseMvcController
    {
        /// <summary>
        /// Create the Excel spreadsheet.
        /// </summary>
        /// <param name="model">Definition of the columns for the spreadsheet.</param>
        /// <param name="data">Grid data.</param>
        /// <param name="title">Title of the spreadsheet.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult ToExcel(string column, string data, string filter, string title)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                /* Create the worksheet. */

                SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
                Excel.AddBasicStyles(spreadsheet);
                Excel.AddAdditionalStyles(spreadsheet);
                Excel.AddWorksheet(spreadsheet, title);
                Worksheet worksheet = spreadsheet.WorkbookPart.WorksheetParts.First().Worksheet;

                /* Get the information needed for the worksheet */

                var columnObject = JsonConvert.DeserializeObject<List<KendoGridColumn>>(column);
                var dataObject = JsonConvert.DeserializeObject<dynamic>(data);
                if (dataObject.GetType() == typeof(string))
                {
                    dataObject = JsonConvert.DeserializeObject<dynamic>(data.Substring(1).Substring(0, data.Length - 2).Replace("\\", ""));
                }

                /* Add the column titles to the worksheet. */
                Excel.SetCellValue(spreadsheet, worksheet, 1, 2, title, false, false);
                Excel.SetCellValue(spreadsheet, worksheet, 1, 3, string.Format(@"Generated On: {0}", DateTime.Now.ToString(PortalEnvironment.DateTimeFormat)), false, false);
                int startrow = 3;
                if (!string.IsNullOrEmpty(filter))
                {
                    var filterObject = JsonConvert.DeserializeObject<dynamic>(filter);
                    foreach (var key in filterObject)
                    {
                        startrow++;
                        //var keyObject = JsonConvert.DeserializeObject<dynamic>(key);
                        var k = key.Name;
                        var v = key.Value;
                        Excel.SetCellValue(spreadsheet, worksheet, 1, Convert.ToUInt32(startrow), string.Format(@"{0} = {1}", GetDisplayName(k), v), false, false);
                    }
                }

                startrow += 3;

                // For each column...
                int m = 0;
                for (int mdx = 0; mdx < columnObject.Count; mdx++)
                {
                    try
                    {
                        if (columnObject[mdx].field != null && columnObject[mdx].field.ToString() != "")
                        {
                            // If the column has a title, use it.  Otherwise, use the field name.
                            Excel.SetColumnHeadingValue(spreadsheet, worksheet, Convert.ToUInt32(m + 1), Convert.ToUInt32(1 + startrow),
                                columnObject[mdx].title == null ? columnObject[mdx].field.ToString() : columnObject[mdx].title.ToString(),
                                false, false);

                            // Is there are column width defined?
                            Excel.SetColumnWidth(worksheet, m + 1, columnObject[mdx].width != null
                                ? Convert.ToInt32(columnObject[mdx].width.ToString()) / 4
                                : 25);

                            // Is there are column Hidden defined?
                            //Excel.SetColumnHidden(worksheet, m + 1, columnObject[mdx].hidden != null && columnObject[mdx].hidden == true);
                            m++;
                        }
                    }
                    catch(RuntimeBinderException ex){}                   
                }

                /* Add the data to the worksheet. */

                // For each row of data...
                int cdx = startrow;
                SetValues(spreadsheet, worksheet, columnObject, dataObject, 0, ref cdx);


                /* Save the worksheet and store it in Session using the spreadsheet title. */

                worksheet.Save();
                spreadsheet.Close();
                byte[] file = stream.ToArray();
                Session[title] = file;
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult ToExcelWithOutHidden(string column, string data, string filter, string title)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                /* Create the worksheet. */

                SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
                Excel.AddBasicStyles(spreadsheet);
                Excel.AddAdditionalStyles(spreadsheet);
                Excel.AddWorksheet(spreadsheet, title);
                Worksheet worksheet = spreadsheet.WorkbookPart.WorksheetParts.First().Worksheet;

                /* Get the information needed for the worksheet */

                var columnObject = JsonConvert.DeserializeObject<List<KendoGridColumn>>(column);
                var dataObject = JsonConvert.DeserializeObject<dynamic>(data);

                /* Add the column titles to the worksheet. */
                Excel.SetCellValue(spreadsheet, worksheet, 1, 2, title, false, false);
                Excel.SetCellValue(spreadsheet, worksheet, 1, 3, string.Format(@"Generated On: {0}", DateTime.Now.ToString(PortalEnvironment.DateTimeFormat)), false, false);
                int startrow = 3;
                if (!string.IsNullOrEmpty(filter))
                {
                    var filterObject = JsonConvert.DeserializeObject<dynamic>(filter);
                    foreach (var key in filterObject)
                    {
                        startrow++;
                        //var keyObject = JsonConvert.DeserializeObject<dynamic>(key);
                        var k = key.Name;
                        var v = key.Value;
                        Excel.SetCellValue(spreadsheet, worksheet, 1, Convert.ToUInt32(startrow), string.Format(@"{0} = {1}", GetDisplayName(k), v), false, false);
                    }
                }

                startrow += 3;

                // For each column...
                int m = 0;
                for (int mdx = 0; mdx < columnObject.Count; mdx++)
                {
                    try
                    {
                        if (columnObject[mdx].field != null && columnObject[mdx].field.ToString() != "")
                        {
                            if ((columnObject[mdx].hidden != null && columnObject[mdx].hidden == true) == false)
                            {
                                // If the column has a title, use it.  Otherwise, use the field name.
                                Excel.SetColumnHeadingValue(spreadsheet, worksheet, Convert.ToUInt32(m + 1), Convert.ToUInt32(1 + startrow),
                                    columnObject[mdx].title == null ? columnObject[mdx].field.ToString() : columnObject[mdx].title.ToString(),
                                    false, false);

                                // Is there are column width defined?
                                Excel.SetColumnWidth(worksheet, m + 1, columnObject[mdx].width != null
                                    ? Convert.ToInt32(columnObject[mdx].width.ToString()) / 4
                                    : 25);
                                m++;
                            }
                        }
                    }
                    catch (RuntimeBinderException ex) { } 
                }

                /* Add the data to the worksheet. */

                // For each row of data...
                int cdx = startrow;
                SetValuesWithOutHidden(spreadsheet, worksheet, columnObject, dataObject, 0, ref cdx);


                /* Save the worksheet and store it in Session using the spreadsheet title. */

                worksheet.Save();
                spreadsheet.Close();
                byte[] file = stream.ToArray();
                Session[title] = file;
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult ToCSV(string column, string data, string filter, string title)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                /* Create the worksheet. */

                SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
                Excel.AddBasicStyles(spreadsheet);
                Excel.AddAdditionalStyles(spreadsheet);
                Excel.AddWorksheet(spreadsheet, title);
                Worksheet worksheet = spreadsheet.WorkbookPart.WorksheetParts.First().Worksheet;


                /* Get the information needed for the worksheet */

                var columnObject = JsonConvert.DeserializeObject<List<KendoGridColumn>>(column);
                var dataObject = JsonConvert.DeserializeObject<dynamic>(data);
                if (dataObject.GetType() == typeof(string))
                {
                    dataObject = JsonConvert.DeserializeObject<dynamic>(data.Substring(1).Substring(0, data.Length - 2).Replace("\\", ""));
                }
                /* Add the column titles to the worksheet. */
                Excel.SetCellValue(spreadsheet, worksheet, 1, 2, title, false, false);
                Excel.SetCellValue(spreadsheet, worksheet, 1, 3, string.Format(@"Generated On: {0}", DateTime.Now.ToString(PortalEnvironment.DateTimeFormat)), false, false);
                int startrow = 3;
                if (!string.IsNullOrEmpty(filter))
                {
                    var filterObject = JsonConvert.DeserializeObject<dynamic>(filter);
                    foreach (var key in filterObject)
                    {
                        startrow++;
                        //var keyObject = JsonConvert.DeserializeObject<dynamic>(key);
                        var k = key.Name;
                        var v = key.Value;
                        Excel.SetCellValue(spreadsheet, worksheet, 1, Convert.ToUInt32(startrow), string.Format(@"{0} = {1}", GetDisplayName(k), v), false, false);
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        startrow++;
                        Excel.SetCellValue(spreadsheet, worksheet, 1, Convert.ToUInt32(startrow), " ", false, false);
                    }

                }

                // startrow += 3;
                // For each column...
                int m = 0;
                for (int mdx = 0; mdx < columnObject.Count; mdx++)
                {
                    try
                    {
                        if (columnObject[mdx].field != null && columnObject[mdx].field.ToString() != "")
                        {
                            // If the column has a title, use it.  Otherwise, use the field name.
                            Excel.SetColumnHeadingValue(spreadsheet, worksheet, Convert.ToUInt32(m + 1), Convert.ToUInt32(1 + startrow),
                                columnObject[mdx].title == null ? columnObject[mdx].field.ToString() : columnObject[mdx].title.ToString(),
                                false, false);

                            // Is there are column width defined?
                            Excel.SetColumnWidth(worksheet, m + 1, columnObject[mdx].width != null
                                ? Convert.ToInt32(columnObject[mdx].width.ToString()) / 4
                                : 25);

                            // Is there are column Hidden defined?
                            //Excel.SetColumnHidden(worksheet, m + 1, columnObject[mdx].hidden != null && columnObject[mdx].hidden == true);
                            m++;
                        }
                    }
                    catch (RuntimeBinderException ex) { } 
                }

                /* Add the data to the worksheet. */

                // For each row of data...
                int cdx = startrow;
                SetValues(spreadsheet, worksheet, columnObject, dataObject, 0, ref cdx);


                /* Save the worksheet and store it in Session using the spreadsheet title. */

                worksheet.Save();

                DocumentFormat.OpenXml.Spreadsheet.SheetData sheetData = worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
                var rows = sheetData.Elements<DocumentFormat.OpenXml.Spreadsheet.Row>().Select(r => new List<string>(r.Elements<DocumentFormat.OpenXml.Spreadsheet.Cell>().Select(s => s.CellValue.Text))).ToList();
                using (System.IO.MemoryStream csvstream = new System.IO.MemoryStream())
                {
                    StreamWriter sw = new StreamWriter(csvstream, System.Text.Encoding.Default);
                    foreach (var row in rows)
                    {
                        sw.WriteLine(string.Join(",", row));
                    }
                    sw.Close();
                    byte[] file = csvstream.ToArray();
                    Session[title] = file;
                }
                spreadsheet.Close();
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private string GetDisplayName(string str)
        {
            switch (str)
            {
                case "status": return "Status";
                case "recordLevel": return "Record Level";
                case "recordType": return "Record Type";
                case "customerCode": return "Customer Name";
                case "propertyCode": return "Property Code";
                case "divisionCode": return "Division Code";
                case "clusterCode": return "Cluster Code";
                case "tradeMix": return "Trade Mix";
                case "startDate": return "Record Creation Date Form";
                case "endDate": return "Record Creation Date To";
                case "endorseStartDate": return "Record Completion Date Form";
                case "endorseEndDate": return "Record Completion Date To";
            }
            return "";
        }
        private static void SetValuesWithOutHidden(SpreadsheetDocument spreadsheet, Worksheet worksheet, List<KendoGridColumn> column, dynamic data, int rdx, ref int cdx)
        {
            rdx++;
            int m = 0;
            for (int idx = 0; idx < data.Count; idx++)
            {
                var dObject = data[idx];
                if (dObject.hasSubgroups == null)
                {
                    // For each column...
                    cdx++;
                    m = 0;
                    for (int mdx = 0; mdx < column.Count; mdx++)
                    {
                        // Set the field value in the spreadsheet for the current row and column.
                        if (column[mdx].field != null && column[mdx].field.ToString() != "")
                        {
                            if ((column[mdx].hidden != null && column[mdx].hidden == true) == false)
                            {
                                var field = column[mdx].field.ToString();
                                Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(m + 1), Convert.ToUInt32(cdx + 1),
                                    dObject[column[mdx].field.ToString()].ToString(),
                                    false, false);
                                m++;
                            }
                        }
                    }
                    if (rdx > 1)
                    {
                        Excel.SetRowOutlineLevel(worksheet, cdx + 1, rdx);
                    }
                }
                else
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < rdx; i++)
                    {
                        sb.Append("    ");
                    }
                    cdx++;
                    string title = sb.ToString() + dObject.field + ":" + dObject.value + "----↓↓↓----Start----";
                    CreateEmptyCell(spreadsheet, worksheet, column, cdx);
                    Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(1), Convert.ToUInt32(cdx + 1),
                            title,
                            false, false);
                    Excel.SetRowOutlineLevel(worksheet, cdx + 1, rdx);
                    Excel.MergeCellofRow(worksheet, cdx + 1);
                    SetValues(spreadsheet, worksheet, column, dObject.items, rdx, ref cdx);
                    cdx++;
                    string end = sb.ToString() + dObject.field + ":" + dObject.value + "----↑↑↑----End----";
                    CreateEmptyCell(spreadsheet, worksheet, column, cdx);
                    Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(1), Convert.ToUInt32(cdx + 1),
                            end,
                            false, false);
                    Excel.SetRowOutlineLevel(worksheet, cdx + 1, rdx);
                    Excel.MergeCellofRow(worksheet, cdx + 1);
                }
            }
        }
        private static void SetValues(SpreadsheetDocument spreadsheet, Worksheet worksheet, List<KendoGridColumn> column, dynamic data, int rdx, ref int cdx)
        {
            rdx++;
            int m = 0;
            for (int idx = 0; idx < data.Count; idx++)
            {
                var dObject = data[idx];
                if (dObject.hasSubgroups == null)
                {
                    // For each column...
                    cdx++;
                    m = 0;
                    for (int mdx = 0; mdx < column.Count; mdx++)
                    {
                        // Set the field value in the spreadsheet for the current row and column.
                        if (column[mdx].field != null && column[mdx].field.ToString() != "")// && dObject[column[mdx].field.ToString()]!=null)
                        {
                            var field = column[mdx].field.ToString();
                            Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(m + 1), Convert.ToUInt32(cdx + 1),
                                (dObject[column[mdx].field.ToString()]==null ? "" : dObject[column[mdx].field.ToString()].ToString()),
                                false, false);

                            m++;
                        }
                    }
                    if (rdx > 1)
                    {
                        Excel.SetRowOutlineLevel(worksheet, cdx + 1, rdx);
                    }
                }
                else
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < rdx; i++)
                    {
                        sb.Append("    ");
                    }
                    cdx++;
                    string title = sb.ToString() + dObject.field + ":" + dObject.value + "----↓↓↓----Start----";
                    CreateEmptyCell(spreadsheet, worksheet, column, cdx);
                    Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(1), Convert.ToUInt32(cdx + 1),
                            title,
                            false, false);
                    Excel.SetRowOutlineLevel(worksheet, cdx + 1, rdx);
                    Excel.MergeCellofRow(worksheet, cdx + 1);
                    SetValues(spreadsheet, worksheet, column, dObject.items, rdx, ref cdx);
                    cdx++;
                    string end = sb.ToString() + dObject.field + ":" + dObject.value + "----↑↑↑----End----";
                    CreateEmptyCell(spreadsheet, worksheet, column, cdx);
                    Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(1), Convert.ToUInt32(cdx + 1),
                            end,
                            false, false);
                    Excel.SetRowOutlineLevel(worksheet, cdx + 1, rdx);
                    Excel.MergeCellofRow(worksheet, cdx + 1);
                }
            }
        }

        private static void CreateEmptyCell(SpreadsheetDocument spreadsheet, Worksheet worksheet, dynamic column, int cdx)
        {
            int m = 0;
            for (int mdx = 0; mdx < column.Count; mdx++)
            {
                // Set the field value in the spreadsheet for the current row and column.
                if (column[mdx].field != null && column[mdx].field.ToString() != "")
                {
                    var field = column[mdx].field.ToString();
                    Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(m + 1), Convert.ToUInt32(cdx + 1),
                        "".ToString(),
                        false, false);
                    m++;
                }
            }
        }

        /// <summary>
        /// Download the spreadsheet.
        /// </summary>
        /// <param name="title">Title of the spreadsheet.</param>
        /// <returns></returns>
        public FileResult Get(string title)
        {
            // Is there a spreadsheet stored in session?
            if (Session[title] != null)
            {
                // Get the spreadsheet from seession.
                byte[] file = Session[title] as byte[];
                string filename = string.Format("{0}.xlsx", title);

                // Remove the spreadsheet from session.
                Session.Remove(title);

                // Return the spreadsheet.
                Response.Buffer = true;

                #region return File(file,...,filename); 会自动添加 Content-Disposition 所以  无需AddHeader("Content-Disposition","...")
                //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename)); 
                #endregion

                return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
            }
            else
            {
                throw new Exception(string.Format("{0} not found", title));
            }
        }

        public FileResult GetCSV(string title)
        {
            // Is there a spreadsheet stored in session?
            if (Session[title] != null)
            {
                // Get the spreadsheet from seession.
                byte[] file = Session[title] as byte[];
                string filename = string.Format("{0}.csv", title);

                // Remove the spreadsheet from session.
                Session.Remove(title);

                // Return the spreadsheet.
                Response.Buffer = true;

                #region return File(file,...,filename); 会自动添加 Content-Disposition 所以  无需AddHeader("Content-Disposition","...")
                //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", filename)); 
                #endregion

                return File(file, "text/plain", filename);
            }
            else
            {
                throw new Exception(string.Format("{0} not found", title));
            }
        }



        public JsonResult ExportAppstoExcel(string pane)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                /* Create the worksheet. */

                SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
                Excel.AddBasicStyles(spreadsheet);
                Excel.AddAdditionalStyles(spreadsheet);
                DataSet ds = new aZaaS.KStar.MenuFacade().GetApps(pane);
                int tableCount = ds.Tables.Count;
                for (int n = 0; n < tableCount; n++)
                {
                    DataTable table = ds.Tables[n];

                    SaveTable(spreadsheet, table);
                }

                spreadsheet.Close();
                byte[] file = stream.ToArray();
                string title = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (string.IsNullOrWhiteSpace(pane))
                {
                    title = "Application_Config_" + title;
                }
                else
                {
                    title = pane + "_Config_" + title;
                }
                Session[title] = file;
                return Json(title);
                //return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xlsx");
            }


        }
        public JsonResult ExportPortalstoExcel()
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                /* Create the worksheet. */

                SpreadsheetDocument spreadsheet = Excel.CreateWorkbook(stream);
                Excel.AddBasicStyles(spreadsheet);
                Excel.AddAdditionalStyles(spreadsheet);

                DataTable table = new aZaaS.KStar.Facades.PortalEnvironment().GetPortals();
                SaveTable(spreadsheet, table);

                spreadsheet.Close();
                byte[] file = stream.ToArray();
                string title = "Portal_Config_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                Session[title] = file;
                return Json(title);
            }
        }

        private void SaveTable(SpreadsheetDocument spreadsheet, DataTable table)
        {
            string tableName = table.TableName;

            Excel.AddWorksheet(spreadsheet, tableName);
            Worksheet worksheet = spreadsheet.WorkbookPart.WorksheetParts.Last().Worksheet;
            int columnscount = table.Columns.Count;
            for (int j = 0; j < columnscount; j++)
            {
                string columnName = table.Columns[j].ColumnName.ToString();
                Excel.SetColumnHeadingValue(spreadsheet, worksheet, Convert.ToUInt32(j + 1), columnName, false, false);
            }
            int rowIndex = 1;
            foreach (DataRow row in table.Rows)
            {
                rowIndex++;
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    string Value = row[i].ToString();
                    Excel.SetCellValue(spreadsheet, worksheet, Convert.ToUInt32(i + 1), Convert.ToUInt32(rowIndex), Value, false, false);
                }
            }

            worksheet.Save();
        }
        [HttpPost]
        [ActionName("ImportfromExcel")]
        public ActionResult ImportsfromExcel(string pane, List<HttpPostedFileBase> files, string type = "app")
        {
            foreach (var file in files)
            {
                var fileName = System.IO.Path.GetFileName(file.FileName);
                string extension = System.IO.Path.GetExtension(fileName);
                if (extension != ".xls" && extension != ".xlsx")
                {
                    return null;
                }
                DataSet ds = ReadExcel(file.InputStream);
                if (type == "portal")
                {
                    new KStar.Facades.PortalEnvironment().SetPortals(ds.Tables["PortalEnvironmentEntity"]);
                }
                else if (type == "app")
                {
                    if (string.IsNullOrWhiteSpace(pane))
                    {
                        new KStar.MenuFacade().SetApps(ds);
                    }
                    else
                    {
                        var MenuTable = ds.Tables["MenuEntity"];
                        if (MenuTable.Rows.Count == 1)
                        {
                            new KStar.MenuFacade().SetApps(ds);
                        }
                    }
                }
            }


            return Content("");
        }
        public ActionResult ImportfromExcel(string pane, string type = "app")
        {
            ViewBag.pane = pane;
            ViewBag.type = type;
            return View();
        }
        #region 导入File to DataSet
        /// <summary>
        /// 按照给定的Excel流组织成Datatable
        /// </summary>
        /// <param name="stream">Excel文件流</param>
        /// <param name="sheetName">须要读取的Sheet</param>
        /// <returns>组织好的DataTable</returns>
        private DataSet ReadExcel(Stream stream)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
            {
                DataSet ds = new DataSet();
                IEnumerable<Sheet> allSheets = document.WorkbookPart.Workbook.Descendants<Sheet>();
                foreach (var sheet in allSheets)
                {
                    WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);
                    //获取Excel中共享数据
                    SharedStringTable stringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                    IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();//获得Excel中得数据行
                    DataTable dt = new DataTable(sheet.Name);
                    //因为须要将数据导入到DataTable中,所以我们假定Excel的第一行是列名,从第二行开端是行数据
                    foreach (Row row in rows)
                    {
                        if (row.RowIndex == 1)
                        {//Excel第一行动列名
                            GetDataColumn(row, stringTable, ref dt);
                            continue;
                        }
                        GetDataRow(row, stringTable, ref dt);//Excel第二行同时为DataTable的第一行数据
                    }
                    ds.Tables.Add(dt);
                }
                return ds;
            }
        }
        /// <summary>
        /// 构建DataTable的列
        /// </summary>
        /// <param name="row">OpenXML定义的Row对象</param>
        /// <param name="stringTablePart"></param>
        /// <param name="dt">须要返回的DataTable对象</param>
        /// <returns></returns>
        public void GetDataColumn(Row row, SharedStringTable stringTable, ref DataTable dt)
        {
            DataColumn col = new DataColumn();
            Dictionary<string, int> columnCount = new Dictionary<string, int>();
            foreach (Cell cell in row)
            {
                string cellVal = GetValue(cell, stringTable);
                col = new DataColumn(cellVal);
                if (IsContainsColumn(dt, col.ColumnName))
                {
                    if (!columnCount.ContainsKey(col.ColumnName))
                        columnCount.Add(col.ColumnName, 0);
                    col.ColumnName = col.ColumnName + (columnCount[col.ColumnName]++);
                }
                dt.Columns.Add(col);
            }
        }
        /// <summary>
        /// 构建DataTable的每一行数据,并返回该Datatable
        /// </summary>
        /// <param name="row">OpenXML的行</param>
        /// <param name="stringTablePart"></param>
        /// <param name="dt">DataTable</param>
        private void GetDataRow(Row row, SharedStringTable stringTable, ref DataTable dt)
        {
            // 读取算法：按行一一读取单位格,若是整行均是空数据
            // 则忽视改行(因为本人的工作内容不须要空行)-_-
            DataRow dr = dt.NewRow();
            int i = 0;
            int nullRowCount = i;
            foreach (Cell cell in row)
            {
                string cellVal = GetValue(cell, stringTable);
                if (cellVal == string.Empty)
                {
                    nullRowCount++;
                }
                dr[i] = cellVal;
                i++;
            }
            if (nullRowCount != i)
            {
                dt.Rows.Add(dr);
            }
        }
        /// <summary>
        /// 获取单位格的值
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="stringTablePart"></param>
        /// <returns></returns>
        private string GetValue(Cell cell, SharedStringTable stringTable)
        {
            //因为Excel的数据存储在SharedStringTable中,须要获取数据在SharedStringTable 中的索引
            string value = string.Empty;
            try
            {
                if (cell.ChildElements.Count == 0)
                    return value;
                value = cell.CellValue.InnerText.ToString();
                if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
                {
                    value = stringTable.ChildElements[Int32.Parse(value)].InnerText;
                }
            }
            catch (Exception)
            {
                value = "N/A";
            }
            return value;
        }
        /// <summary>
        /// 判断网格是否存在列
        /// </summary>
        /// <param name="dt">网格</param>
        /// <param name="columnName">列名</param>
        /// <returns></returns>
        public bool IsContainsColumn(DataTable dt, string columnName)
        {
            if (dt == null || columnName == null)
            {
                return false;
            }
            return dt.Columns.Contains(columnName);
        }
        #endregion

        public JsonResult ExportCharttoXml(Guid? chartId)
        {

            //ChartBO
            OrgChartBO _chartBO = new OrgChartBO();

            string xmlStr;
            string title;
            if (chartId == null)
            {
                xmlStr = _chartBO.ChartsToXml();
                title = "AllChart";
            }
            else
            {
                xmlStr = _chartBO.ChartToXml(chartId.Value);
                title = _chartBO.ReadChart(chartId.Value).Name;
            }
            System.Xml.XmlDocument dom = new System.Xml.XmlDocument();
            dom.LoadXml(xmlStr);
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                dom.Save(stream);
                byte[] file = stream.ToArray();
                Session[title] = file;
                return Json(title, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Download the xml.
        /// </summary>
        /// <param name="title">Title of the xml.</param>
        /// <returns></returns>
        public FileResult GetXml(string title)
        {
            // Is there a spreadsheet stored in session?
            if (Session[title] != null)
            {
                // Get the spreadsheet from seession.
                byte[] file = Session[title] as byte[];
                string filename = string.Format("{0}.xml", title);

                // Remove the spreadsheet from session.
                Session.Remove(title);

                // Return the spreadsheet.
                Response.Buffer = true;

                return File(file, "application/xml", filename);
            }
            else
            {
                throw new Exception(string.Format("{0} not found", title));
            }
        }
    }

    public class KendoGridColumn
    {
        public string encoded { get; set; }
        public string field { get; set; }
        public string title { get; set; }
        public string width { get; set; }
        public string template { get; set; }
        public string headerTemplate { get; set; }        
        public string format { get; set; }
        public bool hidden { get; set; }       
    }
}

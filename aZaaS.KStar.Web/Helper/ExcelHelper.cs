using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Helper
{
    public static class ExcelHelper
    {
        public static DataTable ExcelToTable(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate); //读取文件流  
            XSSFWorkbook workbook = new XSSFWorkbook(fs);  //根据EXCEL文件流初始化工作簿  
            var sheet1 = workbook.GetSheetAt(0); //获取第一个sheet  
            DataTable table = new DataTable();//  
            var row1 = sheet1.GetRow(0);//获取第一行即标头  
            int cellCount = row1.LastCellNum; 
            try
            {
                //把第一行的数据添加到datatable的列名  
                for (int i = row1.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(row1.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                int rowCount = sheet1.LastRowNum; //总行数  
                //把每行数据添加到datatable中  
                for (int i =1; i <=sheet1.LastRowNum; i++)
                {
                    IRow row = sheet1.GetRow(i);
                    DataRow dataRow = table.NewRow();

                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j).ToString();
                    }

                    table.Rows.Add(dataRow);
                }
            }
            catch (Exception ex) { }            
            workbook = null; 
            sheet1 = null;  
            return table;
        }
    }
}
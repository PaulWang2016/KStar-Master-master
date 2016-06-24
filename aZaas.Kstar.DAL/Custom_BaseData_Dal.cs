using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.Kstar.DAL
{
    public class Custom_BaseData_Dal
    {
        static object unique = new object();

        private static int PageSize = 10;
  
        /// <summary>
        /// 获取标贴模板（排序方式暂时默认Name,默认第一条为列标题条目） 
        /// </summary>
        /// <param name="pageIndex">获取地N页数据</param>
        /// <param name="searchString">返回查询到的第N页的数据的json 数组对象</param>
        /// <returns></returns>
        public static string GetlabelTemplates(int pageIndex, string searchString = null)
        {
            #region sql
            string sql = @"select 0 as id,[Number],[Name],[Principal],[Picture],[Description]  from
                            (
                             SELECT
                                   c.name as field, 
                                   ex.value  
                             FROM  
                                 sys.columns c  
                             LEFT OUTER JOIN  
                                 sys.extended_properties ex  
                             ON  
                                ex.major_id = c.object_id 
                                AND ex.minor_id = c.column_id  
                                AND ex.name = 'MS_Description'  
                            WHERE  
                                OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0  
                                AND OBJECT_NAME(c.object_id) = 'Custom_BaseData_labelTemplate'--表名
                             ) as  s pivot
                             ( max(s.value)
                                  for s.field in([Number],[Name],[Principal],[Picture],[Description] )
                             ) as columnsName
 
                            select top 10 * from (select  ROW_NUMBER() over(order by [Number] asc) as [id],[Number],[Name],[Principal],[Picture],[Description] from Custom_BaseData_labelTemplate "
                             + (string.IsNullOrWhiteSpace(searchString) ? "" : string.Format(" where [Number] LIKE N'%{0}%' or [Name] LIKE N'%{0}%' or [Description] LIKE  N'%{0}%' or  [Principal] LIKE N'%{0}%'  ", searchString))
                             + @" ) as Custom_BaseData_labelTemplate  
                             where id>{0}*{1} and id<={0}*({1}+1)";
            #endregion

            sql = string.Format(sql, PageSize, pageIndex);

            DataTableCollection dts = SqlHelper.ExecuteTables(sql);

            string jsonDataBody = Newtonsoft.Json.JsonConvert.SerializeObject(dts[1], Newtonsoft.Json.Formatting.None);
            string jsonDatahead = Newtonsoft.Json.JsonConvert.SerializeObject(dts[0], Newtonsoft.Json.Formatting.None);
            string jsonData = jsonDatahead.Substring(0, jsonDatahead.Length - 1) + (jsonDataBody.Length == 2 ? "]" : "," + jsonDataBody.Substring(1));
            return jsonData;
        }
      
        public static string GetBaseData(string tableName, int pageIndex, string searchString = null, FiltrateEntity[] filter = null, string[] displayField = null)
        {
            tableName = "Custom_BaseData_" + tableName.Trim();
            Dictionary<string, string> dataList = GetColumns(tableName);
            searchString = searchString == null ? "" : searchString.Replace(",", "");
            string fields = "";//显示字段集合
            string likeFieldstr = "";//显示字段
            string sortfield = "ID";
            if (!dataList.Keys.Contains(sortfield))
            {
                sortfield = dataList.Keys.First();
            }
            foreach (string str in dataList.Keys)
            {

                if (displayField == null || displayField.Contains(str))
                    fields += "," + str;
                if (!string.IsNullOrWhiteSpace(searchString))
                {
                    if (dataList.First().Key == str)
                    {
                        if (dataList.Count > 1)
                        {
                            likeFieldstr += " and (" + str + " LIKE N'%" + searchString + "%' ";
                        }
                        else
                        {
                            likeFieldstr += " and " + str + " LIKE N'%" + searchString + "%' ";
                        }
                    }
                    else if (dataList.Last().Key == str)
                    {
                        likeFieldstr += " or " + str + " LIKE N'%" + searchString + "%') ";
                    }
                    else
                    {
                        likeFieldstr += "or " + str + " LIKE N'%" + searchString + "%' ";
                    }
                }
            }
            fields = fields.Remove(0, 1);
            //参数
            List<SqlParameter> parameters;
            //过滤条件
            string filterWhere = FiltrateOperate.ToWhereString(filter, out parameters);

            #region sql
            string sql = @"select 0 as _id,{2}  from
                            (
                             SELECT
                                   c.name as field, 
                                   ex.value
                             FROM  
                                 sys.columns c  
                             LEFT OUTER JOIN  
                                 sys.extended_properties ex  
                             ON  
                                ex.major_id = c.object_id 
                                AND ex.minor_id = c.column_id  
                                AND ex.name = 'MS_Description'  
                            WHERE  
                                OBJECTPROPERTY(c.object_id, 'IsMsShipped')=0  
                                AND OBJECT_NAME(c.object_id) = '{3}'--表名
                             ) as  s pivot
                             ( max(s.value)
                                  for s.field in({2})
                             ) as columnsName
 
                            select top 10 * from (select  ROW_NUMBER() over(order by {4} desc) as [_id],{2} from {3} "
                + (string.IsNullOrWhiteSpace(likeFieldstr + filterWhere) ? "" : " where 1=1 ")
                + (string.IsNullOrWhiteSpace(likeFieldstr) ? "" : likeFieldstr)
                + (string.IsNullOrWhiteSpace(filterWhere) ? "" : filterWhere)
            + @" ) as {3}  
                             where _id>{0}*{1} and _id<={0}*({1}+1)";
            sql = string.Format(sql, PageSize, pageIndex, fields, tableName, sortfield);
            #endregion
            DataTableCollection dts = string.IsNullOrWhiteSpace(filterWhere) ? SqlHelper.ExecuteTables(sql) : SqlHelper.ExecuteTables(sql, parameters.ToArray());


            string jsonDataBody = Newtonsoft.Json.JsonConvert.SerializeObject(dts[1], Newtonsoft.Json.Formatting.None);
            string jsonDatahead = Newtonsoft.Json.JsonConvert.SerializeObject(dts[0], Newtonsoft.Json.Formatting.None);
            string jsonData = jsonDatahead.Substring(0, jsonDatahead.Length - 1) + (jsonDataBody.Length == 2 ? "]" : "," + jsonDataBody.Substring(1));
            return jsonData;
        }

        public static Dictionary<string, string> GetColumns(string tableName)
        {
            string sql = @"select syscolumns.Name,systypes.name as TypeName    from(
                            SELECT  Name,xtype FROM SysColumns  
                                               WHERE id=Object_Id('{0}')
				                    ) as syscolumns left join  systypes on systypes.xtype=SysColumns.xtype  and status =0";
            sql = string.Format(sql, tableName);

            DataTable dt = SqlHelper.ExecuteTable(sql);
            Dictionary<string, string> DataList = new Dictionary<string, string>();
            foreach (DataRow dr in dt.Rows)
            {
                DataList.Add((dr[0] + string.Empty), dr[1] + string.Empty);
            }
            return DataList;
        }
         
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace aZaaS.Kstar.DAL
{
    public class CommonReportDAL
    {
        /// <summary>
        /// 获取当前选择的流程配置信息
        /// </summary>
        /// <param name="ProceSetID"></param>
        /// <returns></returns>
        public static DataSet GetReportInfobyProcessSetID(string ProceSetID)
        {
            SqlParameter[] paras = { new SqlParameter("@ProcSetID", ProceSetID) };
            var ds = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_CommonReportConfig_GetConfigByProcSetID", paras);
            return ds;
        }
        /// <summary>
        /// 获取所有流程
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllProcessList()
        {
            var table = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_CommonReport_GetAllProcessList", null).Tables[0];
            return table;
        }

        public static DataSet Find(string ProcSetID, string strWhere, int pageIndex, int pageSize, string CurrentUser, string SysID, string Status)
        {
            var ds = GetReportInfobyProcessSetID(ProcSetID);
            StringBuilder strSql = new System.Text.StringBuilder();
            string WhereSql = string.Empty;
            ///默认查询的语句,多表查询
            string tableSql = @" FROM view_ProcinstList AS t INNER JOIN ProcessFormHeader AS pfh ON pfh.ProcInstID=t.ProcInstID
                             INNER JOIN ProcessFormContent AS pfc ON pfc.FormID=pfh.FormID
                             INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=t.ProcID
                             INNER JOIN k2.ServerLog.ProcSet AS ps ON p.ProcSetID=ps.ID
                             INNER JOIN [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps ON ps.ID=cps.ProcessSetID 
                             LEFT JOIN ProcessFormCancel AS pfc2 ON pfc2.ProcInstId=t.ProcInstID 
                            where cps.ProcessSetID=" + ProcSetID+"" ;
            //查询字段
            string FieldSql = @"SELECT  ROW_NUMBER() OVER (ORDER BY t.ProcInstID DESC) AS ROWID, t.ProcInstID,cps.ViewUrl,pfc.FormID,
                            right(t.Folio,6) AS FlowNo,
                            SUBSTRING(t.Folio,0,CHARINDEX('-',t.Folio)) AS SUBJECT,
                            t.StartName,
                            t.Destination,
                            CONVERT(nvarCHAR(19),t.StartDate,20) as StartDate,
                            isnull(replace(cast(pfc2.[Status] AS NVARCHAR(20)),'5','作废'),(SELECT ps2.StatusName
                           FROM ProcessStatus AS ps2 WHERE ps2.StatusID=t.[Status])) AS Status " + BuildDynamicField(ds.Tables[1]);
            ///查询条件 p
            string strCondition = BuildDynameSearchSql(ds.Tables[0],strWhere);

            strCondition += " and t.ProcInstID in (" + GenerateSearchSqlBySupervise(SysID, CurrentUser, ProcSetID) + ")";
            if (!string.IsNullOrEmpty(Status))
            {
                if (Status == "9")
                {
                    strCondition += " and t.Status=3 and pfc2.Status=5";
                }
                else
                {
                    strCondition += " and t.Status=" + Status + "  AND t.ProcInstID NOT IN (SELECT ProcessFormCancel.ProcInstId FROM ProcessFormCancel )";
                }
            }
            ///完整的查询语句
            string allSql = FieldSql + tableSql + strCondition;
            string countSql = allSql;
            allSql = "SELECT * FROM (" + allSql + ") AS temp where temp.ROWID BETWEEN " +( Convert.ToInt32((pageIndex - 1) * pageSize) + 1) + " AND " + pageIndex * pageSize;
            allSql += " SELECT COUNT(*) FROM (" + countSql + ") AS temp";
            return SqlHelper.ExecuteDataSet(CommandType.Text, allSql, null);
                             
                        
        }
        /// <summary>
        /// 构建查询字段
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string BuildDynamicField(DataTable table)
        {


            string strFieldSql = "";
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow r = table.Rows[i];
                if (r["FieldType"].ToString() == "文本" || r["FieldType"].ToString().ToUpper() == "TXT")
                {
                    if (!string.IsNullOrEmpty(r["DataResource"].ToString().Trim()))
                    {
                        string data = r["DataResource"].ToString();
                        string[] Strstruct = data.Split(':');
                        if (Strstruct.Length > 1)
                        {
                            string[] FieldList = Strstruct[1].Split(',');
                            if (Strstruct[0].ToUpper().Trim() == "TB")
                            {
                                strFieldSql += ",(SELECT TOP 1 " + FieldList[2] + " FROM " + FieldList[0] + " WHERE " + FieldList[1] + "=xmlData.value('(" + r["xpath"].ToString().Trim() + ")[1]','varchar(100)')) AS " + r["FieldID"].ToString() +" \r\n";
                            }
                            else if (Strstruct[0].ToUpper().Trim() == "SQL")
                            {
                                strFieldSql += "," + Strstruct[1] + " as " + r["FieldID"].ToString()+ "\r\n";
                            }
                        }
                        else
                        {
                            strFieldSql += ",(select top 1 [Name] from [aZaaS.KStar].[dbo].[DataDictionary] where Code=xmlData.value('(" + r["xpath"].ToString().Trim() + ")[1]','varchar(100)')) as " + r["FieldID"].ToString() + " \r\n";
                        }
                    }
                    else
                    {
                        strFieldSql += ",xmlData.value('(" + r["xpath"].ToString().Trim() + ")[1]','nvarchar(1000)') as " + r["FieldID"].ToString() + " \r\n";
                    }
                }
                if (r["FieldType"].ToString() == "时间" || r["FieldType"].ToString().ToUpper()=="DATETIME")
                {
                    strFieldSql += ",Convert(NVARCHAR(19),xmlData.value('(" + r["xpath"].ToString().Trim() + ")[1]','datetime'),20) as " + r["FieldID"].ToString() + " \r\n";
                }
            }
            //if (strFieldSql.Length > 0)
            //{
            //    strFieldSql = strFieldSql.Substring(0, strFieldSql.Length - 1);
            //}
            return strFieldSql;
        }
        /// <summary>
        /// 构建查询条件
        /// </summary>
        /// <param name="table"></param>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public static string BuildDynameSearchSql(DataTable table,string strWhere)
        {
            if (string.IsNullOrEmpty(strWhere))
            {
                return string.Empty;
            }
            string SelectSql = " ";
            string[] searchCondition = strWhere.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in searchCondition)
            {
                string[] myField = s.Split('|');
                if (myField[0] == "Folio")
                {
                    if (!string.IsNullOrEmpty(myField[1]))
                    {
                        SelectSql += " and t.Folio like '%" + myField[1] + "%' \r\n";
                    }
                    continue;
                }
                if (myField[0] == "StartName")
                {
                    if (!string.IsNullOrEmpty(myField[1]))
                    {
                        SelectSql += " and t.StartName like '%" + myField[1] + "%' \r\n";
                    }
                    continue;
                }
                if (myField[0] == "StartDate")
                {
                    if (!string.IsNullOrEmpty(myField[1]))
                    {
                        SelectSql += " and t.StartDate >= '" + myField[1] + " 00:00:00' \r\n";
                    }
                    continue;
                }
                if (myField[0] == "EndDate")
                {
                    if (!string.IsNullOrEmpty(myField[1]))
                    {
                        SelectSql += " and t.StartDate <= '" + myField[1] + " 23:59:59' \r\n";
                    }
                    continue;
                }
                foreach (DataRow r in table.Rows)
                {
                    string fieldID = r["FieldID"].ToString();
                    if (fieldID == myField[0])
                    {
                        if (!string.IsNullOrEmpty(myField[1]))
                        {
                            if (!string.IsNullOrEmpty(r["DataResource"].ToString().Trim()))
                            {
                                string data = r["DataResource"].ToString();
                                string[] Strstruct = data.Split(':');
                                if (Strstruct[0].ToUpper() == "TB")
                                {
                                    string[] FieldList = Strstruct[1].Split(',');
                                    SelectSql += " AND xmlData.value('(" + r["xpath"].ToString().Trim() + ")[1]','nvarchar(1000)') in (SELECT " + FieldList[1] + " FROM  " + FieldList[0] + " where " + FieldList[2] + " LIKE '%" + myField[1] + "%') \r\n";
                                }
                                else if (Strstruct[0].ToUpper() == "DD"){
                                    SelectSql += " AND xmlData.value('(" + r["xpath"].ToString().Trim() + ")[1]','nvarchar(1000)') in (SELECT code FROM  [aZaaS.KStar].[dbo].[DataDictionary] where name LIKE '%" + myField[1] + "%') \r\n";
                                }
                            }
                            else
                            {

                                string[] strOrCondition = r["xpath"].ToString().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (strOrCondition.Length > 1)
                                {
                                    for (int i = 0; i < strOrCondition.Length; i++)
                                    {
                                        string item = strOrCondition[i];
                                        if (i == 0)
                                        {
                                            SelectSql += " AND (xmlData.value('(" + item + ")[1]','nvarchar(1000)') LIKE '%" + myField[1] + "%' \r\n";
                                        }
                                        else if ((i + 1) == strOrCondition.Length)
                                        {
                                            SelectSql += " OR xmlData.value('(" + item + ")[1]','nvarchar(1000)') LIKE '%" + myField[1] + "%') \r\n";
                                        }
                                        else
                                        {
                                            SelectSql += " OR xmlData.value('(" + item + ")[1]','nvarchar(1000)') LIKE '%" + myField[1] + "%' \r\n";
                                        }
                                    }
                                }
                                else
                                {
                                    SelectSql += " and xmlData.value('(" + strOrCondition[0] + ")[1]','nvarchar(1000)') LIKE '%" + myField[1] + "%' \r\n";
                                }
                            }
                        }
                    }
                }
            }
            return SelectSql;

        }
        public static DataTable GetProcessListByUserName(string userName)
        {
            string sql = string.Format(@" SELECT *
                     FROM  [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps INNER JOIN [aZaaS.Framework].dbo.CommonReportPermission AS rps ON cps.ProcessFullName
                    =rps.ProcessFullName 
                INNER JOIN [Role] AS r ON  r.SysId= rps.Role_SysId 
                INNER JOIN RoleUsers AS ru ON ru.Role_SysId = r.SysId where ru.User_SysId='{0}' ",userName);
            var dt = SqlHelper.ExecuteDataSet(CommandType.Text, sql, null).Tables[0];
            return dt;

        }
//        /// <summary>
//        /// 查询模式,流程监控
//        /// </summary>
//        /// <param name="UserIDName"></param>
//        /// <returns></returns>
//        public static string GenerateSearchSqlByMyProcInstance(string UserIDName)
//        {
//            string sql = @" and  t.ProcInstID IN (
//                        select id as procinstid from k2.ServerLog.ProcInst where Originator =  'K2:" + UserIDName + "'";

//            sql += " union all";
//            sql += " select  procinstid from [K2].[ServerLog] .[ActInstSlot] where [user] = 'K2:" + UserIDName + "'";
//            sql += "union all";
//            sql += " select h.ProcInstID ";
//            sql += "from [aZaaS.Framework].[dbo].[ProcessFormCC] c";
//            sql += "inner join [aZaaS.Framework].[dbo].[ProcessFormHeader] h";
//            sql += " on c.FormId = h.FormID";
//            sql += "where c.Receiver = 'K2:" + UserIDName + "' AND c.ReceiverStatus=1)";
//            return sql;
//        }
        /// <summary>
        /// 查询模式.流程督办监控
        /// </summary>
        /// <param name="UserSysID"></param>
        /// <returns></returns>
        public static string GenerateSearchSqlBySupervise(string UserSysID, string UserIDName,string ProcSetID)
        {
            string sql = @" SELECT pi1.ID AS procinstid FROM k2.ServerLog.ProcInst AS pi1 
				INNER JOIN k2.ServerLog.[Proc] AS p ON p.ID=pi1.ProcID
				INNER JOIN k2.ServerLog.ProcSet AS ps ON ps.ID=p.ProcSetId			
WHERE ps.ID  in (SELECT cps.ProcessSetID
  FROM  [aZaaS.KStar].dbo.Configuration_ProcessSet AS cps INNER JOIN [aZaaS.Framework].dbo.CommonReportPermission AS rps ON cps.ProcessFullName
=rps.ProcessFullName 
INNER JOIN [Role] AS r ON  r.SysId= rps.Role_SysId 
INNER JOIN RoleUsers AS ru ON ru.Role_SysId = r.SysId where ru.User_SysId='" + @UserSysID + "') and p.ProcSetID="+ProcSetID+" \r\n";
//            sql += " union \r\n";
//            sql += @"  select id as procinstid from k2.ServerLog.ProcInst where Originator =  'K2:" + UserIDName + "'";
//            sql += " union all";
//            sql += " select  procinstid from [K2].[ServerLog] .[ActInstSlot] where [user] = 'K2:" + UserIDName + "'";
//            sql += " union all   select h.ProcInstID  \r\n";
//            sql += "from [aZaaS.Framework].[dbo].[ProcessFormCC] c \r\n";
//            sql += " inner join [aZaaS.Framework].[dbo].[ProcessFormHeader] h \r\n";
//            sql += " on c.FormId = h.FormID \r\n";
//            sql += "where c.Receiver = '" + UserIDName + "'  ";
//            sql += @" UNION SELECT  procinstid 
//                      FROM [K2].[Server].[ActionActInstRights] R
//	                    INNER JOIN [K2].[SERVER].Actioner AS a
//		                    ON R.ActionerID = A.ID
//                    WHERE [ActionerName]='K2:" +UserIDName+"'";
            return sql;
        }
    }
}

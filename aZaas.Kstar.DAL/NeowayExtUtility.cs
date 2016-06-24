using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using aZaaS.KStar.ProcessForm;
using System.IO;

namespace aZaaS.Kstar.DAL
{
    public class NeowayExtUtility
    {
        static object o = new object();
        public static string InsertFlowSerialNo(long FormId)
        {
            lock (o)
            {
                SqlParameter[] paras = new SqlParameter[3];
                paras[0] = new SqlParameter("@code", SqlDbType.VarChar, 50, "code");
                paras[0].Value = "Global";
                paras[1] = new SqlParameter("@now", SqlDbType.DateTime, 50, "now");
                paras[1].Value = DateTime.Now;
                paras[2] = new SqlParameter("@result", SqlDbType.VarChar, 50, "result");
                paras[2].Direction = ParameterDirection.Output;
                SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.StoredProcedure, "SP_GetSerialNumber", paras);
                string result = paras[2].Value.ToString();
                if (string.IsNullOrEmpty(result)) { return string.Empty; }

                string strSql = "Insert into FlowSerialNo values(@FormId,@FlowSerialNo)";
                SqlParameter[] parameter = {new SqlParameter("@FormId",FormId),
                                      new SqlParameter("@FlowSerialNo",result)};
                int effectrow = SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql, parameter);
                return result;
            }

        }
        public static string GetSerialNo(string code)
        {
            lock (o)
            {
                SqlParameter[] paras = new SqlParameter[3];
                paras[0] = new SqlParameter("@code", SqlDbType.VarChar, 50, "code");
                paras[0].Value = code;
                paras[1] = new SqlParameter("@now", SqlDbType.DateTime, 50, "now");
                paras[1].Value = DateTime.Now;
                paras[2] = new SqlParameter("@result", SqlDbType.VarChar, 50, "result");
                paras[2].Direction = ParameterDirection.Output;
                SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.StoredProcedure, "SP_GetSerialNumber", paras);
                string result = paras[2].Value.ToString();
                return result;
            }

        }
          
       
        /// <summary>
        /// 通用流程实例获取管理模块
        /// </summary>
        /// <param name="processName">流程名称</param>
        /// <param name="Folio">流程主题</param>
        /// <param name="StartDate">发起时间</param>
        /// <param name="FinishDate">流程结束时间</param>
        /// <param name="StartUser">发起人</param>
        /// <param name="Status">状态</param>
        /// <returns></returns>
        public static DataSet GetAllProcessInst(string processName, string Folio, string StartDate, string FinishDate, string StartUser, int Status, int pageSize, int pageIndex, string SysId)
        {
            SqlParameter[] paras ={new SqlParameter("@ProcessName",processName),
                                 new SqlParameter("@Folio",Folio),
                                  new SqlParameter("@StartTime",StartDate),
                                  new SqlParameter("@FinishTime",FinishDate),
                                  new SqlParameter("@StartUser",StartUser),
                                  new SqlParameter("@Status",Status),
                                  new SqlParameter("@PageSize",pageSize),
                                  new SqlParameter("@pageIndex",pageIndex),
                                 new SqlParameter("@SysId",SysId)};
            var dataSet = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "SP_NW_Common_ProcInstManager", paras);
            return dataSet;
        }
        /// <summary>
        /// 通用流程监控获取管理模块
        /// </summary>
        /// <param name="processName">流程名称</param>
        /// <param name="Folio">流程主题</param>
        /// <param name="StartDate">发起时间</param>
        /// <param name="FinishDate">流程结束时间</param>

        /// <param name="Status">状态</param>
        /// <returns></returns>
        public static DataSet GetMyProcessInst(string processName, string Folio, string StartDate, string FinishDate, string CurrentUser, int pageSize, int pageIndex, string StartUser)
        {
            SqlParameter[] paras ={
                                      new SqlParameter("@CurrentUser",CurrentUser),
                                      new SqlParameter("@Folio",Folio),
                                      new SqlParameter("@ProcessSetIds",processName),
                                      new SqlParameter("@StartTime",StartDate),
                                  new SqlParameter("@FinishTime",FinishDate),
                                  new SqlParameter("@PageSize",pageSize),
                                  new SqlParameter("@pageIndex",pageIndex),
                                  new SqlParameter("@StartUser",StartUser)
                                };
            var dataSet = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_ProcessSuperviser_InstList", paras);
            return dataSet;
        }
        public static DataSet GetProcessInstanceByUserName(string UserName, int pageIndex, int pageSize)
        {
            SqlParameter[] paras ={
                                      new SqlParameter("@UserName",UserName),
                                new SqlParameter("@pageIndex",pageIndex),
                                  new SqlParameter("@pageSize",pageSize)
                                  
                                };
            var dataSet = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_GetProcessInstanceDestinationByUserName", paras);
            return dataSet;
        }
 

        public static DataTable GetRoleProcessByRoleID(string roleID)
        {
            SqlParameter[] para = { new SqlParameter("@RoleID", roleID) };
            var table = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_Neoway_GetRoleProcessByRoleID", para).Tables[0];
            return table;
        }
        public static DataTable GetProcessSet()
        {
            var table = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "GetAllProcessSet", null).Tables[0];
            return table;

        } 
        public static DataTable GetDeliveryGoodsRecord(string ContractNo)
        {
            SqlParameter[] paras = {new SqlParameter("@ContractNo",ContractNo)
                              };
            var effectRow = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_NW_DELIVERYGOODS_GETLIST", paras).Tables[0];
            return effectRow;
        } 
         
 
        public static DataTable GetDelegateInfo(string userName, string ProcessFullName)
        {


            SqlParameter[] paras = { new SqlParameter("@userName",userName),
                                       new SqlParameter("@ProcessFullName",ProcessFullName)
                                       
                                   };
            return SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_GetDelegateInfo_byUserName", paras).Tables[0];

        }
   
 
        public static string GetTestProjectSource()
        {
            string strSql = @"
                            select 0 as id,TestType,TestChildType, TestProject, TestRound, OccupyHumanRound, OccupyPrototypeNum  from
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
                                AND OBJECT_NAME(c.object_id) = 'Custom_BaseData_HardwareTestOccupyRes'--表名
                             ) as  s pivot
                             ( max(s.value)
                                  for s.field in(TestType,TestChildType, TestProject, TestRound, OccupyHumanRound, OccupyPrototypeNum )
                             ) as columnsName
                               SELECT [Id]
                          ,[TestType]
                          ,[TestChildType]
                          ,[TestProject]
                          ,[TestRound]
                          ,[OccupyHumanRound]
                          ,[OccupyPrototypeNum]
                      FROM [Custom_BaseData_HardwareTestOccupyRes] ";
            //SqlParameter[] paras = {new SqlParameter("@ContractNo",ContractNo)
            //                   };
            var result = SqlHelper.ExecuteDataSet(CommandType.Text, strSql, null);
            string jsonDataBody = Newtonsoft.Json.JsonConvert.SerializeObject(result.Tables[1], Newtonsoft.Json.Formatting.None);
            string jsonDatahead = Newtonsoft.Json.JsonConvert.SerializeObject(result.Tables[0], Newtonsoft.Json.Formatting.None);
            string jsonData = jsonDatahead.Substring(0, jsonDatahead.Length - 1) + "," + jsonDataBody.Substring(1);
            return jsonData;
            //return effectRow;
        }
 
 
   
        /// <summary>
        ///  保存数据
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static string GetjsonData(string proc, SqlParameter[] Params)
        {
            var jsonData = "{}";
            var data = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, proc, Params);
            if (data != null && data.Tables.Count > 0)
            {
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data.Tables[0], Newtonsoft.Json.Formatting.None);
            }
            return jsonData;
        } 
        /// <summary>
        ///  获取数据
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static Object GetScalarData(string sqlText)
        {
            var data = SqlHelper.ExecuteScalar(SqlHelper.Con, CommandType.Text, sqlText);
            return data;
        } 
 
        public static DataSet GetProcessReminderInfo(string ProcessInstIdS)
        {
            SqlParameter[] paras ={new SqlParameter("@processInstIDS",ProcessInstIdS)
                                  };
            var table = SqlHelper.ExecuteDataSet(CommandType.StoredProcedure, "sp_ProcessReminder", paras);
            return table;
        }
    
        public static void InsertReminderRecord(string title, string body, string reciever)
        {
            //SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strsql, null);
            SqlParameter[] paramerter = {
                                            new SqlParameter("@Title",title),
                                            new SqlParameter("@body",body),
                                            new SqlParameter("@reciever",reciever),
                                            new SqlParameter("@Prompt",""),
                                            new SqlParameter("@Forward",false),
                                            new SqlParameter("@ReplyDate",DateTime.Now.ToString()),
                                            new SqlParameter("@CreateDate",DateTime.Now.ToString())
                                        };
            string strSql = "INSERT INTO QuartzCacheSendMails([Title],[Body] ,[ReplyTo] ,[Prompt],[Forward] ,[ReplyDate] ,[CreateDate]) values(@Title,@body,@reciever,@Prompt,@Forward,@ReplyDate,@CreateDate)";
            SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql, paramerter);
        }

        public static DataTable GetSupervisionList()
        {
            #region
            string sql = @" 
                     SELECT * from (
                            SELECT  
	                            cast(wl.ProcInstID as nvarchar(10)) + '_' + cast(wl.ActInstDestID as nvarchar(10)) as SN,
	                            wl.Destination , 
	                            wl.StartDate ,  
	                            datediff(Minute,wl.StartDate,getdate()) as ProcessedMinutes, 
	                            act.id as ActId, 
	                            act.Name as ActName , 
	                            ca.ProcessTime ProcessedDaysCfg , 
	                            wlh.Data as Url,
	                            pfh.ProcessFolio,
	                            pfh.FormSubject,
	                            pfh.FormID,
	                            U.FirstName ,
	                            U.Email,
								inst.Originator,
								iu.FirstName as OriginatorFirstName,
								iu.UserId 
                              FROM [K2].[ServerLog].[Worklist] wl
	                            inner join [K2].[Server].[WorklistHeader] wlh
		                            on wl.ProcInstID = wlh.ProcInstID
			                            and wl.ActInstDestID = wlh.ActInstDestID
			                            and wl.[Status] = 0
	                            inner join [aZaaS.Framework].[dbo].[ProcessFormHeader] pfh
		                            on wl.ProcInstID = pfh.ProcInstID
	                            inner join [K2].[Server].[Act] act
		                            on wlh.ActID = act.id
	                            inner join [aZaaS.KStar].[dbo].[Configuration_Activity] ca
		                            on wlh.ActID = ca.ActivityID
			                            and ca.ProcessTime is not null
	                            left join [aZaaS.Framework].[dbo].[User] u
		                            on wl.Destination = N'K2:' + u.UserName COLLATE SQL_Latin1_General_CP1_CI_AS
							  left join [K2].[ServerLog].[ProcInst] inst on inst.ID=wl.ProcInstID
							 left join [aZaaS.Framework].[dbo].[User] iu
		                            on inst.Originator = N'K2:' + iu.UserName COLLATE SQL_Latin1_General_CP1_CI_AS
                            where datediff(Minute,wl.StartDate,getdate()) > ca.ProcessTime  * 60 
                            ) as Supervision where SN NOT IN (SELECT SN FROM [aZaaS.Framework].[dbo].[TempSupervision])
							 
                            ";

            #endregion

            return SqlHelper.ExecuteTable(sql);
        }
         
        #region 销差  销假
        public static int UpdateBusinessTravel(string travelNo, int status)
        {
            //添加到数据库
            string sql = "update  Custom_BaseData_BusinessTravel set Status=@Status where BusinessTravelNo=@BusinessTravelNo";
            SqlParameter[] parameter = { new SqlParameter("@BusinessTravelNo", travelNo) ,
                                       new SqlParameter("@Status", status)};
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, parameter);
        }

        public static int UpdateBusinessTravel(string travelNo, string realStartTime, string realEndTime, int status)
        {
            //添加到数据库
            string sql = @"update  Custom_BaseData_BusinessTravel 
                           set Status=@Status,
                               TravelStartTime=@RealStartTime,
                               TravelEndTime=@RealEndTime
                            where BusinessTravelNo=@BusinessTravelNo";
            SqlParameter[] parameter = {   new SqlParameter("@BusinessTravelNo", travelNo),
                                           new SqlParameter("@RealStartTime", realStartTime),
                                           new SqlParameter("@RealEndTime", realEndTime),
                                           new SqlParameter("@Status", status)
                                       };
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, parameter);
        }

        public static int DeleteBusinessTravel(string travelNo)
        {
            //删除
            string sql = "delete from Custom_BaseData_BusinessTravel where BusinessTravelNo=@BusinessTravelNo";
            SqlParameter[] parameter = { new SqlParameter("@BusinessTravelNo", travelNo) };
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, parameter);
        }

        public static int UpdateLeave(string leaveNo, int status)
        {
            //添加到数据库
            string sql = "update  Custom_BaseData_Leave set Status=@Status where LeaveNo=@LeaveNo";
            SqlParameter[] parameter = { new SqlParameter("@LeaveNo", leaveNo) ,
                                       new SqlParameter("@Status", status)};
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, parameter);
        }

        public static int DeleteLeave(string leaveNo)
        {
            //删除
            string sql = "delete Custom_BaseData_Leave where LeaveNo=@LeaveNo";
            SqlParameter[] parameter = { new SqlParameter("@LeaveNo", leaveNo) };
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, parameter);
        }

        public static int UpdateLeave(string leaveNo, string realStartTime, string realEndTime, int status)
        {
            //添加到数据库
            string sql = @"update  Custom_BaseData_Leave 
                            set Status=@Status,
                            RealStartTime=@RealStartTime,
                            RealEndTime=@RealEndTime 
                           where LeaveNo=@LeaveNo";
            SqlParameter[] parameter = { new SqlParameter("@LeaveNo", leaveNo) ,
                                           new SqlParameter("@RealStartTime",realStartTime),
                                           new SqlParameter("@RealEndTime",realEndTime),
                                       new SqlParameter("@Status", status)};
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, sql, parameter);
        }
        #endregion
        /// <summary>
        /// 保存日志，适用于自定义角色中使用
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static int SaveErrorLogToDB(string exception, string methodName)
        {
            string strSql = "INSERT INTO DBLogEntry([ID],[Priority],[Source] ,[Category] ,[Message] ,[OccurTime] ,[Exception]) VALUES(NEWID(),1,'" + methodName + "','" + methodName + "','调用自定义角色出错：','" + DateTime.Now.ToString() + "',N'" + exception + "')";
            return SqlHelper.ExecuteNonQuery(SqlHelper.Con, CommandType.Text, strSql, null);
        }
        #region 用于自定角色
        public static List<string> GetPositionUserName(string OrgNodesGuID, string positionName)
        {

            string sql = @"select u.UserName from PositionOrgNodes as po
                                            left join Position as p  on p.SysId=po.Position_SysId
						                    left join PositionUsers as pu on po.Position_SysId=pu.Position_SysId
						                    left join [User] as u on pu.User_SysId = u.SysId
                        where po.[OrgNode_SysId]=@OrgNode_SysId and p.Name=@Name";
            List<SqlParameter> pList = new List<SqlParameter>();
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@OrgNode_SysId";
            p.SqlDbType = SqlDbType.UniqueIdentifier;
            p.Value = new Guid(OrgNodesGuID);
            p.Size = 50;
            pList.Add(p);

            p = new SqlParameter();
            p.ParameterName = "@Name";
            p.SqlDbType = SqlDbType.NVarChar;
            p.Value = positionName;
            p.Size = 500;
            pList.Add(p);
            System.Data.DataTable dt = aZaaS.Kstar.DAL.SqlHelper.ExecuteTable(sql, pList.ToArray());

            List<string> userNames = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                string userName = row["UserName"] + string.Empty;
                if (!string.IsNullOrWhiteSpace(userName) && userNames.Contains(userName) == false)
                {
                    userNames.Add(userName);
                }
            }
            return userNames;
        } 
        #endregion
    }
}


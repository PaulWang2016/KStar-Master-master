
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.Framework;
using aZaaS.Framework.K2Worklist;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Helper;
using aZaaS.Framework.SQLQuery;
using aZaaS.KStar.MgmtServices;
using System.Data.Entity;



namespace aZaaS.KStar
{
    /// <summary>
    /// 流程服务<see cref="WorkflowClientService"/>方法扩展（KStar-master v1.8)
    /// </summary>
    public static class WorkflowClientServiceExtensions
    {
        #region Workflow Service Extensions

        /// <summary>
        /// 获取指定用户的【我的申请】流程列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">流程列表过滤条件</param>
        /// <returns></returns>
        public static List<ProcessInstance> GetMyStartedProcessInstances(this WorkflowClientService clientService, string userName, ProcessInstanceCriteria criteria)
        {
            var context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);
            context["aZaaS_KStar_ConnectionString"] = new KStarDbContext().Database.Connection.ConnectionString;//TODO:Better ideas?

            return clientService.GetMyStartedProcessInstances(context, criteria);
        }

        /// <summary>
        /// 获取指定用户的【我的参与】流程列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">流程列表过滤条件</param>
        /// <returns></returns>
        public static List<ProcessInstance> GetMyParticipatedProcessInstances(this  WorkflowClientService clientService, string userName, ProcessInstanceCriteria criteria)
        {
            var context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);
            context["aZaaS_KStar_ConnectionString"] = new KStarDbContext().Database.Connection.ConnectionString;//TODO:Better ideas?

            return clientService.GetMyParticipatedProcessInstances(context, criteria);
        }

        /// <summary>
        /// 获取指定用户的【我的参与】流程列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">流程列表过滤条件</param>
        /// <param name="filterActions">流程日志Action过滤（多个以逗号分隔）</param>
        /// <returns></returns>
        public static List<ProcessInstance> GetMyParticipatedProcessInstances(this  WorkflowClientService clientService, string userName, ProcessInstanceCriteria criteria, string filterActions)
        {
            var logCondition = " ";
            var context = new ServiceContext();
            var logService = new ProcessLogService();
            var items = new List<ProcessInstance>();

            context.UserName = K2User.ApplySecurityLabel(userName);
            context["aZaaS_KStar_ConnectionString"] = new KStarDbContext().Database.Connection.ConnectionString;//TODO:Better ideas?

            var myParticipatedInstances = new List<ProcessLog>();
            if (!string.IsNullOrEmpty(filterActions))
            {
                var logs = logService.GetProcessLogByUserAccount(userName);
                filterActions.Split(',').ToList().ForEach(actName =>
                {
                    myParticipatedInstances.AddRange(logs.Where(r => r.ActionName == actName).ToList());
                });
            }

            if (myParticipatedInstances.Any())
            {
                StringBuilder sbInst = new StringBuilder();
                myParticipatedInstances.ForEach(log =>
                {
                    sbInst.Append(log.ProcInstID);
                    sbInst.Append(",");
                });
                logCondition = string.Format(" or pit.id in ( {0} )", sbInst.ToString().Trim(','));
            }

            #region Raw SQL
            /*
            var sql = @"select * from 
                            (select row_number()over(order by {4})rownumber,
                                 pit.ID as Procinstid ,
                                ( SELECT  TOP 1 Act.Name as ActName 
                                  FROM [K2].[ServerLog].[ActInst] AS ActInst
                                  INNER JOIN [K2].[ServerLog].[Act] As Act
                                  ON ActInst.ActID = Act.ID
                                  WHERE   [ProcInstID] =pit.ID
                                  ORDER BY [StartDate] DESC ) AS [ActivityName], --ActivityName
                                pit.[Priority], pit.[Status], pit.StartDate ,pit.FinishDate, pit.Originator , pit.Folio, ps.FullName,cps.ProcessName, cps.ViewUrl

                                from [K2].[ServerLog].[procinst] pit
                                inner join [K2].[ServerLog].[Proc] pc
                                     on pit.ProcID = pc.id
                                inner join [K2].[ServerLog].[ProcSet] ps
                                     on pc.ProcSetID = ps.ID
                                inner join dbo.Configuration_ProcessSet cps
                                     on ps.ID = cps.ProcessSetID
                                     where (pit.id in ( 
	                                    select procinstid from [K2].[ServerLog] .[ActInstSlot]
	                                    where [user] = '{0}' ) {1} ) 
                                    {2}
                              )a
		                    {3}    
                        ;select COUNT(1) Total 
                                from [K2].[ServerLog].[procinst] pit
                                inner join [K2].[ServerLog].[Proc] pc
                                     on pit.ProcID = pc.id
                                inner join [K2].[ServerLog].[ProcSet] ps
                                     on pc.ProcSetID = ps.ID
                                inner join dbo.Configuration_ProcessSet cps
                                     on ps.ID = cps.ProcessSetID
                                     where (pit.id in ( 
	                                    select procinstid from [K2].[ServerLog] .[ActInstSlot]
	                                    where [user] = '{0}' ) {1} ) 
                                    {2}   
                   ";

            */
            #endregion

            var sql = SQLQueryBroker.GetQuery("Framework_WorkflowService_GetMyParticipatedProcessInstances_Actions");

            try
            {
                string sql_condition = " ", sql_pager = " ", sql_order = "";
                foreach (RegularFilter filter in criteria.RegularFilters)
                {
                    switch (filter.Compare)
                    {
                        case CriteriaCompare.Greater:
                            sql_condition += " and " + filter.FieldName + " >'" + filter.Value1 + "'";
                            break;
                        case CriteriaCompare.GreaterOrEqual:
                            sql_condition += " and " + filter.FieldName + " >='" + filter.Value1 + "'";
                            break;
                        case CriteriaCompare.Less:
                            sql_condition += " and " + filter.FieldName + " <'" + filter.Value1 + "'";
                            break;
                        case CriteriaCompare.LessOrEqual:
                            sql_condition += " and " + filter.FieldName + " <='" + filter.Value1 + "'";
                            break;
                        case CriteriaCompare.Like:
                            sql_condition += " and " + filter.FieldName + " like '%" + filter.Value1 + "%'";
                            break;
                        case CriteriaCompare.Equal:
                            sql_condition += " and " + filter.FieldName + " ='" + filter.Value1 + "'";
                            break;
                    }
                }
                for (int i = 0; i < criteria.SortFilters.Count; i++)
                {
                    SortFilter filter = criteria.SortFilters[i];
                    sql_order += "," + filter.FieldName + " " + filter.SortDirection;
                }

                if (string.IsNullOrEmpty(sql_order))
                {
                    sql_order = ",pit.ID ";
                }

                if (criteria.PageSize > 0)
                {
                    sql_pager = "where rownumber>" + (criteria.PageIndex * criteria.PageSize).ToString() + " and rownumber<=" + ((criteria.PageIndex + 1) * criteria.PageSize).ToString();
                }
                sql = string.Format(sql, context.UserName, logCondition, sql_condition, sql_pager, sql_order.Substring(1));

                using (SqlDataReader reader = aZaaS.Framework.Workflow.Util.SqlHelper.ExecuteReader(context["aZaaS_KStar_ConnectionString"], CommandType.Text, sql))
                {
                    while (reader.Read())
                    {
                        items.Add(ConvertToProcessInstance(reader));
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        criteria.TotalCount = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Total"]);
                    }
                }
                return items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取指定用户的【我的代理申请】流程列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">流程列表过滤条件</param>
        /// <returns></returns>
        public static List<ProcessInstance> GetInsteadMyStartedProcessInstances(this  WorkflowClientService clientService, string userName, ProcessInstanceCriteria criteria)
        {
            var context = new ServiceContext();
            var items = new List<ProcessInstance>();

            context.UserName = userName; //Query by KStar user not K2 user!!!
            context["CurrentSecurityLabel"] = K2User.ApplySecurityLabel(userName).Split(':')[0];

            #region Raw SQL

            /*
            var sql = @"select * from 
                            (select row_number()over(order by {4})rownumber,
                                 pit.ID as Procinstid ,pfh.SubmitterAccount,pfh.SubmitterDisplayName,
                                ( SELECT  TOP 1 Act.Name as ActName 
                                  FROM [K2].[ServerLog].[ActInst] AS ActInst
                                  INNER JOIN [K2].[ServerLog].[Act] As Act
                                  ON ActInst.ActID = Act.ID
                                  WHERE   [ProcInstID] =pit.ID
                                  ORDER BY [StartDate] DESC ) AS [ActivityName], --ActivityName
                                pit.[Priority], pit.[Status], pit.StartDate ,pit.FinishDate, pit.Originator , pit.Folio, ps.FullName,cps.ProcessName, cps.ViewUrl
                                from [aZaaS.Framework].[dbo].[ProcessFormHeader] pfh
								inner join [K2].[ServerLog].[procinst] pit
								     on '{0}:'+pfh.SubmitterAccount COLLATE Chinese_PRC_CI_AS = pit.Originator
									 and pfh.ProcInstId=pit.ID
                                     and pfh.IsDraft=0
                                inner join [K2].[ServerLog].[Proc] pc
                                     on pit.ProcID = pc.id
                                inner join [K2].[ServerLog].[ProcSet] ps
                                     on pc.ProcSetID = ps.ID
                                inner join dbo.Configuration_ProcessSet cps
                                     on ps.ID = cps.ProcessSetID
                                    where pfh.ApplicantAccount = '{1}' 
									and pfh.SubmitterAccount!='{1}'
                                    {2}    
                                )a
		                    {3}    
                        ;select COUNT(1) Total 
                                from [aZaaS.Framework].[dbo].[ProcessFormHeader] pfh
								inner join [K2].[ServerLog].[procinst] pit
								on '{0}:'+pfh.SubmitterAccount COLLATE Chinese_PRC_CI_AS =pit.Originator
									 and pfh.ProcInstId=pit.ID
                                     and pfh.IsDraft=0
                                inner join [K2].[ServerLog].[Proc] pc
                                     on pit.ProcID = pc.id
                                inner join [K2].[ServerLog].[ProcSet] ps
                                     on pc.ProcSetID = ps.ID
                                inner join dbo.Configuration_ProcessSet cps
                                     on ps.ID = cps.ProcessSetID
                                    where pfh.ApplicantAccount = '{1}' 
									and pfh.SubmitterAccount!='{1}'
                                    {2}          
                   ";
            */

            #endregion

            var sql = K2SQLScripts.GetInsteadMyStartedProcessInstancesSQL;

            string sql_condition = " ", sql_pager = " ", sql_order = string.Empty;
            foreach (RegularFilter filter in criteria.RegularFilters)
            {
                switch (filter.Compare)
                {
                    case CriteriaCompare.Greater:
                        sql_condition += " and " + filter.FieldName + " >'" + filter.Value1 + "'";
                        break;
                    case CriteriaCompare.GreaterOrEqual:
                        sql_condition += " and " + filter.FieldName + " >='" + filter.Value1 + "'";
                        break;
                    case CriteriaCompare.Less:
                        sql_condition += " and " + filter.FieldName + " <'" + filter.Value1 + "'";
                        break;
                    case CriteriaCompare.LessOrEqual:
                        sql_condition += " and " + filter.FieldName + " <='" + filter.Value1 + "'";
                        break;
                    case CriteriaCompare.Like:
                        sql_condition += " and " + filter.FieldName + " like '%" + filter.Value1 + "%'";
                        break;
                    case CriteriaCompare.Equal:
                        sql_condition += " and " + filter.FieldName + " ='" + filter.Value1 + "'";
                        break;
                }
            }
            for (int i = 0; i < criteria.SortFilters.Count; i++)
            {
                SortFilter filter = criteria.SortFilters[i];
                sql_order += "," + filter.FieldName + " " + filter.SortDirection;
            }
            if (string.IsNullOrEmpty(sql_order))
            {
                sql_order = ",pit.ID ";
            }

            if (criteria.PageSize > 0)
            {
                sql_pager = "where rownumber>" + (criteria.PageIndex * criteria.PageSize).ToString() + " and rownumber<=" + ((criteria.PageIndex + 1) * criteria.PageSize).ToString();
            }

            sql = string.Format(sql, context["CurrentSecurityLabel"], context.UserName, sql_condition, sql_pager, sql_order.Substring(1));

            using (SqlDataReader reader = aZaaS.Framework.Workflow.Util.SqlHelper.ExecuteReader(context[SettingVariable.ConnectionString], CommandType.Text, sql))
            {
                while (reader.Read())
                {
                    items.Add(ConvertToProcessInstance(reader));
                }
                reader.NextResult();
                while (reader.Read())
                {
                    criteria.TotalCount = (reader["Total"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Total"]);
                }
            }


            return items;
        }
        
        public static IEnumerable<PendingApproverData> GetWorkflowTaskPendingApproverData(this  WorkflowClientService clientService,int procInstID)
        {
            var userService = new UserService();
            var pendingApprovers = new List<PendingApproverData>();

            using (KStarDbContext db = new KStarDbContext())
            {
                var sql = string.Format(SQLQueryBroker.GetQuery("Framework_WorkflowService_GetWorklistPendingApprover"), procInstID);
                
                var resultTable = aZaaS.Framework.Workflow.Util.SqlHelper.ExecuteDataset(db.Database.Connection.ConnectionString, CommandType.Text, sql).Tables[0];
                if (resultTable == null || resultTable.Rows.Count == 0)
                    return pendingApprovers;

                foreach (DataRow row in resultTable.Rows)
                {
                    var userName = row["Destination"].ToString();
                    var activityName = row["ActivityName"].ToString();

                    var nameParts = userName.Split(':');
                    userName = nameParts.Length > 1 ? nameParts[1] : userName;
                    var user = userService.ReadUserBase(userName);
                    var displayName = user == null ? "Unknown" : user.FullName;


                    pendingApprovers.Add(NewPendingLogView(displayName, activityName));
                }

                return pendingApprovers;
            }
        }

        #endregion

        #region Workflow Service Business Extensions

        /// <summary>
        /// 获取指定用户的【我的待办】任务数量
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName"></param>
        /// <param name="apiStyle"></param>
        /// <returns></returns>
        public static int GetUserPendingTaskCount(this WorkflowClientService clientService, string userName, WLApiStyle apiStyle = WLApiStyle.Client)
        {
            var totalCount = 0;

            var worklistReader = new K2WorklistReader(ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString);

            switch (apiStyle)
            {
                case WLApiStyle.Prototype:
                    totalCount = worklistReader.TotalCount(K2User.ApplySecurityLabel(userName));
                    break;
                case WLApiStyle.Client:
                    totalCount = clientService.GetWorklistItems(userName, PlatformType.ASP).Count;
                    break;
            }

            return totalCount;
        }

        /// <summary>
        /// 获取指定用户【我的待办】任务列表
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName"></param>
        /// <param name="totalCount"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public static IEnumerable<WorklistTask> GetUserPendingTaskList(this WorkflowClientService clientService, string userName, out int totalCount, Dictionary<WLCField, WLCSortOrder> sorting = null)
        {
            return clientService.GetUserPendingTaskList(userName, null, null, out totalCount, null, null, null, null, sorting);
        }

        /// <summary>
        /// 获取指定用户的【我的待办】任务列表
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="folio"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="processNames">流程系统名称（e.g: KStarWorkflow\TravelRequest)</param>
        /// <param name="sorting"></param>
        /// <param name="apiStyle"></param>
        /// <returns></returns>
        public static IEnumerable<WorklistTask> GetUserPendingTaskList(this WorkflowClientService clientService, string userName, int? page, int? pageSize, out int totalCount, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<WLCField, WLCSortOrder> sorting = null, WLApiStyle apiStyle = WLApiStyle.Client)
        {
            totalCount = 0;
            IEnumerable<WorklistTask> tasks = new List<WorklistTask>();

            switch (apiStyle)
            {
                case WLApiStyle.Client:
                    tasks = GetWorklistItemsByClientAPI(clientService, userName, page, pageSize, folio, startDate, endDate, processNames, sorting, out totalCount);
                    break;
                case WLApiStyle.Prototype:
                    tasks = GetWorklistItemsByPrototypeAPI(clientService, userName, page, pageSize, folio, startDate, endDate, processNames, sorting, out totalCount);
                    break;
            }

            return tasks;
        }

        private static IEnumerable<WorklistTask> GetWorklistItemsByClientAPI(WorkflowClientService clientService, string userName, int? page, int? pageSize, string folio, DateTime? startDate, DateTime? endDate, string[] processNames, Dictionary<WLCField, WLCSortOrder> sorting, out int totalCount)
        {
            var filter = new WorklistItemCriteria();

            filter.UserName = userName;
            filter.PageIndex = (page == null || page.Value <= 0) ? 0 : page.Value - 1;
            filter.PageSize = (pageSize == null || pageSize.Value <= 0) ? -1 : pageSize.Value;

            if (startDate != null)
                filter.AddWLCField(CriteriaLogical.And, WLCField.EventStartDate, CriteriaCompare.GreaterOrEqual, startDate);

            if (endDate != null)
                filter.AddWLCField(CriteriaLogical.And, WLCField.EventStartDate, CriteriaCompare.LessOrEqual, endDate);

            if (!string.IsNullOrEmpty(folio))
                filter.AddWLCField(CriteriaLogical.And, WLCField.ProcessFolio, CriteriaCompare.Like, string.Format("%{0}%", folio));

            if (processNames != null && processNames.Any())
            {
                var index = 0;
                foreach (var processName in processNames)
                {
                    index++;
                    if (index == 1)
                        filter.AddWLCField(CriteriaLogical.And, WLCField.ProcessFullName, CriteriaCompare.Equal, processName);
                    else
                        filter.AddWLCField(CriteriaLogical.Or, WLCField.ProcessFullName, CriteriaCompare.Equal, processName);
                }
            }

            if (sorting == null || !sorting.Any())
                filter.AddWLCSortFiled(WLCField.EventStartDate, WLCSortOrder.Descending);
            else
            {
                foreach (var field in sorting.Keys)
                {
                    filter.AddWLCSortFiled(field, sorting[field]);
                }
            }

            var items = clientService.GetWorklistItemsByPage(PlatformType.ASP, filter);
            totalCount = filter.TotalCount;

            return items.AsWorklistTaskList(clientService);
        }

        private static IEnumerable<WorklistTask> GetWorklistItemsByPrototypeAPI(WorkflowClientService clientService, string userName, int? page, int? pageSize, string folio, DateTime? startDate, DateTime? endDate, string[] processNames, Dictionary<WLCField, WLCSortOrder> sorting, out int totalCount)
        {
            var k2User = K2User.ApplySecurityLabel(userName);
            var worklistReader = new K2WorklistReader(ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString);

            var sort = new Dictionary<string, string>();

            if (sorting != null && sorting.Any())
            {
                foreach (var field in sorting.Keys)
                {
                    sort.Add(Enum.GetName(typeof(WLCField), field), Enum.GetName(typeof(WLCSortOrder), sorting[field]));
                }
            }

            return worklistReader.GetWorklistItems(k2User, page, pageSize, out totalCount, folio, startDate, endDate, processNames, sort).AsWorklistTaskList(clientService);
        }

        /// <summary>
        /// 获取指定用户的【我的待办】任务列表，并根据流程进行分组
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName"></param>
        /// <param name="totals"></param>
        /// <param name="topSize"></param>
        /// <param name="folio"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="processNames"></param>
        /// <param name="apiStyle"></param>
        /// <returns></returns>
        public static Dictionary<string, IEnumerable<WorklistTask>> GetUserPendingTaskListGroupingByProcess(this WorkflowClientService clientService, string userName, out Dictionary<string, int> totals, int? topSize, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, WLApiStyle apiStyle = WLApiStyle.Prototype)
        {
            totals = new Dictionary<string, int>();

            var k2User = K2User.ApplySecurityLabel(userName);
            var groupingTasks = new Dictionary<string, IEnumerable<WorklistTask>>();
            var worklistReader = new K2WorklistReader(ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString);

            //
            if (apiStyle == WLApiStyle.Client)
                throw new NotImplementedException();

            switch (apiStyle)
            {
                case WLApiStyle.Client:
                    //TODO:
                    break;
                case WLApiStyle.Prototype:
                    var innerGroupingItems = worklistReader.GetWorklistItemsGroupingByProcess(k2User, out totals, topSize, folio, startDate, endDate, processNames);
                    innerGroupingItems.Keys.ToList().ForEach(processName => groupingTasks.Add(processName, innerGroupingItems[processName].AsWorklistTaskList(clientService)));
                    break;
            }

            return groupingTasks;
        }

        /// <summary>
        /// 获取指定用户的【我的申请】任务列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">任务过滤条件</param>
        /// <returns></returns>
        public static IEnumerable<WorkflowTask> GetUserRequestTaskList(this  WorkflowClientService clientService, string userName, int? page, int? pageSize, out int totalCount, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<PICField, PICSortOrder> sorting = null)
        {
            var filter = OrganizeTaskFilter(page, pageSize, folio, startDate, endDate, processNames, sorting);

            var processInstances = clientService.GetMyStartedProcessInstances(userName, filter);

            totalCount = filter.TotalCount;
            return processInstances.AsWorkflowTaskList();
        }

        /// <summary>
        /// 获取指定用户的【我的参与】任务列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">任务过滤条件</param>
        /// <returns></returns>
        public static IEnumerable<WorkflowTask> GetUserOnGoingTaskList(this  WorkflowClientService clientService, string userName, int? page, int? pageSize, out int totalCount, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<PICField, PICSortOrder> sorting = null)
        {
            var filter = OrganizeTaskFilter(page, pageSize, folio, startDate, endDate, processNames, sorting);

            var tasks = clientService.GetMyParticipatedProcessInstances(userName, filter);

            totalCount = filter.TotalCount;
            return tasks.AsWorkflowTaskList();

        }

        /// <summary>
        /// 获取指定用户的【我的申请】任务列表
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">任务过滤条件</param>
        /// <param name="filterActions"> 流程日志Action过滤(e.g:"回退到,同意,不同意,不通过,代理给,转交给"</param>
        /// <returns></returns>
        public static IEnumerable<WorkflowTask> GetUserOnGoingTaskList(this  WorkflowClientService clientService, string userName, int? page, int? pageSize, out int totalCount, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<PICField, PICSortOrder> sorting = null, string filterActions = "")
        {
            var filter = OrganizeTaskFilter(page, pageSize, folio, startDate, endDate, processNames, sorting);

            var processInstances = clientService.GetMyParticipatedProcessInstances(userName, filter, filterActions);

            totalCount = filter.TotalCount;
            return processInstances.AsWorkflowTaskList();
        }

        /// <summary>
        /// 获取指定用户的【我的代理申请】任务列表。
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="userName">用户名</param>
        /// <param name="criteria">任务过滤条件</param>
        /// <returns></returns>
        public static IEnumerable<WorkflowTask> GetUserInsteadRequestTasksList(this  WorkflowClientService clientService, string userName, int? page, int? pageSize, out int totalCount, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<PICField, PICSortOrder> sorting = null)
        {
            var filter = OrganizeTaskFilter(page, pageSize, folio, startDate, endDate, processNames, sorting);

            var processInstances = clientService.GetInsteadMyStartedProcessInstances(userName, filter);

            totalCount = filter.TotalCount;
            return processInstances.AsWorkflowTaskList();
        }


        public static IEnumerable<WorkflowTask> GetTransitTaskList(string userName, int? page, int? pageSize, out int totalCount, string folio = null, DateTime? startDate = null, DateTime? endDate = null, string[] processNames = null, Dictionary<PICField, PICSortOrder> sorting = null)
        {
            List<WorkflowTask> workflowTasks = new List<WorkflowTask>();
            using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
            {
                var linq = from pd in db.ProcessPrognosisDetail
                           join p in db.ProcessPrognosis on pd.RSysID equals p.SysID
                           join v in db.view_ProcinstList on p.ProcInstID equals v.ProcInstID
                           where pd.UserName == userName 
                           orderby v.ProcInstID
                           select v;
                if (folio != null)
                {
                    linq = linq.Where(x => x.Folio.Contains(folio));
                }

                linq = linq.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

                totalCount = linq.Count();

                var procinstList = linq.ToList();

                foreach (var item in procinstList)
                {
                    workflowTasks.Add(new WorkflowTask()
                    {
                        ProcInstNo = item.Folio.Split('-').Last(),
                        ProcSubject = item.Folio.Split('-').First(),
                        ActivityName = item.ActName,
                        Procinstid = item.ProcInstID,
                        Priority = 0,
                        Status = (byte)item.Status,
                        StartDate = item.StartDate,
                        FinishDate = item.FinishDate,
                        Originator = CacheAccessor.GetUserFullName(item.Originator),
                        Folio = item.Folio,
                        FullName = item.FullName,
                        ProcessName = item.FullName,
                        ViewUrl = WorkflowTaskViewUrl.Create(item.FullName, item.ProcInstID.ToString()),
                        ViewFlowUrl = ViewFlowUtil.GetViewFlowUlr(item.ProcInstID),
                        StatusDesc = WorkflowStatus.Map((byte)item.Status, item.ProcInstID),
                        PrevApprovers = CacheAccessor.GetApproversAndDelegationName(item.FullName, GetCurrentProcInstDestinationBy(item.ProcInstID, db))

                    });
                }
            } 
            return workflowTasks;
        }


        public static bool IsInTransit(string userName, int procInstID)
        {
            using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
            {
                var linq = from pd in db.ProcessPrognosisDetail
                           join p in db.ProcessPrognosis on pd.RSysID equals p.SysID
                           where pd.UserName == userName && p.ProcInstID == procInstID

                           select p.ProcInstID;

                int count = linq.Count();
                if(count>0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <returns></returns>
        public static string GetCurrentProcInstDestinationBy(int ProcInstID, DbContext db)
        {
            string sql = aZaaS.Framework.SQLQuery.SQLQueryBroker.GetQuery("Framework_WorkflowService_GetCurrentProcInstDestinationBy");
            sql = string.Format(sql, ProcInstID);
            var destinationList = db.Database.SqlQuery<string>(sql).ToList();

            string nameString = string.Empty;

            if (destinationList != null)
            {
                foreach (var destination in destinationList)
                {
                    if (string.IsNullOrWhiteSpace(nameString))
                    {
                        nameString = destination; 
                    }
                    else
                    {
                        nameString +=","+ destination; 
                    }
                } 
            }

            return nameString;
        }

        private static ProcessInstanceCriteria OrganizeTaskFilter(int? page, int? pageSize, string folio, DateTime? startDate, DateTime? endDate, string[] processNames, Dictionary<PICField, PICSortOrder> sorting)
        {
            var filter = new ProcessInstanceCriteria();

            filter.PageIndex = (page == null || page.Value <= 0 ? 1 : page).Value - 1;
            filter.PageSize = (pageSize == null || pageSize.Value <= 0 ? 0 : pageSize).Value;

            if (startDate != null)
                filter.AddPICField(CriteriaLogical.And, PICField.StartDate, CriteriaCompare.GreaterOrEqual, startDate);

            if (endDate != null)
                filter.AddPICField(CriteriaLogical.And, PICField.StartDate, CriteriaCompare.LessOrEqual, endDate);

            if (!string.IsNullOrEmpty(folio))
                filter.AddPICField(CriteriaLogical.And, PICField.Folio, CriteriaCompare.Like, folio);

            if (processNames != null && processNames.Any())
            {
                //NOTE:
                //We only support single process name filtering currently.
                //Because the criteria not supports 'or' operation behind the infrastructure.
                filter.AddPICField(CriteriaLogical.And, PICField.ProcessName, CriteriaCompare.Like, processNames.First());
            }

            if (sorting == null || sorting.Count==0)
                filter.AddPICSortField(PICField.StartDate, PICSortOrder.DESC);
            else
            {
                foreach (var sortField in sorting.Keys)
                {
                    filter.AddPICSortField(sortField, sorting[sortField]);
                }
            }
            return filter;
        }

        #endregion

        #region Helpers

        internal static ProcessInstance ConvertToProcessInstance(SqlDataReader reader)
        {
            ProcessInstance obj = new ProcessInstance();
            obj.ID = (reader["Procinstid"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Procinstid"]);
            obj.Priority = (reader["Priority"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["Priority"]);
            obj.Status = (reader["Status"] == DBNull.Value) ? (ProcInstStatus)0 : (ProcInstStatus)Convert.ToInt32(reader["Status"]);
            obj.StartDate = (reader["StartDate"] == DBNull.Value) ? DateTime.MaxValue : Convert.ToDateTime(reader["StartDate"]);
            obj.FinishDate = (reader["FinishDate"] == DBNull.Value) ? DateTime.MaxValue : Convert.ToDateTime(reader["FinishDate"]);
            obj.Originator = (reader["Originator"] == DBNull.Value) ? "" : reader["Originator"].ToString();
            obj.ActivityName = (reader["ActivityName"] == DBNull.Value) ? "" : reader["ActivityName"].ToString();
            obj.Folio = (reader["Folio"] == DBNull.Value) ? "" : reader["Folio"].ToString();
            obj.FullName = (reader["FullName"] == DBNull.Value) ? "" : reader["FullName"].ToString();
            obj.ProcessName = (reader["ProcessName"] == DBNull.Value) ? "" : reader["ProcessName"].ToString();
            obj.ViewUrl = (reader["ViewUrl"] == DBNull.Value) ? "" : reader["ViewUrl"].ToString();
            obj.BOID = (reader.GetValue(2) == DBNull.Value) ? "" : reader.GetValue(2).ToString();//在我的申请中为提单人账号
            obj.BOOwner = (reader.GetValue(3) == DBNull.Value) ? "" : reader.GetValue(3).ToString();//在我的申请中为提单人名称
            return obj;
        }

        internal static IEnumerable<WorkflowTask> AsWorkflowTaskList(this List<ProcessInstance> procInstances)
        {
            if (procInstances == null || !procInstances.Any())
                return new List<WorkflowTask>();

            return procInstances.Select(item => new WorkflowTask()
                              {
                                  ProcInstNo = item.Folio.Split('-').Last(),//TODO:
                                  ProcSubject = item.Folio.Split('-').First(),//TODO:

                                  ActivityName = item.ActivityName,
                                  Procinstid = item.ID,
                                  Priority = item.Priority,
                                  Status = (byte)item.Status,
                                  StartDate = item.StartDate,
                                  FinishDate = item.FinishDate,
                                  Originator = CacheAccessor.GetUserFullName(item.Originator),
                                  Folio = item.Folio,
                                  FullName = item.FullName,
                                  ProcessName = item.ProcessName,
                                  ViewUrl = WorkflowTaskViewUrl.Create(item.FullName, item.ID.ToString()),
                                  ViewFlowUrl = ViewFlowUtil.GetViewFlowUlr(item.ID),
                                  StatusDesc = WorkflowStatus.Map((byte)item.Status, item.ID),
                                  PrevApprovers =CacheAccessor.GetApproversAndDelegationName(item.FullName,item.PrevApprovers)
                              }).ToList();
        }

        internal static IEnumerable<WorklistTask> AsWorklistTaskList(this List<WorklistItem> worklistItems, WorkflowClientService clientService)
        {
            if (worklistItems == null || !worklistItems.Any())
                return new List<WorklistTask>();

            return worklistItems.Select(item => new WorklistTask()
                              {
                                  ProcInstID = item.ProcInstID.ToString(),
                                  Folio = item.Folio,
                                  ProcInstNo = item.Folio.Split('-').Last(),//TODO:
                                  ProcSubject = item.Folio.Split('-').First(),//TODO:

                                  Originator = CacheAccessor.GetUserFullName(item.Originator),
                                  ProcStartDate = item.ProcStartDate,
                                  ActivityName = item.ActivityName,
                                  StartDate = item.StartDate,
                                  HyperLink = item.Data,
                                  Process = item.ProcDispName,
                                  FullName = item.FullName,
                                  ProcessName = CacheAccessor.GetProcessName(item.FullName),
                                  LastActivityDate = item.AssignedDate,
                                  WorkflowStep = item.ActivityName,
                                  ViewFlowUrl = ViewFlowUtil.GetViewFlowUlr(item.ProcInstID),
                                  BusinessData = clientService.GetBusinessData(item).ToList()
                              }).ToList();
        }
       
        internal static PendingApproverData NewPendingLogView(string taskOwner, string state)
        {
            return new PendingApproverData()
            {
                Name = string.Empty,
                Stage = state,
                Date = string.Empty,
                Action = "Pending",
                TaskOwner = taskOwner,
                Comment = string.Empty
            };
        }

        #endregion
    }

    #region Extensions

    public static class WorklistItemCriteriaExtension
    {
        public static void AddWLCField(this WorklistItemCriteria filter, CriteriaLogical logical, WLCField filterField, CriteriaCompare compare, object filterValue)
        {
            filter.AddRegularFilter(new RegularFilter(logical, Enum.GetName(typeof(WLCField), filterField), compare, filterValue));
        }

        public static void AddWLCSortFiled(this WorklistItemCriteria filter, WLCField sortField, WLCSortOrder sortOrder)
        {
            filter.AddSortFilter(new SortFilter() { FieldName = Enum.GetName(typeof(WLCField), sortField), SortDirection = Enum.GetName(typeof(WLCSortOrder), sortOrder) });
        }

        public static void AddPICField(this ProcessInstanceCriteria filter, CriteriaLogical logicl, PICField filterField, CriteriaCompare compare, object filterValue)
        {
            filter.AddRegularFilter(new RegularFilter() { StartLogical = logicl, FieldName = Enum.GetName(typeof(PICField), filterField), Compare = compare, Value1 = filterValue });
        }

        public static void AddPICSortField(this ProcessInstanceCriteria filter, PICField sortField, PICSortOrder sortOrder)
        {
            filter.AddSortFilter(new SortFilter() { FieldName = Enum.GetName(typeof(PICField), sortField), SortDirection = Enum.GetName(typeof(PICSortOrder), sortOrder) });
        }
    }

    public static class WorkflowSortDescriptorExtension
    {
        public static Dictionary<WLCField, WLCSortOrder> ToWLCFieldSortRules(this List<SortDescriptor> sortRules)
        {
            var sorting = new Dictionary<WLCField, WLCSortOrder>();

            if (sortRules == null || !sortRules.Any())
                return sorting;

            foreach (var rule in sortRules)
            {
                var sortFieldName = string.Empty;

                switch (rule.field)
                {
                    case "Folio":
                        sortFieldName = "ProcessFolio";
                        break;
                    case "StartDate":
                        sortFieldName = "ActivityStartDate";
                        break;
                    case "ActivityName":
                        sortFieldName = "ActivityName";
                        break;
                    case "LastActivityDate":
                        sortFieldName = "EventStartDate";
                        break;
                }

                if (rule.field != "Originator")
                    sorting.Add((WLCField)Enum.Parse(typeof(WLCField), sortFieldName), rule.dir.Equals("asc") ? WLCSortOrder.Ascending : WLCSortOrder.Descending);

            }

            return sorting;
        }

        public static Dictionary<PICField, PICSortOrder> ToPICFieldSortRules(this List<SortDescriptor> sortRules)
        {
            var sorting = new Dictionary<PICField, PICSortOrder>();

            if (sortRules == null || !sortRules.Any())
                return sorting;

            foreach (var rule in sortRules)
            {
                string sortFieldName = string.Empty;

                switch (rule.field)
                {
                    case "Folio":
                    case "StartDate":
                    case "FinishDate":
                    case "ProcessName":
                        sortFieldName = rule.field;
                        break;
                    case "StatusDesc":
                        sortFieldName = "Status";
                        break;
                }
                if (rule.field != "Priority" && rule.field != "ActivityName")
                    sorting.Add((PICField)Enum.Parse(typeof(PICField), sortFieldName), rule.dir.Equals("asc") ? PICSortOrder.ASC : PICSortOrder.DESC);

            }

            return sorting;
        }
    }

    public static class WorklistTaskExtension
    {
        public static void StoreTaskItemUrlArgs(this IEnumerable<WorklistTask> taskItems)
        {
            Hashtable argsMap = null;
            var cacheName = "ProcInstArgs";
            var taskUrlArgsMgr = new ViewFlowArgs();

            if (taskItems != null)
            {
                if (HttpRuntime.Cache[cacheName] == null)
                {
                    argsMap = new Hashtable();
                    taskItems.ToList().ForEach(item =>
                    {
                        var args = item.HyperLink.Split('?').Last();
                        argsMap[item.ProcInstID] = args;
                        taskUrlArgsMgr.SaveFlowArgs(args, item.ProcInstID.ToString());
                    });
                    HttpRuntime.Cache.Insert(cacheName, argsMap);
                }
                else
                {
                    argsMap = (Hashtable)HttpRuntime.Cache[cacheName];

                    foreach (var item in taskItems)
                    {
                        var args = item.HyperLink.Split('?').Last();
                        if (argsMap.Contains(item.ProcInstID) && argsMap[item.ProcInstID].ToString() == args)
                        {
                            continue;
                        }

                        argsMap[item.ProcInstID] = args;
                        taskUrlArgsMgr.SaveFlowArgs(args, item.ProcInstID.ToString());
                    }

                    HttpRuntime.Cache.Remove(cacheName);
                    HttpRuntime.Cache.Insert(cacheName, argsMap);
                }
            }
        }

        public static void StoreTaskItemUrlArgsAsync(this IEnumerable<WorklistTask> taskItems)
        {
            Task.Run(() => taskItems.StoreTaskItemUrlArgs());
        }
    }

    #endregion
}

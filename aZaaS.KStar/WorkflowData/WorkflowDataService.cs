using aZaaS.KStar.MgmtServices;
using aZaaS.Framework.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.DTOs;
using aZaaS.Framework.Workflow.Pager;

namespace aZaaS.KStar.WorkflowData
{
    [Obsolete("该模块已被迁移或者废弃了部分方法，已不再使用！")]
    public class WorkflowDataService
    {
        WorkflowDataManager tenantDatabase;
        private readonly UserService userService;

        public WorkflowDataService(AuthenticationType authType)
        {
            tenantDatabase = new WorkflowDataManager(authType);
            this.userService = new UserService();
        }

        #region For Dashboard

        //public IEnumerable<WorklistItem> GetPendingTasks(string currentUser, AuthenticationType authtype)
        //{
        //    return tenantDatabase.GetPendingTasks(currentUser,authtype);
        //}
        //public IEnumerable<WorklistItem> FindPendingTasks(WorklistItemCriteria criteria, AuthenticationType authtype)
        //{
        //    return tenantDatabase.FindPendingTasks(criteria, authtype);
        //}

        //public IEnumerable<WorklistItem> GetAllPendingTasks(string currentUser, AuthenticationType authtype)
        //{
        //    return tenantDatabase.GetAllPendingTasks(currentUser, authtype);
        //}

        //public IEnumerable<WorkflowTask> GetRequestTasks(string currentUser, string tenantId, string connectionstring)
        //{
        //    return tenantDatabase.GetRequestTasksCommon(currentUser, tenantId, connectionstring);
        //}

        //public IEnumerable<WorkflowTask> GetRequestTasksCommon(string currentUser, string tenantId, string connectionstring)
        //{
        //    return tenantDatabase.GetRequestTasksCommon(currentUser, tenantId, connectionstring);
        //}

        //public IEnumerable<WorkflowTask> FindRequestTasks(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        //{
        //    return tenantDatabase.FindRequestTasksCommon(currentUser, tenantId, connectionstring, criteria);
        //}

        //public IEnumerable<WorkflowTask> FindInsteadRequestTasks(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        //{
        //    return tenantDatabase.FindInsteadRequestTasksCommon(currentUser, tenantId, connectionstring, criteria);
        //}

        //public IEnumerable<WorkflowTask> GetOnGoingTasks(string currentUser, string tenantId, string connectionstring)
        //{
        //    return tenantDatabase.GetOnGoingTasksCommon(currentUser, tenantId, connectionstring);
        //}

        //public IEnumerable<WorkflowTask> GetOnGoingTasksCommon(string currentUser, string tenantId, string connectionstring)
        //{
        //    return tenantDatabase.GetOnGoingTasksCommon(currentUser, tenantId, connectionstring);
        //}

        //public IEnumerable<WorkflowTask> FindOnGoingTasks(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        //{
        //    //return tenantDatabase.FindOnGoingTasksCommon(currentUser, tenantId, connectionstring, criteria);
        //    return tenantDatabase.FindOnGoingTasksWithActionHis(currentUser, tenantId, connectionstring, criteria);
        //}

        //public IEnumerable<RequestMainInfoEntity> GetCompletedTasks(string currentUser)
        //{
        //    return tenantDatabase.GetCompletedTasks(currentUser);
        //}
        //public IEnumerable<RequestMainInfoEntity> FindCompletedTasks(string currentUser, DateTime startDate, DateTime endDate)
        //{
        //    return tenantDatabase.FindCompletedTasks(currentUser, startDate, endDate);
        //}

        //public IEnumerable<RequestMainInfoEntity> GetDrafts(string currentUser)
        //{
        //    return tenantDatabase.GetDrafts(currentUser);
        //}

        #endregion

        //public void SetConfigurationList(List<ConfigurationEntity> configs)
        //{
        //    tenantDatabase.SetConfigurationList(configs);
        //}

        //public List<ConfigurationEntity> GetConfigurationList()
        //{
        //    return tenantDatabase.GetConfigurationList();
        //}

        //public RequestMainInfoEntity GetMainInfo(Guid guid)
        //{
        //    return tenantDatabase.GetMainInfo(guid);
        //}
    }
}

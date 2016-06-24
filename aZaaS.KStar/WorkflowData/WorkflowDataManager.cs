
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Repositories;
using aZaaS.Framework.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Facades;
using aZaaS.Framework;
using aZaaS.Framework.Workflow.Pager;
using System.Web.Configuration;
using System.Data.SqlTypes;

namespace aZaaS.KStar.WorkflowData
{
    [Obsolete("该模块已被迁移或者废弃了部分方法，已不再使用！")]
    class WorkflowDataManager
    {

        private readonly UserService userService;
        private readonly WorkflowClientService workflowService;


        private const int TAKE = 100;

        public WorkflowDataManager(AuthenticationType authType)
        {
            this.userService = new UserService();
            this.workflowService = new WorkflowClientService(authType);
        }


        #region For Dashboard

        #region This is out of date

        /*
        internal IEnumerable<WorklistItem> GetPendingTasks(string currentUser, AuthenticationType authtype)
        {
            var wlItems = new List<WorklistItem>();

            List<WorklistItem> items = null;
            //var processes = AppProcessManager.GetProcesses("TenantDatabase");

            WorkflowManagementService wfMngService = new WorkflowManagementService(authtype);
            var processset = wfMngService.GetProcessSets();

            processset.ToList().ForEach(process =>
            {
                items = workflowService.GetWorklistItemsByProcess(currentUser, PlatformType.ASP, process.FullName);
                items.ForEach(item =>
                {
                    item.BusinessData = workflowService.GetBusinessData(item).ToList();
                    wlItems.Add(item);
                });
            });
            //processes.ToList().ForEach(process =>
            //{
            //    items = workflowService.GetWorklistItemsByProcess(currentUser, PlatformType.ASP, process);
            //    items.ForEach(item =>
            //    {
            //        item.BusinessData = workflowService.GetBusinessData(item).ToList();
            //        wlItems.Add(item);
            //    });
            //});
            return wlItems.OrderByDescending(s => s.StartDate).AsEnumerable();
        }
        internal IEnumerable<WorklistItem> FindPendingTasks(WorklistItemCriteria criteria, AuthenticationType authtype)
        {
            var wlItems = new List<WorklistItem>();

            WorkflowManagementService wfMngService = new WorkflowManagementService(authtype);

            List<WorklistItem> items = workflowService.GetWorklistItemsByPage(PlatformType.ASP, criteria);
            if (items != null)
            {
                items.ForEach(item =>
                {
                    item.BusinessData = workflowService.GetBusinessData(item).ToList();
                    wlItems.Add(item);
                });
            }

            //return wlItems.OrderBy(s => s.StartDate).AsEnumerable();
            return wlItems;
        }

        internal IEnumerable<WorklistItem> GetAllPendingTasks(string currentUser, AuthenticationType authtype)
        {
            var wlItems = new List<WorklistItem>();

            WorkflowManagementService wfMngService = new WorkflowManagementService(authtype);

            List<WorklistItem> items = workflowService.GetWorklistItems(currentUser, PlatformType.ASP);

            return items;
        }

        /// <summary>
        /// 通用获取我申请的流程
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        internal IEnumerable<WorkflowTask> GetRequestTasksCommon(string currentUser, string tenantId, string connectionstring)
        {
            return FindRequestTasksCommon(currentUser, tenantId, connectionstring, null);
        }

        internal IEnumerable<WorkflowTask> FindRequestTasksCommon(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        {
            //todo: 待重构和完善           
            ServiceContext context = new ServiceContext();
            context["ConnectionString"] = connectionstring;
            context["CurrentWorkflowSecurityLabel"] = PortalEnvironment.CurrentWorkflowSecurityLabel;
            context["CurrentUser"] = currentUser;
            context.UserName = currentUser;
            context.TenantID = tenantId;
            List<WorkflowTask> items = workflowService.GetMyStartedProcessInstances(context, criteria).Select(item => new WorkflowTask()
            {
                ActivityName = item.ActivityName,
                Procinstid = item.ID,
                Priority = item.Priority,
                Status = (byte)item.Status,
                StartDate = item.StartDate,
                FinishDate = item.FinishDate,
                Originator = item.Originator,
                Folio = item.Folio,
                FullName = item.FullName,
                ProcessName = item.ProcessName,
                ViewUrl = item.ViewUrl
            }).ToList();
            items.ForEach(item => item.StatusDesc = getWorkflowStatusDesc(item.Status, item.Procinstid));
            return items;
        }
          

        internal IEnumerable<WorkflowTask> GetOnGoingTasksCommon(string currentUser, string tenantId, string connectionstring)
        {
            return FindOnGoingTasksCommon(currentUser, tenantId, connectionstring, null);
        }

        /// <summary>
        /// 通用获取我参与的流程
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        internal IEnumerable<WorkflowTask> FindOnGoingTasksCommon(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        {
            //todo: 待重构和完善
            ServiceContext context = new ServiceContext();           
            context["ConnectionString"] = connectionstring;            
            context["CurrentWorkflowSecurityLabel"] = PortalEnvironment.CurrentWorkflowSecurityLabel;
            context["CurrentUser"] = currentUser;
            context.UserName = currentUser;
            context.TenantID = tenantId;

            List<WorkflowTask> items = workflowService.GetMyParticipatedProcessInstances(context, criteria).Select(item => new WorkflowTask()
            {
                ActivityName = item.ActivityName,
                Procinstid = item.ID,
                Priority = item.Priority,
                Status = (byte)item.Status,
                StartDate = item.StartDate,
                FinishDate = item.FinishDate,
                Originator = item.Originator,
                Folio = item.Folio,
                FullName = item.FullName,
                ProcessName = item.ProcessName,
                ViewUrl = item.ViewUrl
            }).ToList();
            items.ForEach(item => item.StatusDesc = getWorkflowStatusDesc(item.Status));
            return items;
        }

        /// <summary>
        /// 按审批历史获取我参与的流程
        /// </summary>
        /// <param name="currentUser">当前用户</param>
        /// <returns></returns>
        internal IEnumerable<WorkflowTask> FindOnGoingTasksWithActionHis(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        {
            //todo: 待重构和完善
            ServiceContext context = new ServiceContext();
            context["ConnectionString"] = connectionstring;
            context["CurrentWorkflowSecurityLabel"] = PortalEnvironment.CurrentWorkflowSecurityLabel;
            context["CurrentUser"] = currentUser;
            context["ActionName"] = "回退到,同意,不同意,不通过,代理给,转交给";
            context.UserName = currentUser;
            context.TenantID = tenantId;

            List<WorkflowTask> items = workflowService.GetMyParticipatedProcessInstancesWithAction(context, criteria).Select(item => new WorkflowTask()
            {
                ActivityName = item.ActivityName,
                Procinstid = item.ID,
                Priority = item.Priority,
                Status = (byte)item.Status,
                StartDate = item.StartDate,
                FinishDate = item.FinishDate,
                Originator = item.Originator,
                Folio = item.Folio,
                FullName = item.FullName,
                ProcessName = item.ProcessName,
                ViewUrl = item.ViewUrl
            }).ToList();
            items.ForEach(item => item.StatusDesc = getWorkflowStatusDesc(item.Status,item.Procinstid));
            return items;
        }          

        */

        #endregion

        //internal IEnumerable<WorkflowTask> FindInsteadRequestTasksCommon(string currentUser, string tenantId, string connectionstring, ProcessInstanceCriteria criteria)
        //{
        //    //todo: 待重构和完善           
        //    ServiceContext context = new ServiceContext();
        //    context["ConnectionString"] = connectionstring;
        //    context["CurrentWorkflowSecurityLabel"] = PortalEnvironment.CurrentWorkflowSecurityLabel;
        //    context["CurrentUser"] = currentUser;
        //    context.UserName = currentUser;
        //    context.TenantID = tenantId;
        //    List<WorkflowTask> items = workflowService.GetInsteadMyStartedProcessInstances(context, criteria).Select(item => new WorkflowTask()
        //    {
        //        ActivityName = item.ActivityName,
        //        Procinstid = item.ID,
        //        Priority = item.Priority,
        //        Status = (byte)item.Status,
        //        StartDate = item.StartDate,
        //        FinishDate = item.FinishDate,
        //        Originator = item.Originator,
        //        Folio = item.Folio,
        //        FullName = item.FullName,
        //        ProcessName = item.ProcessName,
        //        ViewUrl = item.ViewUrl,
        //        Submitter = item.BOOwner
        //    }).ToList();
        //    items.ForEach(item => item.StatusDesc = getWorkflowStatusDesc(item.Status, item.Procinstid));
        //    return items;
        //}

        //internal IEnumerable<RequestMainInfoEntity> GetDrafts(string currentUser)
        //{
        //    IEnumerable<RequestMainInfoEntity> items = null;
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        string formStatus = FormStatus.Draft.ToString();
        //        string formType = FormType.None.ToString();
        //        items = context.RequestMainInfo.Where(s => s.ApplicantAccount == currentUser && s.FormType != formType && s.FormStatus == formStatus)
        //            .OrderByDescending(s => s.ApplicationDate).ToList();
        //    }
        //    return items;
        //}

        #endregion

        //internal void SetConfiguration(KStarDbContext context, string value, string key)
        //{
        //    ConfigurationEntity configuration = context.Configurations.SingleOrDefault(s => s.ConfigKey == key);
        //    if (configuration == null)
        //    {
        //        configuration = new ConfigurationEntity() { ConfigKey = key, ConfigValue = value };
        //        context.Configurations.Add(configuration);
        //    }
        //    else
        //        configuration.ConfigValue = value;
        //}

        //internal void SetConfigurationList(List<ConfigurationEntity> configs)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        foreach (var item in configs)
        //        {
        //            SetConfiguration(context, item.ConfigValue, item.ConfigKey);
        //        }
        //        context.SaveChanges();
        //    }

        //}

        //internal List<ConfigurationEntity> GetConfigurationList()
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        return context.Configurations.ToList();
        //    }

        //}

        ///// <summary>
        ///// 获取 RequestMainInfo 对象
        ///// </summary>
        //internal RequestMainInfoEntity GetMainInfo(Guid guid)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        return context.RequestMainInfo.FirstOrDefault(s => s.ID == guid);
        //    }
        //}


        ///// <summary>
        ///// 获取流程状态的描述
        ///// </summary>
        ///// <param name="status">状态编号</param>
        ///// <returns></returns>
        //internal string getWorkflowStatusDesc(Byte status)
        //{
        //    //GroupName	StatusID	Status
        //    //Process   	0	Error     
        //    //Process   	1	Running   
        //    //Process   	2	Active    
        //    //Process   	3	Completed 
        //    //Process   	4	Stopped   
        //    //Process   	5	Deleted   

        //    //TODO 使用RM集中管理，并且要支持多语言

        //    switch (status)
        //    {
        //        case 0:
        //            return "错误";
        //        case 1:
        //            return "运行中";
        //        case 2:
        //            return "进行中";
        //        case 3:
        //            return "已完成";
        //        case 4:
        //            return "已停止";
        //        case 5:
        //            return "作废";
        //        default:
        //            break;
        //    }

        //    return "Unkown";
        //}

        ///// <summary>
        ///// 获取流程状态的描述
        ///// </summary>
        ///// <param name="status">状态编号</param>
        ///// <returns></returns>
        //internal string getWorkflowStatusDesc(int status,int procInstId)
        //{
        //    //GroupName	StatusID	Status
        //    //Process   	0	Error     
        //    //Process   	1	Running   
        //    //Process   	2	Active    
        //    //Process   	3	Completed 
        //    //Process   	4	Stopped   
        //    //Process   	5	Deleted   

        //    //TODO 使用RM集中管理，并且要支持多语言
        //    if (status == 3)
        //    {
        //        status = GetFlowStatus(status, procInstId);
        //    }

        //    switch (status)
        //    {
        //        case 0:
        //            return "错误";
        //        case 1:
        //            return "运行中";
        //        case 2:
        //            return "进行中";
        //        case 3:
        //            return "已完成";
        //        case 4:
        //            return "已停止";
        //        case 5:
        //            return "作废";
        //        default:
        //            break;
        //    }

        //    return "Unkown";
        //}

        //private int GetFlowStatus(int status, int procInstId)
        //{
        //    var flowStatus = status;

        //    using (KStarFramekWorkDbContext context = new KStarFramekWorkDbContext())
        //    {
        //        var item = context.ProcessFormCancel.FirstOrDefault(r => r.ProcInstId == procInstId);
        //        if (item != null)
        //        {
        //            flowStatus = item.Status;
        //        }
        //    }

        //    return flowStatus;
        //}
    }
}

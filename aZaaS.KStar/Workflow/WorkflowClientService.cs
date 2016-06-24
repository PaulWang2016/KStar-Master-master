using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Extensions;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.Framework.Logging;
using aZaaS.KStar.Workflow.Configuration;
using System.Transactions;

namespace aZaaS.KStar
{
    /// <summary>
    /// Changed:
    /// 2015-08-14 by BingYi
    /// 1. All k2 userName support auto resolving security label by using <see cref="K2User.ApplySecurityLabel(userName)"/>.
    /// <remarks>
    /// That means when you call <see cref="WorkflowClientService"/>,you don't need to worry about how  the k2 label reolving.
    /// And no need to handle k2 label on top level application,just pass your application username directly.
    /// </remarks>
    /// </summary>
    public class WorkflowClientService
    {
        private readonly WFClientFacade wfClientFacade;
        private readonly ILogger _logger = LogFactory.GetLogger();

        public WorkflowClientService(AuthenticationType authType)
        {
            this.wfClientFacade = new WFClientFacade(authType);
        }

        ///<summary>
        ///模拟用户身份(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="imUserName"></param>
        ///<returns></returns>
        public bool ImpersonateUser(string imUserName)
        {
            
            return this.wfClientFacade.ImpersonateUser(new ServiceContext(), K2User.ApplySecurityLabel(imUserName));
            
        }

        ///<summary>
        ///创建流程的实例(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="fullName"></param>
        ///<param name="version"></param>
        ///<returns></returns>
        public ProcessInstance CreateProcessInstance(string fullName, int version)
        {

            return this.wfClientFacade.CreateProcessInstance(new ServiceContext(), fullName, version);
        }

        ///<summary>
        ///发起流程(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInst"></param>
        ///<param name="sync"></param>
        ///<returns></returns>
        public int StartProcessInstance(ProcessInstance procInst, bool sync)
        {

            return this.wfClientFacade.StartProcessInstance(new ServiceContext(), procInst, sync);
        }


        public int StartProcessInstance(string userName, string fullName, string folio, Dictionary<string, object> datafields)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);


            return this.wfClientFacade.StartProcessInstance(context, fullName, folio, datafields);
        }

        /// <summary>
        /// Update  Process  Folio
        /// </summary>
        /// <param name="procInstID"></param>
        /// <param name="folio"></param>
        public void UpdateProcessFolio(int procInstID, string folio)
        {
            ServiceContext context = new ServiceContext();
            this.wfClientFacade.UpdateProcessFolio(context, procInstID, folio);

        }
        ///<summary>
        ///获取工作项列表(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="platform"></param>
        ///<returns></returns>
        public List<WorklistItem> GetWorklistItems(string userName, PlatformType platform)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.GetWorklistItems(context, platform);
        }


        public List<WorklistItem> GetWorklistItemsByPage(PlatformType platform, WorklistItemCriteria criteria)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(criteria.UserName);

            return this.wfClientFacade.GetWorklistItemsByPage(context, platform,  criteria);
        }

        public List<WorklistItem> GetWorklistItemsByProcess(string userName, PlatformType platform, string processName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.GetWorklistItemsByProcess(context, platform, processName);
        }

        ///<summary>
        ///获取与工作项关联的业务数据
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="item"></param>
        ///<returns></returns>
        public IEnumerable<BusinessDataItem> GetBusinessData(WorklistItem item)
        {
            ServiceContext context = new ServiceContext();
            //context[ContextVariable.ApplicationName.Label()] = applicationName;

            return this.wfClientFacade.GetBusinessData(context, item);
        }

        ///<summary>
        ///打开一个工作项(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="sn"></param>
        ///<returns></returns>
        public WorklistItem OpenWorklistItem(string userName, string sn)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.OpenWorklistItem(context, sn);
        }


        public WorklistItem OpenWorklistItem(string userName, string sn, PlatformType platform, bool allocated)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.OpenWorklistItem(context, sn, platform, allocated);
        }

        ///<summary>
        ///打开一个工作项(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="sn"></param>
        ///<param name="platform"></param>
        ///<param name="allocated"></param>
        ///<returns></returns>
        public WorklistItem OpenWorklistItem2(string userName, string sn, PlatformType platform, bool allocated)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.OpenWorklistItem(context, sn, platform, allocated);
        }

        ///<summary>
        ///打开共享的工作项(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="shareUser"></param>
        ///<param name="sn"></param>
        ///<param name="platform"></param>
        ///<param name="allocated"></param>
        ///<returns></returns>
        public WorklistItem OpenWorklistItem(string userName, string shareUser, string sn, PlatformType platform, bool allocated)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.OpenWorklistItem(context, shareUser, sn, platform, allocated);
        }


        public void BatchAction(List<String> snList, string actionName, string comment, bool sync)
        {

            this.wfClientFacade.BatchAction(new ServiceContext(), snList, actionName, comment, sync);
        }

        ///<summary>
        ///获取关卡执行人 (aZaaS)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="activityName"></param>
        ///<returns></returns>
        public List<TextValueInfo> GetActivityApprovers(int procInstID, string activityName)
        {

            return this.wfClientFacade.GetActivityApprovers(new ServiceContext(), procInstID, activityName);
        }


        public List<DoneWorklistItem> GetMyDoneWorklistItems(WorklistItemCriteria criteria)
        {

            return this.wfClientFacade.GetMyDoneWorklistItems(new ServiceContext(), criteria);
        }

        /// <summary>
        ///  (已有更好的扩展重载代替方法，不建议直接调用！- by BingYi）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<ProcessInstance> GetMyStartedProcessInstances(ServiceContext context, ProcessInstanceCriteria criteria)
        {
            //context["CurrentUser"] = K2User.ApplySecurityLabel(context["CurrentUser"]);
            return this.wfClientFacade.GetMyStartedProcessInstances(context, criteria);
        }

        /// <summary>
        /// (方法已废弃,该方法已被扩展重写)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<ProcessInstance> GetInsteadMyStartedProcessInstances(ServiceContext context, ProcessInstanceCriteria criteria)
        {
            ConfigManager config = new ConfigManager(AuthenticationType.Form);
            return config.GetMyStartedProcessInstances(context, ref criteria);
        }

        /// <summary>
        /// (已有更好的扩展重载代替方法，不建议直接调用！ - by BingYi）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<ProcessInstance> GetMyParticipatedProcessInstances(ServiceContext context, ProcessInstanceCriteria criteria)
        {
            //context["CurrentUser"] = K2User.ApplySecurityLabel(context["CurrentUser"]);
            return this.wfClientFacade.GetMyParticipatedProcessInstances(context, criteria);
        }

        /// <summary>
        /// (方法已废弃,该方法已被扩展重写)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<ProcessInstance> GetMyParticipatedProcessInstancesWithAction(ServiceContext context, ProcessInstanceCriteria criteria)
        {
            var configManager = new ConfigManager(AuthenticationType.Form);

            return configManager.GetMyParticipatedProcessInstancesByAction(context, ref criteria);
        }

        ///<summary>
        ///获取流程实例的DataField值(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<param name="fieldName"></param>
        ///<returns></returns>
        public object GetProcInstDataFieldValue(string userName, int procInstID, string fieldName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            return this.wfClientFacade.GetProcInstDataFieldValue(context, procInstID, fieldName);
        }

        ///<summary>
        ///获取流程实例的DataField字典(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<returns></returns>
        public Dictionary<String, DataField> GetProcInstDataFields(int procInstID)
        {
            return this.wfClientFacade.GetProcInstDataFields(new ServiceContext(), procInstID);
        }

        ///<summary>
        ///获取初始的流程实例(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<returns></returns>
        public ProcessInstance GetProcessInstanceFromOrig(int procInstID)
        {

            return this.wfClientFacade.GetProcessInstanceFromOrig(new ServiceContext(), procInstID);
        }

        ///<summary>
        ///获取单个流程实例(aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="procInstID"></param>
        ///<returns></returns>
        public ProcessInstance GetProcessInstance(int procInstID)
        {

            return this.wfClientFacade.GetProcessInstance(new ServiceContext(), procInstID);
        }


        public void ExecuteAction(string userName, string sn, List<DataField> dataFields, string actionName)
        {
            //ServiceContext context = new ServiceContext();
            //context.UserName = userName;

            this.wfClientFacade.ExecuteAction(sn, dataFields, actionName, string.Empty, false);
        }


        public void ExecuteAction2(string userName, string sn, List<DataField> dataFields, string actionName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            this.wfClientFacade.ExecuteAction(context, sn, dataFields, actionName, string.Empty, false);
        }

        public void ExecuteAction3(string userName, string sn, List<DataField> dataFields, List<DataField> activityDataFields, string actionName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            this.wfClientFacade.ExecuteAction(context, sn, dataFields, activityDataFields, actionName, string.Empty, false);
        }

        public void ExecuteAction4(string userName, string sn, string sharedUser, List<DataField> dataFields, string actionName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            this.wfClientFacade.ExecuteAction(context, sn, sharedUser, dataFields, actionName, string.Empty, false);
        }

        public void ExecuteAction5(string userName, string sn, string sharedUser, List<DataField> dataFields, List<DataField> activityDataFields, string actionName)
        {

            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            this.wfClientFacade.ExecuteAction(context, sn, sharedUser, dataFields, activityDataFields, actionName, string.Empty, false);
        }

        public void Delegate(string userName, string sn, List<String> userNames)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            List<string> labelUserNames = new List<string>();
            foreach (var item in userNames)
            {
                labelUserNames.Add(K2User.ApplySecurityLabel(item));
            } 
            this.wfClientFacade.Delegate(context, sn, labelUserNames);
        }

        ///<summary>
        ///跳转到某个关卡 (aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="sn"></param>
        ///<param name="activityName"></param>
        ///<param name="expireAll"></param>
        ///<param name="sync"></param>
        public void GotoActivity(string userName, string sn, string activityName, bool expireAll, bool sync)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            this.wfClientFacade.GotoActivity(context, sn, activityName, expireAll, sync);
        }


        public void CancelActivity(string sn, List<String> activityNames)
        {

            this.wfClientFacade.CancelActivity(new ServiceContext(), sn, activityNames);
        }

        ///<summary>
        ///转派工作项,for impersonate user
        ///</summary>
        ///<param name="sn"></param>
        ///<param name="toUserName"></param>
        public void Redirect(string sn, string toUserName)
        {

            this.wfClientFacade.Redirect(sn, K2User.ApplySecurityLabel(toUserName));
        }

        ///<summary>
        ///转派工作项 (aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="sn"></param>
        ///<param name="toUserName"></param>
        public void Redirect2(string userName, string sn, string toUserName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            this.wfClientFacade.Redirect(context, sn, toUserName);
        }

        public void RedirectAll(string userName, string toUserName)
        {
            ServiceContext context = new ServiceContext();
            context.UserName = K2User.ApplySecurityLabel(userName);

            using (TransactionScope ts = new TransactionScope())
            {
                List<WorklistItem> worklist = this.wfClientFacade.GetWorklistItems(context, PlatformType.ASP);
                foreach (var item in worklist)
                {
                    this.wfClientFacade.Redirect(context, item.SN, K2User.ApplySecurityLabel(toUserName));
                }
                ts.Complete();
            }
        }

        ///<summary>
        ///释放一个工作项 (aZaaS,K2)
        ///</summary>
        ///<param name="new ServiceContext()"></param>
        ///<param name="sn"></param>
        public void Release(string sn)
        {

            this.wfClientFacade.Release(new ServiceContext(), sn);
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Mvc.ViewResults;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Models;

using aZaaS.Framework.Workflow;
using System.Configuration;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.HtmlFilter;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Workflow.Configuration.Models;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.ParticipantSetService;
using aZaaS.KStar.WorkflowConfiguration;

namespace aZaaS.KStar.Form.Mvc.Controllers
{
    [EnhancedHandleError]
    [Authorize]
    public abstract class KStarFormController : Controller, IStorageBehavior, IExecutionBehavior
    {
        private IStorageProvider _storageProvider;
        private IStorageService _storageService;
        private IWorkflowService _workflowService;
        private IOrganizationService _OrganizationService;
        private WorkflowTaskModel _taskItem;
        private KStarFormSettingProvider _formSettingProvider;
        private ControlTmplProvider _controlTmplProvider;
        private IFormCCProvider _formCCProvider;
        private IFormSignerProvider _formSignerProvider;
        private ProcessLogService _logService;
        protected ConfigManager _configManager;
        private IFormMessageProvider _messageProvider;

        private string _defaultLayoutMasterName = "_KStarForm_Master_Layout";
        private const string COMMON_DATAFIELD_FORMID = "FormId";
        private const string COMMON_DATAFIELD_POSITION = "Position";
        private const string COMMON_DATAFIELD_TASK_ACTIONTAKER = "_TASK_ACTIONTAKER";
        private const string COMMON_DATAFIELD_TASK_ACTIONCOMMENT = "_TASK_ACTIONCOMMENT";
        private const string COMMON_DATAFIELD_TASK_DEFAULTACTION = "_TASK_DEFAULTACTION";
        private const string COMMON_DATAFIELD_TASK_ADDITIONSIGNACTION = "_TASK_ADDITIONSIGNACTION";
        /// <summary>
        /// 流程图默认action 值 为同意
        /// </summary>
        public const string COMMON_DATAFIELD_DEFAULT_ACTION = "同意";
        private bool _isPermissionAnonymous = false;

        //其他三方平台，未登录调用Controller 事件需要使用到的用户信息
        public string OtherPlatformUserName = string.Empty;
        //其他三方平台，未登录调用Controller 事件需要使用到的流程名称
        public string OtherPlatformProcessFullName = string.Empty;


        public KStarFormController()
        {
            _storageProvider = new KStarFormStorageProvider();
            _storageService = new KStarFormStorageService(_storageProvider, this);
            _workflowService = new KStarFormWorkflowService(AuthenticationType.Form, this);
            _OrganizationService = new KStarFormOrganizationService();
            _formSettingProvider = new KStarFormSettingProvider();
            _controlTmplProvider = new ControlTmplProvider();
            _formCCProvider = new KStarFormCCProvider();
            _formSignerProvider = new KStarFormSignerProvider();
            _logService = new ProcessLogService();
            _configManager = new ConfigManager(AuthenticationType.Form);
            _messageProvider = SetKStarFormMessage() ?? new KStarFormMessageProvider();
            BindKStarFormMessage();
        }

        #region Context Properties

        public string UserName
        {
            get
            {
                if (User.Identity.IsAuthenticated)
                {
                    return User.Identity.Name;
                }
                else
                {
                    return OtherPlatformUserName;
                }

            }
        }

        //TODO:
        public WorkflowTaskModel TaskModel
        {
            get
            {
                var serialNo = this.GetSerialNo();

                if (!string.IsNullOrEmpty(serialNo))
                {
                    if (_taskItem == null)
                    {
                        try
                        {
                            _taskItem = _workflowService.GetTaskItem(UserName, this.GetShareUser(), serialNo);
                            _storageProvider.UpdateForm(this.GetFormId(), _taskItem.Folio);

                            //Excluding the addition sign action from the actions
                            if (_taskItem.ActivityDataFields.ContainsKey(COMMON_DATAFIELD_TASK_ADDITIONSIGNACTION))
                                _taskItem.SignActionName = _taskItem.ActivityDataFields[COMMON_DATAFIELD_TASK_ADDITIONSIGNACTION].ToString(); ;

                            if (!string.IsNullOrEmpty(_taskItem.SignActionName))
                            {
                                _taskItem.IsEnableSign = true;
                                _taskItem.Actions.Remove(_taskItem.SignActionName);
                            }
                        }
                        catch
                        {
                            _taskItem = new WorkflowTaskModel();

                            GotoErrorPage();
                        }
                    }
                }

                return _taskItem;
            }
        }

        //TODO:
        public string ActivityName
        {
            get
            {
                return TaskModel == null ? "" : TaskModel.ActivityName;
            }
        }

        public int ActivityID
        {
            get
            {
                return TaskModel == null ? 0 : TaskModel.ActivityId;
            }
        }

        public int FilterActivityID
        {
            get
            {
                int actId = 0;
                switch (this.GetWorkMode())
                {
                    case WorkMode.View:
                    case WorkMode.Review:
                        var activity = _storageProvider.GetActivityByViewMode(this.GetFormId(), this.UserName);
                        actId = activity.ID;
                        break;
                    default:
                        actId = TaskModel == null ? this.GetActivityId() : TaskModel.ActivityId;
                        break;
                }

                return actId;
            }
        }

        public int ProcInstId
        {
            get
            {
                return TaskModel == null ? 0 : TaskModel.ProcInstId;
            }
        }

        public WorkMode FormWorkMode
        {
            get { return this.GetWorkMode(); }
        }

        //是否允许任何人查看表单
        public bool IsPermissionAnonymous
        {
            get { return _isPermissionAnonymous; }
            protected set { _isPermissionAnonymous = value; }
        }

        /// <summary>
        /// ContentData Type
        /// </summary>
        public virtual Type ContentDataType { get; set; }

        /// <summary>
        /// ProcessName
        /// </summary>
        public abstract string ProcessName { get; }

        public IStorageProvider StorageProvider
        {
            get
            {
                return _storageProvider;
            }
        }

        public IWorkflowService WorkflowService
        {
            get { return _workflowService; }
        }

        public IStorageService StorageService
        {
            get { return _storageService; }
        }

        //TODO:

        public KStarFormSettingProvider FormSettingProvider
        {
            get { return _formSettingProvider; }
        }

        public ControlTmplProvider ControlTmplProvider
        {
            get { return _controlTmplProvider; }
        }

        public IFormCCProvider FormCCProvider
        {
            get { return _formCCProvider; }
        }

        public IFormMessageProvider MessageProvider
        {
            get
            {
                return _messageProvider;
            }
        }

        public IOrganizationService OrganizatoinService
        {
            get
            {
                return _OrganizationService;
            }
        }

        #endregion

        #region KStarForm Layout

        public void SetMasterLayout(string masterName)
        {
            _defaultLayoutMasterName = masterName;
        }

        public string MasterLayout
        {
            get { return _defaultLayoutMasterName; }
        }

        #endregion

        #region KStarForm Internal Actions

        [HttpPost]
        public ActionResult _Submit(string jsonData)
        {
            KStarFormModel formModel = null;

            var messageContainer = new List<ResultMessage>();
            var message = new ResultMessage(MessageType.Information, "Submit successfully!");

            try
            {
                formModel = KStarFormModel.Instance(jsonData);

                var workMode = this.GetWorkMode();
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);

                this.OnFormSubmitting(taskContext);

                if (_storageService.ProcessData(storageContext))
                {
                    var newFormId = storageContext.FormId;
                    taskContext.SynchronizeFormId(newFormId);

                    taskContext.DataFields.Clear();
                    taskContext.AddDataField(COMMON_DATAFIELD_FORMID, newFormId);

                    switch (workMode)
                    {
                        case WorkMode.Draft:
                            if (!formModel.IsDraft)
                            {
                                message = ResultMessage.Create(MessageType.Warning, "The form has been submitted, please do not submit duplicate!");
                                messageContainer.Add(message);
                                return Json(messageContainer);
                            }
                            message = NewTaskStart(taskContext, storageContext);
                            break;
                        case WorkMode.Startup:
                            message = NewTaskStart(taskContext, storageContext);
                            break;
                        case WorkMode.Approval:
                            var taskItem = new WorkflowTaskModel();
                            taskItem = _workflowService.GetTaskItem(taskContext.UserName, this.GetShareUser(), taskContext.SerialNo);
                            taskContext.SynchronizeTaskItem(taskItem);

                            if (string.IsNullOrEmpty(taskContext.ActionComment))
                            {
                                taskContext.ActionComment = ConfigurationManager.AppSettings["DefaultApprovalComment"].ToString();
                            }

                            taskContext.AddActivityDataField(COMMON_DATAFIELD_TASK_ACTIONTAKER, this.UserName);
                            taskContext.AddActivityDataField(COMMON_DATAFIELD_TASK_ACTIONCOMMENT, taskContext.ActionComment);
                            taskContext.AddDataField(COMMON_DATAFIELD_POSITION, formModel.Toolbar.PostSysID);
                             
                            ActionTaskExecute(taskContext);

                            message = KStarFormMessage.SubmitSuccessMsg(taskContext, storageContext);
                            break;
                    }

                    //SaveAttachment(formModel.Attachments);
                }

                this.OnFormSubmitted(taskContext);

                messageContainer.Add(message);
            }
            catch (Exception ex)
            {
                messageContainer.Clear();
                messageContainer.Add(KStarFormMessage.SubmitFailMsg(ex));
                return Json(messageContainer);
            }

            return Json(messageContainer);
        }

        private ResultMessage NewTaskStart(WorkflowTaskContext taskContext, StorageContext storageContext)
        {
            if (!taskContext.DataFields.ContainsKey(COMMON_DATAFIELD_POSITION))
            {
                taskContext.AddDataField(COMMON_DATAFIELD_POSITION, string.IsNullOrWhiteSpace(taskContext.FormModel.ApplicantPositionID) ? "NULL" : taskContext.FormModel.ApplicantPositionID);
            }
           
            var message = new ResultMessage(MessageType.Information, "Submit successfully!");
            this.OnWorkflowNewTaskStarting(taskContext);

            if (string.IsNullOrEmpty(taskContext.ProcessName))
            {
                message = ResultMessage.Create(MessageType.Error, string.Format(@"ProcessName Can't be null! "));
                return message;
            }

            var procInstId = _workflowService.StarNewTask(taskContext.UserName, taskContext.ProcessName, taskContext.Folio, taskContext.DataFields);
            taskContext.ProcInstId = procInstId;
            _storageProvider.UpdateFormDraftStatus(taskContext.FormId, procInstId, taskContext.ProcessName, taskContext.Folio, false);
            this.OnWorkflowNewTaskStarted(taskContext);

            message = KStarFormMessage.SubmitSuccessMsg(taskContext, storageContext);

            return message;
        }

        private void ActionTaskExecute(WorkflowTaskContext taskContext)
        {
            this.OnWorkflowTaskExecuting(taskContext);
            //_workflowService.ActionTask(taskContext.UserName, taskContext.SerialNo, this.GetShareUser(), taskContext.DataFields, taskContext.ActivityDataFields, taskContext.ActionName);
            this.OnWorkflowTaskExecute(taskContext);
            UpdateFormSigner(taskContext.ProcInstId, taskContext.ActivityName, this.UserName, taskContext.ActionName);
            this.OnWorkflowTaskExecuted(taskContext);
        }

        [HttpPost]
        public ActionResult _Draft(string jsonData)
        {
            KStarFormModel formModel = null;
            var messages = new List<ResultMessage>();

            try
            {
                formModel = KStarFormModel.Instance(jsonData);

                var storageContext = InitStorageContext(formModel);
                storageContext.FormModel.IsDraft = true;

                this.OnFormDrafting(storageContext);

                var isDone = _storageService.ProcessData(storageContext);
                if (isDone)
                {
                    if (this.GetWorkMode() != WorkMode.Draft)
                    {
                        string strFormat = "{0}?_DraftId={1}";
                        if (Request.Url.AbsoluteUri.IndexOf("?") >= 0)
                            strFormat = "{0}&_DraftId={1}";
                        var draftUrl = string.Format(strFormat, Request.UrlReferrer.AbsoluteUri, storageContext.FormId);
                        _storageProvider.UpdateFormDraftUrl(storageContext.FormId, draftUrl);
                    }
                }

                this.OnFormDrafted(storageContext);

                var formInfo = ResultMessage.Create(MessageType.Warning, storageContext.FormId.ToString());
                var success = KStarFormMessage.SaveSuccessMsg(storageContext);
                messages.Add(formInfo);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.SaveFailMsg(ex));
                return Json(messages);
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _Save(string jsonData)
        {
            KStarFormModel formModel = null;
            var messages = new List<ResultMessage>();

            try
            {
                formModel = KStarFormModel.Instance(jsonData);

                var storageContext = InitStorageContext(formModel);

                var isDone = _storageService.ProcessData(storageContext);

                var serialNo = this.GetSerialNo();
                var actionName = formModel.Toolbar.ActionName;
                var actionComment = formModel.Toolbar.ActionComment;

                _storageProvider.SaveApprovalDraft(serialNo, actionName, actionComment);

                var success = KStarFormMessage.SaveSuccessMsg(storageContext);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.SaveFailMsg(ex));
                return Json(messages);
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _Redirect(string jsonData, List<string> userList)
        {
            var messages = new List<ResultMessage>();
            var user = string.Empty;
            KStarFormModel formModel = null;

            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);

                if (userList != null & userList.Any())
                {
                    user = userList.ToList()[0];
                }

                this.OnWorkflowTaskRedirecting(taskContext);
                _workflowService.Redirect(this.UserName, taskContext.SerialNo, user);
                var userName = _OrganizationService.GetDisplayName(user);
                taskContext.ActionComment = string.Format("{0}: {1};  {2}", taskContext.ActionName, userName, taskContext.ActionComment);
                this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);
                this.OnWorkflowTaskRedirected(taskContext);

                var success = KStarFormMessage.RedirectSuccessMsg(taskContext, storageContext, user);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.RedirectFailMsg(ex));
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _Delegate(string jsonData, List<string> userList)
        {
            var messages = new List<ResultMessage>();
            var users = new StringBuilder();
            KStarFormModel formModel = null;

            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);

                this.OnWorkflowTaskDelegating(taskContext);
                _workflowService.Delegate(this.UserName, this.GetSerialNo(), userList);
                userList.ForEach(user => users.AppendFormat("{0}; ", _OrganizationService.GetDisplayName(user)));
                taskContext.ActionComment = string.Format("{0}: {1};  {2}", taskContext.ActionName, users.ToString(), taskContext.ActionComment);
                this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);
                this.OnWorkflowTaskDelegated(taskContext);

                var success = KStarFormMessage.DelegateSuccessMsg(taskContext, storageContext, userList);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.DelegateFailMsg(ex));
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _GotoActivity(string jsonData, string activityName)
        {
            var messages = new List<ResultMessage>();
            KStarFormModel formModel = null;

            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);
                _storageService.ProcessData(storageContext);
                this.OnWorkflowTaskGoingtoActivity(taskContext, activityName); 
                taskContext.AddDataField(COMMON_DATAFIELD_POSITION, formModel.Toolbar.PostSysID);
                _workflowService.GotoActivity(this.UserName, this.ProcInstId, activityName);
                this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);
                this.OnWorkflowTaskGotoActivity(taskContext, activityName);
                //清除当前环节已经处理的环节信息

                  ActivityParticipantSetService service = new ActivityParticipantSetService();
                  service.ClearProcessActivity(this.ProcInstId, activityName);

                  var success = KStarFormMessage.GotoActivitySuccessMsg(taskContext, storageContext, taskContext.ActionName);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.GotoActivityFailMsg(ex));
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _AddSigner(string jsonData, List<string> userList)
        {
            var messages = new List<ResultMessage>();
            var signers = new StringBuilder();
            KStarFormModel formModel = null;
            var actionName = "加签";
            var comment = string.Empty;

            try
            {
                formModel = KStarFormModel.Instance(jsonData);

                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);
                userList.ToList().ForEach(signer => signers.AppendFormat("{0}; ", _OrganizationService.GetDisplayName(signer)));
                _workflowService.AddSigner(this.UserName, this.GetSerialNo(), userList);

                if (!string.IsNullOrEmpty(formModel.Toolbar.SignActionName))
                {
                    actionName = formModel.Toolbar.SignActionName;
                }

                comment = string.Format("加签给:{0};  {1}", signers, taskContext.ActionComment);

                taskContext.DataFields.Clear();
                taskContext.AddDataField(COMMON_DATAFIELD_FORMID, taskContext.FormId);

                taskContext.AddDataField(COMMON_DATAFIELD_POSITION, formModel.Toolbar.PostSysID);

                taskContext.AddActivityDataField(COMMON_DATAFIELD_TASK_ACTIONTAKER, this.UserName);
                taskContext.AddActivityDataField(COMMON_DATAFIELD_TASK_ACTIONCOMMENT, comment);

                this.OnWorkflowTaskAddingSigner(taskContext);
                _workflowService.ActionTask(taskContext.UserName, taskContext.SerialNo, this.GetShareUser(), taskContext.DataFields, taskContext.ActivityDataFields, actionName);
                this.OnWorkflowTaskAddedSigner(taskContext);

                var success = KStarFormMessage.AddSignerSuccessMsg(taskContext, storageContext, userList);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.AddSignerFailMsg(ex));
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _CarbonCopy(string jsonData, List<string> userList)
        {
            var messages = new List<ResultMessage>();
            KStarFormModel formModel = null;

            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);
                var users = new StringBuilder();
                userList.ForEach(user => users.AppendFormat("{0}; ", _OrganizationService.GetDisplayName(user)));
                taskContext.ActionComment = string.Format("{0}: {1}", taskContext.ActionName, users.ToString());

                this.OnCarbonCopying(userList);
                this.FormCarbonCopy(userList);

                this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);
                this.OnCarbonCopied(userList);

                var success = KStarFormMessage.CarbonCopySuccessMsg(userList);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.CarbonCopyFailMsg(ex));
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _Review(string jsonData)
        {
            KStarFormModel formModel = null;
            var messages = new List<ResultMessage>();

            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);

                var formId = formModel.FormId;
                var activityId = this.GetActivityId();
                //var formCcId = this.GetFormCcId();
                var activityInfo = _configManager.GetActivityInfo(taskContext.ProcInstId, activityId);
                var comment = formModel.Toolbar.ReviewComment;
                taskContext.ActivityName = activityInfo.Name;

                if (_formCCProvider.IsAlreadyReview(formId, activityId, this.UserName))
                {
                    messages.Add(ResultMessage.Create(MessageType.Warning, "该表单已经审阅，请勿重复审阅!"));
                    return Json(messages);
                }

                //_formCCProvider.UpdateReceiverStatus(formCcId, comment);
                _formCCProvider.UpdateReceiverStatus(formId, activityId, this.UserName, comment);

                taskContext.ActionComment = comment;
                this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);

                var success = KStarFormMessage.ReviewSuccessMsg(taskContext, storageContext);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.ReviewFailMsg(ex));
            }

            return Json(messages);
        }

        [HttpPost]
        public ActionResult _DeleteProess(string jsonData)
        {
            var messages = new List<ResultMessage>();
            var user = string.Empty;
            KStarFormModel formModel = null;

            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);

                this.OnWorkflowTaskCanceling(taskContext);

                _workflowService.CancelActivity(taskContext.ProcInstId);
                //存入作废记录表
                _storageProvider.ProcInstCancel(taskContext.ProcInstId);

                this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);

                this.OnWorkflowTaskCanceled(taskContext);

                var success = KStarFormMessage.DeleteSuccessMsg(taskContext, storageContext);
                messages.Add(success);
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.DeleteFailMsg(ex));
            }

            return Json(messages);
        }
        [HttpPost]
        public ActionResult _Undo(string jsonData)
        {
            var messages = new List<ResultMessage>();
            KStarFormModel formModel = null;
            try
            {
                formModel = KStarFormModel.Instance(jsonData);
                var taskContext = InitTaskContext(formModel);
                var storageContext = InitStorageContext(formModel);

                this.OnWorkflowUndoing(taskContext);

                var logs = _logService.GetProcessLogByProcInstID(taskContext.ProcInstId);
                //判断初次发起流程时才允许 撤回
                if (logs != null && logs.Count() == 1)
                {
                    var activity = _configManager.GetUndoActivityByCurrentActivityId(this.UserName, formModel.ActivityId);
                    _workflowService.GotoActivity(UserName, taskContext.ProcInstId, activity.Name);

                    this.OnWorkflowUndoed(taskContext);

                    var success = KStarFormMessage.UndoSuccessMsg(taskContext, storageContext);
                    messages.Add(success);

                    this.AddProcessLog(taskContext, formModel.Toolbar.PostSysID);
                }
            }
            catch (Exception ex)
            {
                messages.Add(KStarFormMessage.UndoFailMsg(ex));
            }

            return Json(messages);
        }

        private WorkflowTaskContext InitTaskContext(KStarFormModel formModel)
        {
            return new WorkflowTaskContext(formModel, this.UserName, this.GetSerialNo());
        }

        private StorageContext InitStorageContext(KStarFormModel formModel)
        {
            return new StorageContext(formModel, formModel.FormId, UserName, this.GetWorkMode());
        }

        #endregion

        #region KStarForm ViewResult

        protected internal KStarFormViewResult KStarFormView()
        {
            return this.KStarFormView(null, null);
        }

        protected internal KStarFormViewResult KStarFormView(string viewName)
        {
            return this.KStarFormView(viewName, null);
        }

        protected internal KStarFormViewResult KStarFormView(object model)
        {
            return this.KStarFormView(null, model);
        }

        protected internal KStarFormViewResult KStarFormView(string viewName, object model)
        {
            if (model != null)
            {
                base.ViewData.Model = model;
            }

            return new KStarFormViewResult() { ViewName = viewName, MasterName = _defaultLayoutMasterName, ViewData = base.ViewData, TempData = base.TempData, ViewEngineCollection = this.ViewEngineCollection };
        }

        #endregion

        #region KStarForm StorageService Behaviors

        public virtual bool OnDataValidating(StorageContext context)
        {
            return true;
        }

        public virtual void OnBeforeDataStored(StorageContext context)
        {

        }

        public virtual void OnAfterDataStored(StorageContext context)
        {

        }

        public virtual object OnDataStoring(StorageContext context)
        {
            return null;
        }

        public virtual object OnDataDrafting(StorageContext context)
        {
            return null;
        }

        public virtual object OnDataUpdating(StorageContext context)
        {
            return null;
        }

        protected virtual void OnFormDrafting(StorageContext context)
        {

        }

        protected virtual void OnFormDrafted(StorageContext context)
        {

        }

        public virtual void OnDataStoredError(StorageContext context, Exception exception)
        {

        }

        #endregion

        #region KStarForm Execution Behaviors

        protected virtual void OnFormSubmitting(WorkflowTaskContext context)
        {

        }

        protected virtual void OnFormSubmitted(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowNewTaskStarting(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowNewTaskStarted(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskDelegating(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskDelegated(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskRedirecting(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskRedirected(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskExecuting(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskExecute(WorkflowTaskContext context)
        {
            _workflowService.ActionTask(context.UserName, context.SerialNo, this.GetShareUser(), context.DataFields, context.ActivityDataFields, context.ActionName);
        }

        public virtual void OnWorkflowTaskExecuted(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskAddingSigner(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskAddedSigner(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskGoingtoActivity(WorkflowTaskContext context, string activityName)
        {

        }

        public virtual void OnWorkflowTaskGotoActivity(WorkflowTaskContext context, string activityName)
        {

        }

        public virtual void OnWorkflowTaskCanceling(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowTaskCanceled(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowUndoing(WorkflowTaskContext context)
        {

        }

        public virtual void OnWorkflowUndoed(WorkflowTaskContext context)
        {

        }

        public virtual void OnAttachmentUploading()
        {

        }

        public virtual void OnAttachmentUploaded()
        {

        }

        public virtual void OnCarbonCopying(IList<string> ccList)
        {

        }

        public virtual void OnCarbonCopied(IList<string> ccList)
        {

        }

        public virtual void OnWorkflowReworkActivity(WorkflowTaskContext context)
        {

        }

        #endregion

        #region KStarForm Load Behaviors

        public virtual KStarFormModel OnKStarFormLoaded(KStarFormModel model)
        {
            return model;
        }

        public virtual string OnKStarFormFiltering(string html, int formId)
        {
            return html;
        }

        public virtual string OnKStarFormFilter(string html, int formId)
        {
            var processName = GetProcessFullNameByStartUrl();

            if (this.FilterActivityID == 0 && this.FormWorkMode != WorkMode.Review)
            {
                var controlFilter = new HtmlControlFilter(html, this.FormWorkMode, processName);
                controlFilter.Filter(this.FilterActivityID);

                html = controlFilter.Render();
            }
            else
            {
                html = this.Filter(html, this.FormWorkMode, GetOperActivitys(), this.FilterActivityID, processName);
            }

            return html;
        }

        public virtual string OnKStarFormFiltered(string html, int formId)
        {
            return html;
        }

        #endregion

        #region KStarForm UploadCompent Actions

        [HttpPost]
        public ActionResult _AttachmentUpload()
        {
            var uploadResults = new List<AttachmentModel>();
            var downloadUrl = GetDownloadUrl(true);

            uploadResults = _storageProvider.AttachmentUpload(Request.Files, this.UserName, this.ActivityName, downloadUrl).ToList();

            var jsonString = JsonHelper.SerializeObject(uploadResults);
            return Content(jsonString);
        }

        private string GetDownloadUrl(bool cached)
        {
            return Url.Action(cached ? "DownloadCacheFile" : "DownloadFile", RouteData.Values["controller"].ToString());
        }

        public ActionResult DownloadFile(Guid fileGuid)
        {
            var file = _storageProvider.GetDownloadFile(fileGuid);

            if (file == null)
                return new HttpNotFoundResult("File not found");

            if (System.IO.File.Exists(file.StoragePath))
                //return File(_storageProvider.GetFetchFile(file), file.FileType, Path.ChangeExtension(file.NewFileName, file.FileExtension));
                return File(_storageProvider.GetFetchFile(file), file.FileType, file.NewFileName + file.FileExtension);

            return new HttpNotFoundResult("File not found");

        }

        public ActionResult DownloadCacheFile(Guid fileGuid)
        {
            var file = _storageProvider.GetCachedFile(fileGuid);

            if (file == null)
                return new HttpNotFoundResult("File not found");

            if (System.IO.File.Exists(file.StoragePath))
                return File(file.StoragePath, file.FileType, file.FileName);

            return new HttpNotFoundResult("File not found");

        }

        #endregion

        #region KStarForm Message

        public virtual IFormMessageProvider SetKStarFormMessage()
        {
            return null;
        }

        public virtual void BindKStarFormMessage()
        {
            KStarFormMessage.SaveSuccessMsg = KStarFormMessage.SaveSuccessMsg ?? _messageProvider.GetSaveSuccessMsg;
            KStarFormMessage.SaveFailMsg = KStarFormMessage.SaveFailMsg ?? _messageProvider.GetSaveFailMsg;
            KStarFormMessage.SubmitSuccessMsg = KStarFormMessage.SubmitSuccessMsg ?? _messageProvider.GetSubmitSuccessMsg;
            KStarFormMessage.SubmitFailMsg = KStarFormMessage.SubmitFailMsg ?? _messageProvider.GetSubmitFailMsg;
            KStarFormMessage.RedirectSuccessMsg = KStarFormMessage.RedirectSuccessMsg ?? _messageProvider.GetRedirectSuccessMsg;
            KStarFormMessage.RedirectFailMsg = KStarFormMessage.RedirectFailMsg ?? _messageProvider.GetRedirectFailMsg;
            KStarFormMessage.DelegateSuccessMsg = KStarFormMessage.DelegateSuccessMsg ?? _messageProvider.GetDelegateSuccessMsg;
            KStarFormMessage.DelegateFailMsg = KStarFormMessage.DelegateFailMsg ?? _messageProvider.GetDelegateFailMsg;
            KStarFormMessage.GotoActivitySuccessMsg = KStarFormMessage.GotoActivitySuccessMsg ?? _messageProvider.GetGotoActivitySuccessMsg;
            KStarFormMessage.GotoActivityFailMsg = KStarFormMessage.GotoActivityFailMsg ?? _messageProvider.GetGotoActivityFailMsg;
            KStarFormMessage.AddSignerSuccessMsg = KStarFormMessage.AddSignerSuccessMsg ?? _messageProvider.GetAddSignerSuccessMsg;
            KStarFormMessage.AddSignerFailMsg = KStarFormMessage.AddSignerFailMsg ?? _messageProvider.GetAddSignerFailMsg;
            KStarFormMessage.CarbonCopySuccessMsg = KStarFormMessage.CarbonCopySuccessMsg ?? _messageProvider.GetCarbonCopySuccessMsg;
            KStarFormMessage.CarbonCopyFailMsg = KStarFormMessage.CarbonCopyFailMsg ?? _messageProvider.GetCarbonCopyFailMsg;
            KStarFormMessage.DeleteSuccessMsg = KStarFormMessage.DeleteSuccessMsg ?? _messageProvider.GetDeleteSuccessMsg;
            KStarFormMessage.DeleteFailMsg = KStarFormMessage.DeleteFailMsg ?? _messageProvider.GetDeleteFailMsg;
            KStarFormMessage.ReviewSuccessMsg = KStarFormMessage.ReviewSuccessMsg ?? _messageProvider.GetReviewSuccessMsg;
            KStarFormMessage.ReviewFailMsg = KStarFormMessage.ReviewFailMsg ?? _messageProvider.GetReviewFailMsg;

            KStarFormMessage.UndoSuccessMsg = KStarFormMessage.UndoSuccessMsg ?? _messageProvider.GetUndoSuccessMsg;
            KStarFormMessage.UndoFailMsg = KStarFormMessage.UndoFailMsg ?? _messageProvider.GetUndoFailMsg;
        }

        #endregion

        #region KStarForm Custom Behaviors

        [HttpGet]
        public JsonResult GetUserInfo(string userName)
        {
            var userInfo = _OrganizationService.GetUserInfo(userName);

            return Json(userInfo, JsonRequestBehavior.AllowGet);
        }

        public string GetProcessFullNameByStartUrl()
        {
            return _configManager.GetProcessFullNameByStartUrl(Request.Url.ToString().Split('?').First(), Request.RawUrl.Split('?').First());
        }

        public void UpdateFormSigner(int procInstId, string activityName, string userName, string actionName)
        {
            var signerItem = _formSignerProvider.GetPendingSigners(procInstId, activityName, userName);

            if (signerItem != null)
            {
                _formSignerProvider.UpdateApprovalAction(procInstId, userName, actionName);
            }
        }

        public void FormCarbonCopy(List<string> userList)
        {
            FormCC(userList, this.GetFormId());
        }

        public void FormCarbonCopy(List<string> userList, int formId)
        {
            FormCC(userList, formId);
        }

        private void FormCC(List<string> userList, int formId)
        {
            var fromCCList = new List<ProcessFormCC>();
            var activityId = this.ActivityID;
            var activityName = this.ActivityName;
            var originator = this.UserName;
            var originatorName = _OrganizationService.GetDisplayName(originator);
            string url = "";
            if (this.User.Identity.IsAuthenticated)
            {
                url = Request.UrlReferrer.LocalPath;
            }
            else
            {
                //没认证则使用第三方提供的流程名称执行
                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    Configuration_ProcessSet entity = dbContext.Configuration_ProcessSetSet.Where(x => x.ProcessFullName == OtherPlatformProcessFullName).FirstOrDefault();
                    url = entity.ApproveUrl;
                }
            }
            var viewUrl = string.Format("{0}?_FormId={1}&ActivityId={2}", url, formId, activityId);
            //= string.Format("{0}?_FormId={1}&ActivityId={2}", , formId, activityId);

            userList.ToList().ForEach(user =>
            {
                fromCCList.Add(new ProcessFormCC()
                {
                    FormId = formId,
                    Originator = originator,
                    OriginatorName = originatorName,
                    Receiver = user,
                    ReceiverName = _OrganizationService.GetDisplayName(user),
                    FormViewUrl = viewUrl,
                    ReceiverStatus = false,
                    ActivityId = activityId,
                    ActivityName = activityName
                });
            });

            _formCCProvider.Save(fromCCList);
            //_formCCProvider.Save(fromCCList, localPath);
        }

        public UserDto GetLoginUser()
        {
            return _OrganizationService.GetUserData(this.UserName);
        }

        public IList<Activity> GetReworkActivityList(KStarFormModel formModel)
        {
            var context = InitTaskContext(formModel);
            context.ReworkActivitys = GetReworkActivityList(formModel.ActivityId);

            this.OnWorkflowReworkActivity(context);

            return context.ReworkActivitys;
        }

        private IList<Activity> GetReworkActivityList(int activityId)
        {
            var list = _configManager.GetReworkActivityListByActId(activityId);

            return list;
        }

        public HashSet<int> GetOperActivitys()
        {
            int formId = this.GetFormId();
            var userName = this.UserName;
            var activityIds = _storageProvider.GetOperActivitys(formId, userName);

            return activityIds;
        }

        protected void AddProcessLog(WorkflowTaskContext taskContext, string PostSysID=null)
        {
            var processLog = new ProcessLog()
            {
                ProcInstID = taskContext.ProcInstId,
                ActivityName = taskContext.ActivityName,
                ActionName = taskContext.ActionName,
                ProcessName = taskContext.ProcessName,
                CommentDate = DateTime.Now,
                Comment = taskContext.ActionComment,
                OrigUserName = _OrganizationService.GetDisplayName(taskContext.UserName),
                OrigUserAccount = taskContext.UserName,
                UserName = _OrganizationService.GetDisplayName(this.UserName),
                UserAccount = this.UserName,
                SN = PostSysID
            };

            _logService.AddProcessLog(processLog);
        }

        public Activity GetCurrActivity(int procInstId)
        {
            var activity = _configManager.GetCurrActivityInfo(procInstId);

            return activity;
        }

        public bool ViewAuthorization()
        {
            //是否允许任何人访问为True,直接通过验证
            if (_isPermissionAnonymous)
            {
                return true;
            }

            var formId = this.GetFormId();
            var userName = this.UserName;
            var procInstId = 0;
            var processName = this.GetProcessFullNameByStartUrl();

            var formHeader = _storageProvider.GetProcessFormHeaderByFormId(formId);
            if (formHeader != null)
            {
                procInstId = formHeader.ProcInstID.Value;
            }

            //1.检查是否操作过表单
            var logList = _logService.GetProcessLogByProcInstID(procInstId);
            var identity = logList.FirstOrDefault(r => r.UserAccount.ToUpper() == userName.ToUpper()) != null;

            if (identity)
            {
                return true;
            }

            //2.检查是否被抄送
            identity = _formCCProvider.IsReceiver(formId, userName);

            if (identity)
            {
                return true;
            }

            //3.检查是否为加签人
            identity = _formSignerProvider.IsSigner(procInstId, userName);

            if (identity)
            {
                return true;
            }

            //4.检查是否为申请人
            identity = _storageProvider.IsApplicatant(formId, userName);

            if (identity)
            {
                return true;
            }

            //5.检查流程权限配置
            identity = _configManager.CheckRoleProcessSet(userName, processName);

            if (identity)
            {
                return true;
            }

            //6、是否在途人员
            identity = WorkflowClientServiceExtensions.IsInTransit(userName, procInstId);
            if (identity)
            {
                return true;
            }
            return identity;
        }

        public bool StartupAuthorization()
        {
            //是否允许任何人访问为True,直接通过验证
            if (_isPermissionAnonymous)
            {
                return true;
            }

            var userName = this.UserName;
            var processName = this.GetProcessFullNameByStartUrl();
            var identity = _configManager.VerfyStartPermission(processName, userName);

            return identity;
        }

        public string GetProcessDescription()
        {
            string description = string.Empty;
            var fullname = GetProcessFullNameByStartUrl();
            if (!string.IsNullOrEmpty(fullname))
            {
                var process = _configManager.GetProcessSetByFullName(this.UserName, fullname, false);
                if (process != null)
                {
                    description = process.Description;
                }
            }
            return description;
        }

        public bool IsEnableUndo(int procInstID)
        {
            var log = _logService.GetProcessLogByProcInstID(procInstID);

            if (log.Count == 1)
            {
                log = log.Where(r => r.OrigUserAccount.ToUpper() == this.UserName.ToUpper()).ToList();
            }

            return log.Count() == 1;
        }

        public bool IsEnableFormCopy(int procInstId)
        {
            var log = _logService.GetProcessLogByProcInstID(procInstId);

            var firstRecord = log.FirstOrDefault(r => r.CommentDate == log.Min(a => a.CommentDate) && r.OrigUserAccount.ToUpper() == this.UserName.ToUpper());

            return firstRecord != null;
        }

        private bool IsViewForm(int formId)
        {
            var processHeader = _storageProvider.GetProcessFormHeaderByFormId(formId);

            if (processHeader == null)
            {
                return false;
            }

            var procInstid = processHeader.ProcInstID;

            var processLog = _logService.GetProcessLogByProcInstID(procInstid.Value);

            var processes = processLog.Where(r => r.UserAccount.ToUpper() == this.UserName.ToUpper());

            return processes.Count() > 0;
        }

        private void GotoErrorPage()
        {
            var formId = this.GetFormId();
            if (IsViewForm(formId))
            {
                var url = string.Format("{0}?_FormId={1}", Request.Url.AbsoluteUri.Split('?').First(), formId);
                Response.Redirect(url);
            }
            else
            {
                Response.Redirect("/KStarFormAuditInvalid.html");
            }
        }

        public string Filter(string html, WorkMode workMode, IEnumerable<int> activityIds, int currActivityId, string processName)
        {
            IEnumerable<string> idList = new HashSet<string>();
            var formSettingProvider = new KStarFormSettingProvider();
            if (workMode == WorkMode.Review)
            {
                workMode = WorkMode.View;
            }

            if (formSettingProvider.IsCombinRights(processName))
            {
                foreach (var activityId in activityIds)
                {
                    var settingCount = formSettingProvider.GetControlSettings(processName, activityId, workMode).Count();

                    if (settingCount == 0)
                    {
                        continue;
                    }

                    var controlFilter = new HtmlControlFilter(html, workMode, processName);
                    controlFilter.Filter(activityId);

                    idList = idList.Union(controlFilter.GetShowIdList());
                }
            }

            var currFilter = new HtmlControlFilter(html, workMode, processName);
            currFilter.Filter(currActivityId, idList);

            return currFilter.Render();
        }

        #endregion

    }
}

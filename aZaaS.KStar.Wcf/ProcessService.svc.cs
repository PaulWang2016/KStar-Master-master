using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Configuration;

using aZaaS.KStar;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.ProcessForm;
using System.Reflection;
using aZaaS.KStar.Wcf.DataContracts;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.WorkflowData;
using System.Configuration;
using aZaaS.Kstar.DAL;
using System.Data;
using aZaaS.KStar.ParticipantSetService;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.WorkflowConfiguration;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ProcessLogService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ProcessLogService.svc or ProcessLogService.svc.cs at the Solution Explorer and start debugging.
    [ServiceErrorHandlerBehaviour]
    public class ProcessService : IProcessService
    {
        private const string OPERATION_FLAG = "+";
        private string _defaultSysAccount;
        private string _defaultSysAccountName;
        private string _defaultSkipActionName;
        private string _defaultK2ServiceAccount;

        private readonly ConfigManager _wfConfigManger;
        private readonly UserService _userService;
        private readonly ProcessLogService _logService;
        private readonly WorkflowManagementService _wfMgmtService;
        private readonly IFormCCProvider _formCCProvider;
        private readonly SignerRepository _signerRepository;
        private readonly IStorageProvider _storageProvider;
        // private string serverUrl = ConfigurationManager.AppSettings["ServerUrl"].ToString();

        public ProcessService()
        {
            _userService = new UserService();
            _logService = new ProcessLogService();

            var authType = AuthConfig.AuthType;
            _wfConfigManger = new ConfigManager(authType);
            _wfMgmtService = new WorkflowManagementService(authType);

            _formCCProvider = new KStarFormCCProvider();
            _signerRepository = new SignerRepository();
            _storageProvider = new KStarFormStorageProvider();
        }

        public string DefaultSystemAccount
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultSysAccount))
                {
                    _defaultSysAccount = ConfigurationManager.AppSettings["DefaultSystemAccount"] ?? "System";
                }

                return _defaultSysAccount;
            }
        }

        public string DefaultSystemAccountName
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultSysAccountName))
                {
                    _defaultSysAccountName = ConfigurationManager.AppSettings["DefaultSystemAccountName"] ?? "KStar";
                }

                return _defaultSysAccountName;
            }
        }

        public string DefaultSkipActionName
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultSkipActionName))
                {
                    _defaultSkipActionName = ConfigurationManager.AppSettings["DefaultSkipActionName"] ?? "SKIP";
                }

                return _defaultSkipActionName;
            }
        }

        public int SkipApprovalLogIndex
        {
            get
            {
                int skipLogIndex = 0;
                int.TryParse(ConfigurationManager.AppSettings["SkipApprovalLogIndex"], out skipLogIndex);

                return skipLogIndex;
            }
        }

        public string K2ServiceAccount
        {
            get
            {

                if (string.IsNullOrEmpty(_defaultK2ServiceAccount))
                {
                    _defaultK2ServiceAccount = ConfigurationManager.AppSettings["K2ServiceAccount"];
                }

                return _defaultK2ServiceAccount;
            }
        }

        public void RecordProcessLog(int procInstId, string processName, string taskOwner, string actionTaker, string activityName, string actionName, string comment = "",string post="")
        {

            if (procInstId <= 0)
                throw new ArgumentOutOfRangeException("procInstID");
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("processName");
            if (string.IsNullOrEmpty(taskOwner))
                throw new ArgumentNullException("taskOwner");
            if (string.IsNullOrEmpty(actionTaker))
                throw new ArgumentNullException("actionTaker");
            if (string.IsNullOrEmpty(activityName))
                throw new ArgumentNullException("activityName");
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");


            var ownerName = StringUtil.TrimUserName(taskOwner);
            var takerName = StringUtil.TrimUserName(actionTaker);

            var owner = _userService.ReadUserBase(ownerName);
            if (owner == null)
                throw new InvalidOperationException("The task owner was not found!");

            var taker = _userService.ReadUserBase(takerName);
            if (taker == null)
                throw new InvalidOperationException("The action taker was not found!");


            var processLog = new ProcessLog()
            {
                ProcInstID = procInstId,
                ActivityName = activityName,
                ActionName = actionName,
                ProcessName = processName,
                CommentDate = DateTime.Now,
                Comment = comment,
                OrigUserName = owner.FullName,
                OrigUserAccount = ownerName,
                UserName = taker.FullName,
                UserAccount = takerName,
                SN = post
            };

            _logService.AddProcessLog(processLog);

        }

        public IEnumerable<ProcessLogData> GetProcessLogList(int procInstId)
        {

            if (procInstId <= 0)
                throw new ArgumentOutOfRangeException("procInstID");

            var items = _logService.GetProcessLogByProcInstID(procInstId);

            if (items == null || items.Count == 0)
                return new List<ProcessLogData>();

            return items.OrderBy(item => item.CommentDate)
                        .Select(item => new ProcessLogData()
                        {
                            ProcInstID = item.ProcInstID,
                            ProcessName = item.ProcessName,
                            TaskOwner = item.OrigUserAccount,
                            TaskOwnerName = item.OrigUserName,
                            ActionTaker = item.UserAccount,
                            ActionTakerName = item.UserName,
                            ActivityName = item.ActivityName,
                            ActionName = item.ActionName,
                            CreatedDate = item.CommentDate,
                            Comment = item.Comment
                        });
        }

        public IEnumerable<string> GetActivityParticipants(int procInstId, string activityName)
        { 
            //var procId = GetProcessInstanceProcId(procInstId);
            var procInst = GetProcessInstance(procInstId);
            var procId = procInst.ProcID;

            IEnumerable<string> participants = new HashSet<string>();

            // Apply template
             ActivityParticipantSetService activityParticipantSetService = new ActivityParticipantSetService();
             activityParticipantSetService.ApplyProcessActivity(procInst.FullName, procInst.ID, activityName,addOriginalParticipants);

             if (activityParticipantSetService.HasUnPeekedProcessActivityParticipantSet(procInst.ID, activityName))
             { 
                 participants = activityParticipantSetService.PopParticipants(procInst.ID, activityName, ResolveParticipants);
             }
             else
             {
                 //Gets the Org users
                 participants = _wfConfigManger.GetActivityConfiguredGeneralParticipants(procId, activityName);
                 //TODO: Resolves the CustomRole users & merge with participants 
                 var roleParticipants = GetActivityConfiguredCustomRoleParticipants(procId, procInstId, activityName, procInst.FullName);
                 participants = participants.Union(roleParticipants);

             }
              
            //Skip the approvers that have been approved.
            var procConfig = GetProcessConfigByProcInstId(procInstId);
            if (procConfig != null && procConfig.NotAssignIfApproved)
            {
                participants = SkipActivityTakenParticipants(procInstId, activityName, participants);
            }

            //Get signers that added to current acitivty
            //TODO:Move the logic to KStarFormWorkflowService as api.
            var signParticipants = new HashSet<string>();
            var signers = _signerRepository.GetPendingSigners(procInstId, activityName);
            if (signers != null && signers.Any())
            {
                signers.ForEach(s =>
                {
                    signParticipants.Add(s.SignerName);
                    signParticipants.Add(s.UserName);
                });
                participants = RemoveActivityTakenParticipants(procInstId, activityName, participants);
            }

            participants = participants.Union(signParticipants);
            #region Larry于16年1月14号修改，增加判断越秀类型的同一审批人跳过方法（即以当前审批人的最大权限去审批） ，
            var UserNameList = participants.ToList();

            var plength = UserNameList.Count();

            for (int i = 0; i < plength; i++)
            {

                var exist = CalcSkipActivityTakenParticipants(procInstId, activityName, UserNameList[i]);
                if (exist)
                {
                    UserNameList[i] = K2ServiceAccount;
                }
            }
            #endregion
            #region Larry于11月18号修改，增加判断如果无审批人就自动通过该节点
            if (participants.Count() == 0)
            {
                List<string> list = new List<string>();
                list.Add(K2ServiceAccount);
                participants=participants.Union(list);
            }
            #endregion
            return participants;
        }


        /// <summary>
        /// 获取当前实例环节参与的审批人
        /// </summary>
        /// <param name="procInstId"></param>
        /// <param name="activityName"></param>
        /// <returns></returns>
        public IEnumerable<string> GetActivityNotPopParticipants(int procInstId, string activityName)
        {
            //var procId = GetProcessInstanceProcId(procInstId);
            var procInst = GetProcessInstance(procInstId);
            var procId = procInst.ProcID;

            IEnumerable<string> participants = new HashSet<string>();

            // Apply template
            ActivityParticipantSetService activityParticipantSetService = new ActivityParticipantSetService();
            activityParticipantSetService.ApplyProcessActivity(procInst.FullName, procInst.ID, activityName, addOriginalParticipants,false);

            if (activityParticipantSetService.HasUnPeekedProcessActivityParticipantSet(procInst.ID, activityName))
            {
                participants = activityParticipantSetService.GetParticipants(procInst.ID, activityName, ResolveParticipants);
            }
            else
            {
                //Gets the Org users
                participants = _wfConfigManger.GetActivityConfiguredGeneralParticipants(procId, activityName);
                //TODO: Resolves the CustomRole users & merge with participants 
                var roleParticipants = GetActivityConfiguredCustomRoleParticipants(procId, procInstId, activityName, procInst.FullName);
                participants = participants.Union(roleParticipants);

            }

            //Skip the approvers that have been approved.
            var procConfig = GetProcessConfigByProcInstId(procInstId);
            if (procConfig != null && procConfig.NotAssignIfApproved)
            {
                participants = SkipActivityTakenParticipants(procInstId, activityName, participants);
            }

            //Get signers that added to current acitivty
            //TODO:Move the logic to KStarFormWorkflowService as api.
            var signParticipants = new HashSet<string>();
            var signers = _signerRepository.GetPendingSigners(procInstId, activityName);
            if (signers != null && signers.Any())
            {
                signers.ForEach(s =>
                {
                    signParticipants.Add(s.SignerName);
                    signParticipants.Add(s.UserName);
                });
                participants = RemoveActivityTakenParticipants(procInstId, activityName, participants);
            }

            participants = participants.Union(signParticipants);
            #region Larry于16年1月14号修改，增加判断越秀类型的同一审批人跳过方法（即以当前审批人的最大权限去审批） ，
            var UserNameList = participants.ToList();

            var plength = UserNameList.Count();

            for (int i = 0; i < plength; i++)
            {

                var exist = CalcSkipActivityTakenParticipants(procInstId, activityName, UserNameList[i]);
                if (exist)
                {
                    UserNameList[i] = K2ServiceAccount;
                }
            }
            #endregion
            #region Larry于11月18号修改，增加判断如果无审批人就自动通过该节点
            if (UserNameList.Count() == 0)
            {
                List<string> list = new List<string>();
                list.Add(K2ServiceAccount);
                UserNameList.Union(list);
            }
            #endregion
            return UserNameList;
        }

        private bool CalcSkipActivityTakenParticipants(int procInstId, string activityName, string participants)
        {
            //KStarDbContext
            using (KStarFramekWorkDbContext dbContext = new KStarFramekWorkDbContext())
            {
                var entity = dbContext.ProcessPrognosis.Where(x => x.SourceName == activityName && x.ProcInstID == procInstId).FirstOrDefault();
                if (entity == null) return false;
                var linq = from ps in dbContext.ProcessPrognosis
                           join psd in dbContext.ProcessPrognosisDetail on ps.SysID equals psd.RSysID
                           into joinTemp
                           from tmp in joinTemp.DefaultIfEmpty()
                           where ps.ProcInstID == procInstId && ps.LinkOrder >= entity.LinkOrder && tmp.UserName==participants
                           orderby ps.LinkOrder ascending
                           select tmp.UserName;
                return linq.Count() > 0;
            }
        }
        /// <summary>
        /// 添加原始配置元素
        /// </summary>
        /// <param name="procInstId"></param>
        /// <param name="activityName"></param>
        /// <param name="db"></param>
        private void addOriginalParticipants(int procInstId, string activityName, KStarFramekWorkDbContext db, ProcessActivityParticipantSet parameter)
        {
            var procInst = GetProcessInstance(procInstId);
            var participants = _wfConfigManger.GetActivityConfigParticipants(procInst.ProcID, activityName);

            string itemString = Newtonsoft.Json.JsonConvert.SerializeObject(parameter);
            var itemEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessActivityParticipantSet>(itemString);
            itemEntity.IsOriginal = true;
            itemEntity.Priority = int.MaxValue;
            itemEntity.SetID = Guid.NewGuid();
            itemEntity.DateAssigned = DateTime.Now;
            //添加环节
            db.ProcessActivityParticipantSet.Add(itemEntity);
            //添加人

            foreach (var _entry in participants)
            {
                db.ProcessActivityParticipantSetEntry.Add(new ProcessActivityParticipantSetEntry() { SetID = itemEntity.SetID, EntryType = _entry.UserType, EntryName = _entry.Value, EntryID = new Guid(_entry.Key) });
            }
        }

        /// <summary>
        /// 解析人员
        /// </summary>
        /// <param name="procInstId"></param>
        /// <param name="activityName"></param>
        /// <param name="ParticipantSetEntry"></param>
        /// <param name="Participant"></param>
        private void ResolveParticipants(int procInstId, string activityName, List<ProcessActivityParticipantSetEntry> ParticipantSetEntry, List<string> Participant)
        {
            IEnumerable<string> _participants = new HashSet<string>();

            var procInst = GetProcessInstance(procInstId);
            var procId = procInst.ProcID;
            //解析
            foreach (var item in ParticipantSetEntry)
            {
                //  item.EntryID
                var plist = _wfConfigManger.ResolveParticipants(item.EntryType, item.EntryID.Value);

                _participants = _participants.Union(plist);
                //解析自定义角色

                if (item.EntryType == "CustomType")
                {
                    var context = new CustomRoleContext();
                    var formHeader = _storageProvider.GetProcessFormHeaderByProcInstId(procInstId);
                    var formId = 0;
                    if (formHeader != null)
                    {
                        formId = formHeader.FormID;
                    }

                    var formContext = _storageProvider.GetProcessFormContent(formHeader.FormID);

                    context.Key = item.EntryID.Value;
                    context.ProcInstID = procInstId;
                    context.Requester = formHeader.ApplicantAccount;
                    context.FormInfo = GetFormInfo(formHeader);
                    context.ProcessFullName = procInst.FullName;
                    context.ActivityName = activityName;
                    context.FormId = formId;
                    context.ProcessFormHeader = formHeader;
                    context.ProcessFormContent = formContext;

                    var customRole = CustomRoleManager.Current.GetService(item.EntryID.Value);
                    if (customRole != null)
                        _participants = _participants.Union(customRole.Execute(context));

                } 
            } 
            foreach (var item in _participants)
            {
                Participant.Add(item);
            } 
        }

        public bool HasAttachActivityParticipant(int procInstId, string activityName)
        {
            var procInst = GetProcessInstance(procInstId);
          
            // Apply template
            ActivityParticipantSetService activityParticipantSetService = new ActivityParticipantSetService();
           // activityParticipantSetService.ApplyProcessActivity(procInst.FullName, procInst.ID, procInst.ActivityName);

            if (activityParticipantSetService.HasUnPeekedProcessActivityParticipantSet(procInst.ID, activityName))
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetActivityParticipantEmail(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");

            var user = _userService.ReadUserBase(StringUtil.TrimUserName(userName));
            if (user == null)
                throw new InvalidOperationException(string.Format("The user: {0} was not found!"));

            return user.Email;
        }

        public string GetActivityParticipantEmails(int procInstId, string activityName)
        {
            var procInst = GetProcessInstance(procInstId);
            var procId = procInst.ProcID;
            var participantEmails = new HashSet<string>();

            //Get the Org user emails
            var participants = _wfConfigManger.GetActivityConfiguredGeneralParticipants(procId, activityName);
            foreach (var userName in participants)
            {
                var user = _userService.ReadUserBase(userName);
                if (user == null)
                    continue;

                participantEmails.Add(user.Email);
            }

            //TODO: Resolves the CustomRole user mails & merge with participantEmails 
            var roleParticipants = GetActivityConfiguredCustomRoleParticipants(procId, procInstId, activityName, procInst.FullName);
            foreach (var userName in roleParticipants)
            {
                var user = _userService.ReadUserBase(userName);
                if (user == null)
                    continue;

                participantEmails.Add(user.Email);
            }

            return string.Join(";", participantEmails.ToArray());
        }

        public string GetActivityParticipantCCEmails(int procInstId, string activityName)
        {
            return string.Empty;
        }

        private int GetProcessInstanceProcId(int procInstId)
        {
            var inst = _wfMgmtService.GetProcessInstance(procInstId);
            if (inst == null)
                throw new InvalidOperationException(string.Format("The process instance: {0} was not found!", procInstId));

            return inst.ProcID;
        }

        private ProcessInstance GetProcessInstance(int procInstId)
        {
            var inst = _wfMgmtService.GetProcessInstance(procInstId);
            if (inst == null)
                throw new InvalidOperationException(string.Format("The process instance: {0} was not found!", procInstId));

            return inst;
        }

        private IEnumerable<string> GetActivityConfiguredCustomRoleParticipants(int procId, int procInstId, string activityName, string processName)
        {
            IEnumerable<string> participants = new HashSet<string>();
            var roleKeys = _wfConfigManger.GetActivityConfiguredCustomRoles(procId, activityName);

            foreach (var roleKey in roleKeys)
            {
                var context = new CustomRoleContext();
                var formHeader = _storageProvider.GetProcessFormHeaderByProcInstId(procInstId);
                var formId = 0;
                if (formHeader != null)
                {
                    formId = formHeader.FormID;
                }

                var formContext = _storageProvider.GetProcessFormContent(formHeader.FormID);

                context.Key = roleKey;
                context.ProcInstID = procInstId;
                context.Requester = formHeader.ApplicantAccount;
                context.FormInfo = GetFormInfo(formHeader);
                context.ProcessFullName = processName;
                context.ActivityName = activityName;
                context.FormId = formId;
                context.ProcessFormHeader = formHeader;
                context.ProcessFormContent = formContext;

                var customRole = CustomRoleManager.Current.GetService(roleKey);

                participants = participants.Union(customRole.Execute(context));
            }

            return participants;
        }

        private IEnumerable<string> GetActivityConfiguredCustomRoleParticipants(int procId, int procInstId, string activityName, string processName, string refType, string operateType)
        {
            IEnumerable<string> participants = new HashSet<string>();
            var roleKeys = _wfConfigManger.GetActivityConfiguredCustomRoles(procId, activityName, refType, operateType);

            foreach (var roleKey in roleKeys)
            {
                var context = new CustomRoleContext();
                var formHeader = _storageProvider.GetProcessFormHeaderByProcInstId(procInstId);
                var formId = 0;
                if (formHeader != null)
                {
                    formId = formHeader.FormID;
                }

                var formContext = _storageProvider.GetProcessFormContent(formHeader.FormID);

                context.Key = roleKey;
                context.ProcInstID = procInstId;
                context.Requester = formHeader.ApplicantAccount;
                context.FormInfo = GetFormInfo(formHeader);
                context.ProcessFullName = processName;
                context.ActivityName = activityName;
                context.FormId = formId;
                context.ProcessFormHeader = formHeader;
                context.ProcessFormContent = formContext;

                var customRole = CustomRoleManager.Current.GetService(roleKey);

                participants = participants.Union(customRole.Execute(context));
            }

            return participants;
        }

        private IEnumerable<string> SkipActivityTakenParticipants(int procInstId, string activityName, IEnumerable<string> participants)
        {
            var passParticipants = new List<string>();
            var ignoreParicipants = new List<string>();

            var approvalLogs = _logService.GetProcessLogByProcInstID(procInstId);
            if (approvalLogs == null || !approvalLogs.Any()) { return participants; }
            var processLog = approvalLogs.Where(p => p.ActionName == "回退到" || p.ActionName == "拒绝").OrderByDescending(p=>p.CommentDate).ToList();
            if (processLog != null && processLog.Count()>0)
            {
                var date = processLog[0].CommentDate;
                approvalLogs = approvalLogs.Where(p => p.CommentDate > date).ToList();
            }

            if (SkipApprovalLogIndex > 0)
            {   //Maybe the first item is submit log,we should remove it from the list.
                approvalLogs = approvalLogs.OrderBy(l => l.CommentDate).ToList();
                approvalLogs.RemoveRange(0, SkipApprovalLogIndex);
            }

            if (!approvalLogs.Any()) { return participants; }

            foreach (var userName in participants)
            {
                bool isExists = approvalLogs.Any(l => l.UserAccount.Equals(userName, StringComparison.OrdinalIgnoreCase)
                                            || l.OrigUserAccount.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (!isExists) //TODO: Checking if activity returnable activity, the approved user can be passed.
                {
                    passParticipants.Add(userName);
                }
                else
                {
                    ignoreParicipants.Add(userName);

                    //var comment = string.Format("{0} will be skipped for activity [{1}]",userName,activityName);
                    RecordOperationLog(procInstId, "", userName, DefaultSystemAccount, activityName, DefaultSkipActionName, DefaultSkipActionName);
                }
            }

            //NOTE: #ignoreParicipants.Any() == skip mode
            //If on skip mode with no destinations,we add [K2Service Account] as destination
            if (!passParticipants.Any() && ignoreParicipants.Any())
                passParticipants.Add(K2ServiceAccount);


            return passParticipants;
        }

        private IEnumerable<string> RemoveActivityTakenParticipants(int procInstId, string activityName, IEnumerable<string> participants)
        {
            //NOTE:
            //Checking whether the participant has been taken action to the task(activity),
            //If yes,we would delete the spepcified participant from the participant list.

            var tempParticipants = new List<string>();

            var criteria = new PageCriteria() { PageSize = int.MaxValue };
            criteria.RegularFilters = new List<RegularFilter>()
            {
                 new RegularFilter(CriteriaLogical.And,"ProcInstID", CriteriaCompare.Equal,procInstId),
                 new RegularFilter(CriteriaLogical.And,"ActivityName", CriteriaCompare.Equal,activityName),
            };

            var approvalLogs = _logService.GetProcessLogs(criteria);
            if (approvalLogs == null || approvalLogs.Count == 0)
                return participants;

            foreach (var userName in participants)
            {
                bool isExists = approvalLogs.Any(l => l.UserAccount.Equals(userName, StringComparison.OrdinalIgnoreCase)
                                            || l.OrigUserAccount.Equals(userName, StringComparison.OrdinalIgnoreCase));
                if (!isExists)
                    tempParticipants.Add(userName);
            }

            return tempParticipants;
        }

        private Dictionary<string, string> GetFormInfo(aZaaS.KStar.Form.Models.ProcessFormHeader formHeader)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (formHeader != null)
            {
                foreach (PropertyInfo property in formHeader.GetType().GetProperties())
                {
                    var obj = property.GetValue(formHeader);

                    if (obj != null)
                    {
                        dic.Add(property.Name, obj.ToString());
                    }
                }
            }

            return dic;
        }

        public ProcessSetInfo GetProcessConfigByFullName(string fullname)
        {
            ProcessSetInfo result = new ProcessSetInfo();
            var process = _wfConfigManger.GetProcessSetByFullName(string.Empty, fullname, false);
            if (process != null)
            {
                result.ID = process.ID;
                result.ProcessSetID = process.ProcessSetID;
                result.Configuration_CategoryID = process.Configuration_CategoryID;
                result.ProcessSetNo = process.ProcessSetNo;
                result.ProcessFullName = process.ProcessFullName;
                result.ProcessName = process.ProcessName;
                result.OrderNo = process.OrderNo;
                result.StartUrl = process.StartUrl;
                result.ViewUrl = process.ViewUrl;
                result.ApproveUrl = process.ApproveUrl;
                result.NotAssignIfApproved = process.NotAssignIfApproved;
                result.Description = process.Description;
                result.IsCommon = process.IsCommon;
            }
            return result;
        }

        public ProcessSetInfo GetProcessConfigByProcInstId(int procInstId)
        {
            ProcessSetInfo result = new ProcessSetInfo();

            var procInst = _wfMgmtService.GetProcessInstance(procInstId);
            if (procInst == null)
                throw new InvalidOperationException(string.Format("The process instance:{0} was not found!", procInst));

            var process = _wfConfigManger.GetProcessSetByFullName(string.Empty, procInst.FullName, false);
            if (process != null)
            {
                result.ID = process.ID;
                result.ProcessSetID = process.ProcessSetID;
                result.Configuration_CategoryID = process.Configuration_CategoryID;
                result.ProcessSetNo = process.ProcessSetNo;
                result.ProcessFullName = process.ProcessFullName;
                result.ProcessName = process.ProcessName;
                result.OrderNo = process.OrderNo;
                result.StartUrl = process.StartUrl;
                result.ViewUrl = process.ViewUrl;
                result.ApproveUrl = process.ApproveUrl;
                result.NotAssignIfApproved = process.NotAssignIfApproved;
                result.Description = process.Description;
                result.IsCommon = process.IsCommon;
            }
            return result;
        }


        private void RecordOperationLog(int procInstId, string processName, string taskOwner, string actionTaker, string activityName, string actionName, string comment = "")
        {

            if (procInstId <= 0)
                throw new ArgumentOutOfRangeException("procInstID");
            if (string.IsNullOrEmpty(taskOwner))
                throw new ArgumentNullException("taskOwner");
            if (string.IsNullOrEmpty(actionTaker))
                throw new ArgumentNullException("actionTaker");
            if (string.IsNullOrEmpty(activityName))
                throw new ArgumentNullException("activityName");
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");

            string ownerName = string.Empty, ownerDisplayName = string.Empty;
            string takerName = string.Empty, takerDisplayName = string.Empty;

            if (taskOwner.Equals(DefaultSystemAccount, StringComparison.OrdinalIgnoreCase))
            {
                ownerName = DefaultSystemAccount;
                ownerDisplayName = DefaultSystemAccountName;
            }
            else
            {
                ownerName = StringUtil.TrimUserName(taskOwner);
                var owner = _userService.ReadUserBase(ownerName);
                if (owner == null)
                    throw new InvalidOperationException("The task owner was not found!");

                ownerDisplayName = owner.FullName;
                ownerName = string.Format("{0}{1}", OPERATION_FLAG, ownerName);
            }

            if (actionTaker.Equals(DefaultSystemAccount, StringComparison.OrdinalIgnoreCase))
            {
                takerName = DefaultSystemAccount;
                takerDisplayName = DefaultSystemAccountName;
            }
            else
            {
                takerName = StringUtil.TrimUserName(actionTaker);
                var taker = _userService.ReadUserBase(takerName);
                if (taker == null)
                    throw new InvalidOperationException("The action taker was not found!");

                takerDisplayName = taker.FullName;
                takerName = string.Format("{0}{1}", OPERATION_FLAG, takerName);
            }


            var processLog = new ProcessLog()
            {
                ProcInstID = procInstId,
                ActivityName = activityName,
                ActionName = actionName,
                ProcessName = processName,
                CommentDate = DateTime.Now,
                Comment = comment,
                OrigUserName = ownerDisplayName,
                OrigUserAccount = ownerName,
                UserName = takerDisplayName,
                UserAccount = takerName
            };

            _logService.AddProcessLog(processLog);

        }

        public void SaveKStarFormEndCCUser(int procInstId, string activityName, string comment)
        {
            var procInst = _wfMgmtService.GetProcessInstance(procInstId);
            if (procInst == null)
                throw new InvalidOperationException(string.Format("The process instance:{0} was not found!", procInst));

            int activityId = _wfConfigManger.GetActivityIdFromServer(procInst.ProcID, activityName);

            IStorageProvider formStorage = new KStarFormStorageProvider();
            var Header = formStorage.GetProcessFormHeaderByProcInstId(procInstId);

            var process = _wfConfigManger.GetProcessSetByFullName(string.Empty, procInst.FullName, false);
            var viewUrl = string.Format("{0}?_FormId={1}&ActivityId={2}", process.ViewUrl.Split('?').First(), Header.FormID, activityId);
            //var ccUserList = process.EndCc;
            var table = CustomExtUtility.GetProcessReminderInfo(Header.ProcInstID.ToString());
            var refType = "ProcessSet";
            var operateType = "EndCc";
            //Gets the Org users
            var participants = _wfConfigManger.GetActivityConfiguredGeneralParticipants(procInst.ProcID, procInstId, activityName, refType, operateType);


            //TODO: Resolves the CustomRole users & merge with participants 
            var roleParticipants = GetActivityConfiguredCustomRoleParticipants(procInst.ProcID, procInstId, activityName, procInst.FullName, refType, operateType);
            participants = participants.Union(roleParticipants);
            //IOrganizationService v = new KStarFormOrganizationService();

            var ccUsers = new List<ProcessFormCC>();

            participants.ToList().ForEach(user =>
            {
                ccUsers.Add(new ProcessFormCC()
                {
                    FormId = Header.FormID,
                    Originator = DefaultSystemAccount,
                    OriginatorName = DefaultSystemAccountName,
                    Receiver = user,
                    ReceiverName = _userService.GetUsersDisplayNameByUserName(new List<string>() { user })[0],
                    FormViewUrl = viewUrl,
                    ReceiverStatus = false,
                    CreatedDate = DateTime.Now,
                    ActivityId = activityId,
                    ActivityName = activityName,
                    Comment = comment
                });
            });

            _formCCProvider.Save(ccUsers);


            try
            {
                var url = string.Empty;
                if (table.Tables.Count > 1)
                {
                    url = table.Tables[1].Rows[0][0].ToString();
                }
                foreach (var s in participants)
                {
                    try
                    {
                        if (table != null && table.Tables.Count > 0 && table.Tables[0].Rows.Count > 0)
                        {
                            var row = table.Tables[0].Rows[0];

                            var userEmail = _userService.GetUserMailAddress(s);
                            HashSet<string> hsUser = new HashSet<string>();
                            hsUser.Add(s);
                            viewUrl = url + "?strUrl=" + viewUrl + "&FromMail=1&authorname=" + s;
                            var DisplayName = _userService.GetUsersDisplayNameByUserName(hsUser.ToList())[0];
                            string title = GenerateEmailTitle(row);
                            if (!string.IsNullOrEmpty(userEmail))
                            {
                                string body = GenerateEmailFormat(row, viewUrl, DisplayName);

                                CustomExtUtility.InsertReminderRecord(title, body, userEmail);
                            }
                            else
                            {
                                LogFactory.GetLogger().Write(new LogEvent
                                {
                                    Source = "结束邮件抄送遍历",
                                    Message = "用户：【" + s + "】 没有邮件地址",
                                    Exception = null,
                                    OccurTime = DateTime.Now
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFactory.GetLogger().Write(new LogEvent
                        {
                            Source = "结束邮件抄送遍历",
                            Message = "用户：【" + s + "】" + ex.Message,
                            Exception = ex,
                            OccurTime = DateTime.Now
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogFactory.GetLogger().Write(new LogEvent
                {
                    Source = "结束邮件抄送主体",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
            }
        }

        public void SaveKStarFormReworkCCUser(int procInstId, string activityName, string comment)
        {
            var procInst = _wfMgmtService.GetProcessInstance(procInstId);
            if (procInst == null)
                throw new InvalidOperationException(string.Format("The process instance:{0} was not found!", procInst));

            int activityId = _wfConfigManger.GetActivityIdFromServer(procInst.ProcID, activityName);

            IStorageProvider formStorage = new KStarFormStorageProvider();
            var formId = formStorage.GetFormId(procInstId);

            var process = _wfConfigManger.GetProcessSetByFullName(string.Empty, procInst.FullName, false);
            //var user = new OrgNodeDTO().
            var originator = procInst.Originator;
            var viewUrl = string.Format("{0}?_FormId={1}&ActivityId={2}", process.ViewUrl.Split('?').First(), formId, activityId);
            //var ccUserList = process.ReworkCc;

            var refType = "ProcessSet";
            var operateType = "ReworkCc";
            //Gets the Org users
            var participants = _wfConfigManger.GetActivityConfiguredGeneralParticipants(procInst.ProcID, procInstId, activityName, refType, operateType);

            //TODO: Resolves the CustomRole users & merge with participants 
            var roleParticipants = GetActivityConfiguredCustomRoleParticipants(procInst.ProcID, procInstId, activityName, procInst.FullName, refType, operateType);
            participants = participants.Union(roleParticipants);

            var ccUsers = new List<ProcessFormCC>();

            participants.ToList().ForEach(user =>
            {
                ccUsers.Add(new ProcessFormCC()
                {
                    FormId = formId,
                    Originator = DefaultSystemAccount,
                    OriginatorName = DefaultSystemAccountName,
                    Receiver = user,
                    ReceiverName = _userService.GetUsersDisplayNameByUserName(new List<string>() { user })[0],
                    FormViewUrl = viewUrl,
                    ReceiverStatus = false,
                    CreatedDate = DateTime.Now,
                    ActivityId = activityId,
                    ActivityName = activityName,
                    Comment = comment
                });
            });

            _formCCProvider.Save(ccUsers);
        }
        public string GenerateEmailTitle(DataRow row)
        {
            string Subject = row["subject"].ToString();
            string Title = "审阅：流程中心 - {0}({1})发起的【{2}】需要您审阅";
            string startUser = row["Originator"].ToString().Replace("K2:", "");
            if (!string.IsNullOrEmpty(startUser) && startUser.IndexOf(@"\") > -1)
            {
                startUser = startUser.Substring(startUser.LastIndexOf(@"\") + 1);
            }
            Title = string.Format(Title, row["StartName"].ToString(), startUser, row["ProcessName"].ToString());
            if (!string.IsNullOrEmpty(row["subject"].ToString()))
            {
                Title = row["subject"].ToString().Replace("审批", "审阅");
                Title = Title.Replace("待办：", "审阅：");
            }
            return Title;
        }
        public string GenerateEmailFormat(DataRow row, string transferUrl, string displayName)
        {

            StringBuilder strContext = new StringBuilder("<span style='font-family:Arial sans-serif ;color:#1F497D;'>Dear," + displayName + "<br/><br/>您在流程中心中有一个<span style='color:red'>审阅</span>事项。 ");
            strContext.Append(" <a href='" + transferUrl + "'>请点击这里打开处理</a> <br/>");
            strContext.Append("以下是该事项的简要内容：</span><br/>");

            strContext.Append("<table style='border:none;border-collapse:collapse' cellspacing='0' cellpadding='0'><tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>");
            strContext.Append("流程名称:</td><td>&nbsp;</td><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>" + row["ProcessName"].ToString() + "</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>流程实例号:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>" + row["Folio"].ToString().Split('-').Last() + "</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>抄送环节名称:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>结束</td></tr>");

            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请人:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>" + row["StartName"].ToString() + "</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请时间:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>" + Convert.ToDateTime(row["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "</td></tr>");
            strContext.Append("</table>");
            strContext.Append("<br/><span style='font-family:Arial sans-serif ;color:#1F497D;'>温馨提示：此邮件由系统自动发出，请不要答复该邮件。若对流程内容有疑问，请与发起人联系，若有技术问题，请与流程程架构部（peng.fuze@innos.com）联系。 </span>");

            return strContext.ToString();

        }


        public bool LienRule(int procInstId, string lienName, string activityName, string action)
        {
            //1、获取目标actname
            var procInst = GetProcessInstance(procInstId);
            string ruleString = _wfConfigManger.GetLienRule(procInstId, lienName, procInst.FullName, activityName);
            if (!string.IsNullOrWhiteSpace(ruleString))
            {
                try
                {
                    return _wfConfigManger.ExecuteLienRule(procInstId, activityName,procInst.FullName, action, ruleString);
                }
                catch (Exception ex)
                {
                    LogFactory.GetLogger().Write(new LogEvent
                    {
                        Source = string.Format("LienRule procInstId：{0},LienName：{1}", procInstId, lienName),
                        Message = ex.Message,
                        Exception = ex,
                        OccurTime = DateTime.Now
                    });
                } 
            }
            return true;
        }

        /// <summary>
        /// 流程结束
        /// </summary>
        /// <param name="procInstId"></param>
        public void ProcessFinish(int procInstId)
        {
            try
            {
                using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
                {
                    var list = db.ProcessPrognosis.Where(x => x.ProcInstID == procInstId).ToList();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            db.ProcessPrognosis.Remove(item);
                            var listDetail = db.ProcessPrognosisDetail.Where(x => x.RSysID == item.SysID).ToList();
                            if (listDetail != null)
                                db.ProcessPrognosisDetail.RemoveRange(listDetail);
                        } 
                        db.SaveChanges();
                    }
                        
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}

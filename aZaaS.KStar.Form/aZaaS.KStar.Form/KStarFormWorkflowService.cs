using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Repositories;
 
namespace aZaaS.KStar.Form
{ 
    public class KStarFormWorkflowService : IWorkflowService
    {
        private readonly WorkflowClientService _wfClientService;
        private readonly WorkflowManagementService _wfManagementService; 

        private IExecutionBehavior _executionBehavior;
        private readonly SignerRepository _signerRepository;
        private readonly ProcessLogService _processLogService;

        public KStarFormWorkflowService(AuthenticationType authtype)
        {
            _wfClientService = new WorkflowClientService(authtype);
            _wfManagementService = new WorkflowManagementService(authtype);
            _signerRepository = new SignerRepository();
            _processLogService = new ProcessLogService();
        }

        public KStarFormWorkflowService(AuthenticationType authtype, IExecutionBehavior executionBehavior)
        {
            _wfClientService = new WorkflowClientService(authtype);
            _wfManagementService = new WorkflowManagementService(authtype);
            _executionBehavior = executionBehavior;
            _signerRepository = new SignerRepository();
            _processLogService = new ProcessLogService();
        }

        public int StarNewTask(string userName, string processName, string processFolio, Dictionary<string, object> datafields)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("processName");
            if (processFolio == null)
                throw new ArgumentNullException("processFolio");

            return _wfClientService.StartProcessInstance(userName, processName, processFolio, datafields);
        }
        public void UpdateProcessFolio(int procInstID, string folio)
        {
            _wfClientService.UpdateProcessFolio(procInstID, folio);
        }
        public WorkflowTaskModel GetTaskItem(string userName, string shareUser, string serialNo)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");

            //List<WorklistItem> worklists = _wfClientService.GetWorklistItems(userName, PlatformType.ASP);
            //WorklistItem workItem = _wfClientService.OpenWorklistItem(userName,serialNo);
            WorklistItem workItem = _wfClientService.OpenWorklistItem(userName, shareUser, serialNo, PlatformType.ASP, false);
            if (workItem != null)
            {
                var actions = new List<string>();
                workItem.Actions.ToList().ForEach(act => actions.Add(act.Name));

                var taskDataFields = new Dictionary<string, object>();
                foreach (var item in workItem.ProcessInstance.DataFields)
                {
                    if (!taskDataFields.ContainsKey(item.Key))
                        taskDataFields.Add(item.Key, item.Value.Value);
                }

                var taskActivityDataFields = new Dictionary<string, object>();
                foreach (var item in workItem.ActivityDataFields)
                {
                    if (!taskActivityDataFields.ContainsKey(item.Key))
                        taskActivityDataFields.Add(item.Key, item.Value);
                }

                //TODO:Includes DataFields etc..
                return new WorkflowTaskModel()
                {
                    Folio = workItem.Folio,
                    ProcInstId = workItem.ProcInstID,
                    ActivityId = workItem.ActID,
                    ActivityName = workItem.ActivityName,
                    AssignedDate = workItem.AssignedDate,
                    Data = workItem.Data,
                    Destination = workItem.Destination,
                    Actions = actions,
                    SerialNo = workItem.SN,
                    DataFields = taskDataFields,
                    ActivityDataFields = taskActivityDataFields
                };
            }
            return null;
        }

        public IEnumerable<string> GetTaskActions(string userName, string serialNo)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");

            var actions = new List<string>();

            //var wlitem = _wfClientService.OpenWorklistItem(userName, serialNo);
            var wlitem = _wfClientService.OpenWorklistItem(userName, serialNo, PlatformType.ASP, false);
            wlitem.Actions.ForEach(act => actions.Add(act.Name));

            return actions;
        }

        public void ActionTask(string userName, string serialNo, Dictionary<string, object> datafields, Dictionary<string, object> actDatafields, string actionName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");

            var sDatafield = GetDatafieldNameValues(datafields);
            var sActDatafield = GetDatafieldNameValues(actDatafields);

            _wfClientService.ExecuteAction3(userName, serialNo, sDatafield, sActDatafield, actionName);
        }

        public void ActionTask(string userName, string serialNo, string shareUser, Dictionary<string, object> datafields, Dictionary<string, object> actDatafields, string actionName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");

            var sDatafield = GetDatafieldNameValues(datafields);
            var sActDatafield = GetDatafieldNameValues(actDatafields);

            _wfClientService.ExecuteAction5(userName, serialNo, shareUser, sDatafield, sActDatafield, actionName);
        }

        public void Delegate(string userName, string serialNo, IEnumerable<string> userNames)
        {
            if (userNames == null || userNames.Count() == 0)
                throw new ArgumentNullException("userNames");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");

            _wfClientService.Delegate(userName, serialNo, userNames.ToList());
        }

        public void Redirect(string userName, string serialNo, string touserName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");

            _wfClientService.Redirect2(userName, serialNo, touserName);
        }

        public void AddSigner(string userName, string serialNo, IEnumerable<string> userNames)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");
            if (userNames == null || userNames.Count() == 0)
                throw new ArgumentNullException("userNames");

            //WorklistItem workitem = _wfClientService.OpenWorklistItem(userName, serialNo);
            WorklistItem workitem = _wfClientService.OpenWorklistItem(userName, serialNo, PlatformType.ASP, false);

            if (workitem != null)
            {
                List<ProcessFormSigner> signers = new List<ProcessFormSigner>();
                foreach (var name in userNames)
                {
                    signers.Add(new ProcessFormSigner()
                    {
                        ActivityName = workitem.ActivityName,
                        ProcessInstId = workitem.ProcInstID,
                        UserName = userName,
                        SignerName = name,
                        SignerDate = DateTime.Now
                    });
                }
                _signerRepository.AddSigner(signers);
            }
        }

        public void GotoActivity(string userName, string serialNo, string activityName)
        {
            if (string.IsNullOrEmpty(activityName))
                throw new ArgumentNullException("activityName");
            if (string.IsNullOrEmpty(serialNo))
                throw new ArgumentNullException("serialNo");
            _wfClientService.GotoActivity(userName, serialNo, activityName, false, false);
        }

        public void GotoActivity(string userName, int procInstID, string activityName)
        {
            if (string.IsNullOrEmpty(activityName))
                throw new ArgumentNullException("activityName");
            _wfManagementService.GotoActivity(procInstID, activityName, false);
        }

        public void CancelActivity(int procInstID)
        { 

            _wfManagementService.CancelActivity(procInstID);
        }

        public List<ProcessLogModel> GetProcessLog(int procInstID)
        {
            var processLogs = _processLogService.GetProcessLogByProcInstID(procInstID);

            var processLogModels = new List<ProcessLogModel>();

            processLogs.ForEach(log =>
            {
                var logModel = new ProcessLogModel()
                {
                    ProcInstID = log.ProcInstID,
                    ProcessName = log.ProcessName,
                    CreatedDate = log.CommentDate,
                    TaskOwner = log.OrigUserAccount,
                    TaskOwnerName = log.OrigUserName,
                    ActionTaker = log.UserAccount,
                    ActionTakerName = log.UserName,
                    ActivityName = log.ActivityName,
                    ActionName = log.ActionName,
                    Comment = log.Comment,
                    PostSysID = log.SN,
                    PostName = GetPostName(log.SN)

                };

                processLogModels.Add(logModel);
            });

            return processLogModels;
        }

        private string GetPostName(string PostSysID)
        {
            if (string.IsNullOrWhiteSpace(PostSysID) || PostSysID == "null")
            {
                return string.Empty;

            }
            else
            {
                try
                {
                    Guid sysID = Guid.Parse(PostSysID);
                    using (KStarFramekWorkDbContext db = new KStarFramekWorkDbContext())
                    {
                        var linq = from p in db.Positions
                                   join po in db.PositionOrgNodes on p.SysId equals po.Position_SysId
                                   join o in db.OrgNodes on po.OrgNode_SysId equals o.SysId
                                   where p.SysId == sysID
                                   select new
                                   {
                                       OName = o.Name,
                                       PName = p.Name
                                   };

                        var s = linq.FirstOrDefault();
                        if (s == null) return string.Empty;
                        return s.OName + s.PName;
                    }

                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        private List<DataField> GetDatafieldNameValues(Dictionary<string, object> sDatafield)
        {
            var datafields = new List<DataField>();

            if (sDatafield != null)
            {
                foreach (KeyValuePair<string, object> kv in sDatafield)
                {
                    datafields.Add(new DataField() { Name = kv.Key, Value = kv.Value });
                }
            }
            return datafields;
        }
    }
}

using aZaaS.Framework.Workflow;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.WorkflowData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form
{
    public class WorkflowService : IWorkflowService
    {
        private readonly WorkflowClientService _wfClientService;
        public WorkflowService(AuthenticationType authtype)
        {
            this._wfClientService = new WorkflowClientService(authtype);
        }

        public int StarNewTask(string userName, string processName, Dictionary<string, object> datafields)
        {
            return _wfClientService.StartProcessInstance(userName, processName, "", datafields); 
        }

        public ViewModels.WorkflowTaskModel GetTaskItem(string userName, string serialNo)
        {
            List<WorklistItem> worklists=_wfClientService.GetWorklistItems(userName, PlatformType.ASP);
            WorklistItem workitem=worklists.Where(x=>x.SN==serialNo).FirstOrDefault();
            if (workitem != null)
            {
                return new ViewModels.WorkflowTaskModel()
                {
                    ActivityName = workitem.ActivityName,
                    AssignedDate = workitem.AssignedDate,
                    Data = workitem.Data,
                    Destination = workitem.Destination
                };
            }
            return null;
        }

        public IEnumerable<string> GetTaskActions(string userName, string serialNo)
        {
            return null;
        }

        public void ActionTask(string userName, string serialNo, Dictionary<string, string> datafields, Dictionary<string, object> actDatafields, string actionName)
        {
            throw new NotImplementedException();
        }

        public void Delegate(string serialNo, IEnumerable<string> userNames)
        {
            _wfClientService.Delegate(serialNo, userNames.ToList());
        }

        public void Redirect(string serialNo, string userName)
        {
            _wfClientService.Redirect(serialNo, userName);
        }

        public void AddSigner(string serialNo, IEnumerable<string> userNames)
        {
            throw new NotImplementedException();
        }

        public void GotoActivity(string serialNo, string activityName)
        {
            _wfClientService.GotoActivity(serialNo, activityName,false,false);
        }


        public void ActionTask(string userName, string serialNo, Dictionary<string, object> datafields, Dictionary<string, object> actDatafields, string actionName)
        {
            throw new NotImplementedException();
        }

        public void AddSigner(string userName, string serialNo, IEnumerable<string> userNames)
        {
            throw new NotImplementedException();
        }

        public int StarNewTask(WorkflowTaskContext context)
        {
            throw new NotImplementedException();
        }


        public void ActionTask(WorkflowTaskContext context)
        {
            throw new NotImplementedException();
        }
    }
}

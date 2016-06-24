using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IWorkflowService 
    {
        int StarNewTask(string userName, string processName, string processFolio, Dictionary<string, object> datafields);
        WorkflowTaskModel GetTaskItem(string userName, string shareUser, string serialNo);
        IEnumerable<string> GetTaskActions(string userName, string serialNo);
        void ActionTask(string userName, string serialNo, Dictionary<string, object> datafields, Dictionary<string, object> actDatafields, string actionName);
        void ActionTask(string userName, string serialNo, string shareUser, Dictionary<string, object> datafields, Dictionary<string, object> actDatafields, string actionName);
        void Delegate(string userName, string serialNo, IEnumerable<string> userNames);
        void Redirect(string userName, string serialNo, string touserName);
        void AddSigner(string userName, string serialNo, IEnumerable<string> userNames);
        void GotoActivity(string userName, string serialNo, string activityName);
        void GotoActivity(string userName, int procInstID, string activityName); 
        void CancelActivity(int procInstID);
        List<ProcessLogModel> GetProcessLog(int procInstID);
    }
}

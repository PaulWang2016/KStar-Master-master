using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using aZaaS.KStar;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TaskService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TaskService.svc or TaskService.svc.cs at the Solution Explorer and start debugging.
    public class TaskService : ITaskService
    {
        private WorkflowClientService _wfClientService;

        public TaskService()
        {
            var authType = AuthConfig.AuthType;
            _wfClientService = new WorkflowClientService(authType);
        }

        public IEnumerable<string> GetTaskActions(string userName,string serialNumber)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNumber))
                throw new ArgumentNullException("serailNumber");

            var actions = new List<string>();

            var wlitem = _wfClientService.OpenWorklistItem(userName, serialNumber);
            wlitem.Actions.ForEach(act => actions.Add(act.Name));

            return actions;
        }

        public void ActionTask(string userName,string serialNumber,string sDatafield,string sActDatafield, string actionName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(serialNumber))
                throw new ArgumentNullException("serailNumber");
            if (string.IsNullOrEmpty(actionName))
                throw new ArgumentNullException("actionName");

            var datafields = GetDatafieldNameValues(sDatafield);
            var actDatafields = GetDatafieldNameValues(sActDatafield);

            _wfClientService.ExecuteAction3(userName, serialNumber, datafields, actDatafields, actionName);
        }

        private List<DataField> GetDatafieldNameValues(string sDatafield)
        {
            var datafields = new List<DataField>();

            if (!string.IsNullOrEmpty(sDatafield))
            {
                var fields = sDatafield.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                fields.ToList().ForEach(item =>
                {
                    var kv = item.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (kv.Any() && kv.Length == 2)
                        datafields.Add(new DataField() { Name = kv.First(), Value = kv.Last() });
                });
            }

            return datafields;
        }
    }
}

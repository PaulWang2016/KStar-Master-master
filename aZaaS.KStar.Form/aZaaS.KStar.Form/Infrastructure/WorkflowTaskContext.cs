using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.ViewModels;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Form.Infrastructure
{
    public class WorkflowTaskContext : ContextBase
    {
        public int ProcInstId { get; set; }
        public string Folio { get; set; }
        public string ProcessName { get; set; }
        public string SerialNo { get; private set; }
        public string ActionName { get; private set; }
        public string ActivityName { get; set; }
        public string ActionComment { get; set; }

        public string RedirectToUser { get; private set; }
        public HashSet<string> DelegateUsers { get; private set; }
        public HashSet<string> NeedTaskSigners { get; private set; }
        public IList<Activity> ReworkActivitys { get; set; }

        public Dictionary<string, object> DataFields { get;  set; }
        public Dictionary<string, object> ActivityDataFields { get;  set; }

        public WorkflowTaskModel CurrentTaskItem { get; set; }

        public WorkflowTaskContext(KStarFormModel formModel, string userName, string serialNo)
            : base(formModel)
        {
            base.UserName = userName;
            ProcInstId = formModel.ProcInstId;
            var toolbar = formModel.Toolbar ?? new ToolbarActionModel();
            formModel.Toolbar = toolbar;
            ActionName = formModel.Toolbar.ActionName;
            ActionComment = formModel.Toolbar.ActionComment;
            ActivityName = formModel.ActivityName;

            Folio = formModel.ProcessFolio;
            SerialNo = serialNo;
            DelegateUsers = new HashSet<string>();
            NeedTaskSigners = new HashSet<string>();
            DataFields = new Dictionary<string, object>();
            ActivityDataFields = new Dictionary<string, object>();
            ReworkActivitys = new List<Activity>();
        }


        public void SynchronizeTaskItem(WorkflowTaskModel taskItem)
        {
            if (taskItem == null)
                throw new ArgumentNullException("taskItem");

            this.CurrentTaskItem = taskItem;
            this.Folio = taskItem.Folio;
            this.ProcInstId = taskItem.ProcInstId;
            this.ActivityName = taskItem.ActivityName;

            //TODO:More properties ...
        }

        public void AddDataField(string name, object value)
        {
            if (DataFields.ContainsKey(name))
                DataFields[name] = value;
            else
                DataFields.Add(name,value);
        }

        public void AddDataFields(Dictionary<string, object> dataFields)
        {
            DataFields = DataFields.Union(dataFields).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void AddActivityDataField(string name, object value)
        {
            if (ActivityDataFields.ContainsKey(name))
                ActivityDataFields[name] = value;
            else
                ActivityDataFields.Add(name, value);
        }

        public void AddActivityDataFields(Dictionary<string, object> activityDataFields)
        {
            ActivityDataFields = ActivityDataFields.Union(activityDataFields).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void AddDelegateUser(string userName)
        {
            DelegateUsers.Add(userName);
        }

        public void AddTaskSigner(string userName)
        {
            NeedTaskSigners.Add(userName);
        }

    }
}

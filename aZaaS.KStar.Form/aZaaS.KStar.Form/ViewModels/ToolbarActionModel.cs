using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Form.ViewModels
{
    public class ToolbarActionModel
    {
        public ToolbarActionModel()
        {
            this.IsEnableSign = false;
            this.SignActionName = string.Empty;
            this.ActionName = string.Empty;
            this.ActionComment = string.Empty;
            this.ReviewComment = string.Empty;
            this.TaskActions = new List<string>();
            this.DelegateUsers = new HashSet<string>();
            this.NeedTaskSigners = new HashSet<string>();
            this.ReworkActivitys = new List<Activity>();
            this.Posts = new List<Post>(); 

        }

        public IEnumerable<Post> Posts { get; set; }

        public string PostSysID{ get; set; }
        public string ActionName { get; set; }
        public string ActionComment { get; set; }
        public string ReviewComment { get; set; }
        public bool IsEnableSign { get; set; }
        public string SignActionName { get; set; }
        public bool IsEnableUndo { get; set; }
        public bool IsEnableFormCopy { get; set; }
        public bool IsPredict { set; get; }
        public IEnumerable<string> TaskActions { get; set; }

        //Below properties used for 
        public string RedirectToUser { get;  set; }
        public string GotoActivityName { get; set; }
        public string ViewFlowUrl { get; set; }
        public HashSet<string> DelegateUsers { get;  set; }
        public HashSet<string> NeedTaskSigners { get;  set; }
        public IList<Activity> ReworkActivitys { get; set; }

        //TODO: Includes taskitem's properties

    }

    public class Post
    {
        public string Name { set; get; }
        public string SysID { set; get; }
    }
}

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class TaskInfo : BaseEntity
    {
        public ProcBaseInfo ProcBaseInfo { get; set; }

        public BizInfo BizInfo { get; set; }

        public ProcLogInfo ProcLogInfo { get; set; }

        public Actions Actions { get; set; }

        public TaskInfo()
        {
            this.ProcBaseInfo = new ProcBaseInfo();
            this.BizInfo = new BizInfo();
            this.ProcLogInfo = new ProcLogInfo();
            this.Actions = new Actions();
        }
    }

    public class ProcBaseInfo : BaseEntity
    {
        public Group Group { get; set; }
    }

    public class BizInfo : BaseEntity
    {
        public BizInfo() { this.Groups = new List<Group>(); }
        public List<Group> Groups { get; set; }
    }

    public class ProcLogInfo : BaseEntity
    {
        public Group Group { get; set; }
    }

    public class Actions
    {
        public List<Item> Items { get; set; }
    }
}

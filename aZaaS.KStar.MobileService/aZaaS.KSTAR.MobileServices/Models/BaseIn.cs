using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class BaseIn
    {
        public string UserToken { get; set; }
        public string Mask { get; set; }
    }

    public class TaskInfo_In : BaseIn
    {
        public string SN { get; set; }
    }

    public class TaskList_In : BaseIn
    {
        public Filter Filter { get; set; }
        public Paging Paging { get; set; }
        public Sorting Sorting { get; set; }
    }

    public class ExecuteTask_In : BaseIn
    {
        public string SN { get; set; }
        public string Action { get; set; }
        public string Opinion { get; set; }
    }
}
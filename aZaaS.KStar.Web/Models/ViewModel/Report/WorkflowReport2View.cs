using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel.Report
{
    public class WorkflowReport2View
    {
        public string ProcessFullname { get; set; }


        public string DisplayName { get; set; }
     
        public string Avg_Consuming_Second { get; set; }

        public string Avg_Consuming_Second_Value { get; set; }

        public int ProcInst_Count { get; set; }

        public string Percentage { get; set; }

    }
}
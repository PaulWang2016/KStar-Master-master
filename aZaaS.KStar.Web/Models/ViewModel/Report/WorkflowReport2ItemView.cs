using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel.Report
{
    public class WorkflowReport2ItemView
    {
        public int ActivityID { get; set; }

        public string ActivityName { get; set; }

        public string DisplayName { get; set; }

        public string Avg_Consuming_Second { get; set; }

        public string Avg_Consuming_Second_Value { get; set; }

        public string Percentage { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
    public class WorkflowReport2MasterDTO
    {
        public string ProcessFullname { get; set; }

        public int RunningCount { get; set; }

        public int Avg_Consuming_Second { get; set; }
        
        public int ProcInst_Count { get; set; }



    }
}

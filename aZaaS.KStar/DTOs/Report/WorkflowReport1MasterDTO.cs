using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
    public class WorkflowReport1MasterDTO
    {
        public string ProcessFullname { get; set; }

        public int TotalCount { get; set; }

        public int RunningCount { get; set; }

        public int CompletedCount { get; set; }
    }
}

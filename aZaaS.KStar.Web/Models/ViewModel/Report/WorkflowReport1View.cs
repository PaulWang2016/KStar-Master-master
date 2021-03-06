﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel.Report
{
    public class WorkflowReport1View
    {
        public string ProcessFullname { get; set; }

        public string DisplayName { get; set; }

        public int TotalCount { get; set; }

        public int RunningCount { get; set; }

        public int CompletedCount { get; set; }

        public string Percentage { get; set; }
    }
}
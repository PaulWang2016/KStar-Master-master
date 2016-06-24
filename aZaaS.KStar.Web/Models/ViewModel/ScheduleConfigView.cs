using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class ScheduleConfigView
    {
        public string TaskName { get; set; }
        public string TypeName { get; set; }

        public string AssemblyName { get; set; }
        public DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public DateTime? LastRunTime { get; set; }
        public string PrivateBinPath { get; set; }
        public int RunCount { get; set; }
        public int Status { get; set; }
        
        public string NotificationReceiver { get; set; }
        public bool OnError { get; set; }
        public bool OnExec { get; set; }

        public DateTime? ExitOn { get; set; }
        public double? Interval { get; set; }
        public int IntervalType { get; set; }
        public DateTime StartTime { get; set; }

        public string TriggerDescription { get; set; }


        public string SystemName { get; set; }
        public string TargetName { get; set; }
        public string SourceName { get; set; }
    }
}
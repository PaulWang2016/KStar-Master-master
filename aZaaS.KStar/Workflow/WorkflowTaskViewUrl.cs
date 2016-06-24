
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.Framework.Workflow;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar
{
    public static class WorkflowTaskViewUrl
    {
        public static string Create(string processFullName, string procInstId)
        {
            string viewurl = string.Empty;

            var svc = new ConfigManager(AuthenticationType.Windows);
            var process = svc.GetProcessSetByFullName(processFullName);
            if (process != null)
            {
                var service = new ViewFlowArgs();
                viewurl = service.FormatViewUrl(process.ViewUrl, procInstId);
            }

            return viewurl;
        }
    }
}

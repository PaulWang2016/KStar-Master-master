using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using aZaaS.Framework;

namespace aZaaS.KStar
{
    public static class ViewFlowUtil
    {
        public static string GetViewFlowUlr(int procInstID)
        {
            ServiceContext context = new ServiceContext();
            var viewFlowUrl = ConfigurationManager.AppSettings["ViewFlowUrl"];

            if (WorkflowEngineConfig.CurrentEngine == WorkflowEngine.aZaaS)
                return string.Format("{0}?ProcInstID={1}", viewFlowUrl, procInstID);

            var serverName = context[SettingVariable.ServerName];
            var serverPort = context[SettingVariable.ServerPort];

            // to be compatible web designer Workflow
            // var viewUrl = string.Format("{0}?ProcessID={1}", viewFlowUrl, procInstID);
            var viewUrl = string.Format("{0}?K2Server={1}:{2}&ProcessID={3}", viewFlowUrl, serverName, serverPort, procInstID);
            return viewUrl;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.Framework.Workflow;

namespace aZaaS.KStar
{
    public static class WorkflowEngineConfig
    {
        const string ENGINE_K2_LABEL = "K2";
        const string ENGINE_AZAAS_LABEL = "aZaaS";
        const string WORKFLOW_ENGINE_KEY = "WorkFlowEngine";

        public static WorkflowEngine CurrentEngine
        {
            get
            {
                var engine = ConfigurationManager.AppSettings[WORKFLOW_ENGINE_KEY];

                if (string.IsNullOrEmpty(engine))
                    return WorkflowEngine.K2;

                if (engine.Equals(ENGINE_K2_LABEL, StringComparison.OrdinalIgnoreCase))
                    return WorkflowEngine.K2;

                if (engine.Equals(ENGINE_AZAAS_LABEL, StringComparison.OrdinalIgnoreCase))
                    return WorkflowEngine.aZaaS;

                throw new InvalidOperationException("Invalid workflow engine!");
            }
        }
    }


    public enum WorkflowEngine
    {
        K2,
        aZaaS
    }
}

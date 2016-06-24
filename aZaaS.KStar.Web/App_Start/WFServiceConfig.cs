using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using Autofac;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Web
{
    public static class WFServiceConfig
    {
        public static void RegisterWorkflowServices(ContainerBuilder builder)
        {
            //Inside the aZaaS.Framework,
            //we used K2 engine as the default workflow engine,
            //so by default the k2 workflow service were registed as the default service.
            //and in here we can override the services if we use other workflow engine services(e.g: aZaaS).

            if (WorkflowEngineConfig.CurrentEngine == WorkflowEngine.aZaaS)
            {
                //builder.RegisterType<aZaaS.Framework.BPMWorkflow.BusinessDataRepository>();
                //builder.RegisterType<aZaaS.Framework.BPMWorkflow.BusinessColumnRepository>();
                //builder.RegisterType<aZaaS.Framework.BPMWorkflow.BusinessDataService>();

                //builder.Register(c => new aZaaS.Framework.BPMWorkflow.WFClientService(AuthenticationType.Form, c.Resolve<aZaaS.Framework.BPMWorkflow.BusinessDataService>())).Keyed<IWFClientService>(AuthenticationType.Form);
                //builder.Register(c => new aZaaS.Framework.BPMWorkflow.WFClientService(AuthenticationType.Windows, c.Resolve<aZaaS.Framework.BPMWorkflow.BusinessDataService>())).Keyed<IWFClientService>(AuthenticationType.Windows);
                //builder.Register(c => new aZaaS.Framework.BPMWorkflow.WFManagementService(AuthenticationType.Windows, c.Resolve<aZaaS.Framework.BPMWorkflow.BusinessDataService>())).Keyed<IWFManagementService>(AuthenticationType.Windows);
                //builder.Register(c => new aZaaS.Framework.BPMWorkflow.WFManagementService(AuthenticationType.Form, c.Resolve<aZaaS.Framework.BPMWorkflow.BusinessDataService>())).Keyed<IWFManagementService>(AuthenticationType.Form);
            }
        }
    }

}
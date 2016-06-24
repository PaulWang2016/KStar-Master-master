using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

using aZaaS.Framework.SQLQuery;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar
{
    public static class SQLQueryConfiguration
    {
        public static void Initialize()
        {

            //<!---------  This is OPTIONAL in default k2 DB environment --------->

            K2SQLScripts.GetMyStartedProcessInstanceSQLProvider = () =>
            {
                return SQLQueryBroker.GetQuery("Framework_WorkflowService_GetMyStartedProcessInstances");
            };

            K2SQLScripts.GetMyParticipatedProcessInstancesSQLProvider = () =>
            {
                return SQLQueryBroker.GetQuery("Framework_WorkflowService_GetMyParticipatedProcessInstances");
            };

            K2SQLScripts.GetInsteadMyStartedProcessInstancesSQLProvider = () =>
            {
                return SQLQueryBroker.GetQuery("Framework_WorkflowService_GetInsteadMyStartedProcessInstances");
            };

            //<!---------  This is optional in default k2 DB environment --------->

            var webRoot = HostingEnvironment.MapPath("~/");
            SQLQueryBroker.SetQueryContainer(new XmlQueryContainer(webRoot));
        }
    }
}

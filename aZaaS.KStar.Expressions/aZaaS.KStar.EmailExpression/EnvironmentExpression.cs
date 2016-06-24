
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;

using aZaaS.KStar;
using aZaaS.Framework;
using aZaaS.Framework.Template;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;

namespace aZaaS.KStar.EmailExpression
{
    [Export("Environment", typeof(ITemplateExpression))]
    public class EnvironmentExpression : DynamicObject, ITemplateExpression
    {
        public List<StringTable> stringTables = new List<StringTable>();

        public void Fill(ServiceContext context)
        {
            var environment = context["Environment"] ?? "Production";
            var auth = context["__AuthenticationType"] ?? AuthenticationType.Windows.ToString();
            var authType = (AuthenticationType)Enum.Parse(typeof(AuthenticationType), auth);

            var wfMgmtService = new WorkflowManagementService(authType);
            stringTables = wfMgmtService.GetStringTables(environment, new PageCriteria() { PageSize = int.MaxValue });
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Count() != 1)
            {
                base.TryGetIndex(binder, indexes, out result);
            }

            var indexStr = indexes.Single() as string;
            result = stringTables.Where(t => t.Name == indexStr).Select(t => t.Value).FirstOrDefault();
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = stringTables.Where(t => t.Name == binder.Name).Select(t => t.Value).FirstOrDefault();
            return true;
        }
    }
}

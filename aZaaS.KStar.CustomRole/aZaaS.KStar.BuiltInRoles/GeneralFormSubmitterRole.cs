using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar;
using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.BuiltInRoles
{
    public class GeneralFormSubmitterRole : CustomRoleBase
    {
        public GeneralFormSubmitterRole()
            : base("EF2E9BC8-08A6-438D-8789-ABC5C026223E")
        {
            
        }

        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();

            try
            {
                var _wfConfigManger = new ConfigManager(AuthenticationType.Windows);
                var formHeader = _wfConfigManger.GetProcesFormHeader(context.ProcInstID);

                returnUsers.Add(formHeader.SubmitterAccount);
            }
            catch (Exception)
            {
            }

            return returnUsers;
        }
    }
}

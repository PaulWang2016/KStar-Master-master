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
    public class FormRelatedUsersRole : CustomRoleBase
    {
        public FormRelatedUsersRole()
            : base("C3AA9E38-BB27-415C-8A07-7B2A9DB592E1")
        {
            
        }

        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();

            try
            {
                ProcessLogService _logService = new ProcessLogService();
                var logs = _logService.GetProcessLogByProcInstID(context.ProcInstID);

                if (logs != null && logs.Count > 0)
                    logs.ForEach(item =>
                    {
                        returnUsers.Add(item.OrigUserAccount);
                        returnUsers.Add(item.UserAccount);
                    });
               
            }
            catch (Exception)
            {
            }

            return returnUsers;
        }
    }
}

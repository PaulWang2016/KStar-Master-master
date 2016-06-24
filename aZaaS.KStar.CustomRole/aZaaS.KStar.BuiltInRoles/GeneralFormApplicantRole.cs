using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.CustomRole;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.BuiltInRoles
{
    public class GeneralFormApplicantRole : CustomRoleBase
    {
        public GeneralFormApplicantRole()
            : base("39D11300-49D5-44B8-8E14-FB2878BEAA22")
        {

        }

        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();

            try
            {
                returnUsers.Add(context.Requester);
            }
            catch (Exception)
            {
            }

            return returnUsers;
        }
    }
}

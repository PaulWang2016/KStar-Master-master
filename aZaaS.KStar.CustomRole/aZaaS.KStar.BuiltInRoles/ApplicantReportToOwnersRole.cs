using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

using aZaaS.KStar.CustomRole;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.BuiltInRoles
{
    public class ApplicantReportToOwnersRole : CustomRoleBase
    {
        public ApplicantReportToOwnersRole()
            : base("678603C7-8DA5-4062-86B1-0646E6A8CD51")
        {
            
        }

        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();

            try
            {
                var _userService = new UserService();
                var user = _userService.ReadUserWithOwners(context.Requester);
                user.ReportTo.ToList().ForEach(u => returnUsers.Add(u.UserName));
            }
            catch (Exception)
            {
                //TODO:Record the exception to error log. 
            }

            return returnUsers;
        }
    }
}

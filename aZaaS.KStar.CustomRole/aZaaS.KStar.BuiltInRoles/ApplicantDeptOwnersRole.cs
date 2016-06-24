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
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.BuiltInRoles
{
    public class ApplicantDeptOwnersRole : CustomRoleBase
    {
        //private readonly OrgChartService _chartService;
        //private readonly UserService _userService;

        public ApplicantDeptOwnersRole()
            : base("34058CE6-149B-4D9F-9E5E-CAE0B1ECCEC7")
        {
            //_userService = new UserService();
            //_chartService = new OrgChartService();            
        }

        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();

            try
            {
                /* METHOD 1 */
                //var user =  _userService.ReadUserWithNodes(context.Requester);
                //if (user.Nodes != null && user.Nodes.Count() > 0)
                //{
                //    user.Nodes.ToList().ForEach(node =>
                //    {
                //        var orgNode = _chartService.ReadNodeWithFields(node.SysID);
                //        if (orgNode != null)
                //        {
                //            var field = orgNode.ExtendItems.FirstOrDefault(f => f.Name.Equals("DeptOwner"));
                //            if (field != null)
                //                returnUsers.Add(field.Value);
                //        }
                //    });
                //}

                OrgChartService _chartService=new OrgChartService();
                UserService _userService = new UserService();

                var OrgNodeIdKey = "ApplicantOrgNodeID";

                if (context.FormInfo.ContainsKey(OrgNodeIdKey))
                {
                    var applicantOrgNodeId = Guid.Parse(context.FormInfo[OrgNodeIdKey]);
                    var orgNode = _chartService.ReadNodeWithFields(applicantOrgNodeId);
                    if (orgNode != null)
                    {
                        var field = orgNode.ExtendItems.FirstOrDefault(f => f.Name.Equals("DeptOwner"));
                        if (field != null)
                            returnUsers.Add(field.Value);
                    }   
                }
 
            }
            catch (Exception)
            {
            }

            return returnUsers;
        }
    }
}

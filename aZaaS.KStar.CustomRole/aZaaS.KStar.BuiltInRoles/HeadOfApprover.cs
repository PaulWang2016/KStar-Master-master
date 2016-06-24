using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using aZaaS.KStar;
using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.MgmtServices;
using aZaaS.Kstar.DAL;

namespace aZaaS.KStar.BuiltInRoles
{
    /// <summary>
    /// 审批人的部门领导
    /// </summary>
   public class HeadOfApprover : CustomRoleBase
    {
       public HeadOfApprover()
           : base("0250AAEF-FFD4-4D4D-BE12-0F0958523D97")
        {
                      
        }
       public override IEnumerable<string> Execute(CustomRoleContext context)
       {
           ///上下文中取不到当前审批人信息，所以需要去流程审批日志里面去取当前审批的账号，参数为实例ID,和当前节点名称
           var returnUsers = new HashSet<string>();
           var ProcInstID = context.ProcInstID;
           var ActivityName = context.ActivityName;
           var UserId =string.Empty;
           var strSql = @"SELECT UserAccount From [aZaaS.Framework].[dbo].[ProcessLog] where ProcInstID=" + ProcInstID + " and  ActivityName='" + ActivityName + "' order by CommentDate desc";
           var UserName = CustomExtUtility.GetScalarData(strSql).ToString() ;
           if (!string.IsNullOrEmpty(UserName))
           {
               UserId = UserName.Substring(UserName.LastIndexOf(@"\") + 1);
           }
           try
           {
               var result = OrgUtility.GetUserHead(UserId);
               foreach (DataRow r in result.Rows)
               {
                   returnUsers.Add(r["UserId"].ToString());
               }

            
           }
           catch (Exception ex)
           { }
           return returnUsers;
       }
    }
}

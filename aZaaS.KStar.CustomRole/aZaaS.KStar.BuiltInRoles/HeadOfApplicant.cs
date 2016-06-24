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
using aZaaS.Kstar.DAL;

namespace aZaaS.KStar.BuiltInRoles
{
    /// <summary>
    /// 申请人的部门领导
    /// </summary>
   public class HeadOfApplicant : CustomRoleBase
    {
       public HeadOfApplicant()
           : base("03510F82-9824-4489-BACC-5F503349B1C5")
        {
                      
        }
       public override IEnumerable<string> Execute(CustomRoleContext context)
       {
           List<string> userNameList = new List<string>();
           try
           {
               //取得当前申请人的所在的部门ID
               var orgNodeID = context.FormInfo["ApplicantOrgNodeID"].ToString();
               
               try
               {
                   //根据部门ID，和岗位取得当前的审批人
                   userNameList = CustomExtUtility.GetPositionUserName(orgNodeID, "部门经理");

               }
               catch (Exception ex)
               {
                   CustomExtUtility.SaveErrorLogToDB(ex.ToString(), "FinanceManagerRole");
               }
           }
           catch (Exception ex)
           { }
           return userNameList;
       }

    }
}

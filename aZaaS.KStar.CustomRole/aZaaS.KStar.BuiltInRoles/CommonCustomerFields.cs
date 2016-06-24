using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using aZaaS.KStar.CustomRole;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Data;
using aZaaS.Kstar.DAL;
namespace aZaaS.KStar.BuiltInRoles
{
   /// <summary>
    /// 选取表单字段通用角色
   /// </summary>
    public class CommonCustomerFields:CustomRoleBase
    {
        //static object lockObject = new object();
        public CommonCustomerFields()
            : base("F6F3FA8A-89F0-49C9-88F5-F8A7D08490A4")
        {
            
        } 
        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();
            try
            {

                string processName = context.ProcessFullName;
                string activityName = context.ActivityName;
                int formid = context.FormId;
                string domain = ConfigurationSettings.AppSettings["WindowDomain"].ToString();
                string Approvers = CustomExtUtility.GetProcessApprover(processName, activityName, formid);
                if (!string.IsNullOrEmpty(Approvers))
                {
                    string[] Users = Approvers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in Users)
                    { 
                        returnUsers.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                CustomExtUtility.SaveErrorLogToDB(ex.ToString(), "CommonCustomerFields");
            }
            return returnUsers;
        }
             
    } 
}

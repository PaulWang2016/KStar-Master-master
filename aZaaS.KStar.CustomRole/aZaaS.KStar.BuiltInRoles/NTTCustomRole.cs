using aZaaS.Kstar.DAL;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.DTOs.CustomRole;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.BuiltInRoles
{
    /// <summary>
    /// NTT Demo 自定义角色
    /// </summary>
    public class NTTCustomRole : CustomRoleBase
    {
        public NTTCustomRole()
            : base("9D82EFEE-7291-472B-B2F0-6FA856C5E0B1")
        {

        }

        public override IEnumerable<string> Execute(CustomRoleContext context)
        {
            var returnUsers = new HashSet<string>();

            try
            {  
                if (context != null)
                {
                    string _connString = System.Configuration.ConfigurationManager.ConnectionStrings["K2DB"].ToString();

                     var DataFieldCode = "User";

                       var sql = string.Format(@"SELECT Name as DataFieldCode,Value as DataFieldValue 
                                                  FROM [K2].[ServerLog].[ProcInstData] 
                                                  WHERE ProcInstID = {0} and Name='{1}' ", context.ProcInstID, DataFieldCode);

                       DataTable table = SqlHelper.ExecuteTable(_connString, sql, null);

                       if (table != null)
                       {
                           var ArrValue = table.Rows[0]["DataFieldValue"].ToString().Split(',');

                           if (ArrValue.Length > 1)
                           {
                               foreach (string Value in ArrValue)
                               {
                                   returnUsers.Add(Value);
                               }
                           }
                           else
                           {
                               returnUsers.Add(ArrValue[0]);
                           }
                       }
                      
                }
                else
                {
                    returnUsers.Add("BOB");
                }
            }
            catch (Exception ex)
            {
                CustomExtUtility.SaveErrorLogToDB(ex.ToString(), "NTTCustomRole");
            }

            return returnUsers;
        }

    }
}

using aZaaS.KStar.DTOs.CustomRole;
using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole
{
    public class CustomDaoManager
    {

        public ProcInstDataDTO GetProcInstDataItem(int prosInstId, string DataFieldCode)
        {
            using (K2DBContext context = new K2DBContext())
            {
                var sql = string.Format(@"SELECT Name as DataFieldCode,Value as DataFieldValue 
                                         FROM [K2].[ServerLog].[ProcInstData] 
                                         where ProcInstID = {0} and Name='{1}' ", prosInstId, DataFieldCode);
                var Item = context.Database.SqlQuery<ProcInstDataDTO>(sql).FirstOrDefault();
                return Item;
            }
        }

    }
}

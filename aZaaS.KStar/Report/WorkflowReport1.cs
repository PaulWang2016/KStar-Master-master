using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Repositories;
using aZaaS.Framework.SQLQuery;

namespace aZaaS.KStar.Report
{
    public class WorkflowReport1
    {
        public List<WorkflowReport1MasterDTO> GetMasterData()
        {
            var sql = SQLQueryBroker.GetQuery("SQL_WorkflowReport1_GetMasterData");

            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<WorkflowReport1MasterDTO>(sql).ToList<WorkflowReport1MasterDTO>();
                return list;
            }

        }

        public List<WorkflowReport1ItemDTO> GetItemsData(string fullname)
        {
            
           var sql = string.Format(SQLQueryBroker.GetQuery("SQL_WorkflowReport1_GetItemsData"), fullname);

            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<WorkflowReport1ItemDTO>(sql).ToList<WorkflowReport1ItemDTO>();
                return list;
            }

        }

    }
}

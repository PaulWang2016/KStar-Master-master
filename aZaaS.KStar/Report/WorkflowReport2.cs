using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework.SQLQuery;

namespace aZaaS.KStar.Report
{
    public class WorkflowReport2
    {


        public List<WorkflowReport2MasterDTO> GetMasterData()
        {
            var sql = SQLQueryBroker.GetQuery("SQL_WorkflowReport2_GetMasterData");

            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<WorkflowReport2MasterDTO>(sql).ToList<WorkflowReport2MasterDTO>();
                return list;
            }
        }


        public List<WorkflowReport2ItemDTO> GetActInstData(string fullname)
        {

            var sql =string.Format(SQLQueryBroker.GetQuery("SQL_WorkflowReport2_GetActInstData"), fullname);
                    
            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<WorkflowReport2ItemDTO>(sql).ToList<WorkflowReport2ItemDTO>();
                return list;
            }


        }


        public List<ActInstSlotItemDTO> GetActInstSlotData(string fullname, string ActivityName)
        {


            var sql =string.Format(SQLQueryBroker.GetQuery("SQL_WorkflowReport2_GetActInstSlotData"), fullname, ActivityName);

            using (K2DBContext context = new K2DBContext())
            {
                var list = context.Database.SqlQuery<ActInstSlotItemDTO>(sql).ToList<ActInstSlotItemDTO>();
                return list;
            }



        }

    }
}

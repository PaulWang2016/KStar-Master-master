using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.Report
{
    public class FeedbackManager
    {

        public IList<FeedbackEntity> GetReportFeedbackByParnentID(Guid parnentID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    return ctx.ReportFeedback.Where(x => x.ReportInfoID == parnentID).ToList();
                }
                catch
                {
                    return null;
                }
                
            }
        }

        public FeedbackEntity GetReportFeedback(Guid ID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {

                return ctx.ReportFeedback.Where(x => x.ID == ID).FirstOrDefault();
            }
        }

        



        public bool AddReportFeedback(FeedbackEntity report)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    ctx.ReportFeedback.Add(report);
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}

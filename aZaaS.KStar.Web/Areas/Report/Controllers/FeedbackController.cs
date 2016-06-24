using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Report;
using Newtonsoft.Json;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
     [EnhancedHandleError]
    public class FeedbackController : BaseMvcController
    {
        //
        // GET: /Report/Feedback/
        FeedbackManager mgrFeedback = new FeedbackManager();

        public ActionResult Index()
        {
            return View();
        }



        public ActionResult GetReportFeedbackList(Guid parentID)
        {
            var reports = mgrFeedback.GetReportFeedbackByParnentID(parentID);
            return PartialView(reports);
        }
        [HttpPost]
        public JsonResult AddFeedback(FeedbackEntity report)
        {
            report.ID = Guid.NewGuid();
            report.CommitDate = DateTime.Now;
            report.UserId = this.HttpContext.CurrentRequesterID(); 

            //添加到数据库
            mgrFeedback.AddReportFeedback(report);


            return Json(new { Result = true }, "text/html");
        }







        [HttpPost]
        public JsonResult GetReportFeedback(Guid parentID)
        {
            var reports = mgrFeedback.GetReportFeedbackByParnentID(parentID);
            return Json(reports);
        }







    }
}

using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Report;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
     [EnhancedHandleError]
    public class HomeController : BaseMvcController
    {
        //
        // GET: /Report/Home/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult WorkflowReport1(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Report/Views/Parts/_WorkflowReport1.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult WorkflowReport2(bool isAjax = false)
        {

            if(isAjax)
            {

                return PartialView("~/Areas/Report/Views/Parts/_WorkflowReport2.cshtml");
            }


            return View("_SingPage");

           
        
        }
         /// <summary>
         /// 流程处理完成率
         /// </summary>
         /// <param name="isAjax"></param>
         /// <returns></returns>
        public ActionResult CompletionRate(bool isAjax = false)
        {

            if (isAjax)
            {

                return PartialView("~/Areas/Report/Views/Parts/_CompletionRate.cshtml");
            }


            return View("_SingPage");



        }

         /// <summary>
         /// 流程处理时长
         /// </summary>
         /// <param name="isAjax"></param>
         /// <returns></returns>
        public ActionResult ProcDealDuration(bool isAjax = false)
        {
            
            if (isAjax)
            {

                return PartialView("~/Areas/Report/Views/Parts/_ProcDealDuration.cshtml");
            }


            return View("_SingPage");



        }

        /// <summary>
        /// 流程使用频率
        /// </summary>
        /// <param name="isAjax"></param>
        /// <returns></returns>
        public ActionResult UseFrequency(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Report/Views/Parts/_UseFrequency.cshtml");
            }
            return View("_SingPage");
        }

        /// <summary>
        /// 流程使用频率
        /// </summary>
        /// <param name="isAjax"></param>
        /// <returns></returns>
        public ActionResult ProcessApprovalConsumingSecond(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Report/Views/Parts/_ProcessApprovalConsumingSecond.cshtml");
            }
            return View("_SingPage");
        }
        /// <summary>
        /// 流程通用报表
        /// </summary>
        /// <param name="isAjax"></param>
        /// <returns></returns>
        public ActionResult CommonReport(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Report/Views/Parts/_CommonReport.cshtml");
            }
            return View("_SingPage");
        }

        public JsonResult GetAllCategoryList()
        {
            ReportDaoManager manager = new ReportDaoManager();
            return Json(manager.GetAllCategoryList(), JsonRequestBehavior.AllowGet);
        } 
    }
}

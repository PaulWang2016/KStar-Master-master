using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.ViewModel.Report;
using aZaaS.KStar.Report;
using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Localization;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
     [EnhancedHandleError]
    public class WorkflowReport1Controller : BaseMvcController
    {

        public JsonResult GetWorkflowReport1List()
        {

            List<WorkflowReport1View> items = new List<WorkflowReport1View>();

            WorkflowReport1 report = new WorkflowReport1();

            var data = report.GetMasterData();

            double sum = data.Sum(x => x.TotalCount) * 1.00;

            foreach (var item in data)
            {
                var svc = new ConfigManager(this.AuthType);
                svc.TenantID = TenantID();
                var fullName = svc.GetProcessSetByFullName(this.CurrentUser, item.ProcessFullname);

                items.Add(new WorkflowReport1View
                {
                    ProcessFullname = item.ProcessFullname,
                    DisplayName = fullName,
                    TotalCount = item.TotalCount,
                    RunningCount = item.RunningCount,
                    CompletedCount = item.CompletedCount,
                    Percentage = ((item.TotalCount / sum) * 100).ToString("N2")
                });
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetWorkflowReport1ItemList(string parms)
        {
            //todo: 需要增加防止SQL注入攻击的逻辑
            parms = parms.Replace('/', '\\');

            List<WorkflowReport1ItemView> items = new List<WorkflowReport1ItemView>();

            WorkflowReport1 report = new WorkflowReport1();

            var data = report.GetItemsData(parms);


            double sum = data.Sum(x => x.TotalCount) * 1.00;

            foreach (var item in data)
            {
                items.Add(new WorkflowReport1ItemView
                {
                    ActivityName = item.ActivityName,
                    DisplayName = LocalizationResxExtend.DBResxFor_K2_ActInst_ActivityName(item.ActivityName.Replace("\\", "")),
                    TotalCount = item.TotalCount,
                    RunningCount = item.RunningCount,
                    ExpiredCount = item.ExpiredCount,
                    CompletedCount = item.CompletedCount,
                    Percentage = ((item.TotalCount / sum) * 100).ToString("N2")
                });
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index()
        {
            return View();
        }

    }
}

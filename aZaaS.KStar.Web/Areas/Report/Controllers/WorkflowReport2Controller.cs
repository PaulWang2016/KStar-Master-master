using aZaaS.KStar.Localization;
using aZaaS.KStar.Report;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.ViewModel.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    [EnhancedHandleError]
    public class WorkflowReport2Controller : BaseMvcController
    {
        //
        // GET: /Report/WorkflowReport2/

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetWorkflowReport2List()
        {
            List<WorkflowReport2View> items = new List<WorkflowReport2View>();

            WorkflowReport2 report = new WorkflowReport2();

            var data = report.GetMasterData();
            double sum = data.Sum(x => x.Avg_Consuming_Second) * 1.00;
            foreach (var item in data)
            {
                var svc = new ConfigManager(this.AuthType);
                svc.TenantID = TenantID();
                var fullName = svc.GetProcessSetByFullName(this.CurrentUser, item.ProcessFullname);

                items.Add(new WorkflowReport2View
                {
                    Avg_Consuming_Second = TimeFormat(item.Avg_Consuming_Second),
                    Avg_Consuming_Second_Value = item.Avg_Consuming_Second.ToString(),
                    ProcessFullname = item.ProcessFullname,
                    DisplayName = fullName,
                    ProcInst_Count = item.ProcInst_Count,
                    Percentage = ((item.Avg_Consuming_Second / sum) * 100).ToString("N2")
                });


            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetActInstData(string parms)
        {
            parms = parms.Replace('/', '\\');
            List<WorkflowReport2ItemView> items = new List<WorkflowReport2ItemView>();

            WorkflowReport2 report = new WorkflowReport2();

            var data = report.GetActInstData(parms);
            double sum = data.Sum(x => x.Avg_Consuming_Second) * 1.00;

            if (sum == 0)
            {
                sum = 1;
            }
            foreach (var item in data)
            {
                items.Add(new WorkflowReport2ItemView
                {
                    ActivityID = item.ActivityID,
                    ActivityName = item.ActivityName,
                    DisplayName = LocalizationResxExtend.DBResxFor_K2_ActInst_ActivityName(item.ActivityName.Replace("\\", "")),
                    Avg_Consuming_Second = TimeFormat(item.Avg_Consuming_Second),
                    Avg_Consuming_Second_Value = item.Avg_Consuming_Second.ToString(),
                    Percentage = ((item.Avg_Consuming_Second / sum) * 100).ToString("N2")
                });
            }

            return Json(items, JsonRequestBehavior.AllowGet);

        }



        public JsonResult GetActInstSlotData(string parms)
        {

            parms = parms.Replace('/', '\\');
            string[] parmAry = parms.Split(',');
            string Fullname = parmAry[0];
            string ActivityName = parmAry[1];

            List<ActInstSlotItemView> items = new List<ActInstSlotItemView>();
            WorkflowReport2 report = new WorkflowReport2();
            var data = report.GetActInstSlotData(Fullname, ActivityName);
            double sum = data.Sum(x => x.Avg_Consuming_Second) * 1.00;

            foreach (var item in data)
            {
                items.Add(new ActInstSlotItemView
                {
                    User = item.User,
                    Avg_Consuming_Second = TimeFormat(item.Avg_Consuming_Second),
                    Avg_Consuming_Second_Value = item.Avg_Consuming_Second.ToString(),
                    Percentage = ((item.Avg_Consuming_Second / sum) * 100).ToString("N2")

                });
            }

            return Json(items, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 审批时间：分钟，如果大于24*60 则显示x天xx小时xx分钟，否则显示xx小时xx分钟xx秒，如果不够1小时，则显示xx分钟xx秒
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string TimeFormat(int totals)
        {
            if (totals == 0) return "0 ";

            System.Text.StringBuilder result = new System.Text.StringBuilder();

            TimeSpan ts = new TimeSpan(0, 0, totals);

            if (ts.Days != 0) result.Append(ts.Days.ToString() + "D ");
            if (ts.Hours != 0) result.Append(ts.Hours.ToString() + "H ");
            if (ts.Minutes != 0) result.Append(ts.Minutes.ToString() + "M ");
            if (ts.Days == 0 && ts.Seconds != 0) result.Append(ts.Seconds.ToString() + "S ");

            return result.ToString();

        }
    }
}

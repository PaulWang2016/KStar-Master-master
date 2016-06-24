using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Helper;
using aZaaS.KStar.LogRequestProvider;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class LogRequestController : BaseMvcController
    {
        //
        // GET: /Maintenance/InsteadRequest/

        private readonly LogRequestManager logRequestManager;

        public LogRequestController()
        {
            logRequestManager = new LogRequestManager();
        }

        public JsonResult Find(string action,string username,DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            int total = 0;
            if (startDate == null)
            {
                startDate = InitStartDate(startDate);
            }
            if (endDate == null)
            {
                endDate = InitEndDate(endDate);
            }
            var items = logRequestManager.GetLogRequestByPage(action, username, startDate, endDate, page, pageSize, sort, out total).ToList();

            return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetLogRequestTaskForExcel(string action, string username, DateTime? startDate, DateTime? endDate)
        {
            int total = 0;
            if (startDate == null)
            {
                startDate = InitStartDate(startDate);
            }
            if (endDate == null)
            {
                endDate = InitEndDate(endDate);
            }
            var items = logRequestManager.GetLogRequestByPage(action, username, startDate, endDate, 1, int.MaxValue, new List<SortDescriptor>(), out total).ToList();            
            return Json(items.ToList(), JsonRequestBehavior.AllowGet);

        }
    }
}

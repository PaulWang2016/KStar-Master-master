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
using aZaaS.KStar.Extensions;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class InsteadRequestController : BaseMvcController
    {

        private readonly WorkflowClientService _workflowClientService;
        public InsteadRequestController()
        {
            _workflowClientService = new WorkflowClientService(this.AuthType);
        }

        [HttpGet]
        public JsonResult Find(DateTime? startDate, DateTime? endDate, string folio, string processName, int? page = 1, int? pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            var totalRecord = 0;

            startDate = startDate.ToBeginDateTime();
            startDate = endDate.ToEndDateTime();
            var sorting = sort.ToPICFieldSortRules();
            var processNames = CacheAccessor.ProcessNamesFilter(processName);

            var taskItems = _workflowClientService.GetUserInsteadRequestTasksList(this.CurrentUser, page, pageSize, out totalRecord, folio, startDate, endDate, processNames, sorting);

            return Json(new { total = totalRecord, data = taskItems }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetInsteadRequestTaskForExcel(DateTime? startDate, DateTime? endDate, string folio, string processName)
        {
            var totalRecord = 0;

            startDate = startDate.ToBeginDateTime();
            startDate = endDate.ToEndDateTime();
            var processNames = CacheAccessor.ProcessNamesFilter(processName);

            var taskItems = _workflowClientService.GetUserInsteadRequestTasksList(this.CurrentUser, null, null, out totalRecord, folio, startDate, endDate, processNames, null);

            return Json(taskItems, JsonRequestBehavior.AllowGet);

        }
    }
}

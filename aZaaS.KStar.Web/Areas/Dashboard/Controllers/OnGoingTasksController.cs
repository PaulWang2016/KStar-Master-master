
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.DTOs;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Extensions;

namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
    [EnhancedHandleError]
    public class OnGoingTasksController : BaseMvcController
    {
        private readonly WorkflowClientService _workflowClientService;
        public OnGoingTasksController()
        {
            _workflowClientService = new WorkflowClientService(this.AuthType);
        }

        [Obsolete("该方法目前平台已废弃，如果重新使用请参照Find方法的方式")]
        public JsonResult Get()
        {
            //ProcessInstanceCriteria criteria = new ProcessInstanceCriteria();
            //criteria.PageIndex = 0;
            //criteria.PageSize = int.MaxValue;
            //IEnumerable<WorkflowTask> items = tenantDatabaseService.FindOnGoingTasks(this.CurrentUser, TenantID(), ConnectionString("aZaaSKStar"),criteria);
            //items.ToList().ForEach(item =>
            //{
            //    item.ProcInstNo = item.Folio.Split('-').Last();
            //    item.ProcSubject = item.Folio.Replace("-" + item.ProcInstNo, "");
            //    item.Originator = GetOriginator(item.Originator.Split(':').Last());
            //    item.ViewFlowUrl = ViewFlowUtil.GetViewFlowUlr(item.Procinstid);
            //    item.ViewUrl = GetViewUrl(item.FullName, item.Procinstid.ToString());
            //});

            //return Json(items.OrderByDescending(s => s.StartDate).ToList(), JsonRequestBehavior.AllowGet);

            throw new NotImplementedException();
        }

        public JsonResult Find(DateTime? startDate, DateTime? endDate, string folio, string processName, int? page = 1, int? pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            var totalRecord = 0;

            startDate = startDate.ToBeginDateTime();
            startDate = endDate.ToEndDateTime();
            var sorting = sort.ToPICFieldSortRules();
            var processNames = CacheAccessor.ProcessNamesFilter(processName);

            var taskItems = _workflowClientService.GetUserOnGoingTaskList(this.CurrentUser, page, pageSize, out totalRecord, folio, startDate, endDate, processNames, sorting);

            return Json(new { total = totalRecord, data = taskItems }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetOnGoingTasksForExcel(DateTime? startDate, DateTime? endDate, string folio, string processName)
        {
            var totalRecord = 0;

            startDate = startDate.ToBeginDateTime();
            startDate = endDate.ToEndDateTime();
            var processNames = CacheAccessor.ProcessNamesFilter(processName);

            var taskItems = _workflowClientService.GetUserOnGoingTaskList(this.CurrentUser, null, null, out totalRecord, folio, startDate, endDate, processNames, null);

            return Json(taskItems, JsonRequestBehavior.AllowGet);

        }
    }
}

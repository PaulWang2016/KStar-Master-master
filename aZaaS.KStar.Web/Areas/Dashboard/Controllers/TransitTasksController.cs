using aZaaS.KStar.Helper;
using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Extensions;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
        [EnhancedHandleError]
    public class TransitTasksController : BaseMvcController
    {
        //
        // GET: /Dashboard/TransitTasks/
         private readonly WorkflowClientService _workflowClientService;
         public TransitTasksController()
        {
            _workflowClientService = new WorkflowClientService(this.AuthType);
        }
         public JsonResult Find(string folio, string processName, int? page = 1, int? pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
         {
             var totalRecord = 0;

             var sorting = sort.ToPICFieldSortRules();
             var processNames = CacheAccessor.ProcessNamesFilter(processName);



             var taskItems = WorkflowClientServiceExtensions.GetTransitTaskList(this.CurrentUser, page, pageSize, out totalRecord, folio, null, null, processNames, sorting);

             return Json(new { total = totalRecord, data = taskItems }, JsonRequestBehavior.AllowGet);

         }

    }
}

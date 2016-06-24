using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading.Tasks;

using aZaaS.KStar;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Web.Controllers;

using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Extensions;



namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
    [EnhancedHandleError]
    public class PendingTasksController : BaseMvcController
    {
        private readonly WorkflowClientService _workflowClientService;

        public PendingTasksController()
        {
            _workflowClientService = new WorkflowClientService(this.AuthType);
        }

        [HttpGet]
        public JsonResult Count()
        {
            var totalRecord = _workflowClientService.GetUserPendingTaskCount(this.CurrentUser, WLApiStyle.Prototype);

            return Json(new { total = totalRecord }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Group(WorkView wv, int? page, int? pageSize, string startdate = "", string enddate = "")
        {
            var totals = new Dictionary<string, int>();
            var result = new List<KeyValueEntity>(); ;

            var folio = wv.Folio;
            var orginator = wv.Originator;
            DateTime? startDate = startdate.ToBeginDateTime();
            DateTime? endDate = enddate.ToEndDateTime();
            var processNames = CacheAccessor.ProcessNamesFilter(wv.ProcessName);

            var taskGroups = _workflowClientService.GetUserPendingTaskListGroupingByProcess(this.CurrentUser, out totals, pageSize ?? 5, folio, startDate, endDate, processNames);

            taskGroups.Keys.ToList().ForEach(processName =>
            {
                var taskItems = taskGroups[processName];

                //Run on async mode
                taskItems.StoreTaskItemUrlArgsAsync();

                if (!string.IsNullOrEmpty(orginator))
                    taskItems = taskItems.Where(w => w.Originator.Contains(orginator));

                taskItems = taskItems.OrderByDescending(item => item.StartDate).ToList();
                result.Add(new KeyValueEntity() { Key = processName, Value = taskItems });
            });

            return Json(new { result = result, totals = totals }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult List(WorkView wv, int? page, int? pageSize, string startdate = "", string enddate = "")
        {
            var totalRecord = 0;

            var folio = wv.Folio;
            var orginator = wv.Originator;
            DateTime? startDate = startdate.ToBeginDateTime();
            DateTime? endDate = enddate.ToEndDateTime();
            var processNames = CacheAccessor.ProcessNamesFilter(wv.ProcessName);

            var taskItems = _workflowClientService.GetUserPendingTaskList(this.CurrentUser, page, pageSize, out totalRecord, folio, startDate, endDate, processNames,null, WLApiStyle.Prototype);

            //Run on async mode
            taskItems.StoreTaskItemUrlArgsAsync();

            if (!string.IsNullOrEmpty(orginator))
                taskItems = taskItems.Where(w => w.Originator.Contains(orginator));

            taskItems = taskItems.OrderByDescending(w => w.StartDate).ToList();

            return Json(new { data = taskItems, total = totalRecord }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Find(string start, string end, string folio, string processName, int? page = 1, int? pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            var totalRecord = 0;

            var sorting = sort.ToWLCFieldSortRules();
            DateTime? startDate = start.ToBeginDateTime();
            DateTime? endDate = end.ToEndDateTime();
            var processNames = CacheAccessor.ProcessNamesFilter(processName);
            
            var taskItems = _workflowClientService.GetUserPendingTaskList(this.CurrentUser, page, pageSize, out totalRecord, folio, startDate, endDate, processNames, sorting, WLApiStyle.Prototype);

            //Run on async mode
            taskItems.StoreTaskItemUrlArgsAsync();

            return Json(new { total = totalRecord, data = taskItems });
        }

        public JsonResult GetPendingTaskForExcel(DateTime? start, DateTime? end, string folio, string processName)
        {
            var totalRecord = 0;

            DateTime? startDate = start.ToBeginDateTime();
            DateTime? endDate = end.ToEndDateTime();
            var processNames = CacheAccessor.ProcessNamesFilter(processName);

            var taskItems = _workflowClientService.GetUserPendingTaskList(this.CurrentUser, null, null, out totalRecord, folio, startDate, endDate, processNames, null, WLApiStyle.Prototype);

            return Json(taskItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessNameList(string type)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            List<ProcessSet> processset = new List<ProcessSet>();
            if (type == "initialized")
            {
                List<Configuration_ProcessSetDTO> processsets = svc.GetProcessSets(this.CurrentUser);
                for (int i = 0; i < processsets.Count; i++)
                {
                    processset.Add(new ProcessSet()
                    {
                        FullName = processsets[i].ProcessFullName,
                        DisplayName = processsets[i].ProcessName
                    });
                }
            }
            else
            {
                WorkflowManagementService wfMngService = new WorkflowManagementService(this.AuthType);
                processset = wfMngService.GetProcessSets();
                for (int i = 0; i < processset.Count; i++)
                {
                    processset[i].DisplayName = svc.GetProcessSetByFullName(this.CurrentUser, processset[i].FullName);
                }
            }
            return Json(processset, JsonRequestBehavior.AllowGet);
        }

        private DateTime? GetProcessTime(string processFullName, int activityId, DateTime startDate)
        {
            DateTime? dt = new DateTime();
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            Configuration_ProcessSetDTO procssset = svc.GetProcessSetByFullName(this.CurrentUser, processFullName, true);
            if (procssset != null)
            {
                List<Configuration_ActivityDTO> list = new List<Configuration_ActivityDTO>();
                foreach (var version in procssset.ProcessVersionList)
                {
                    if (version != null && version.ActivityList != null && version.ActivityList.Count > 0)
                    {
                        list.AddRange(version.ActivityList);
                    }
                }

                Configuration_ActivityDTO activity = list.Where(x => x.ActivityID == activityId).FirstOrDefault();
                if (activity != null && activity.ProcessTime != null && activity.ProcessTime > 0)
                {
                    dt = startDate.AddHours(Convert.ToDouble(8 * activity.ProcessTime));
                }
                else
                {
                    dt = null;
                }
            }
            return dt;
        }

    }
}

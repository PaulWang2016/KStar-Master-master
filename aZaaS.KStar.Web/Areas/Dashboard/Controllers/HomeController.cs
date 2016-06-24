using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.KStar;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Extensions;
using aZaaS.Framework.Workflow.Util;


namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
    public class HomeController : BaseMvcController
    {
        private readonly UserService _userService;
        private readonly ProcessLogService _processLogService;

        public HomeController()
        {
            _userService = new UserService();
            _processLogService = new ProcessLogService();
        }

        public ActionResult Index(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/Parts/_DashboardView.cshtml");
            }
            return View("_SingPage");
        }

        public ActionResult PendingTasks(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/Parts/_PendingTasks.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult RequestTasks(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/Parts/_RequestTasks.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult OnGoingTasks(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/Parts/_OnGoingTasks.cshtml", true);
            }
            return View("_SingPage");
        }

        public ActionResult CompletedTasks(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/Parts/_CompletedTasks.cshtml", true);
            }
            return View("_SingPage");
        }
        public ActionResult ProcessSupervise(bool isAjax = false)
        {

            return PartialView("~/Areas/Dashboard/Views/ProcessSupervise/Index.cshtml", true);

            // return View("_SingPage");
        }
        public ActionResult MyProcessInstance(bool isAjax = false)
        {

            return PartialView("~/Areas/Dashboard/Views/MyProcessInstance/Index.cshtml", true);

            // return View("_SingPage");
        }

        public ActionResult SIMList(bool isAjax = false)
        {
            if (isAjax)
            {
                return PartialView("~/Areas/Dashboard/Views/SIMList/Index.cshtml");
            }
            return View("_SingPage");
            //return PartialView("~/Areas/Maintenance/Views/ProcessSuperviseManage/Index.cshtml");
        }

        public ActionResult ApprovalHistory(int procInstID)
        {
            if (procInstID == 0)
                throw new ArgumentOutOfRangeException("procInstID");

            ViewBag.isAsyn = true;
            return View("_History", InitApprovalHistoryData(procInstID));
        }

        #region ApprovalHistory

        private List<HistoryView> InitApprovalHistoryData(int procInstID)
        {
            List<ProcessLog> processLogItems = new List<ProcessLog>();

            processLogItems = _processLogService.GetProcessLogs(new Framework.Workflow.Pager.PageCriteria()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
                RegularFilters = new List<Framework.Workflow.Pager.RegularFilter>() { 
                    new Framework.Workflow.Pager.RegularFilter(){ 
                        FieldName="ProcInstID", 
                        Compare=aZaaS.Framework.Workflow.Pager.CriteriaCompare.Equal, 
                        Value1=procInstID }
                    }
            });

            if (processLogItems == null)
                return new List<HistoryView>();

            string datetimeFormat = PortalEnvironment.DateTimeFormat;
            var historyLogs = processLogItems.Select(s => new HistoryView()
            {
                Stage = s.ActivityName,
                TaskOwner = s.OrigUserName,
                Name = s.UserName,
                Action = s.ActionName,
                Comment = s.Comment,
                Date = s.CommentDate.ToString(datetimeFormat)
            }).ToList();

            var workflowClientServce = new WorkflowClientService(AuthenticationType.Windows);
            var pendingApprovers = workflowClientServce.GetWorkflowTaskPendingApproverData(procInstID);
            if (pendingApprovers.Any())
                historyLogs.AddRange(pendingApprovers.ToHistoryViews());

            return historyLogs;

        }

        #endregion
    }


    internal static class DTOExtensions
    {
        public static IEnumerable<HistoryView> ToHistoryViews(this IEnumerable<PendingApproverData> pendingApprovers)
        {
            var historyViews = new List<HistoryView>();

            if (pendingApprovers == null || !pendingApprovers.Any())
                return historyViews;

            foreach (var item in pendingApprovers)
            {
                var history = new HistoryView()
                {
                    Action = item.Action,
                    Comment = item.Comment,
                    Date = item.Date,
                    Name = item.Name,
                    Stage = item.Stage,
                    TaskOwner = item.TaskOwner
                };

                historyViews.Add(history);
            }

            return historyViews;
        }
    }
}

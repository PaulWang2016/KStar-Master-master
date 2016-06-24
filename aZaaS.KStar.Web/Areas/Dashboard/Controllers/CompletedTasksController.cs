
using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
    [Obsolete("该模块目前在平台中没有使用到，标记为废弃模块！")]
    [EnhancedHandleError]
    public class CompletedTasksController : BaseMvcController
    {
        //private readonly WorkflowDataService tenantDatabaseService;
        //public CompletedTasksController()
        //{
        //    tenantDatabaseService = new WorkflowDataService(this.AuthType);
        //}

        //public JsonResult Get()
        //{
        //    IEnumerable<TaskView> items = tenantDatabaseService.GetCompletedTasks(this.CurrentUser).Select(s => new TaskView()
        //    {
        //        Id = s.ID,
        //        Folio = s.FormNo,
        //        ProcInstNo = s.FormNo.Split('-').Last(),
        //        ProcSubject = s.FormNo.Split('-').First(),
        //        WorkflowStep = s.ActivityState,
        //        Status = s.FormStatus,
        //        Requester = s.RequesterName,
        //        //CurrentActivity = s.RecordLevelType == RecordLevel.Customer.ToString() ? s.CustomerType.GetStringFromat() : s.UnitType.GetStringFromat(),
        //        PropertyCode = s.PropertyCode,
        //        UnitCode = s.UnitCode,
        //        SubmitDate = s.ApplicationDate.Value,
        //        Type = s.FormType,// == @"Request" ? "TDRW - Request Form" : "TDCW - Change Form",
        //        //ImportantDate = s.ApplicationDate.Value,
        //        ClusterCode = s.CustomerCode,
        //        CustomerLevel = s.RecordLevelType,
        //        //CustomerName = s.CustomerName(),
        //        //PropertyName = s.PropertyName(),
        //        ModifiedDate = s.ModifyDate.Value,
        //        //LastActivityUser = s.ModifiedBy(),
        //        LastActivityDate = s.ActivityDate
        //    });


        //    return Json(items.OrderByDescending(s => s.LastActivityDate).ToList(), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult Find(DateTime? startDate, DateTime? endDate, string folio)
        //{
        //    startDate = InitStartDate(startDate);
        //    endDate = InitEndDate(endDate);
        //    IEnumerable<TaskView> items = tenantDatabaseService.FindCompletedTasks(this.CurrentUser, startDate.Value, endDate.Value).Select(s => new TaskView()
        //    {
        //        Id = s.ID,
        //        Folio = s.FormNo,
        //        ProcInstNo = s.FormNo.Split('-').Last(),
        //        ProcSubject = s.FormNo.Split('-').First(),
        //        WorkflowStep = s.ActivityState,
        //        Status = s.FormStatus,
        //        Requester = s.RequesterName,
        //        //CurrentActivity = s.RecordLevelType == RecordLevel.Customer.ToString() ? s.CustomerType.GetStringFromat() : s.UnitType.GetStringFromat(),
        //        PropertyCode = s.PropertyCode,
        //        UnitCode = s.UnitCode,
        //        SubmitDate = s.ApplicationDate.Value,
        //        Type = s.FormType,// == @"" ? "TDRW - Request Form" : "TDCW - Change Form",
        //        //ImportantDate = s.ApplicationDate.Value,
        //        ClusterCode = s.CustomerCode,
        //        CustomerLevel = s.RecordLevelType,
        //        ModifiedDate = s.ModifyDate.Value,
        //        //CustomerName = s.CustomerName(),
        //        //PropertyName = s.PropertyName(),
        //        //LastActivityUser = s.ModifiedBy(),
        //        LastActivityDate = s.ActivityDate
        //    });
        //    if (!string.IsNullOrEmpty(folio))
        //    {
        //        items = items.Where(x => (x.Folio ?? "").ToUpper().Contains(folio.ToUpper())).ToList();
        //    }
        //    return Json(items.OrderByDescending(s => s.LastActivityDate).ToList(), JsonRequestBehavior.AllowGet);
        //}

    }
}

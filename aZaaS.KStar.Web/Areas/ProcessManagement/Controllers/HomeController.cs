using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.ProcessManagement.Controllers
{
    public class HomeController : BaseMvcController
    {
        //
        // GET: /ProcessManagement/ProcessStart/

        public ActionResult ProcessStart(bool isAjax = false)
        {
            if (isAjax)
            {                
                return PartialView("~/Areas/ProcessManagement/Views/ProcessStart.cshtml");
            }
            return View("_SingPage");
        }

        [HttpPost]
        public JsonResult GetProcessCommonList(int ProcessCategory=0, string ProcessName="")
        {
            ConfigManager manager = new ConfigManager(this.AuthType);
            manager.TenantID = TenantID();
            var categoryList = manager.GetStartProcessList(this.CurrentUser, true,ProcessCategory,ProcessName);
            return Json(categoryList);
        }

        [HttpPost]
        public JsonResult GetProcessAllList(int ProcessCategory = 0, string ProcessName = "")
        {
            ConfigManager manager = new ConfigManager(this.AuthType);
            manager.TenantID = TenantID();
            var categoryList = manager.GetStartProcessList(this.CurrentUser, false, ProcessCategory, ProcessName);
            return Json(categoryList);
        }

        [HttpPost]
        public JsonResult SaveCommonProcess(int configProcSetID)
        {
            ConfigManager manager = new ConfigManager(this.AuthType);
            manager.TenantID = TenantID();
            manager.SaveCommonProcess(this.CurrentUser, configProcSetID);
            return GetProcessCommonList();
        }

        [HttpPost]
        public JsonResult DeleteCommonProcess(int configProcSetID)
        {
            ConfigManager manager = new ConfigManager(this.AuthType);
            manager.TenantID = TenantID();
            manager.DeleteCommonProcess(this.CurrentUser, configProcSetID);
            return GetProcessCommonList();
        }
    }
}

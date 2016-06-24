using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Workflow.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class ProcessPermissionController : BaseMvcController
    {
        //
        // GET: /Maintenance/ProcessPermission/

        public JsonResult SaveRoleProcess(Guid roleId, List<string> addProcess, List<string> removeProcess)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();

            if (addProcess != null)
                foreach (var process in addProcess)
                {
                    svc.AddRoleProcessSet(roleId, process);
                }
            if (removeProcess != null)
                foreach (var process in removeProcess)
                {
                    svc.DeleteRoleProcessSet(roleId, process);
                }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessByRole(Guid roleid)
        {
            List<Process> process = new List<Process>();
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            List<Role_ProcessSetDTO> roleprocess = svc.GetRoleProcessSet(roleid, this.CurrentUser);
            process = roleprocess.Select(set => new Process()
            {
                Name = set.ProcessFullName,
                Folder = "",
                FullName = set.ProcessFullName,
                DisplayName = set.ProcessDispalyName
            }).ToList();
            return Json(process, JsonRequestBehavior.AllowGet);
        }

    }
}

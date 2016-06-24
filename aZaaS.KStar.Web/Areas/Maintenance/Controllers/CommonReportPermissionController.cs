using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.Web.Models.BasisEntity;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class CommonReportPermissionController : BaseMvcController
    {
        //
        // GET: /Maintenance/CommonReportPermission/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult SaveRoleProcess(string roleId, List<string> addProcess, List<string> removeProcess)
        {

            if (addProcess != null)
                foreach (var process in addProcess)
                {
                    AddRoleProcessSet(roleId, process);
                }
            if (removeProcess != null)
                foreach (var process in removeProcess)
                {
                    DeleteRoleProcessSet(roleId, process);
                }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcessByRole(string roleid)
        {
            List<Process> process = new List<Process>();

            try
            {
                using (BasisEntityContainer dbContext = new BasisEntityContainer())
                {
                    var result = dbContext.CommonReportPermission.Where(p => p.Role_SysId == roleid).ToList();

                    foreach (CommonReportPermission crp in result)
                    {
                        Process p = new Process();
                        p.Name = crp.ProcessFullName;
                        p.Folder = "";
                        p.FullName = crp.ProcessFullName;
                        p.DisplayName = GetProcessSetByFullName(crp.ProcessFullName);
                        process.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
           
            return Json(process, JsonRequestBehavior.AllowGet);
        }
        public void AddRoleProcessSet(string roleid, string processFullName)
        {
           // BasisEntityContainer
            
            using (BasisEntityContainer dbContext = new BasisEntityContainer())
            {
                var process = dbContext.CommonReportPermission.Where(x => x.Role_SysId == roleid && x.ProcessFullName == processFullName).FirstOrDefault();
                if (process == null)
                {
                    dbContext.CommonReportPermission.Add(new CommonReportPermission() { ProcessFullName = processFullName, Role_SysId = roleid });
                    dbContext.SaveChanges();
                }
            }
        }

        public void DeleteRoleProcessSet(string roleid, string processFullName)
        {
            using (BasisEntityContainer dbContext = new BasisEntityContainer())
            {
                var process = dbContext.CommonReportPermission.Where(x => x.Role_SysId == roleid && x.ProcessFullName == processFullName).FirstOrDefault();
                if (process != null)
                {
                    dbContext.CommonReportPermission.Remove(process);
                    dbContext.SaveChanges();
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class ProcessController : BaseMvcController
    {
        private readonly AppDelegateFacade appelegate;
        public ProcessController()
        {
            appelegate = new AppDelegateFacade(this.AuthType);
        }

        public JsonResult Get()
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            WorkflowManagementService wfMngService = new WorkflowManagementService(this.AuthType);
            IEnumerable<Process> process = wfMngService.GetProcessSets().Select(set => new Process()
            {
                Name = set.Name,
                Folder = set.Folder,
                FullName = set.FullName,
                DisplayName = svc.GetProcessSetByFullName(this.CurrentUser, set.FullName)
            });

            //IEnumerable<Process> process = appelegate.GetProcessSet().Select(set => new Process()
            //{
            //    Name = set.Name,
            //    Folder = set.Folder,
            //    FullName = set.FullName
            //});


            return Json(process, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcess(string key)
        {
            List<Process> process = new List<Process>();
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            WorkflowManagementService wfMngService = new WorkflowManagementService(this.AuthType);
            var processset = wfMngService.GetProcessSets();
            process = processset.Select(set => new Process()
            {
                Name = set.FullName,
                Folder = set.Folder,
                FullName = set.FullName,
                DisplayName = svc.GetProcessSetByFullName(this.CurrentUser, set.FullName)
            }).ToList();

            if (!string.IsNullOrEmpty(key))
            {
                process = process.Where(x => x.DisplayName.Contains(key) || x.FullName.ToLower().Contains(key.ToLower())).ToList();
            }
            return Json(process, JsonRequestBehavior.AllowGet);
        }

    }
}

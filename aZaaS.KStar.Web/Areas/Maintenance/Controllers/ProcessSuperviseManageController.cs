using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Acs;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.ViewModel;
using System.Data;
using aZaaS.Kstar.DAL;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class ProcessSuperviseManageController : BaseMvcController
    {
        RoleBO roleBO = new RoleBO();
        //
        // GET: /Maintenance/ProcessSuperviseManage/
        private AcsManager _acsManager = new AcsManager();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetRoleList()
        {
            List<Role> roleList = new List<Role>();
            foreach (var item in roleBO.GetAllRoles())
            {
                roleList.Add(new Role { ID = item.SysID.ToString(), Name = item.Name });
            }
            return Json(roleList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoleProcessList(string roleID)
        {
            List<RoleProcess> roleList = new List<RoleProcess>();
            var table = CustomExtUtility.GetRoleProcessByRoleID(roleID);
            foreach(DataRow r in table.Rows)
            {
                RoleProcess p = new RoleProcess();
                p.RoleID = r["RoleSysId"].ToString();
                p.ProcessFullName = r["ProcessFullName"].ToString();
                roleList.Add(p);
            }

            return Json(roleList, JsonRequestBehavior.AllowGet);
        }

    }
    public class RoleProcess
    {
        public string RoleID { get; set; }
        public string ProcessFullName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Acs;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class PermissionController : BaseMvcController
    {
        //
        // GET: /Maintenance/Permissions/
        private AcsManager _acsManager = new AcsManager();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPermission()
        {
            return Json(_acsManager.GetPermission(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreatePermission(Permission item)
        {
            _acsManager.CreatePermission(item);
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdatePermission(Permission item)
        {
            _acsManager.UpdatePermission(item);
            return Json(item, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelPermission(List<string> idList)
        {
            _acsManager.DelPermission(idList);
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
    }
}

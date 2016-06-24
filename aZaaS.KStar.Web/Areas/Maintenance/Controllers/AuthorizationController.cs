using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Acs;
using aZaaS.Framework.ACS.Core;
using aZaaS.Framework.Facade;
using aZaaS.KStar.Web.Areas.Maintenance.Models;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class AuthorizationController : BaseMvcController
    {
        //
        // GET: /Maintenance/Authorization/
        private AcsManager _acsManager = new AcsManager();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUserAndRole()
        {
            return Json(_acsManager.GetUserAndRole(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindUserAndRole(string input)
        {
            return Json(_acsManager.FindUserAndRole(input), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindResourcePermission(string input)
        {
            return Json(_acsManager.FindResourcePermisssion(input), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindAuthori(string input)
        {
            return Json(_acsManager.FindAuthori(input), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllAuthorization()
        {
            return Json(_acsManager.GetAllAuthorization(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuthorization(Guid id)
        {

            return Json(_acsManager.GetAuthorization(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateAuthorization(Guid AuthorityId, bool Granted, List<Guid> ResourcePermissionIdList)
        {
            _acsManager.CreateAuthorization(AuthorityId, Granted, ResourcePermissionIdList);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAuthorization(AuthorizationMatrix item)
        {
            _acsManager.UpdateAuthorization(item);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelAuthorization(List<string> idList)
        {
            _acsManager.DelAuthorization(idList);
            return Json(idList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DisableAuthorization(List<string> idList)
        {
            _acsManager.DisableAuthorization(idList);

            return Json(idList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EnableAuthorization(List<string> idList)
        {
            _acsManager.EnableAuthorization(idList);
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
    }
}

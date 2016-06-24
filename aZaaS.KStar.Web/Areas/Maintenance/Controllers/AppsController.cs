using aZaaS.KStar;
using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class AppsController : BaseMvcController
    {
        private static readonly MenuFacade _menufacade = new MenuFacade();

        public JsonResult GetApps()
        {
            return Json(_menufacade.GetTop(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateMenu(Menu menu)
        {
            menu.Id = Guid.NewGuid();
            return Json(_menufacade.CreateMenu(menu), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateMenu(Menu menu)
        {
            return Json(_menufacade.UpdateMenu(menu), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelMenu(List<string> keyList)
        {
            return Json(_menufacade.DelMenu(keyList), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMenuItem(string key, string Id = "")
        {
            List<MenuItem> model;
            if (Id == "")
            {
                model = _menufacade.GetMenuItem(key);
            }
            else
            {
                model = _menufacade.GetMenuItem(key, Id);
            }
            return Json(model.Select(item => new
            {
                Id = item.Id,
                DisplayName = item.DisplayName,
                Hyperlink = item.Hyperlink,
                IconKey = item.IconKey,
                Kind = item.Kind,
                Target = item.Target.ToString(),
                ParentId = item.Parent == null ? null : item.Parent.Id.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult CreateMenuItem(MenuItem menu, string menuKey, string ParentId = "")
        {

            var item = _menufacade.CreateMenuItem(menu, menuKey, ParentId);
            return Json(new { Id = item.Id, DisplayName = item.DisplayName, Hyperlink = item.Hyperlink, IconKey = item.IconKey, Kind = item.Kind, Target = item.Target.ToString(), ParentId = item.Parent == null ? null : item.Parent.Id.ToString() }, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public JsonResult UpdateMenuItem(MenuItem menu, string ParentId = "")
        {
            var item = _menufacade.UpdateMenuItem(menu, ParentId);
            return Json(new { Id = item.Id, DisplayName = item.DisplayName, Hyperlink = item.Hyperlink, IconKey = item.IconKey, Kind = item.Kind, Target = item.Target.ToString(), ParentId = item.Parent == null ? null : item.Parent.Id.ToString() }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelMenuItem(string id)
        {
            _menufacade.DelMenuItem(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}

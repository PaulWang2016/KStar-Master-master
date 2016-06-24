using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Areas.Maintenance.Models;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.Acs;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class ResourcePermissionController : BaseMvcController
    {
        //
        // GET: /Maintenance/ResourcePermissions/
        private AcsManager _acsManager = new AcsManager();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAllResourcePermission()
        {

            return Json(_acsManager.GetAllResourcePermission(), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetResourceList()
        {
            ResourceFacade resource = new ResourceFacade();
            var test = resource.GetResources();
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetResourcePermission(Guid id)
        {
            return Json(_acsManager.GetResourcePermission(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindResource(string input)
        {
            ResourceFacade resource = new ResourceFacade();
            var test = resource.FindResources(input);
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FindPermission(string input)
        {

            return Json(_acsManager.FindPermission(input), JsonRequestBehavior.AllowGet);
        }

        //SqlMethods.Like(p.Description, "%" + p.Customer.Name + "%"
        public JsonResult CreateResourcePermission(Guid PermissionSysId, string ResourceType, List<string> ResourceIdList)
        {
            _acsManager.CreateResourcePermission(PermissionSysId, ResourceType, ResourceIdList);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DelResourcePermission(List<string> idList)
        {
            _acsManager.DelResourcePermission(idList);
            return Json(idList, JsonRequestBehavior.AllowGet);
        }


        #region 已经扔到KStar
        //private List<ResourcePermissionView> _GetAllResourcePermission()
        //{
        //    using (AMSDbContext context = new AMSDbContext("aZaaSAMSDB"))
        //    {
        //        List<ResourcePermissionView> list = new List<ResourcePermissionView>();
        //        var items = acs.ResourcePermissions().Select(s => new ResourcePermissionView()
        //         {
        //             Permission = s.Permission.Name,
        //             PermissionSysId = s.PermissionSysId,
        //             ResourceId = s.ResourceId,
        //             ResourceType = s.ResourceType,
        //             SysId = s.SysId
        //         });
        //        MenuFacade menu = new MenuFacade();
        //        DocumentFacade Doc = new DocumentFacade();
        //        WidgetFacade widget = new WidgetFacade();
        //        foreach (var item in items)
        //        {
        //            switch (item.ResourceType)
        //            {
        //                case "MenuItem":
        //                    item.Resource = menu.GetMenuItem(Guid.Parse(item.ResourceId)).DisplayName;
        //                    break;
        //                case "Menu":
        //                    item.Resource = menu.GetMenu(Guid.Parse(item.ResourceId)).DisplayName;
        //                    break;
        //                case "DocumentLibrary":
        //                    item.Resource = Doc.GetDocumentLibrary(Guid.Parse(item.ResourceId)).DisplayName;
        //                    break;
        //                case "DocumentItem":
        //                    item.Resource = Doc.GetDocumentItem(Guid.Parse(item.ResourceId)).DisplayName;
        //                    break;
        //                case "Widget":
        //                    item.Resource = widget.GetWidget(Guid.Parse(item.ResourceId)).Title;
        //                    break;
        //                case "": break;
        //            }
        //            list.Add(item);
        //        }
        //        return list;
        //    }
        //}
        //private ResourcePermissionView _GetResourcePermission(Guid id)
        //{
        //    ResourcePermissionView item;

        //    item = acs.ResourcePermissions().Where(s => s.SysId == id).Select(s => new ResourcePermissionView()
        //      {
        //          PermissionSysId = s.PermissionSysId,
        //          ResourceId = s.ResourceId.ToString(),
        //          SysId = s.SysId,
        //          ResourceType = s.ResourceType
        //      }).FirstOrDefault();
        //    MenuFacade menu = new MenuFacade();
        //    DocumentFacade Doc = new DocumentFacade();
        //    WidgetFacade widget = new WidgetFacade();
        //    switch (item.ResourceType)
        //    {
        //        case "MenuItem":
        //            item.Resource = menu.GetMenuItem(Guid.Parse(item.ResourceId)).DisplayName;
        //            break;
        //        case "Menu":
        //            item.Resource = menu.GetMenu(Guid.Parse(item.ResourceId)).DisplayName;
        //            break;
        //        case "DocumentLibrary":
        //            item.Resource = Doc.GetDocumentLibrary(Guid.Parse(item.ResourceId)).DisplayName;
        //            break;
        //        case "DocumentItem":
        //            item.Resource = Doc.GetDocumentItem(Guid.Parse(item.ResourceId)).DisplayName;
        //            break;
        //        case "Widget":
        //            item.Resource = widget.GetWidget(Guid.Parse(item.ResourceId)).Title;
        //            break;
        //        default: break;
        //    }
        //    item.Permission = acs.Permissions().Where(m => m.SysId == item.PermissionSysId).Select(m => m.Name).FirstOrDefault();

        //    return item;
        //}

        #endregion
    }

}

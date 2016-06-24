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
    public class DocumentsController : BaseMvcController
    {
        private static readonly DocumentFacade _docfacade = new DocumentFacade();

        public JsonResult GetMenuKey()
        {
            MenuFacade facade = new MenuFacade();
            return Json(facade.GetTop().Select(s => new { id = s.Id, key = s.Key }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLibrary()
        {
            var test = _docfacade.GetAllLibrary().Select(s => new
            {
                Id = s.Id,
                Items = s.Items,
                Key = s.Key,
                MenuID = s.MenuID,
                IconPath = s.IconPath,
                DisplayName = s.DisplayName,
                MenuKey = _docfacade.GetMenuKeyById(s.MenuID)
            }).ToList();
            return Json(_docfacade.GetAllLibrary().Select(s => new
            {
                Id = s.Id,
                Items = s.Items,
                Key = s.Key,
                MenuID = s.MenuID,
                IconPath = s.IconPath,
                DisplayName = s.DisplayName,
                MenuKey = _docfacade.GetMenuKeyById(s.MenuID)
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLibrary(Guid DocumentLibraryID)
        {
            return Json(_docfacade.GetLibrary(DocumentLibraryID, true), JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateLibrary(DocumentLibrary lib, string MenuKey)
        {
            MenuFacade facade = new MenuFacade();
            lib.Id = Guid.NewGuid();
            lib.MenuID = facade.GetTop().Where(m => m.Key == MenuKey).Select(s => s.Id).FirstOrDefault();
            _docfacade.CreateLibrary(lib, null);
            var model = _docfacade.GetLibrary(lib.Id, false);
            return Json(new
            {
                Id = model.Id,
                Items = model.Items,
                Key = model.Key,
                MenuID = model.MenuID,
                IconPath = model.IconPath,
                DisplayName = model.DisplayName,
                MenuKey = _docfacade.GetMenuKeyById(model.MenuID)
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateLibrary(DocumentLibrary lib, string MenuKey)
        {
            MenuFacade facade = new MenuFacade();
            lib.MenuID = facade.GetTop().Where(m => m.Key == MenuKey).Select(s => s.Id).FirstOrDefault();
            _docfacade.UpdateLibrary(lib);
            var model = _docfacade.GetLibrary(lib.Id, false);
            return Json(new
            {
                Id = model.Id,
                Items = model.Items,
                Key = model.Key,
                MenuID = model.MenuID,
                IconPath = model.IconPath,
                DisplayName = model.DisplayName,
                MenuKey = _docfacade.GetMenuKeyById(lib.MenuID)
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateDocumentItem(DocumentItem dto, Guid DocumentLibraryID)
        {
            dto.SysID = Guid.NewGuid();
            _docfacade.CreateDocumentItem(dto, DocumentLibraryID);

            return Json(dto, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDocumentItem(DocumentItem dto)
        {
            _docfacade.UpdateDocumentItem(dto);

            return Json(dto, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelLibrary(List<Guid> idList)
        {
            return Json(_docfacade.DelLibrary(idList), JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelDocumentItem(List<Guid> idlist)
        {
            return Json(_docfacade.DelDocumentItem(idlist), JsonRequestBehavior.AllowGet);
        }

    }
}

using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class MyDraftsController : BaseMvcController
    {
        private IStorageProvider _storageProvider;

        public MyDraftsController()
        {
            _storageProvider = new KStarFormStorageProvider();
        }

        public JsonResult Get(DateTime? startDate, DateTime? endDate, string folio, int page = 1, int pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            int total = 0;
            if (startDate == null)
            {
               startDate=InitStartDate(startDate);
            }
            if (endDate == null)
            {
                endDate = InitEndDate(endDate);
            }
            var items = _storageProvider.GetDraftsByPage(this.CurrentUser, startDate, endDate,folio, page, pageSize, sort, out total).ToList();

            return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMyDraftsForExcel(DateTime? startDate, DateTime? endDate, string folio)
        {
            int total = 0;
            if (startDate == null)
            {
                startDate = InitStartDate(startDate);
            }
            if (endDate == null)
            {
                endDate = InitEndDate(endDate);
            }
            var items = _storageProvider.GetDraftsByPage(this.CurrentUser, startDate, endDate, folio, 1, int.MaxValue, new List<SortDescriptor>(), out total).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDraft(int draftId)
        {
            _storageProvider.DeleteDraft(draftId);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Find(DateTime startDate, DateTime? endDate, string folio)
        {
            var items = _storageProvider.GetDrafts(CurrentUser);

            if (startDate != null)
            {
                items = items.Where(r => r.CreatedDate >= startDate).ToList();
            }

            if (endDate != null)
            {
                items = items.Where(r => r.CreatedDate <= endDate).ToList();
            }

            if (!string.IsNullOrEmpty(folio))
            {
                items = items.Where(r => r.FormSubject == folio).ToList();
            }

            return Json(items.OrderByDescending(r => r.CreatedDate), JsonRequestBehavior.AllowGet);
        }
    }
}

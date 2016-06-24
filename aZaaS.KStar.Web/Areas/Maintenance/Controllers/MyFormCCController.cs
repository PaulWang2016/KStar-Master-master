using aZaaS.KStar.Form;
using aZaaS.KStar.Form.Infrastructure;
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
    public class MyFormCCController : BaseMvcController
    {
        private IFormCCProvider _formCCProvider;

        public MyFormCCController()
        {
            _formCCProvider = new KStarFormCCProvider();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyFormCC(string startDate, string endDate, string formSubject, string processName, string receiveStatus)
        {
            var items = _formCCProvider.SendFormCC(CurrentUser);

            DateTime? start = null, end = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                start = DateTime.Parse(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                end = DateTime.Parse(endDate);
            }
            start = InitStartDate(start);
            end = InitEndDate(end);

            if (start != null)
            {
                items = items.Where(r => r.CreatedDate >= start).ToList();
            }

            if (end != null)
            {
                items = items.Where(r => r.CreatedDate <= end).ToList();
            }

            if (!string.IsNullOrWhiteSpace(formSubject))
            {
                items = items.Where(r => r.ProcInstNo.Contains(formSubject)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(processName))
            {
                items = items.Where(r => r.ProcessFolio.Contains(processName)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(receiveStatus))
            {
                items = items.Where(r => r.ReceiverStatus == (receiveStatus == "1" ? true : false)).ToList();
            }

            items = items.OrderByDescending(r => r.CreatedDate).ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormCCToMe(string startDate, string endDate, string formSubject, string processName, string receiveStatus)
        {
            var items = _formCCProvider.ReceiveFormCC(CurrentUser);

            DateTime? start = null, end = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                start = DateTime.Parse(startDate);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                end = DateTime.Parse(endDate);
            }
            start = InitStartDate(start);
            end = InitEndDate(end);

            if (start != null)
            {
                items = items.Where(r => r.CreatedDate >= start).ToList();
            }

            if (end != null)
            {
                items = items.Where(r => r.CreatedDate <= end).ToList();
            }

            if (!string.IsNullOrWhiteSpace(formSubject))
            {
                items = items.Where(r => r.ProcInstNo.Contains(formSubject)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(processName))
            {
                items = items.Where(r => r.ProcessFolio.Contains(processName)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(receiveStatus))
            {
                items = items.Where(r => r.ReceiverStatus == (receiveStatus == "1" ? true : false)).ToList();
            }

            items = items.OrderByDescending(r => r.CreatedDate).ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult MyFormCC(string startDate, string endDate, string formSubject, string processName, string receiveStatus, int page = 1, int pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        //{
        //    int total = 0;
        //    DateTime? start = null, end = null;
        //    if (!string.IsNullOrEmpty(startDate))
        //    {
        //        start = DateTime.Parse(startDate);
        //    }
        //    if (!string.IsNullOrEmpty(endDate))
        //    {
        //        end = DateTime.Parse(endDate);
        //    }
        //    start = InitStartDate(start);
        //    end = InitEndDate(end);
        //    var items = _formCCProvider.SendFormCC(CurrentUser, start, end, receiveStatus, page, pageSize, sort, out total);
        //    items = items.OrderByDescending(r => r.CreatedDate).ToList();
        //    return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult FormCCToMe(string startDate, string endDate, string formSubject, string processName, string receiveStatus, int page = 1, int pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        //{
        //    int total = 0;
        //    DateTime? start = null, end = null;
        //    if (!string.IsNullOrEmpty(startDate))
        //    {
        //        start = DateTime.Parse(startDate);
        //    }
        //    if (!string.IsNullOrEmpty(endDate))
        //    {
        //        end = DateTime.Parse(endDate);
        //    }
        //    start = InitStartDate(start);
        //    end = InitEndDate(end);

        //    var items = _formCCProvider.ReceiveFormCC(CurrentUser, start, end, receiveStatus, page, pageSize, sort, out total);
        //    items = items.OrderByDescending(r => r.CreatedDate).ToList();

        //    return Json(new { total = total, data = items }, JsonRequestBehavior.AllowGet);
        //}
    }
}

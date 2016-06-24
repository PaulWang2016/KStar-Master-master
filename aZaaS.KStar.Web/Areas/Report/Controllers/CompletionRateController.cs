using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Report;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    public class CompletionRateController : Controller
    {
        //
        // GET: /Report/CompletionRate/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCompletionRate(string _sDate = "", string _fDate = "", string _deptId = "", string _procSetID = "", string _startUserId = "", int _processCategory = 0)
        {
            ReportDaoManager dto = new ReportDaoManager();
            //return Json(new Random().Next(1,100), JsonRequestBehavior.AllowGet);
            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(_sDate))
            {
                _startDate = DateTime.ParseExact(_sDate, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(_fDate))
            {
                _endDate = DateTime.ParseExact(_fDate, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            return Json(dto.GetProcInstCount(_startDate, _endDate
                , _deptId, _procSetID, _startUserId, _processCategory) * 100, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcInstByStatus(string _sDate = "", string _fDate = "", string _deptId = "", int _procSetID = 0
            , string _startUserId = "", int _processCategory = 0, int _status = 0, int page = 1, int pageSize = 20)
        {
            ReportDaoManager dto = new ReportDaoManager();
            //return Json(new Random().Next(1,100), JsonRequestBehavior.AllowGet);
            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(_sDate))
            {
                _startDate = DateTime.ParseExact(_sDate, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(_fDate))
            {
                _endDate = DateTime.ParseExact(_fDate, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            int total = 0;
            List<ProcDealDurationMasterDTO> list = dto.GetProcInstByStatus(out total,_startDate, _endDate
                , _deptId, _procSetID, _startUserId, _processCategory,_status,page,pageSize);
            return Json(new {
                total = total,
                data = list
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcInstGroupByStatus(string _sDate = "", string _fDate = "", string _deptId = "", string _procSetID = "", string _startUserId = "", int _processCategory = 0)
        {
            ReportDaoManager dto = new ReportDaoManager();
            //return Json(new Random().Next(1,100), JsonRequestBehavior.AllowGet);
            DateTime? _startDate = null;
            if (!string.IsNullOrEmpty(_sDate))
            {
                _startDate = DateTime.ParseExact(_sDate, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }

            DateTime? _endDate = null;
            if (!string.IsNullOrEmpty(_fDate))
            {
                _endDate = DateTime.ParseExact(_fDate, "dd/MM/yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }

            List<ProcInstStatusGroupDTO> list = dto.GetProcInstGroupByStatus(_startDate, _endDate
                , _deptId, _procSetID, _startUserId, _processCategory);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}

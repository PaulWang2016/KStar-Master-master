using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Report;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    public class ProcDealDurationController : Controller
    {
        //
        // GET: /Report/CompletionRate/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProcDealDurationList(string _sDate = "", string _fDate = "", string _deptId = "", string _procSetID = "", string _startUserId = "", int _processCategory = 0, int page = 1, int pageSize = 20)
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
            List<ProcDealDurationMasterDTO> list = dto.GetProcDealDurationList(out total,_startDate, _endDate
                , _deptId, _procSetID, _startUserId, _processCategory,page,pageSize);
            foreach(ProcDealDurationMasterDTO entity in list){
                entity.TotalConsumingSecondStr = TimeFormat(entity.TotalConsumingSecond);
            }
            return Json(new
            {
                data = list,
                total = total
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProcDealDurationItemList(int _procInstId=0)
        {
            ReportDaoManager dto = new ReportDaoManager();
            List<ProcDealDurationItemDTO> list = dto.GetProcDealDurationItemList(_procInstId);
            foreach (ProcDealDurationItemDTO entity in list)
            {
                entity.TotalConsumingSecondStr = TimeFormat(entity.TotalConsumingSecond);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        
        /// <summary>
        /// 审批时间：分钟，如果大于24*60 则显示x天xx小时xx分钟，否则显示xx小时xx分钟xx秒，如果不够1小时，则显示xx分钟xx秒
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string TimeFormat(int totals)
        {
            if (totals == 0) return "0 ";

            System.Text.StringBuilder result = new System.Text.StringBuilder();

            TimeSpan ts = new TimeSpan(0, 0, totals);

            if (ts.Days != 0) result.Append(ts.Days.ToString() + "天 ");
            if (ts.Hours != 0) result.Append(ts.Hours.ToString() + "时 ");
            if (ts.Minutes != 0) result.Append(ts.Minutes.ToString() + "分 ");
            if (ts.Days == 0 && ts.Seconds != 0) result.Append(ts.Seconds.ToString() + "秒 ");

            return result.ToString();

        }
    }
}

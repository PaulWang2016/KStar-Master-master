using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.DTOs.Report;
using aZaaS.KStar.Report;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    public class ProcessApprovalConsumingSecondController : Controller
    {
        //
        // GET: /Report/CompletionRate/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 流程实例处理时间top10
        /// </summary>
        /// <param name="_sDate"></param>
        /// <param name="_fDate"></param>
        /// <param name="_topOrbottom"></param>
        /// <param name="_procSetID"></param>
        /// <param name="_processCategory"></param>
        /// <returns></returns>
        public JsonResult GetProcessApprovalConsumingSecondData(string _sDate = "", string _fDate = "", int _topOrbottom = 0, string _procSetID = "", int _processCategory = 0, int _actId = 0)
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
            List<ProcessApprovalConsumingSecondDTO> list = dto.GetProcessApprovalConsumingSecondData(_startDate, _endDate
                , _topOrbottom, _procSetID, _processCategory, _actId);
            foreach (ProcessApprovalConsumingSecondDTO entity in list)
            {
                entity.CasumeSecondFomatStr = TimeFormat(entity.CasumeSecond);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 流程实例环节处理时候
        /// </summary>
        /// <param name="_procIntID"></param>
        /// <returns></returns>
        public JsonResult GetProcessActConsumingSecondData(int _procIntID=0)
        {
            ReportDaoManager dto = new ReportDaoManager();
            List<ProcessApprovalConsumingSecondDTO> list = dto.GetProcessActConsumingSecondData(_procIntID);
            foreach (ProcessApprovalConsumingSecondDTO entity in list)
            {
                entity.CasumeSecondFomatStr = TimeFormat(entity.CasumeSecond);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProcActList(int _procSetID=0)
        {
            ReportDaoManager dto = new ReportDaoManager();
            return Json(dto.getProcActList(_procSetID), JsonRequestBehavior.AllowGet);
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

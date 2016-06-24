using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.Configuration;
using aZaaS.Framework.Facade;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Logging;
using aZaaS.Kstar.DAL;
using aZaaS.KStar.Helper;
using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Areas.Dashboard.Controllers;



namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class ProcessInstanceManageAddSignerController : Controller
    {
        //
        // GET: /Maintenance/ProcessInstanceManageAddSigner/
        string viewFlow = ConfigurationManager.AppSettings["ViewFlowUrl"].ToString();
        string k2server = ConfigurationManager.AppSettings["ServerName"].ToString();
        string windowDomain = ConfigurationManager.AppSettings["WindowDomain"].ToString();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 取所有待办列表（管理员)
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="Folio"></param>
        /// <param name="startDate"></param>
        /// <param name="finishDate"></param>
        /// <param name="StartUser"></param>
        /// <returns></returns>
        public JsonResult GetProcessInstanceList(string processName, string Folio, string startDate, string finishDate, string StartUser)
        {

            if (string.IsNullOrEmpty(processName))
            {
                processName = "";
            }
            int pageIndex = 0;
            string page = Request["page"];
            string Size = Request["pageSize"];
            int pageSize = 0;
            int.TryParse(page, out pageIndex);
            int.TryParse(Size, out pageSize);
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (pageSize == 0)
            {
                pageSize = 20;
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                startDate = Convert.ToDateTime(startDate).ToString("yyyy-MM-dd");
            }
            if (!string.IsNullOrEmpty(finishDate))
            {
                finishDate = Convert.ToDateTime(finishDate).ToString("yyyy-MM-dd");
            }
            //Status = 0;
            var result = CustomExtUtility.GetAllProcessInstSign(processName, Folio, startDate, finishDate, StartUser, 0, pageSize, pageIndex, "");
            List<ProcessInstView> list = new List<ProcessInstView>();
            foreach (DataRow r in result.Tables[0].Rows)
            {
                ProcessInstView pv = new ProcessInstView();
                pv.ID = r["ID"].ToString();
                pv.ProcessName = r["ProcessName"].ToString();
                string ProcessFolio = r["Folio"].ToString();
                pv.Title = ProcessFolio.Split('-').First();
                pv.FlowNo = ProcessFolio.Split('-').Last();
                pv.StartUser = r["StartName"].ToString();
                pv.OperateUser = r["HandlerUser"].ToString();
                pv.Folio = r["Folio"].ToString();
                pv.StartDate = r["StartDate"].ToString();
                pv.FinishDate = r["FinishDate"].ToString();
                pv.Originator = r["Originator"].ToString();
                pv.Status = r["Status"].ToString();
                pv.ActName = r["ActName"].ToString();
                if (!pv.Status.Equals("已完成") && !pv.Status.Equals("错误") && !pv.Status.Equals("已删除"))
                {
                    try
                    {
                        pv.TaskStartDate = Convert.ToDateTime(r["TaskStartDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                        pv.RuningTime = RunHowLongTime(r["TaskStartDate"].ToString());
                    }
                    catch
                    {
                        pv.TaskStartDate = string.Empty;
                        pv.RuningTime = string.Empty;
                    }
                }
                else
                {
                    pv.TaskStartDate = string.Empty;
                    pv.RuningTime = string.Empty;
                }
                pv.SN = Convert.ToString(r["SN"] == DBNull.Value ? string.Empty : r["SN"]);
                pv.Destination = r["Destination"].ToString();
                pv.ViewUrl = r["ViewUrl"].ToString().Replace("{_FormId}", r["FormId"].ToString());
                pv.ViewFlowUrl = viewFlow + "?K2Server=" + k2server + "&ProcessID=" + pv.ID;
                list.Add(pv);
            }
            int total = 0;
            string rowcount = result.Tables[1].Rows[0][0].ToString();
            int.TryParse(rowcount, out total);
            var output = new { data = list, total = total };
            var json = Json(output, JsonRequestBehavior.AllowGet);
            return json;
        }
        public string RunHowLongTime(string TaskDate)
        {
            try
            {
                var CurrentDate = DateTime.Now;
                var TaskStartDate = Convert.ToDateTime(TaskDate);
                TimeSpan ts = CurrentDate - TaskStartDate;
                string runingTime = "已停留";
                if (ts.Days >= 1)
                {
                    runingTime += string.Format("{0}天", ts.Days);
                }
                if (ts.Hours >= 1)
                {
                    runingTime += string.Format("{0}小时", ts.Hours < 10 ? "0" + ts.Hours.ToString() : ts.Hours.ToString());
                }
                if (ts.Minutes >= 1)
                {
                    runingTime += string.Format("{0}分", ts.Minutes);
                }
                //= string.Format("运行{0}天{1}时{2}分", ts.TotalDays, ts.TotalHours, ts.TotalMinutes);
                return runingTime + "-" + ts.TotalHours;
            }
            catch { return string.Empty; }
        }

    }
}


using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.DTOs;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Areas.Dashboard.Controllers;
using aZaaS.Kstar.DAL;
using System.Data;
using System.Configuration;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Facade;
using aZaaS.Framework;
using aZaaS.Framework.Logging;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    public class DimissionRedirectController : BaseMvcController
    {
        //
        // GET: /Maintenance/DimissionRedirect/
        string viewFlow = ConfigurationManager.AppSettings["ViewFlowUrl"].ToString();
        string k2server = ConfigurationManager.AppSettings["ServerName"].ToString();
        Framework.Workflow.AuthenticationType AuthType = Framework.Workflow.AuthenticationType.Form;
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Find(string DimissionUserName)
        {
            //this.CurrentUser
            string page = Request["page"];
            string Size = Request["pageSize"];
            int pageSize = 0;
            int pageIndex = 0;
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
            if (!string.IsNullOrEmpty(DimissionUserName))
            {
                var result = CustomExtUtility.GetProcessInstanceByUserName(DimissionUserName, pageIndex, pageSize);
                List<ProcessInstView> list = new List<ProcessInstView>();
                foreach (DataRow r in result.Tables[0].Rows)
                {
                    ProcessInstView pv = new ProcessInstView();
                    pv.ID = r["ProcInstID"].ToString();
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
                    pv.TaskStartDate = Convert.ToDateTime(r["TaskStartDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                    pv.RuningTime = RunHowLongTime(r["TaskStartDate"].ToString());
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
            return null;
        }
        public string RunHowLongTime(string TaskDate)
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
            
            //WorkflowClientService ws = new WorkflowClientService(AuthType);
            //ws.RedirectAll("", "");
        }
        public JsonResult GetProcessSetConfigByUserName(string UserName)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                ConfigManager config = new ConfigManager(AuthType);
                var list = config.GetRelatedProcessSetByUserName(UserName);
                var output = new { data = list, total = list.Count };
                var json = Json(output, JsonRequestBehavior.AllowGet);
                return json;
            }
            return null;
        }
        public JsonResult LeaveUserRedirctTo(string LeaveUser, string RedirectUser)
        {
            try
            {
                WorkflowClientService ws = new WorkflowClientService(AuthType);
                ws.RedirectAll(LeaveUser, RedirectUser);
                ConfigManager config = new ConfigManager(AuthType);
                var Success = config.TransferStartPermission(LeaveUser, RedirectUser);
                if (!Success)
                {
                    return Json(new { Status = -1 });
                }

                return Json(new { Status = 1 });
            }
            catch (Exception ex)
            {
                LogFactory.GetLogger().Write(new LogEvent
                {
                    Source = "离职转交方法",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
                return Json(new { Status = -1 });
            }
        }

    }
}

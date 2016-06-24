using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using aZaaS.Framework.Facade;
using aZaaS.Framework;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Logging;
using aZaaS.Kstar.DAL;
using aZaaS.KStar.Helper;



namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
    [EnhancedHandleError]
    public class ProcessSuperviseController : BaseMvcController
    {
        private readonly ProcessLogService _processLogService;
        private readonly WorkflowManagementService _workflowMgmtService;

        string viewFlow = ConfigurationManager.AppSettings["ViewFlowUrl"].ToString();
        string k2server = ConfigurationManager.AppSettings["ServerName"].ToString();
        string windowDomain = ConfigurationManager.AppSettings["WindowDomain"].ToString();

        public ProcessSuperviseController()
        {
            _processLogService = new ProcessLogService();
            _workflowMgmtService = new WorkflowManagementService(AuthenticationType.Windows);
        }

        //
        // GET: /Dashboard/ProcessSupervise/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Find(string processName, string Folio, string startDate, string finishDate, string startUser, int Status, string SysId)
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
            Status = 0;
            var result = CustomExtUtility.GetAllProcessInst(processName, Folio, startDate, finishDate, startUser, Status, pageSize, pageIndex, SysId);
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

        public JsonResult GetAllRecord(string processName, string Folio, string startDate, string finishDate, string startUser, int Status, string SysId)
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
            Status = 0;
            var result = CustomExtUtility.GetAllProcessInst(processName, Folio, startDate, finishDate, startUser, Status, pageSize, pageIndex, SysId);
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
                string runtime = string.Empty;
                if (!pv.Status.Equals("已完成") && !pv.Status.Equals("错误") || !pv.Status.Equals("已删除"))
                {
                    runtime = RunHowLongTime(r["TaskStartDate"].ToString());
                    pv.RuningTime = runtime;
                    pv.TaskStartDate = !string.IsNullOrEmpty(r["TaskStartDate"].ToString()) ? Convert.ToDateTime(r["TaskStartDate"].ToString()).ToString("yyyy-MM-dd HH:mm") : "";
                }
                else
                {
                    pv.TaskStartDate = string.Empty;
                    pv.RuningTime = runtime;
                }
                pv.ActName = r["ActName"].ToString();

                pv.Destination = r["Destination"].ToString();
                pv.ViewUrl = r["ViewUrl"].ToString().Replace("{_FormId}", r["FormId"].ToString());
                pv.ViewFlowUrl = viewFlow + "?K2Server=" + k2server + "&ProcessID=" + pv.ID;
                list.Add(pv);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult StopProcessInstance(IList<ProcessActionModel> items)
        {
            var success = true;
            var actionComment = string.Empty;

            try
            {
                if (items == null)
                    throw new ArgumentNullException("items");

                foreach (var processItem in items)
                {
                    if (processItem.ProcInstID == 0
                        || string.IsNullOrEmpty(processItem.ProcessOwner)
                        || string.IsNullOrEmpty(processItem.ProcessName))
                        throw new InvalidOperationException("参数无效！");

                    _workflowMgmtService.StopProcessInstance(processItem.ProcInstID);
                    AppendProcessLog(processItem, "终止");
                }
            }
            catch (Exception ex)
            {
                success = false;

                LogFactory.GetLogger().Write(new LogEvent
                {
                    Category = "KStar.Portal",
                    Source = "流程督办-终止流程",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
            }

            return Json(new { success = success });
        }

        [HttpPost]
        public JsonResult RedirectWorklistItem(IList<ProcessActionModel> items)
        {
            var success = true;

            try
            {
                if (items == null)
                    throw new ArgumentNullException("items");

                foreach (var processItem in items)
                {
                    if (string.IsNullOrEmpty(processItem.SerialNumber)
                        || string.IsNullOrEmpty(processItem.ProcessOwner)
                        || string.IsNullOrEmpty(processItem.ProcessName)
                        || string.IsNullOrEmpty(processItem.RedirectToUser))
                        throw new InvalidOperationException("参数无效！");

                    _workflowMgmtService.RedirectWorklistItem(processItem.SerialNumber, K2User.ApplySecurityLabel(processItem.RedirectToUser), false);
                    AppendProcessLog(processItem, "转交");
                }
            }
            catch (Exception ex)
            {
                success = false;

                LogFactory.GetLogger().Write(new LogEvent
                {
                    Category = "KStar.Portal",
                    Source = "流程督办-转交流程",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
            }

            return Json(new { success = success });
        }


        [HttpPost]
        public JsonResult ProcessReminder(string processInstIDs)
        {
            int reuslt = 0;
            try
            {
                var table = CustomExtUtility.GetProcessReminderInfo(processInstIDs);
                string url = string.Empty;

                StringBuilder emailSql = new StringBuilder();
                if (table.Tables.Count > 1)
                {
                    url = table.Tables[1].Rows[0][0].ToString();
                }
                foreach (DataRow row in table.Tables[0].Rows)
                {
                    if (row["Status"].ToString() == "已完成" || row["Status"].ToString() == "错误" || row["Status"].ToString() == "已删除") { continue; }
                    string Title = GenerateEmailTitle(row);
                    string processEmail = GenerateEmailFormat(row, url, string.Empty);
                    //emailSql.Append("INSERT INTO QuartzCacheSendMails([Title],[Body] ,[ReplyTo] ,[Prompt],[Forward] ,[ReplyDate] ,[CreateDate]) VALUES(" + Title + "," + processEmail + "," + row["Email"].ToString() + ",");
                    //emailSql.Append("'" + string.Empty + "',false,'" + DateTime.Now.ToString() + "','" + DateTime.Now.ToString() + "'");
                    //string InsertSql = emailSql.ToString();
                    CustomExtUtility.InsertReminderRecord(Title, processEmail, row["Email"].ToString());
                    string toUser = row["Originator"].ToString().Replace("K2:", "");
                    string ProcessFullName = row["ProcessFullName"].ToString().Replace("K2:", "");
                    DataTable delegateTable = CustomExtUtility.GetDelegateInfo(toUser, ProcessFullName);
                    if (delegateTable != null && delegateTable.Rows.Count > 0)
                    {
                        foreach (DataRow r in delegateTable.Rows)
                        {
                            string delegateUser = r["ToUser"].ToString();
                            processEmail = GenerateEmailFormat(row, url, delegateUser);
                            CustomExtUtility.InsertReminderRecord(Title, processEmail, r["ToEmail"].ToString());
                        }
                    }
                }
                reuslt = 1;
            }
            catch (Exception ex)
            {
                LogFactory.GetLogger().Write(new LogEvent
                {
                    Source = "发送邮件催办",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
                reuslt = -1;
            }
            return Json(new { Status = reuslt });

        }


        /// <summary>
        /// 计算运行多长时间了。
        /// </summary>
        /// <param name="TaskDate"></param>
        /// <returns></returns>
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

        public JsonResult GetProcess()
        {
            var result = CustomExtUtility.GetProcessSet();
            List<ProcessSetInfo> info = new List<ProcessSetInfo>();
            foreach (DataRow r in result.Rows)
            {
                ProcessSetInfo i = new ProcessSetInfo();
                i.processFullName = r["ProcessFullName"].ToString();
                i.ProcessName = r["ProcessName"].ToString();
                i.ProcSetID = r["ProcessSetID"].ToString();
                info.Add(i);
            }

            var json = Json(info, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string GenerateEmailTitle(DataRow row)
        {

            var info = GetUserInfo(CurrentUser);
            string Subject = row["subject"].ToString();
            string prefix = "【" + info.FirstName + "】催办：";
            string Title = prefix + "流程中心 - {0}({1})发起的【{2}】需要您处理";
            string startUser = row["Originator"].ToString().Replace("K2:", "").Replace(windowDomain.ToLower(), "").Replace(windowDomain.ToUpper(), "").Replace(@"\", "");
            Title = string.Format(Title, row["StartName"].ToString(), startUser, row["ProcessName"].ToString());
            if (!string.IsNullOrEmpty(row["subject"].ToString()))
            {
                Title = row["subject"].ToString().Replace("待办：", prefix);
            }
            return Title;
        }

        public string GenerateEmailFormat(DataRow row, string transferUrl, string shareUser)
        {
            if (!string.IsNullOrEmpty(shareUser))
            {
                shareUser = "&SharedUser=" + shareUser;
            }
            StringBuilder strContext = new StringBuilder("<span style='font-family:Arial sans-serif ;color:#1F497D;'>Dear,{0}<br/><br/>您在流程中心中有一个<span style='color:red'>审批</span>事项。 ");
            strContext.Append(" <a href='{6}?strUrl={7}?_FormId={8}&SN={9}&FromMail=1&authorname={10}" + shareUser + "'>请点击这里打开处理</a> <br/>");
            strContext.Append("以下是该事项的简要内容：</span><br/>");

            strContext.Append("<table style='border:none;border-collapse:collapse' cellspacing='0' cellpadding='0'><tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>");
            strContext.Append("流程名称:</td><td>&nbsp;</td><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{1}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>流程实例号:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{2}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>当前环节名称:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{3}</td></tr>");

            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请人:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{4}</td></tr>");
            strContext.Append("<tr><td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>申请时间:</td><td>&nbsp;</td>");
            strContext.Append("<td style='border:none;font-family:Arial sans-serif ;color:#1F497D;'>{5}</td></tr>");
            strContext.Append("</table>");
            strContext.Append("<br/><span style='font-family:Arial sans-serif ;color:#1F497D;'>温馨提示：此邮件由系统自动发出，请不要答复该邮件。若对流程内容有疑问，请与发起人联系，若有技术问题，请与流程程架构部（peng.fuze@innos.com）联系。 </span>");
            string body = string.Format(strContext.ToString(), row["HandlerUser"].ToString(), row["ProcessName"].ToString(), row["Folio"].ToString().Split('-').Last(), row["ActName"].ToString(), row["StartName"].ToString()
                , Convert.ToDateTime(row["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"), transferUrl, row["ApproveUrl"].ToString(), row["FormID"].ToString(), row["SN"].ToString(), row["Destination"].ToString());
            return body;

        }

        private void AppendProcessLog(ProcessActionModel processItem, string actionName)
        {
            var actionComment = string.Empty;
            var redirectToUser = string.Empty;
            var redirectToUserName = string.Empty;

            var actionTaker = this.CurrentUser.Split(':')[0];
            var processOwner = processItem.ProcessOwner.Split(':')[0];

            if (!string.IsNullOrEmpty(processItem.RedirectToUser))
            {
                redirectToUser = processItem.RedirectToUser.Split(':')[0];
                redirectToUserName = CacheAccessor.GetUserFullName(redirectToUser);
            }

            var actionTakerName = CacheAccessor.GetUserFullName(actionTaker);
            var processOwnerName = CacheAccessor.GetUserFullName(processOwner);

            switch (actionName)
            {
                case "终止":
                    actionComment = string.Format("{0} 终止了该流程实例！", actionTakerName);
                    break;
                case "转交":
                    actionComment = string.Format("{0} 转交了该任务项至 {1}！", actionTakerName, redirectToUserName);
                    break;
            }


            var processLog = new ProcessLog()
            {
                ProcInstID = processItem.ProcInstID,
                ActivityName = processItem.ActivityName,
                ActionName = actionName,
                ProcessName = processItem.ProcessName,
                CommentDate = DateTime.Now,
                Comment = actionComment,
                OrigUserName = processOwnerName,
                OrigUserAccount = processOwner,
                UserName = actionTakerName,
                UserAccount = actionTaker
            };

            _processLogService.AddProcessLog(processLog);
        }

    }

    public class ProcessInstView
    {
        public string ID { get; set; }
        public string ProcessName { get; set; }
        public string Folio { get; set; }
        public string FlowNo { get; set; }
        public string OperateUser { get; set; }
        public string StartUser { get; set; }
        public string Title { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string Originator { get; set; }
        public string ViewFlowUrl { get; set; }
        public string ViewUrl { get; set; }
        public string Status { get; set; }
        public string ActName { get; set; }
        public string SN { get; set; }
        public string Destination { get; set; }
        public string TaskStartDate { get; set; }
        public string RuningTime { get; set; }
    }
    public class ProcessSetInfo
    {
        public string ProcessName { get; set; }
        public string processFullName { get; set; }
        public string ProcSetID { get; set; }
    }

    public class ProcessActionModel
    {
        public int ProcInstID { get; set; }
        public string ProcessName { get; set; }
        public string SerialNumber { get; set; }
        public string ProcessOwner { get; set; }
        public string ActivityName { get; set; }
        public string RedirectToUser { get; set; }

    }
}

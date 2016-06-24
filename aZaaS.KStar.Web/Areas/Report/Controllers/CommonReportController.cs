using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using aZaaS.Kstar.DAL;
using Newtonsoft;
using Newtonsoft.Json;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Logging;
using System.Configuration;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    public class CommonReportController : Controller
    {
        //
        // GET: /Report/CommonReport/
        string viewFlow = ConfigurationManager.AppSettings["ViewFlowUrl"].ToString();
        string k2server = ConfigurationManager.AppSettings["ServerName"].ToString();
        string windowDomain = ConfigurationManager.AppSettings["WindowDomain"].ToString();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllProcessList()
        {
            string jsonData = string.Empty;
            List<ProcessModel> list = new List<ProcessModel>();
            try
            {

                var ds = CommonReportDAL.GetAllProcessList();
               // List<ProcessModel> list = new List<ProcessModel>();
                foreach (DataRow r in ds.Rows)
                {
                    ProcessModel model = new ProcessModel();
                    model.ProcessName = r["ProcessName"].ToString();
                    model.ProcessSetID = r["ProcessSetID"].ToString();
                    list.Add(model);
                }
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                jsonData = "[]";
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllProcessListByUserName(string UserName)
        {
            string jsonData = string.Empty;
            List<ProcessModel> list = new List<ProcessModel>();
            try
            {

                var ds = CommonReportDAL.GetProcessListByUserName(UserName);
                // List<ProcessModel> list = new List<ProcessModel>();
                foreach (DataRow r in ds.Rows)
                {
                    ProcessModel model = new ProcessModel();
                    model.ProcessName = r["ProcessName"].ToString();
                    model.ProcessSetID = r["ProcessSetID"].ToString();
                    list.Add(model);
                }
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                jsonData = "[]";
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommonReportConfig(string ProcSetID)
        {
            string jsonData = string.Empty;
            try
            {
                //ProcSetID = "3";
                var ds = CommonReportDAL.GetReportInfobyProcessSetID(ProcSetID);
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                jsonData = "[]";
            }
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Find(string ProcSetID, string strWhere, string CurrentUser, string SysID,string Status)
        {
            string jsonData = string.Empty;
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
            int total = 0;
            var objData = new DataSet();
            try
            {
                objData = CommonReportDAL.Find(ProcSetID, strWhere, pageIndex, pageSize, CurrentUser, SysID,Status);
                objData.Tables[0].TableName = "DataList";
                if (!objData.Tables[0].Columns.Contains("ViewFlowUrl"))
                {
                    objData.Tables[0].Columns.Add("ViewFlowUrl");
                }
                if (objData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in objData.Tables[0].Rows)
                    {
                        r["ViewUrl"] = r["ViewUrl"].ToString().Replace("{_FormId}", r["FormId"].ToString());
                        r["ViewFlowUrl"] = viewFlow + "?K2Server=" + k2server + "&ProcessID=" + r["ProcInstId"].ToString();
                        //if (r["Status"].ToString() == "运行中")
                        //{
                            r["StartDate"] = Convert.ToDateTime(r["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                        //}
                        var dest = r["Destination"].ToString();
                        if (!string.IsNullOrEmpty(dest))
                        {
                            dest = dest.Substring(dest.LastIndexOf(@"\") + 1);
                            var u = OrgUtility.GetUser(dest, string.Empty);
                            if (u != null && u.Rows.Count > 0)
                            {
                                r["Destination"] = u.Rows[0]["FirstName"].ToString();
                            }
                        }
                    }
                }
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(objData.Tables[0]);
                total = Convert.ToInt32(objData.Tables[1].Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                LogFactory.GetLogger().Write(new LogEvent
                {
                    Source = "流程通用报表",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
                jsonData = "[]";
            }
            var output = new { data = jsonData, total = total };
            var json = Json(output, JsonRequestBehavior.AllowGet);
            //return Json(jsonData, JsonRequestBehavior.AllowGet);
            return json;
        }
        public JsonResult GetAllRecord(string ProcSetID, string strWhere, string CurrentUser, string SysID,string Status)
        {
            string jsonData = string.Empty;
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
                pageSize = 100000;
            }
            int total = 0;
            var objData = new DataSet();
            DataTable source = new DataTable();
            try
            {
                objData = CommonReportDAL.Find(ProcSetID, strWhere, pageIndex, pageSize, CurrentUser, SysID, Status);
                objData.Tables[0].TableName = "DataList";
                if (!objData.Tables[0].Columns.Contains("ViewFlowUrl"))
                {
                    objData.Tables[0].Columns.Add("ViewFlowUrl");
                }
                if (objData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in objData.Tables[0].Rows)
                    {
                        r["ViewUrl"] = r["ViewUrl"].ToString().Replace("{_FormId}", r["FormId"].ToString());
                        r["ViewFlowUrl"] = viewFlow + "?K2Server=" + k2server + "&ProcessID=" + r["ProcInstId"].ToString();
                        //if (r["Status"].ToString() == "运行中")
                        //{
                            r["StartDate"] = Convert.ToDateTime(r["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm").Substring(0,16);
                        //}
                    }
                }
                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(objData.Tables[0]);
                //total = Convert.ToInt32(objData.Tables[1].Rows[0][0].ToString());
                source = objData.Tables[0];
            }
            catch (Exception ex)
            {
                LogFactory.GetLogger().Write(new LogEvent
                {
                    Source = "流程通用报表",
                    Message = ex.Message,
                    Exception = ex,
                    OccurTime = DateTime.Now
                });
                //jsonData = "[]";
            }
            //var output = new { data = jsonData, total = total };
            var json = Json(jsonData, JsonRequestBehavior.AllowGet);
            //return Json(jsonData, JsonRequestBehavior.AllowGet);
            return json;
        }
    }
    public class ProcessModel
    {
        public string ProcessSetID { get; set; }
        public string ProcessName { get; set; }
    }
}

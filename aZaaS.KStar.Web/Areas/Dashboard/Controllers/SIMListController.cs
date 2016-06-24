using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Utilities;
using aZaaS.Framework.Workflow.Pager;
using aZaaS.KStar.Web.Models;
using System.Data;
using aZaas.Kstar.DAL;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Web.Areas.JSRSIMManagement.Models;
using System.Configuration;
using System.Data.SqlClient;

namespace aZaaS.KStar.Web.Areas.Dashboard.Controllers
{
    public class SIMListController : BaseMvcController
    {
        //
        // GET: /Dashboard/SIMList/

        //
        // GET: /Dashboard/ProcessSupervise/
        string viewFlow = ConfigurationManager.AppSettings["ViewFlowUrl"].ToString();
        string k2server = ConfigurationManager.AppSettings["ServerName"].ToString();

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult Find(string startDate, string endDate, string LongNumber, string Borrower, int page = 1, int pageSize = 20)
        {
            JSRSIMManagementModel model = new JSRSIMManagementModel();
            ProcessInstanceCriteria criteria = new ProcessInstanceCriteria();

            var TotalCount = 0;
            var pageCount = 0;
            var whereSql = string.Empty;
            var SIMManager = NeowayExtUtility.GetScalarData(string.Format("SP_NW_GetPositionUsers '{0}'", "SIM卡管理员"));
            var currentUser = HttpContext.Session["CurrentUser"].ToString();
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                //criteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "BorrowDate", Compare = CriteriaCompare.GreaterOrEqual, Value1 = startDate });
                whereSql = whereSql + string.Format(" and(BorrowDate >='{0}' and BorrowDate<>'' and BorrowDate is not null ) ", startDate);
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                //criteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "BorrowDate", Compare = CriteriaCompare.LessOrEqual, Value1 = endDate });
                whereSql = whereSql + string.Format(" and(BorrowDate <='{0}' and BorrowDate<>'' and BorrowDate is not null ) ", endDate);
            }
            if (!string.IsNullOrEmpty(LongNumber))
            {
                //criteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "LongNumber", Compare = CriteriaCompare.Like, Value1 = LongNumber });
                whereSql = whereSql + string.Format(" and(LongNumber like '%{0}%' ) ", LongNumber);
            }
            if (!string.IsNullOrEmpty(Borrower))
            {
                //criteria.AddRegularFilter(new RegularFilter() { StartLogical = CriteriaLogical.And, FieldName = "Borrower", Compare = CriteriaCompare.Like, Value1 = Borrower });
                whereSql = whereSql + string.Format(" and(Borrower like '%{0}%' ) ", Borrower);
            }
            //var whereSql = GetMyFilter(criteria);
            if (SIMManager.ToString().IndexOf(currentUser) == -1)
            {
                whereSql = whereSql + string.Format(" and(BorrowerUserName='{0}' or SIMStatus='Foruse') ", currentUser);
            }

            var Data = NeowayExtUtility.GetSIMListData(page, pageSize, "NW_SIMManagement", "FormId", model.paramsString, "CreatedDate", whereSql, out TotalCount, out pageCount);
            var DataList = JsonHelper.ConvertToModel<List<JSRSIMManagementModel>>(Data);
            for (int i = 0; i < DataList.Count(); i++)
            {
                DataList[i].SIMStatus = getSIMStatus(DataList[i].SIMStatus);
            }
            return Json(new { total = TotalCount, data = DataList.OrderByDescending(p => p.CreatedDate) }, JsonRequestBehavior.AllowGet);
        }
        #region        GetMyFilter
        public string GetMyFilter(ProcessInstanceCriteria criteria)
        {
            string text2 = " ";
            try
            {
                foreach (RegularFilter current in criteria.RegularFilters)
                {
                    switch (current.Compare)
                    {
                        case CriteriaCompare.Equal:
                            {
                                object obj = text2;
                                text2 = string.Concat(new object[]
					{
						obj, 
						" and ", 
						current.FieldName, 
						" ='", 
						current.Value1, 
						"'"
					});
                                break;
                            }
                        case CriteriaCompare.Less:
                            {
                                object obj2 = text2;
                                text2 = string.Concat(new object[]
					{
						obj2, 
						" and ", 
						current.FieldName, 
						" <'", 
						current.Value1, 
						"'"
					});
                                break;
                            }
                        case CriteriaCompare.LessOrEqual:
                            {
                                object obj3 = text2;
                                text2 = string.Concat(new object[]
					{
						obj3, 
						" and ", 
						current.FieldName, 
						" <='", 
						current.Value1, 
						"'"
					});
                                break;
                            }
                        case CriteriaCompare.Greater:
                            {
                                object obj4 = text2;
                                text2 = string.Concat(new object[]
					{
						obj4, 
						" and ", 
						current.FieldName, 
						" >'", 
						current.Value1, 
						"'"
					});
                                break;
                            }
                        case CriteriaCompare.GreaterOrEqual:
                            {
                                object obj5 = text2;
                                text2 = string.Concat(new object[]
					{
						obj5, 
						" and ", 
						current.FieldName, 
						" >='", 
						current.Value1, 
						"'"
					});
                                break;
                            }
                        case CriteriaCompare.Like:
                            {
                                object obj6 = text2;
                                text2 = string.Concat(new object[]
					{
						obj6, 
						" and ", 
						current.FieldName, 
						" like '%", 
						current.Value1, 
						"%'"
					});
                                break;
                            }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            return text2;
        }

        #endregion
        public JsonResult GetProcess()
        {
            var result = NeowayExtUtility.GetProcessSet();
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

        string getSIMStatus(string SIMStatus)
        {
            var returnStr = "待使用";
            switch (SIMStatus)
            { 
                case "Foruse":
                    returnStr = "待使用";
                    break;
                case "Using":
                    returnStr = "使用中";
                    break;
                case "Cancel":
                    returnStr = "已注销";
                    break;
                case "Canceling":
                    returnStr = "注销中";
                    break;
                case "Broken":
                    returnStr = "已损坏";
                    break;
                case "Lose":
                    returnStr = "已丢失";
                    break;
                case "Makeup":
                    returnStr = "补办中";
                    break;
                default:
                    returnStr = "";
                    break;
            }
            return returnStr;
        }
    }
   
}


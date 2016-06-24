using aZaaS.KStar.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models;
using System.Collections;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    [EnhancedHandleError]
    public class ReportFavouriteController : BaseMvcController
    {

        ReportFavouriteManager favMgr = new ReportFavouriteManager();
        // GET: /Report/ReportFavourite/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCategories(string ID = "")
        {
            var items = new List<ReportTreeItem>();
            var categories = new List<ReportFavouriteEntity>();
            if (string.IsNullOrEmpty(ID) || this.User == null || this.User.Identity.IsAuthenticated == false)
            {
                items.Add(new ReportTreeItem()
                {
                    ID = "00000000-0000-0000-0000-000000000000",
                    DisplayName = "_ALL_",
                    HasChildren = true,
                    Type = "Category"
                });
                return Json(items.OrderBy(o => o.DisplayName), JsonRequestBehavior.AllowGet);
            }
            var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
            var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);
      
            categories = ReportFavouriteManager.GetFavoritesCategoryByParentID(new Guid(ID), userInfo.SysID);

            foreach (var category in categories)
            {
                var item = new ReportTreeItem
                {
                    ID = category.ID.ToString(),
                    ParentID = category.ParnentID.ToString(),
                    DisplayName = category.Name,
                    HasChildren = true,
                    Type = category.Comment
                };
                items.Add(item);
            }

            return Json(items.OrderBy(o => o.DisplayName), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetClientReports(string categoryID, string sort = "", bool isRate = false)
        {
            ReportInfoManager _reportManager = new ReportInfoManager();

            var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
            var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);

            var reports = ReportFavouriteManager.GetFavouriteReports(categoryID, userInfo.SysID, sort);


            var items = new List<ReportItemModel>();
            reports.ToList().ForEach(report =>
            {
                var item = new ReportItemModel();

                var permissions = _reportManager.GetReportPermissions(report.ID);
                var roles = ReportFavouriteManager.GetUserRoles(userInfo.SysID);

                item.FromReport(report, permissions);

                bool ispermissions = false;
                foreach (Guid role in roles)
                {

                    if (permissions.Where(x => roles.Contains(x.RoleID)).Count() > 0)
                    {
                        ispermissions = true;
                        break;
                    }
                } 
                if (ispermissions)
                {
                    //是否获取统计率
                    if (isRate)
                    {
                        item.Rate = _reportManager.GetReportRate(report.ID, userInfo.SysID) + string.Empty;
                    }

                    items.Add(item);
                }
            });

            return Json(new { total = reports.Count, data = items }); 
        }

        [HttpPost]
        public ActionResult AddCategories(string parentID, string name, string comment)
        {
            ResultMessage reslut = new ResultMessage(); 
            try
            {
                var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
                var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);
                reslut.IsOK = ReportFavouriteManager.AddFavorites(parentID, name, userInfo.SysID, comment);
            }
            catch (Exception ex)
            {
                reslut.IsOK = false;
                reslut.Message = ex.Message;
            } 
            return Json(reslut);
        }

        public ActionResult UpdateCategories(string id, string name, string comment)
        {
            ResultMessage reslut = new ResultMessage();
            try
            { 
                reslut.IsOK = ReportFavouriteManager.UpdateCategories(id, name, comment);
            }
            catch (Exception ex)
            {
                reslut.IsOK = false;
                reslut.Message = ex.Message;
            }
            return Json(reslut);
        }

        public ActionResult DeleteCategories(string id)
        {
            ResultMessage reslut = new ResultMessage();

            try
            {

                reslut.IsOK = ReportFavouriteManager.DeleteCategories(id);
              

            }
            catch (Exception ex)
            {
                reslut.Message = ex.Message;
            }
            return Json(reslut);
        }
          [HttpPost]
        public ActionResult RemoveFavorites(string id, string categoryID)
        {
            ResultMessage reslut = new ResultMessage();

            try
            {

                var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
                var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);
                ReportFavouriteManager.RemoveFavorites(id,categoryID, userInfo.SysID);
                reslut.IsOK = true; 
            }
            catch (Exception ex)
            {
                reslut.Message = ex.Message;
            }
            return Json(reslut);
        }
         
        public class ResultMessage
        {
            public bool IsOK { set; get; }
            public string Message { set; get; } 
        }

    }
}

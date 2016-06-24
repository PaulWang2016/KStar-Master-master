using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar;
using aZaaS.KStar.Report;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Helper;
using aZaaS.KStar.Repositories;


namespace aZaaS.KStar.Web.Areas.Report.Controllers
{
    [EnhancedHandleError]
    public class ReportCenterController : BaseMvcController
    {
        private readonly UserBO _userService;
        private readonly RoleBO _roleService;
        private readonly ReportInfoManager _reportManager;

        public ReportCenterController()
        {
            _userService = new UserBO();
            _roleService = new RoleBO();
            _reportManager = new ReportInfoManager();
        }


        [HttpPost]
        public ActionResult Index()
        { 
            return View();
        }


        [HttpPost]
        public ActionResult Statistics()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Myfavorites()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Manage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddCategory(Category category)
        {
            Guid parentID;
            if (!Guid.TryParse(category.ParentID, out parentID))
            {
                parentID = Guid.Empty;
            }

            var entity = new ReportCategoryEntity()
            {
                ID = Guid.NewGuid(),
                ParnentID = parentID,
                Category = category.DisplayName,
                Comment = category.Comment
            };

            _reportManager.AddReportCategory(entity);

            ReportTreeItem item = new ReportTreeItem
            {
                ID = entity.ID.ToString(),
                ParentID = entity.ParnentID.ToString(),
                DisplayName = entity.Category,
                HasChildren = false,
                Type = "Category"
            };
            return Json(item);
        }

        [HttpPost]
        public ActionResult AddReport(ReportItemModel report)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            if (string.IsNullOrEmpty(report.Name))
                throw new ArgumentNullException("报表名称未填写！");
            if (string.IsNullOrEmpty(report.ReportUrl))
                throw new ArgumentNullException("报表Url未填写！");
            if (string.IsNullOrEmpty(report.Department))
                throw new ArgumentException("报表部门未设置！");
            if (string.IsNullOrEmpty(report.Roles))
                throw new ArgumentException("报表权限未设置！");


            report.ID = Guid.NewGuid();
            report.Rate = "0";
            report.PublishedDate = DateTime.Now;

            if (Request.Files != null && Request.Files.Count > 0)
                report.ImageThumbPath = SaveReportImageThumb(Request.Files[0]);

            var permissions = report.ResolvePermissions();
            permissions.ForEach(item => item.ReportID = report.ID);

            _reportManager.AddReportInfo(report.ToEntity(), permissions);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult EditReport(ReportItemModel report)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            var originReport = _reportManager.GetReportInfo(report.ID);
            if (originReport == null)
                throw new InvalidOperationException("The speicified report was not found.");

            originReport.FromData(report);

            if (Request.Files != null && Request.Files.Count > 0
                && !string.IsNullOrEmpty(Request.Files[0].FileName))
                originReport.ImageThumbPath = SaveReportImageThumb(Request.Files[0]);

            var permissions = report.ResolvePermissions();

            _reportManager.UpdateReportInfo(originReport, permissions);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult EditCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            var categoryID = Guid.Parse(category.CategoryID);
            var originCategory = _reportManager.GetCategory(categoryID);
            if (originCategory == null)
                throw new InvalidOperationException("The specified category was not found.");

            originCategory.FromData(category);
            _reportManager.UpdateCategory(originCategory);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult RemoveReport(Guid? reportID)
        {
            if (reportID == null)
                throw new ArgumentNullException("reportID");

            var report = _reportManager.GetReportInfo(reportID.Value);
            if (report == null)
                throw new InvalidOperationException("The specified report was not found.");

            _reportManager.RemoveReport(reportID.Value);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult RemoveCategory(Guid? categoryID)
        {
            if (categoryID == null)
                throw new ArgumentNullException("categoryID");

            var originCategory = _reportManager.GetCategory(categoryID.Value);
            if (originCategory == null)
                throw new InvalidOperationException("The speicified category was not found.");

            _reportManager.RemoveCategory(categoryID.Value);

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult GetReport(Guid? reportID)
        {
            if (reportID == null)
                throw new ArgumentNullException("reportID");

            var report = _reportManager.GetReportInfo(reportID.Value);
            var permissions = _reportManager.GetReportPermissions(reportID.Value);

            var reportData = new ReportItemModel();
            reportData.FromReport(report, permissions);

            return Json(reportData);
        }

        [HttpPost]
        public ActionResult GetCategory(Guid? categoryID)
        {
            if (categoryID == null)
                throw new ArgumentNullException("categoryID");

            var category = _reportManager.GetCategory(categoryID.Value);

            return Json(category);
        }

        [HttpPost]
        public ActionResult GetReports(string categoryID,
                                        string keySearch = "",
                                        string startDate = "", string endDate = "",
                                        string status = "", string level = "0",
                                        int? page = 1, int? pageSize = 20, [ModelBinder(typeof(JsonListBinder<SortDescriptor>))]List<SortDescriptor> sort = null)
        {
            var totalRows = 0;
            SortDescriptor sorter = null;

            var categoryIDs = new List<Guid>();
            if (!string.IsNullOrEmpty(categoryID))
                categoryIDs = _reportManager.GetCategories(Guid.Parse(categoryID)).Select(c => c.ID).ToList();

            if (sort != null && sort.Any() && sort.Count >= 1)
                sorter = sort.First();

            var reports = _reportManager.GetReportInfos(out totalRows,
                                                        categoryIDs, status, level, keySearch,
                                                        startDate, endDate, page, pageSize,
                                                        sorter == null ? "" : sorter.field, sorter == null ? "" : sorter.dir);

              var items = new List<ReportItemModel>();
            reports.ToList().ForEach(report =>
            {
                var item = new ReportItemModel();

                var permissions = _reportManager.GetReportPermissions(report.ID);
                item.FromReport(report, permissions);

                items.Add(item);
            });

            return Json(new { total = totalRows, data = items });
        }

        [HttpGet]
        public JsonResult GetCategories(string ID = "")
        {
            var items = new List<ReportTreeItem>();
            var categories = new List<ReportCategoryEntity>();

            if (string.IsNullOrEmpty(ID))
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

            categories.AddRange(_reportManager.GetReportCategoryByParentID(Guid.Parse(ID)));

            foreach (var category in categories)
            {
                var item = new ReportTreeItem
                {
                    ID = category.ID.ToString(),
                    ParentID = category.ParnentID.ToString(),
                    DisplayName = category.Category,
                    HasChildren = true,
                    Type = "Category"
                };
                items.Add(item);
            }

            return Json(items.OrderBy(o => o.DisplayName), JsonRequestBehavior.AllowGet);
        }


        private string SaveReportImageThumb(HttpPostedFileBase file)
        {
            var storageFileName = string.Empty;
            var filePath = Server.MapPath("~/images/report/");
            var fileName = Path.GetFileName(file.FileName);

            if (!string.IsNullOrEmpty(fileName))
            {
                file.SaveAs(Path.Combine(filePath, fileName));
                storageFileName = string.Format("/images/report/{0}", fileName);
            }

            return storageFileName;
        }


        [HttpGet]
        public ActionResult ViewReport(Guid? reportID)
        {
            if (reportID == null)
                throw new ArgumentNullException("reportUrl");

            var report = _reportManager.GetReportInfo(reportID.Value);
            if (report == null)
                return HttpNotFound("The specified report was not found.");

            //TODO:计算点击率
            //TODO:添加点击记录
            StatisticsClick(reportID.Value.ToString());
        
            var formatedReportUrl = ReportUrlResolver.ApplyQueryParameters(CurrentUserEntry, report.ReportUrl);

            return Redirect(formatedReportUrl);
        }


        /// <summary>
        /// 报表统计点击
        /// </summary>
        /// <param name="reportID"></param>
        [HttpPost]
        public void StatisticsClick(string reportID)
        {

            if (!this.User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(reportID)) return;
            var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
            var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);

            ReportStatistics entity = new ReportStatistics();
            entity.ID = Guid.NewGuid();
            entity.ReportID = new Guid(reportID);
            entity.SysID = userInfo.SysID;
            entity.UserHostAddress = this.Request.UserHostAddress;
            entity.UserId = userInfo.UserId;
            entity.FirstName = userInfo.FirstName;
            entity.Browser = this.Request.Browser.Browser;
            entity.CreateTime = DateTime.Now; 
            using (KStarDbContext ctx = new KStarDbContext())
            {
                ctx.ReportStatistics.Add(entity); 
                ctx.SaveChanges(); 
            } 
        }


        /// <summary>
        /// 获取用户角色信息
        /// </summary>
        /// <returns></returns>
        private IList<System.Guid> GetUserRoles(string userName)
        {
            IList<System.Guid> Roles = null;
            using (KStarFramekWorkDbContext kstr = new KStarFramekWorkDbContext())
            {
                var linq = from u in kstr.User
                           join r in kstr.RoleUser
                                 on u.SysId equals r.User_SysId
                                  into pro
                           from po in pro.DefaultIfEmpty()
                           where u.UserName == userName
                           select po.Role_SysId;

                Roles = linq.ToList();
            } 
            return Roles;
        }


   

 
        [HttpPost]
        public ActionResult GetClientReports(string categoryID, string sort = "", bool isRate = false)
        { 
            var categoryIDs = new List<Guid>();
            if (!string.IsNullOrEmpty(categoryID))
                categoryIDs = _reportManager.GetCategories(Guid.Parse(categoryID)).Select(c => c.ID).ToList();
            //获取用户信息
            var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
            var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);

            var reports = ReportFavouriteManager.GetReportInfos(userInfo.SysID,categoryIDs, sort);
                  
            var items = new List<ReportItemModel>();
            reports.ToList().ForEach(report =>
            {
                var item = new ReportItemModel();
                item.FromReport(report, null); 
                //是否获取统计率
                if (isRate)
                {
                    item.Rate = _reportManager.GetReportRate(report.ID, userInfo.SysID) + string.Empty;
                } 
                items.Add(item); 
            }); 
            return Json(new { total = reports.Count, data = items });
        }

         
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="reportID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Favorites(string reportID, string name, string favouriteID)
        {
            ReportFavouriteController.ResultMessage result = new ReportFavouriteController.ResultMessage();

            if (!this.User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(reportID))
            {
                result.Message = "Users are not certification";
                return Json(result);

            }

            var _OrganizationService = new aZaaS.KStar.Form.KStarFormOrganizationService();
            var userInfo = _OrganizationService.GetUser(this.User.Identity.Name);

            Report_FavouriteEntity entity = new Report_FavouriteEntity();
            entity.ID = Guid.NewGuid();
            entity.ReportInfoID = new Guid(reportID);
            entity.UserID = userInfo.SysID;
            entity.FavouriteDate = DateTime.Now;
            entity.FavouriteID = new Guid(favouriteID);
            entity.Name = name;

            try
            {
                ReportFavouriteManager.Favorites(entity);
                result.IsOK = true;
            }
            catch (Exception ex)
            {
                result.IsOK = false;
                result.Message = ex.Message;
            } 
            return Json(result);
        }
        
        [HttpPost]
        public ActionResult StatisticsRoport(string StartDate, string EndDate)
        {
           

            using (KStarDbContext ctx = new KStarDbContext())
            {
                var rstable= from s in ctx.ReportStatistics select s;

                 if (!string.IsNullOrWhiteSpace(StartDate) && !string.IsNullOrWhiteSpace(EndDate) )
                {
                    DateTime startDate= DateTime.Parse(StartDate);
                    DateTime endDate =DateTime.Parse(EndDate);
                    if (startDate.CompareTo(endDate) <= 0)
                    {
                      
                        rstable = rstable.Where(x => x.CreateTime >= startDate && x.CreateTime <= endDate);
                    }
                }

                 var linq = from s in rstable group s by s.ReportID into pro select new { ReportID = pro.Key, count = pro.Count() };

               
                var linq2 = from rs in linq
                            join r in ctx.ReportInfo on rs.ReportID equals r.ID
                                into pro
                            from rf in pro.DefaultIfEmpty()

                            select new { name = rf.Name, count = rs.count };



                return   Json( linq2.ToList(), JsonRequestBehavior.AllowGet); 
            } 
        }
    }
}


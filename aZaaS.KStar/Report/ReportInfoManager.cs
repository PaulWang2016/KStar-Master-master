using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace aZaaS.KStar.Report
{
    public class ReportInfoManager
    {

        public IList<ReportInfoEntity> GetReportInfos(Guid categoryID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportInfo.Where(x => x.ParnentID == categoryID).ToList();
            }
        }

        public IList<ReportInfoEntity> GetReportInfos(out int totalRows,
                                                    List<Guid> categoryIDs,
                                                    string status = "",
                                                    string level = "0",
                                                    string keySearch = "",
                                                    string startDate = "", string endDate = "",
                                                    int? page = 1, int? pageSize = 20, string sort = "", string dir = "")
        {
            totalRows = 0;

            using (var ctx = new KStarDbContext())
            {
                var q = from ri in ctx.ReportInfo
                        select ri;

                var pStatus = status == "1" ? "下架" : "上架";

                var pLevel = "";
                switch (level)
                {
                    case "1": pLevel = "员工级"; break;
                    case "2": pLevel = "部门级"; break;
                    case "3": pLevel = "公司级"; break;
                    default: break;
                }

                if (level != "0")
                {
                    q = q.Where(p => p.Level == pLevel);
                }

                if (!string.IsNullOrWhiteSpace(keySearch))
                {
                    q = q.Where(p => p.Name.Contains(keySearch) || p.Level.Contains(keySearch) || p.ReportCode.Contains(keySearch));
                }

                var start = startDate == "" ? DateTime.Now.AddMonths(-1) : DateTime.Parse(startDate);
                var end = endDate == "" ? DateTime.Now : DateTime.Parse(endDate).AddHours(23).AddMinutes(59).AddSeconds(59);

                var pr = q.ToList();
                if (categoryIDs.Count > 0)
                    q = q.Where(p => categoryIDs.Contains(p.ParnentID));

                q = q.Where(p => p.Status == pStatus && p.PublishedDate >= start && p.PublishedDate <= end);

                if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(dir))
                {
                    switch (sort)
                    {
                        case "ReportCode":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.ReportCode) : q.OrderBy(x => x.ReportCode);
                            break;
                        case "PublishedDate":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.PublishedDate) : q.OrderBy(x => x.PublishedDate);
                            break;
                        case "Level":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.Level) : q.OrderBy(x => x.Level);
                            break;
                        case "Department":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.Department) : q.OrderBy(x => x.Department);
                            break;
                        case "Category":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.Category) : q.OrderBy(x => x.Category);
                            break;
                        case "Status":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.Status) : q.OrderBy(x => x.Status);
                            break;
                        case "Name":
                            q = dir.Equals("desc") ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name);
                            break;
                    }
                }
                else
                    q = q.OrderByDescending(o => o.PublishedDate);

                totalRows = q.Count();

                if (page != null && pageSize != null)
                    return q.Skip(pageSize.Value * (page.Value - 1)).Take(pageSize.Value).ToList();            

                return q.ToList();
            }
        }

        public ReportInfoEntity GetReportInfo(Guid id)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportInfo.Where(x => x.ID == id).FirstOrDefault();
            }
        }

        public IList<ReportInfoEntity> GetAllReportInfos()
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportInfo.ToList();
            }
        }

        public ReportCategoryEntity GetCategory(Guid categoryID)
        {
            using (var ctx = new KStarDbContext())
            {
                return ctx.ReportCategory.FirstOrDefault(c => c.ID == categoryID);
            }
        }

        public IList<ReportCategoryEntity> GetReportCategoryByParentID(Guid? parnentID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportCategory.Where(x => x.ParnentID == parnentID).ToList<ReportCategoryEntity>();
            }
        }

        public IList<ReportCategoryEntity> GetCategories(Guid categoryID, bool includeChildren = true)
        {
            var categories = new List<ReportCategoryEntity>();
            using (KStarDbContext cxt = new KStarDbContext())
            {
                if (categoryID == Guid.Empty)
                {
                    var roots = cxt.ReportCategory.Where(c => c.ParnentID == Guid.Empty).ToList();

                    categories.AddRange(roots);
                    roots.ForEach(root =>
                    {
                        if (includeChildren)
                            RecursiveCategoryChildren(categories, cxt, root.ID);
                    });
                }
                else
                {
                    var self = cxt.ReportCategory.FirstOrDefault(c => c.ID == categoryID);

                    categories.Add(self);
                    if (includeChildren)
                        RecursiveCategoryChildren(categories, cxt, categoryID);
                }

                return categories;
            }
        }

        private void RecursiveCategoryChildren(List<ReportCategoryEntity> categories, KStarDbContext cxt, Guid categoryID)
        {
            var chilren = cxt.ReportCategory.Where(x => x.ParnentID == categoryID).ToList<ReportCategoryEntity>();

            if (chilren != null && chilren.Any())
            {
                categories.AddRange(chilren);

                chilren.ForEach(c =>
                {
                    RecursiveCategoryChildren(categories, cxt, c.ID);
                });
            }

        }

        public List<ReportPermission> GetReportPermissions(Guid reportID)
        {
            using (var ctx = new KStarDbContext())
            {
                return ctx.ReportPermissions.Where(p => p.ReportID == reportID).ToList();
            }
        }

        /// <summary>
        /// 获取报表点击率
        /// </summary>
        /// <returns></returns>
        public int GetReportRate(Guid reportID, Guid sysID)
        {
            using (var ctx = new KStarDbContext())
            {
                return ctx.ReportStatistics.Where(p => p.ReportID == reportID && p.SysID == sysID).Count();
            } 
        }

        public bool HasPermissions(IEnumerable<Guid> roles, Guid reportID)
        {
            using (var ctx = new KStarDbContext())
            {
               return ctx.ReportPermissions.Any(rp => rp.ReportID == reportID && roles.Contains(rp.RoleID));
            }
        }

        public bool AddReportCategory(ReportCategoryEntity entity)
        {
            using (var ctx = new KStarDbContext())
            {

                ctx.ReportCategory.Add(entity);
                ctx.SaveChanges();
            }
            return true;
        }

        public bool AddReportInfo(ReportInfoEntity report)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    //var rep = ctx.ReportInfo.First();
                    ctx.ReportInfo.Add(report);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        public bool AddReportInfo(ReportInfoEntity report, IEnumerable<ReportPermission> permissions)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                ctx.ReportInfo.Add(report);

                if (permissions != null && permissions.Any())
                {
                    ctx.ReportPermissions.AddRange(permissions);
                }

                ctx.SaveChanges();

            }

            return true;
        }

        public bool UpdateReportInfo(ReportInfoEntity newReportInfo)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    var oldReportInfo = ctx.ReportInfo.FirstOrDefault(x => x.ID == newReportInfo.ID);
                    oldReportInfo.Name = newReportInfo.Name;
                    oldReportInfo.Department = newReportInfo.Department;
                    oldReportInfo.PublishedDate = newReportInfo.PublishedDate;
                    oldReportInfo.Level = newReportInfo.Level;
                    oldReportInfo.Category = newReportInfo.Category;
                    oldReportInfo.ReportCode = newReportInfo.ReportCode;
                    oldReportInfo.Status = newReportInfo.Status;
                    //oldReportInfo.Rate = newReportInfo.Rate;
                    oldReportInfo.ReportUrl = newReportInfo.ReportUrl;
                    oldReportInfo.ImageThumbPath = newReportInfo.ImageThumbPath;
                    oldReportInfo.Comment = newReportInfo.Comment;
                    //oldReportInfo.IsFavourite = newReportInfo.IsFavourite;
                    ctx.SaveChanges();

                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        public bool UpdateReportInfo(ReportInfoEntity newReportInfo, IEnumerable<ReportPermission> permissions)
        {

            using (var ctx = new KStarDbContext())
            {

                ctx.Entry<ReportInfoEntity>(newReportInfo).State = EntityState.Modified;

                if (permissions != null && permissions.Any())
                {
                    var originPermissions = ctx.ReportPermissions.Where(p => p.ReportID == newReportInfo.ID).ToList();
                    ctx.ReportPermissions.RemoveRange(originPermissions);

                    ctx.ReportPermissions.AddRange(permissions);
                }

                ctx.SaveChanges();
            }
            return true;
        }

        public bool UpdateCategory(ReportCategoryEntity newCategory)
        {
            using (var ctx = new KStarDbContext())
            {
                ctx.Entry<ReportCategoryEntity>(newCategory).State = EntityState.Modified;
                ctx.SaveChanges();
            }

            return true;
        }

        public bool UpdateReportRate(ReportInfoEntity newReportInfo)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    var oldReportInfo = ctx.ReportInfo.FirstOrDefault(x => x.ID == newReportInfo.ID);
                    oldReportInfo.Rate = newReportInfo.Rate;
                    ctx.SaveChanges();

                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除指定的报表信息
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public bool DeleteReportInfo(ReportInfoEntity report)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    ctx.ReportInfo.Remove(report);

                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        public bool RemoveCategory(Guid categoryID)
        {
            using (var ctx = new KStarDbContext())
            {
                if (categoryID == Guid.Empty)
                    throw new InvalidOperationException("You can not remove this root category.");

                var category = ctx.ReportCategory.FirstOrDefault(c => c.ID == categoryID);

                var categoryIDs = GetCategories(categoryID).Select(c => c.ID);

                var reports = ctx.ReportInfo.Where(r => categoryIDs.Contains(r.ParnentID)).ToList();
                foreach (var item in reports)
                {
                    item.ParnentID = Guid.Empty;
                }

                ctx.ReportCategory.Remove(category);
                ctx.SaveChanges();
            }

            return true;
        }

        public bool RemoveReport(Guid reportID)
        {
            using (var ctx = new KStarDbContext())
            {
                if (reportID == Guid.Empty)
                    throw new InvalidOperationException("Invalid report.");

                var report = ctx.ReportInfo.FirstOrDefault(r => r.ID == reportID);

                var statisticsList = ctx.ReportStatistics.Where(x => x.ReportID == reportID);
                ctx.ReportInfo.Remove(report);
                //删除统计列表
                ctx.ReportStatistics.RemoveRange(statisticsList);
                ctx.SaveChanges();
            }

            return true;
        }

    }
}

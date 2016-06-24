using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace aZaaS.KStar.Report
{
    public class ReportCategoryManager
    {

        /// 获取报表类别的根节点
        /// </summary>
        /// <param name="rootID"></param>
        /// <returns></returns>
        public ReportCategoryEntity GetCategoryRoot(Guid rootID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportCategory.FirstOrDefault(x => x.ID == rootID);
            }
        }


        public IList<ReportCategoryEntity> GetReportIDCategory(Guid categoryID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    return ctx.ReportCategory.Where(x => x.ParnentID == categoryID).ToList<ReportCategoryEntity>();
                }
                catch
                {
                    return null;
                }

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


        /// <summary>
        /// 获取指定节点下的子节点的集合
        /// </summary>
        /// <param name="parnentID">父节点的ID</param>
        /// <returns></returns>
        /// 
        public IList<ReportCategoryEntity> GetReportCategoryByParentID(Guid? parnentID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportCategory.Where(x => x.ParnentID == parnentID).ToList<ReportCategoryEntity>();
            }
        }

        public IList<ReportCategoryEntity> GetReportIDByCategory(String Category)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    return ctx.ReportCategory.Where(x => x.Category == Category).ToList<ReportCategoryEntity>();
                }
                catch
                {
                    return null;
                }

            }
        }

        /// <summary>
        /// 添加一个新的类别，注意要为其指定ParentID
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool AddReportCategory(ReportCategoryEntity entity)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    ctx.ReportCategory.Add(entity);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }
        public bool UpdateReportCategory(Guid oldID, ReportCategoryEntity newEntity)
        {


            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    var oldCategoryReport = ctx.ReportCategory.FirstOrDefault(x => x.ID == oldID);
                    oldCategoryReport.Category = newEntity.Category;
                    oldCategoryReport.Comment = newEntity.Comment;
                    oldCategoryReport.ParnentID = newEntity.ParnentID;

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
        /// 从数据库中删除指定的报表分类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool DeleteReportCategory(ReportCategoryEntity entity)
        {
            var result = true;
            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    ctx.ReportCategory.Remove(entity);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;
        }

        public bool DeleteReport(Guid id)
        {
            var result = true;

            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    var item = ctx.ReportCategory.Where(p => p.ID == id).FirstOrDefault();
                    if (item != null)
                    {
                        ctx.ReportCategory.Remove(item);
                        ctx.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                }
            }
            return result;

        }
        public bool UpdateModifyNameFavourite(Guid ID, ReportCategoryEntity newEntity)
        {
            var result = true;

            using (KStarDbContext ctx = new KStarDbContext())
            {
                try
                {
                    var oldModifyNameCategory = ctx.ReportCategory.FirstOrDefault(x => x.ID == ID);
                    oldModifyNameCategory.Category = newEntity.Category;



                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    result = false;
                }
            }
            return result;

        }





    }
}

using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Report
{
    public class ReportFavouriteManager
    {

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Favorites(Report_FavouriteEntity entity)
        {

            try
            {
                using (KStarDbContext ctx = new KStarDbContext())
                {
                    var count = ctx.Report_Favourite.Where(x => x.ReportInfoID == entity.ReportInfoID && x.FavouriteID == entity.FavouriteID && x.UserID == entity.UserID).Count();
                    if (count <= 0)
                    {
                        ctx.Report_Favourite.Add(entity);
                        ctx.SaveChanges();
                    }
                     //已经存在的分类（自定义分类下）
                }

            }
            catch (Exception ex)
            {
                //TODO:
                throw ex;
            }

            return true;
        }


        /// <summary>
        ///  
        /// </summary>
        /// <param name="reportID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<ReportFavouriteEntity> GetCategory(Guid categoryID, Guid userID)
        {
            try
            {
                using (KStarDbContext ctx = new KStarDbContext())
                {
                    List<ReportFavouriteEntity> category = ctx.ReportFavourite.Where(x => x.ID == categoryID && x.UserID == userID).ToList();
                    return category;
                }
            }
            catch (Exception ex)
            {
                //TODO:
                return new List<ReportFavouriteEntity>();
            } 
        }

        public static List<ReportFavouriteEntity> GetFavoritesCategoryByParentID(Guid? parnentID, Guid userID)
        {
            using (KStarDbContext ctx = new KStarDbContext())
            {
                return ctx.ReportFavourite.Where(x => x.ParnentID == parnentID && x.UserID == userID).ToList<ReportFavouriteEntity>();
            }
        }


        public static bool AddFavorites(string parentID, string name, Guid userID, string comment)
        {
            try
            {
                ReportFavouriteEntity entity = new ReportFavouriteEntity();
                    entity.ID = Guid.NewGuid();
                    entity.Name = name;
                    entity.ParnentID = new Guid(parentID);
                    entity.UserID = userID;
                    entity.Comment = comment;
             
                using (KStarDbContext ctx = new KStarDbContext())
                {
                    //同一节点下的名称要唯一
                    var count = ctx.ReportFavourite.Where(x => x.UserID == entity.UserID && x.ParnentID == entity.ParnentID && x.Name == entity.Name).Count();

                   if (count <= 0)
                   {
                       ctx.ReportFavourite.Add(entity);
                       ctx.SaveChanges();
                   }
                   else
                   {
                       throw new Exception("The classification of already exists.");
                   } 
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }


        public static bool UpdateCategories(string id, string name, string comment)
        {
            try
            {
                Guid guid = new Guid(id);
             
                using (KStarDbContext ctx = new KStarDbContext())
                {

                    var entity = ctx.ReportFavourite.Where(x => x.ID == guid).FirstOrDefault();
                    if (entity==null)
                    {
                        throw new Exception("This category does not exist.");
                    }
                    entity.Name = name;
                    entity.Comment = comment;

                    ctx.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        public static bool DeleteCategories(string id)
        {
            Guid guid = new Guid(id);
            try
            {
                using (KStarDbContext ctx = new KStarDbContext())
                {
                    var reportFavouriteList = ctx.ReportFavourite.Where(x => x.ID == guid).ToList();
                    if (reportFavouriteList == null || reportFavouriteList.Count == 0) throw new Exception("This category does not exist");
                    var Report_FavouriteList = ctx.Report_Favourite.Where(x => x.FavouriteID == guid).ToList();
                    ctx.ReportFavourite.RemoveRange(reportFavouriteList);
                    ctx.Report_Favourite.RemoveRange(Report_FavouriteList);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
         
            return true;
        }


        public static void RemoveFavorites(string id, string favouriteID, Guid userID)
        {
            Guid guid = new Guid(id);

            try
            {
                using (KStarDbContext ctx = new KStarDbContext())
                {
                    var linq = ctx.Report_Favourite.Where(x => x.ReportInfoID == guid && x.UserID == userID);
                    if (!string.IsNullOrWhiteSpace(favouriteID))
                    {
                        var _favouriteID = new Guid(favouriteID);
                        linq = linq.Where(x => x.FavouriteID == _favouriteID);
                    }
                    var enityList = linq.ToList();
                    if (enityList != null)
                    {
                        ctx.Report_Favourite.RemoveRange(enityList);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 获取报表信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static List<ReportInfoEntity> GetFavouriteReports(string id, Guid userID,string soft)
        {
            try
            {
                Guid categoryID = string.IsNullOrEmpty(id) ?  new Guid() : new Guid(id);
                using (KStarDbContext ctx = new KStarDbContext())
                { 
                    var linqfc = from fc in ctx.ReportFavourite select fc;
                    if (string.IsNullOrEmpty(id) || id == new Guid().ToString())
                    {
                        linqfc = linqfc.Where(x => x.UserID == userID);
                    }
                    else
                    {
                        linqfc = linqfc.Where(x => x.UserID == userID && x.ID == categoryID);
                    }

                    var linq = from fc in linqfc
                               join r in ctx.Report_Favourite on fc.ID equals r.FavouriteID
                               into pro
                               from rf in pro.DefaultIfEmpty()
                               join r in ctx.ReportInfo on rf.ReportInfoID equals r.ID
                               select r;

                    switch (soft)
                    {
                        case "Score": return linq.OrderByDescending(x => x.Rate).ToList();
                        case "Date": return linq.OrderByDescending(x => x.PublishedDate).ToList();
                        case "Level": return linq.OrderByDescending(x => x.Level).ToList();
                        case "Department": return linq.OrderByDescending(x => x.Department).ToList();
                        case "Category": return linq.OrderByDescending(x => x.Category).ToList();
                    }

                    return linq.ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static IList<ReportInfoEntity> GetReportInfos(Guid userID, List<Guid> categoryIDs, string type = "")
        {
            using (KStarDbContext ctx = new KStarDbContext())
            { 
              var identitys=  ReportFavouriteManager.GetIdentity(userID);
                 
                //权限
              var rp = from p in ctx.ReportPermissions where identitys.Contains(p.RoleID) select p;
                //报表
              var q = from p in rp
                        join ri in ctx.ReportInfo on p.ReportID equals ri.ID
                        select ri;
                 //分类
                if (categoryIDs.Count > 0)
                    q = q.Where(p => categoryIDs.Contains(p.ParnentID));
                 
                switch (type)
                {
                    case "Score": return q.OrderByDescending(x => x.Rate).ToList();
                    case "Date": return q.OrderByDescending(x => x.PublishedDate).ToList();
                    case "Level": return q.OrderByDescending(x => x.Level).ToList();
                    case "Department": return q.OrderByDescending(x => x.Department).ToList();
                    case "Category": return q.OrderByDescending(x => x.Category).ToList();

                    default: return q.ToList();
                }
            }
        }
  

       /// <summary>
        /// 获取用户角色信息
        /// </summary>
        /// <returns></returns>
        public static IList<System.Guid> GetUserRoles(Guid sysID)
        {
            IList<System.Guid> Roles = null;
            try
            { 
                using (KStarFramekWorkDbContext kstr = new KStarFramekWorkDbContext())
                {
                    var linq =
                               from r in kstr.RoleUser
                               where r.User_SysId == sysID
                               select r.Role_SysId;


                    Roles = linq.ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return Roles;
        }

        public static IList<System.Guid> GetIdentity(Guid sysID)
        {
            //用户、职位、部门、系统角色

            try
            {

                using (KStarFramekWorkDbContext kstr = new KStarFramekWorkDbContext())
                {
                    var rolelinq = from r in kstr.RoleUser
                                   where r.User_SysId == sysID
                                   select r.Role_SysId;

                    var orglinq = from o in kstr.UserOrgNodes
                                  where o.User_SysId == sysID
                                  select o.OrgNode_SysId;

                    var pling = from p in kstr.PositionUsers
                                where p.User_SysId == sysID
                                select p.Position_SysId;

                    var result = rolelinq.Union(orglinq).Union(pling).ToList();

                    if (result != null)
                    {
                        result.Add(sysID);

                        return result;
                    }
                    else
                    {
                        return new List<System.Guid>();
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<System.Guid>();
            }

           
        }
    }
}

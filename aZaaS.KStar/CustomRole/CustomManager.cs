using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.CustomRole.Models;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.CustomRole
{
    public class CustomManager
    {
        #region CustomRoleCategory
        /// <summary>
        /// 创建角色目录
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public Guid CreateCategory(CustomRoleCategory category)
        {
            CustomRoleCategory parent = null;

            if (category == null)
            {
                throw new ArgumentException("category is not assigned.");
            }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                if (category.Parent_SysId != Guid.Empty)
                {
                    parent = dbContext.CustomRoleCategory.SingleOrDefault(x => x.SysID == category.Parent_SysId);

                    if (parent == null)
                    {
                        throw new ArgumentException("can not found the specified category parent.");
                    }
                }

                dbContext.CustomRoleCategory.Add(category);
                dbContext.SaveChanges();

                return category.SysID;
            }
        }

        public void UpdateCategory(CustomRoleCategory category)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.CustomRoleCategory.FirstOrDefault(r => r.SysID == category.SysID);
                item.Name = category.Name;

                dbContext.SaveChanges();
            }
        } 

        /// <summary>
        /// 获取角色目录
        /// </summary>
        /// <param name="parentId">父节点ID</param>
        /// <returns></returns>
        public List<CustomRoleCategory> GetCategoryByParentId(Guid parentId)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var list = dbContext.CustomRoleCategory.Where(r => r.Parent_SysId == parentId).ToList();

                return list;
            }
        }

        /// <summary>
        /// 删除Category
        /// </summary>
        /// <param name="id">List_Position</param>
        /// <returns></returns>
        public bool DestroyCategory(Guid id)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var categorys = dbContext.CustomRoleCategory.Where(r => r.SysID == id || r.Parent_SysId == id);

                if (categorys == null)
                {
                    return false;
                }

                foreach (var category in categorys)
                {
                    //删除目录
                    dbContext.CustomRoleCategory.Remove(category);

                    //删除目录下的角色
                    DestroyRoleClassifyByParentId(category.SysID);
                }

                dbContext.SaveChanges();
            }

            return true;
        }

        #endregion

        #region CustomRoleClassify
        public List<CustomRoleClassify> GetEnabledClassify()
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var list = dbContext.CustomRoleClassify.Where(r => r.Status == "Y").ToList();

                return list;
            }
        }


        /// <summary>
        /// 创建角色分类
        /// </summary>
        /// <param name="classify"></param>
        /// <returns></returns>
        public Guid CreateCustomRoleClassify(CustomRoleClassify classify)
        {
            if (classify == null)
            {
                throw new ArgumentException("classify is not assigned.");
            }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var parent = dbContext.CustomRoleCategory.SingleOrDefault(r => r.SysID == classify.Category_SysId);

                if (parent == null)
                {
                    throw new ArgumentException("can not found the specified classify parent.");
                }

                dbContext.CustomRoleClassify.Add(classify);
                dbContext.SaveChanges();

                return classify.SysID;
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="classify"></param>
        public void UpdateCustomRole(CustomRoleClassify classify)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.CustomRoleClassify.FirstOrDefault(r => r.SysID == classify.SysID);
                item.RoleName = classify.RoleName;
                item.Status = classify.Status;

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="classify"></param>
        public Guid UpdateCustomRoleStatus(CustomRoleClassify classify)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.CustomRoleClassify.FirstOrDefault(r => r.SysID == classify.SysID);
                item.Status = classify.Status;

                dbContext.SaveChanges();

                return item.Category_SysId;
            }
        } 

        /// <summary>
        /// 获取角色目录下角色分类
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<CustomRoleClassify> GetClassifyByCategoryId(Guid categoryId)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var list = dbContext.CustomRoleClassify.Where(r => r.Category_SysId == categoryId).ToList();

                return list;
            }
        }

        /// <summary>
        /// 获取角色目录下角色分类
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<CustomRoleClassify> GetEnabledClassifyByCategoryId(Guid categoryId)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var list = dbContext.CustomRoleClassify.Where(r => r.Category_SysId == categoryId && r.Status == "Y").ToList();

                return list;
            }
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CustomRoleClassify GetClassifyById(Guid Id)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.CustomRoleClassify.FirstOrDefault(r => r.SysID == Id);

                return item;
            }
        }

        /// <summary>
        /// 删除Classify
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DestroyRoleClassify(Guid id)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var entity = dbContext.CustomRoleClassify.FirstOrDefault(r => r.SysID == id);

                if (entity == null)
                {
                    return false;
                }

                dbContext.CustomRoleClassify.Remove(entity);

                dbContext.SaveChanges();
            }

            return true;
        }

        /// <summary>
        /// 删除Classify
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DestroyRoleClassifyByParentId(Guid id)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var list = dbContext.CustomRoleClassify.Where(r => r.Category_SysId == id);

                if (list == null)
                {
                    return false;
                }

                foreach(var classify in list)
                {
                    dbContext.CustomRoleClassify.Remove(classify);
                }

                dbContext.SaveChanges();
            }

            return true;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.Framework.Organization.Facade;
using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar
{
    /// <summary>
    /// Role business object.
    /// </summary>
    public class RoleBO : AbstractBO
    {
        private readonly RoleFacade roleFacade;

        public RoleBO()
        {
            this.roleFacade = new RoleFacade();
        }

        ///<summary>
        /// Creates a new role.
        ///</summary>
        ///<param name="role">The new role instance</param>
        ///<returns>The new role id</returns>
        public Guid CreateRole(RoleWithRelationshipsDto roleDTO)
        {
            roleDTO.NullThrowArgumentEx("role dto is not assigned.");

            return this.roleFacade.CreateRole(Mapper.Map<RoleWithRelationshipsDto, Role>(roleDTO));
        }

        ///<summary>
        ///Checks whether the specified role name exists.
        ///</summary>
        ///<param name="roleName">The specified role name</param>
        ///<returns>True or false</returns>
        public bool RoleNameExists(string roleName)
        {
            return this.roleFacade.RoleNameExists(roleName);
        }

        ///<summary>
        ///Retrieves role according to the specified role id.
        ///</summary>
        ///<param name="roleId">The specified role id</param>
        ///<returns>The matching role instance</returns>
        public RoleWithRelationshipsDto ReadRole(Guid roleId)
        {
            return Mapper.Map<Role, RoleWithRelationshipsDto>(this.roleFacade.ReadRole(roleId));
        }

        ///<summary>
        ///Updates the specified role.
        ///</summary>
        ///<param name="role">The specified role instance</param>
        public void UpdateRole(RoleWithRelationshipsDto roleDTO)
        {
            roleDTO.NullThrowArgumentEx("role dto is not assigned.");

            this.roleFacade.UpdateRole(Mapper.Map<RoleWithRelationshipsDto, Role>(roleDTO));
        }

        ///<summary>
        ///Deletes a specified role.
        ///</summary>
        ///<param name="roleId">The specified role id</param>
        public void DeleteRole(Guid roleId)
        {
            this.roleFacade.DeleteRole(roleId);
        }

        ///<summary>
        ///Retrieves all roles.
        ///</summary>
        ///<returns>All roles list</returns>
        public IEnumerable<RoleWithRelationshipsDto> GetAllRoles()
        {
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleWithRelationshipsDto>>(this.roleFacade.GetAllRoles());
        }

        ///<summary>
        ///Checks whether a user is assigned to the specified role.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<param name="roleId">Teh specified role id</param>
        ///<returns>True or false</returns>
        public bool RoleUserExists(Guid userId, Guid roleId)
        {
            return this.roleFacade.RoleUserExists(userId, roleId);
        }

        ///<summary>
        ///Retrieves role list of the specified user.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<returns>The matching role list</returns>
        public IEnumerable<RoleWithRelationshipsDto> GetUserRoles(Guid userId)
        {
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleWithRelationshipsDto>>(this.roleFacade.GetUserRoles(userId));
        }

        ///<summary>
        ///Retrieves user list of the specified role.
        ///</summary>
        ///<param name="roleId">The specified role id</param>
        ///<returns>The matching user list</returns>
        public IEnumerable<UserBaseDto> GetRoleUsers(Guid roleId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.roleFacade.GetRoleUsers(roleId));
        }
        public IEnumerable<UserBaseDto> GetRoleUsersWithPage(Guid roleId, System.Linq.Expressions.Expression<Func<User, bool>> express, int page, int pageSize, out int total)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.roleFacade.GetRoleUsersWithPage(roleId,express,page,pageSize,out total));
        }
        /// <summary>
        /// Retrieves role Category according to the specified category id. 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public RoleCategoryWithRelationshipsDto ReadCategory(Guid categoryId)
        {
            return Mapper.Map<RoleCategory, RoleCategoryWithRelationshipsDto>(this.roleFacade.ReadCategory(categoryId));
        }
        /// <summary>
        /// Retrieves role Category according to the specified category Name. 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public RoleCategoryWithRelationshipsDto ReadCategory(string categoryName)
        {
            return Mapper.Map<RoleCategory, RoleCategoryWithRelationshipsDto>(this.roleFacade.ReadCategory(categoryName));
        }
        /// <summary>
        /// Updates the specified role category.
        /// </summary>
        /// <param name="category"></param>
        public void UpdateCategory(RoleCategoryWithRelationshipsDto category)
        {
            this.roleFacade.UpdateCategory(Mapper.Map<RoleCategoryWithRelationshipsDto, RoleCategory>(category));
        }
        /// <summary>
        /// Updates the parent of the specified role category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="parentId"></param>
        public void UpdateCategoryParent(Guid categoryId, Guid parentId)
        {
            this.roleFacade.UpdateCategoryParent(categoryId, parentId);
        }
        /// <summary>
        ///  Retrieves all role categories.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RoleCategoryWithChildCategoriesDto> GetAllCategories()
        {
            return Mapper.Map<IEnumerable<RoleCategory>, IEnumerable<RoleCategoryWithChildCategoriesDto>>(this.roleFacade.GetAllCategories());
        }
        /// <summary>
        /// Retrieves all child categories of the specified role category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<RoleCategoryWithChildCategoriesDto> GetChildCategories(Guid categoryId)
        {
            return Mapper.Map<IEnumerable<RoleCategory>, IEnumerable<RoleCategoryWithChildCategoriesDto>>(this.roleFacade.GetChildCategories(categoryId));
        }

        public RoleCategoryWithParentDto GetRoleCategoryWithParent(Guid categoryId)
        {
            return Mapper.Map<RoleCategory, RoleCategoryWithParentDto>(this.roleFacade.ReadCategory(categoryId));
        }
        /// <summary>
        /// Updates the category of the specified role.
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="categoryId"></param>
        public void UpdateRoleCategory(Guid roleId, Guid categoryId)
        {
            this.roleFacade.UpdateRoleCategory(roleId,categoryId);
        }
        /// <summary>
        ///  Creates a new Category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public Guid CreateCategory(RoleCategoryWithRelationshipsDto category)
        {
            return this.roleFacade.CreateCategory(Mapper.Map<RoleCategoryWithRelationshipsDto, RoleCategory>(category));
        }

        public bool CategoryNameExists(string categoryName)
        {
            return this.roleFacade.CategoryNameExists(categoryName);
        }
        /// <summary>
        /// delete a specified role category.
        /// </summary>
        /// <param name="categoryId"></param>
        public void DeleteCategory(Guid categoryId)
        {
            this.roleFacade.DeleteCategory(categoryId);
        }
        /// <summary>
        /// Retrieves all roles of the specified role category.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<RoleWithCategoryDto> GetCategoryRoles(Guid categoryId)
        {
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleWithCategoryDto>>(this.roleFacade.GetCategoryRoles(categoryId));
        }
    }
}

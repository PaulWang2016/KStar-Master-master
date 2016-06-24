using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.Framework.Organization.Facade;
using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar
{
    public class PositionBO : AbstractBO
    {
        private readonly PositionFacade positionFacade;

        public PositionBO()
        {
            this.positionFacade = new PositionFacade();
        }

        ///<summary>
        ///Creates a new position.
        ///</summary>
        ///<param name="position">The new position instance</param>
        ///<returns>The new position id</returns>
        public Guid CreatePosition(PositionWithRelationshipsDto positionDTO)
        {
            positionDTO.NullThrowArgumentEx("position dto is not assigned.");

            return this.positionFacade.CreatePosition(Mapper.Map<PositionWithRelationshipsDto, Position>(positionDTO));
        }

        ///<summary>
        ///Checks whether the specified position name is exists.
        ///</summary>
        ///<param name="positionName">The specified position name</param>
        ///<returns>True or false</returns>
        public bool PositionNameExists(string positionName)
        {
           return this.positionFacade.PositionNameExists(positionName);
        }

        ///<summary>
        ///Retrieves position according to the specified position id.
        ///</summary>
        ///<param name="positionId">The specified poistion id</param>
        ///<returns>The matching position instance</returns>
        public PositionWithRelationshipsDto ReadPosition(Guid positionId)
        {
            return Mapper.Map<Position, PositionWithRelationshipsDto>(this.positionFacade.ReadPosition(positionId));
        }

        ///<summary>
        ///Retrieves position according to the specified position name.
        ///</summary>
        ///<param name="positionId">The specified poistion name</param>
        ///<returns>The matching position instance</returns>
        public PositionWithRelationshipsDto ReadPosition(string positionName)
        {
            return Mapper.Map<Position, PositionWithRelationshipsDto>(this.positionFacade.ReadPosition(positionName));
        }

        ///<summary>
        ///Updates the specified position
        ///</summary>
        ///<param name="position">The specified poistion instance</param>
        public void UpdatePosition(PositionWithRelationshipsDto positionDTO)
        {
            positionDTO.NullThrowArgumentEx("position dto is not assigned.");

            this.positionFacade.UpdatePosition(Mapper.Map<PositionWithRelationshipsDto, Position>(positionDTO));
        }

        ///<summary>
        ///Updates the category of the specified position.
        ///</summary>
        ///<param name="positionId">The specified position id</param>
        ///<param name="categoryId">The specified category id</param>
        public void UpdatePositionCategory(Guid positionId, Guid categoryId)
        {
            this.positionFacade.UpdatePositionCategory(positionId, categoryId);
        }

        ///<summary>
        ///Deletes a specified postion.
        ///</summary>
        ///<param name="positionId">The specified postion id</param>
        public void DeletePosition(Guid positionId)
        {
            this.positionFacade.DeletePosition(positionId);
        }

        ///<summary>
        ///Creates a new position category.
        ///</summary>
        ///<param name="category">The specified category id</param>
        ///<returns>The new category id</returns>
        public Guid CreateCategory(PositionCategoryWithRelationshipsDto categoryDTO)
        {
            categoryDTO.NullThrowArgumentEx("category dto is not assigned.");

            return this.positionFacade.CreateCategory(Mapper.Map<PositionCategoryWithRelationshipsDto, PositionCategory>(categoryDTO));
        }

        ///<summary>
        ///Checks whether the specified category name is exists.
        ///</summary>
        ///<param name="categoryName">The specified category name</param>
        ///<returns>True or false</returns>
        public bool CategoryNameExists(string categoryName)
        {
            return this.positionFacade.CategoryNameExists(categoryName);
        }

        ///<summary>
        ///Retrieves position category according to the specified category id.
        ///</summary>
        ///<param name="categoryId">The specified category id</param>
        ///<returns>The matching category instance</returns>
        public PositionCategoryWithRelationshipsDto ReadCategory(Guid categoryId)
        {
            return Mapper.Map<PositionCategory, PositionCategoryWithRelationshipsDto>(this.positionFacade.ReadCategory(categoryId));
        }

        ///<summary>
        ///Retrieves position category according to the specified category name.
        ///</summary>
        ///<param name="categoryId">The specified category name</param>
        ///<returns>The matching category instance</returns>
        public PositionCategoryWithRelationshipsDto ReadCategory(string categoryName)
        {
            return Mapper.Map<PositionCategory, PositionCategoryWithRelationshipsDto>(this.positionFacade.ReadCategory(categoryName));
        }

        ///<summary>
        ///Updates the specified position category.
        ///</summary>
        ///<param name="category">The specified category instance</param>
        public void UpdateCategory(PositionCategoryWithRelationshipsDto categoryDTO)
        {
            categoryDTO.NullThrowArgumentEx("category dto is not assigned");

            this.positionFacade.UpdateCategory(Mapper.Map<PositionCategoryWithRelationshipsDto, PositionCategory>(categoryDTO));
        }

        /// <summary>
        /// Updates the specified postion category parent.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="parentId"></param>
        public void UpdateCategoryParent(Guid categoryId, Guid parentId)
        {
            this.positionFacade.UpdateCategoryParent(categoryId, parentId);
        }

        ///<summary>
        ///Deletes a specified position category.
        ///</summary>
        ///<param name="categoryId">The specified category id</param>
        public void DeleteCategory(Guid categoryId)
        {
            this.positionFacade.DeleteCategory(categoryId);
        }

        ///<summary>
        ///Assigns the specified user to the specified position.
        ///</summary>
        ///<param name="positionId">The specified position id</param>
        ///<param name="userId">The specified user id</param>
        public void AppendUser( Guid positionId, Guid userId)
        {
            this.positionFacade.AppendUser(positionId, userId);
        }

        ///<summary>
        ///Removes a user from the specified position.
        ///</summary>
        ///<param name="positionId">The specified position id</param>
        ///<param name="userId">The specified user id</param>
        public void RemoveUser(Guid positionId, Guid userId)
        {
            this.positionFacade.RemoveUser(positionId, userId);
        }

        ///<summary>
        ///Check whether the specified user is already assigened to the specified postion.
        ///</summary>
        ///<param name="userName">The specification user name</param>
        ///<param name="positionName">The specified position name</param>
        ///<returns>True or false</returns>
        public bool PositionUserExists(Guid userId, Guid positionId)
        {
            return this.positionFacade.PositionUserExists(userId, positionId);
        }

        ///<summary>
        ///Assigns a field to the specified position.
        ///</summary>
        ///<param name="nodeId">The specified position id</param>
        ///<param name="field">The specified field</param>
        public void AppendExField(Guid sysId, PositionExFieldDTO fieldDTO)
        {
            fieldDTO.NullThrowArgumentEx("filed is dto is not assigned.");

            this.positionFacade.AppendField(sysId, Mapper.Map<PositionExFieldDTO, PositionExtend>(fieldDTO));
        }

        ///<summary>
        ///Removes a field from the specified position.
        ///</summary>
        ///<param name="nodeId">The specified position id</param>
        ///<param name="fieldId">The specified field id</param>
        public void RemoveExField(Guid sysId, string name)
        {
            this.positionFacade.RemoveField(sysId, name);
        }

        ///<summary>
        ///Updates the specified field.
        ///</summary>
        ///<param name="field">The specified field instance</param>
        public void UpdateExField(PositionExFieldDTO fieldDTO)
        {
            fieldDTO.NullThrowArgumentEx("field dto is not assigned.");

            this.positionFacade.UpdateField(Mapper.Map<PositionExFieldDTO, PositionExtend>(fieldDTO));
        }

        ///<summary>
        ///Retrieves all positions.
        ///</summary>
        ///<returns>All positions list</returns>
        public IEnumerable<PositionWithRelationshipsDto> GetAllPositons()
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithRelationshipsDto>>(this.positionFacade.GetAllPositions());
        }

        ///<summary>
        ///Retrieves all position categories.
        ///</summary>
        ///<returns>All position categories list</returns>
        public IEnumerable<PositionCategoryWithRelationshipsDto> GetAllCategories()
        {
            return Mapper.Map<IEnumerable<PositionCategory>, IEnumerable<PositionCategoryWithRelationshipsDto>>(this.positionFacade.GetAllCategories());
        }

        ///<summary>
        ///Retrieves all users of the specified position.
        ///</summary>
        ///<param name="positionName">The specified position id</param>
        ///<returns>The matching users list</returns>
        public IEnumerable<UserBaseDto> GetPositionUsers(Guid positionId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.positionFacade.GetPositionUsers(positionId));
        }

        ///<summary>
        ///Retrieves all positons of the specified user.
        ///</summary>
        ///<param name="userName">The specified user name</param>
        ///<returns>The matching positions list</returns>
        public IEnumerable<PositionWithRelationshipsDto> GetUserPositions(string userName)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithRelationshipsDto>>(this.positionFacade.GetUserPositions(userName));
        }

        ///<summary>
        ///Retrieves all positons of the specified user.
        ///</summary>
        ///<param name="userName">The specified user id</param>
        ///<returns>The matching positions list</returns>
        public IEnumerable<PositionWithRelationshipsDto> GetUserPositions(Guid userId)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithRelationshipsDto>>(this.positionFacade.GetUserPositions(userId));
        }

        ///<summary>
        ///Retrieves all positions of the specified category.
        ///</summary>
        ///<param name="categoryId">The specified category id</param>
        ///<returns>The macthing positions list</returns>
        public IEnumerable<PositionWithRelationshipsDto> GetCategoryPositions(Guid categoryId)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithRelationshipsDto>>(this.positionFacade.GetCategoryPositions(categoryId));
        }

        ///<summary>
        ///Retrieves all positions of the specified category.
        ///</summary>
        ///<param name="categoryId">The specified category id</param>
        ///<returns>The macthing positions list</returns>
        public IEnumerable<PositionWithFieldsDto> GetPositionsWithFieldByCategory(Guid categoryId)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithFieldsDto>>(this.positionFacade.GetCategoryPositions(categoryId));
        }

        public IEnumerable<PositionCategoryWithRelationshipsDto> GetChildCategories(Guid categoryId)
        {
            return Mapper.Map<IEnumerable<PositionCategory>, IEnumerable<PositionCategoryWithRelationshipsDto>>(this.positionFacade.GetChildCategories(categoryId));
        }

        ///<summary>
        ///Retrieves positions according to the specified query expression.
        ///</summary>
        ///<param name="expression">The specified query expression</param>
        ///<returns>The matching positions list</returns>
        public IEnumerable<PositionWithRelationshipsDto> GetPositions(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithRelationshipsDto>>(this.positionFacade.GetPositions(expression));
        }
    }
}

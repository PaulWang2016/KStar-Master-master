using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Organization.Facade;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Extend;

namespace aZaaS.KStar.MgmtServices
{
    public class PositionService
    {
        private readonly PositionFacade positionFacade;

        public PositionService()
        {
            this.positionFacade = new PositionFacade();
        }

        public PositionCategoryBaseDto ReadPositionCategoryBase(Guid categoryId)
        {
            return Mapper.Map<PositionCategory, PositionCategoryBaseDto>(this.positionFacade.ReadCategory(categoryId));
        }

        public PositionCategoryWithParentDto ReadPositionCategoryWithParent(Guid categoryId)
        {
            return Mapper.Map<PositionCategory, PositionCategoryWithParentDto>(this.positionFacade.ReadCategory(categoryId));
        }

        public IEnumerable<UserWithFieldsDto> GetPositionUsersWithFields(Guid positionId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithFieldsDto>>(this.positionFacade.GetPositionUsers(positionId));
        }
        public IEnumerable<UserBaseDto> GetPositionUsersBase(Guid positionId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.positionFacade.GetPositionUsers(positionId));
        }
        public IEnumerable<UserWithRelationshipsDto> GetPositionUsersWithRelationships(Guid positionId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.positionFacade.GetPositionUsers(positionId));
        }


        public IEnumerable<PositionCategoryWithChildCategoriesDto> GetAllCategoriesBase()
        {
            return Mapper.Map<IEnumerable<PositionCategory>, IEnumerable<PositionCategoryWithChildCategoriesDto>>(this.positionFacade.GetAllCategories());
        }
        public IEnumerable<PositionCategoryWithChildCategoriesDto> GetChildCategoriesBase(Guid categoryId)
        {
            return Mapper.Map<IEnumerable<PositionCategory>, IEnumerable<PositionCategoryWithChildCategoriesDto>>(this.positionFacade.GetChildCategories(categoryId));
        }

        public PositionWithCategoryDto ReadPositionWithCategory(Guid positionId)
        {
            return Mapper.Map<Position, PositionWithCategoryDto>(this.positionFacade.ReadPosition(positionId));
        }

        public PositionWithFieldsDto ReadPositionWithFields(Guid positionId)
        {
            return Mapper.Map<Position, PositionWithFieldsDto>(this.positionFacade.ReadPositionWithFields(positionId));
        }

        public IEnumerable<PositionBaseDto> GetAllPositonBases()
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionBaseDto>>(this.positionFacade.GetAllPositions());
        }

        public IEnumerable<PositionBaseDto> GetNonReferencePositions()
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionBaseDto>>(this.positionFacade.GetNonReferencePositions());
        }

        public IEnumerable<PositionWithCategoryDto> GetAllPositonsWithCategory()
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithCategoryDto>>(this.positionFacade.GetAllPositions());
        }
        public IEnumerable<PositionWithCategoryDto> GetPositions(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<Position>, IEnumerable<PositionWithCategoryDto>>(this.positionFacade.GetPositions(expression));
        }



        public PositionWithUsersDto ReadPositionWithUsers(string positionName)
        {
            return Mapper.Map<Position, PositionWithUsersDto>(this.positionFacade.ReadPosition(positionName));
        }

        public FieldBase[] GetPositionExtendFieldsDefition()
        {
            return this.positionFacade.GetExtendFiledsDefintion();
        }

        public void SavePositionExtendFieldsDefition(FieldBase[] fields)
        {
            this.positionFacade.SaveExtendFiledsDefintion(fields);
        }

        public IEnumerable<OrgNodeWithRelationshipsDto> GetPositionNodes(Guid positionId)
        {
            return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithRelationshipsDto>>(this.positionFacade.GetPositionNodes(positionId));
        }

    }
}

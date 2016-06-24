using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Workflow;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Organization.FieldExtend;
using aZaaS.Framework.Extend;
using aZaaS.KStar.DataDictionary;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.LogRequestProvider;

namespace aZaaS.KStar
{
    public class AutoMapperConfiguration
    {
        /// <summary>
        /// Initialize the mapping relationships of entities and dtos 
        /// </summary>
        public static void Initialize()
        {
            Mapper.CreateMap<OrgChart, OrgChartDTO>();
            Mapper.CreateMap<OrgNode, OrgNodeDTO>();
            Mapper.CreateMap<OrgNodeExtend, OrgNodeExFieldDTO>();
            Mapper.CreateMap<Role, RoleDTO>();
            Mapper.CreateMap<Role, RoleWithCategoryDto>();
            Mapper.CreateMap<User, UserDTO>();
            Mapper.CreateMap<User, UserDto>();
            Mapper.CreateMap<UserExtend, UserExFieldDTO>();
            Mapper.CreateMap<Position, PositionDTO>();
            Mapper.CreateMap<PositionExtend, PositionExFieldDTO>();
            Mapper.CreateMap<PositionCategory, PositionCategoryDTO>();
            Mapper.CreateMap<BusinessDataConfig, BusinessDataConfigDTO>()
                  .ForMember(dest => dest.SysID, meo => meo.ResolveUsing(bdc => bdc.Id));
            Mapper.CreateMap<BusinessDataColumn, BusinessDataColumnDTO>()
                .ForMember(dest => dest.SysID, meo => meo.ResolveUsing( bdc => bdc.Id));


            Mapper.CreateMap<OrgChartDTO, OrgChart>();
            Mapper.CreateMap<OrgNodeDTO, OrgNode>();
            Mapper.CreateMap<OrgNodeExFieldDTO, OrgNodeExtend>();
            Mapper.CreateMap<RoleDTO, Role>();
            Mapper.CreateMap<RoleWithCategoryDto,Role>();
            Mapper.CreateMap<UserDTO, User>();
            Mapper.CreateMap<UserDto, User>();
            Mapper.CreateMap<UserExFieldDTO, UserExtend>();
            Mapper.CreateMap<PositionDTO, Position>();
            Mapper.CreateMap<PositionExFieldDTO, PositionExtend>();
            Mapper.CreateMap<PositionCategoryDTO, PositionCategory>();
            Mapper.CreateMap<BusinessDataConfigDTO, BusinessDataConfig>()
                  .ForMember(dest => dest.Id, meo => meo.ResolveUsing(bdcd => bdcd.SysID));
            Mapper.CreateMap<BusinessDataColumnDTO, BusinessDataColumn>()
                  .ForMember(dest => dest.Id,meo => meo.ResolveUsing(bdcd => bdcd.SysID));                    

            Mapper.CreateMap<OrgChart, OrgChartBaseDto>();
            Mapper.CreateMap<OrgChartBaseDto, OrgChart>();

            Mapper.CreateMap<OrgChart, OrgChartWithNodesDto>();
            Mapper.CreateMap<OrgChartWithNodesDto, OrgChart>();

            Mapper.CreateMap<OrgChart, OrgChartWithRelationshipsDto>();
            Mapper.CreateMap<OrgChartWithRelationshipsDto, OrgChart>();

            Mapper.CreateMap<OrgChart, OrgChartWithRootDto>();
            Mapper.CreateMap<OrgChartWithRootDto, OrgChart>();

            Mapper.CreateMap<OrgNode, OrgNodeBaseDto>();
            Mapper.CreateMap<OrgNodeBaseDto, OrgNode>();

            Mapper.CreateMap<OrgNode, OrgNodeWithChartDto>();
            Mapper.CreateMap<OrgNodeWithChartDto, OrgNode>();

            Mapper.CreateMap<OrgNode, OrgNodeWithChildNodesDto>();
            Mapper.CreateMap<OrgNodeWithChildNodesDto, OrgNode>();

            Mapper.CreateMap<OrgNode, OrgNodeWithFieldsDto>();
            Mapper.CreateMap<OrgNodeWithFieldsDto, OrgNode>();
            Mapper.CreateMap<OrgNodeExtend, ExtensionFieldDto>();
            Mapper.CreateMap<ExtensionFieldDto, OrgNodeExtend>();

            Mapper.CreateMap<OrgNode, OrgNodeWithParentDto>();
            Mapper.CreateMap<OrgNodeWithParentDto, OrgNode>();

            Mapper.CreateMap<OrgNode, OrgNodeWithPositionsDto>();
            Mapper.CreateMap<OrgNodeWithPositionsDto, OrgNode>();

            Mapper.CreateMap<OrgNode, OrgNodeWithRelationshipsDto>();
            Mapper.CreateMap<OrgNodeWithRelationshipsDto, OrgNode>();


            Mapper.CreateMap<OrgNode, OrgNodeWithChartParentDto>();
            Mapper.CreateMap<OrgNodeWithChartParentDto, OrgNode>();


            Mapper.CreateMap<OrgNode, OrgNodeWithUsersDto>();
            Mapper.CreateMap<OrgNodeWithUsersDto, OrgNode>();

            Mapper.CreateMap<Position, PositionBaseDto>();
            Mapper.CreateMap<PositionBaseDto, Position>();

            Mapper.CreateMap<PositionCategory, PositionCategoryBaseDto>();
            Mapper.CreateMap<PositionCategoryBaseDto, PositionCategory>();

            Mapper.CreateMap<PositionCategory, PositionCategoryWithChildCategoriesDto>();
            Mapper.CreateMap<PositionCategoryWithChildCategoriesDto, PositionCategory>();

            Mapper.CreateMap<PositionCategory, PositionCategoryWithParentDto>();
            Mapper.CreateMap<PositionCategoryWithParentDto, PositionCategory>();

            Mapper.CreateMap<PositionCategory, PositionCategoryWithPositionsDto>();
            Mapper.CreateMap<PositionCategoryWithPositionsDto, PositionCategory>();

            Mapper.CreateMap<PositionCategory, PositionCategoryWithRelationshipsDto>();
            Mapper.CreateMap<PositionCategoryWithRelationshipsDto, PositionCategory>();

            Mapper.CreateMap<Position, PositionWithCategoryDto>();
            Mapper.CreateMap<PositionWithCategoryDto, Position>();

            Mapper.CreateMap<Position, PositionWithNodesDto>();
            Mapper.CreateMap<PositionWithNodesDto, Position>();

            Mapper.CreateMap<Position, PositionWithRelationshipsDto>();
            Mapper.CreateMap<PositionWithRelationshipsDto, Position>();

            Mapper.CreateMap<Position, PositionWithUsersDto>();
            Mapper.CreateMap<PositionWithUsersDto, Position>();

            Mapper.CreateMap<PositionExtend, ExtensionFieldDto>();
            Mapper.CreateMap<ExtensionFieldDto, PositionExtend>();

            Mapper.CreateMap<Position, PositionWithFieldsDto>();
            Mapper.CreateMap<PositionWithFieldsDto, Position>();

            Mapper.CreateMap<Role, RoleBaseDto>();
            Mapper.CreateMap<RoleBaseDto, Role>();

            Mapper.CreateMap<Role, RoleWithRelationshipsDto>();
            Mapper.CreateMap<RoleWithRelationshipsDto, Role>();

            
            Mapper.CreateMap<RoleCategory, RoleCategoryBaseDto>();
            Mapper.CreateMap<RoleCategoryBaseDto, RoleCategory>();

            Mapper.CreateMap<RoleCategory, RoleCategoryWithChildCategoriesDto>();
            Mapper.CreateMap<RoleCategoryWithChildCategoriesDto,RoleCategory>();

            Mapper.CreateMap<RoleCategory, RoleCategoryWithParentDto>();
            Mapper.CreateMap<RoleCategoryWithParentDto,RoleCategory>();

            Mapper.CreateMap<RoleCategory, RoleCategoryWithRelationshipsDto>();
            Mapper.CreateMap<RoleCategoryWithRelationshipsDto,RoleCategory>();

            Mapper.CreateMap<RoleCategory, RoleCategoryWithRolesDto>();
            Mapper.CreateMap<RoleCategoryWithRolesDto,RoleCategory>();

            Mapper.CreateMap<User, UserBaseDto>();
            Mapper.CreateMap<UserBaseDto, User>();

            Mapper.CreateMap<User, UserWithFieldsDto>();
            Mapper.CreateMap<UserWithFieldsDto, User>();
            Mapper.CreateMap<UserExtend, ExtensionFieldDto>();
            Mapper.CreateMap<ExtensionFieldDto, UserExtend>();

            Mapper.CreateMap<User, UserWithNodesDto>();
            Mapper.CreateMap<UserWithNodesDto, User>();

            Mapper.CreateMap<User, UserWithOwnersDto>();
            Mapper.CreateMap<UserWithOwnersDto, User>();

            Mapper.CreateMap<User, UserWithPositionsDto>();
            Mapper.CreateMap<UserWithPositionsDto, User>();

            Mapper.CreateMap<User, UserWithRelationshipsDto>();
            Mapper.CreateMap<UserWithRelationshipsDto, User>();

            Mapper.CreateMap<User, UserWithRolesDto>();
            Mapper.CreateMap<UserWithRolesDto, User>();

            Mapper.CreateMap<Fx_Extend, Fx_ExtendDto>();

            Mapper.CreateMap<DataDictionaryEntity, DataDictionaryBaseDto>();
            Mapper.CreateMap<DataDictionaryBaseDto, DataDictionaryEntity>();

            Mapper.CreateMap<DataDictionaryEntity, DataDictionaryWithChildDto>();
            Mapper.CreateMap<DataDictionaryWithChildDto, DataDictionaryEntity>();

            Mapper.CreateMap<LogRequest, LogRequestDto>();
            Mapper.CreateMap<LogRequestDto, LogRequest>();
        }
    }
}

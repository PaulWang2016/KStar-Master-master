using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework;
using aZaaS.Framework.Organization.Facade;
using aZaaS.Framework.Organization.UserManagement;

namespace aZaaS.KStar.MgmtServices
{
    public class RoleService
    {
        private readonly RoleFacade roleFacade;

        public RoleService()
        {
            this.roleFacade = new RoleFacade();
        }
        public RoleBaseDto ReadRoleBase(Guid roleId)
        {
            return Mapper.Map<Role, RoleBaseDto>(this.roleFacade.ReadRole(roleId));
        }
        public IEnumerable<RoleBaseDto> GetAllRoles()
        {
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleBaseDto>>(this.roleFacade.GetAllRoles());
        }
        public IEnumerable<UserBaseDto> GetRoleUsers(Guid roleId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.roleFacade.GetRoleUsers(roleId));
        }

        public IEnumerable<UserBaseDto> GetRoleUsers(string rolename)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.roleFacade.GetRoleUsers(rolename));
        }

        public IEnumerable<RoleBaseDto> GetUserRoles(Guid userId)
        {
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleBaseDto>>(this.roleFacade.GetUserRoles(userId));
        }

        public IEnumerable<RoleBaseDto> GetUserRoles(string username)
        {
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleBaseDto>>(this.roleFacade.GetUserRoles(username));
        }

        public IEnumerable<UserWithFieldsDto> GetRoleUsersWithFields(Guid roleId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithFieldsDto>>(this.roleFacade.GetRoleUsers(roleId));
        }
        public bool RoleUserExists(Guid userId, Guid roleId)
        {
            return this.roleFacade.RoleUserExists(userId, roleId);
        }
    }
}

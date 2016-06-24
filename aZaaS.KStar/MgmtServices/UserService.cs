using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.Facade;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Extend;
using aZaaS.Framework.Organization.OrgChart;


namespace aZaaS.KStar.MgmtServices
{
    public class UserService
    {
        private readonly UserFacade userFacade;

        public UserService()
        {
            this.userFacade = new UserFacade();
        }

        public UserWithRelationshipsDto ReadUser(Guid userId)
        {
            return Mapper.Map<User, UserWithRelationshipsDto>(this.userFacade.ReadUser(userId));
        }
        public UserWithRelationshipsDto ReadUser(string userName)
        {
            return Mapper.Map<User, UserWithRelationshipsDto>(this.userFacade.ReadUser(userName));
            
        }
        public UserBaseDto ReadUserBase(Guid userId)
        {
            return Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(userId));
        }
        public UserBaseDto ReadUserBase(string userName)
        {
            return Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(userName));

        }
        public UserDto ReadUserInfo(String userName)
        {
            return Mapper.Map<User, UserDto>(this.userFacade.ReadUser(userName));
        }
        public UserWithFieldsDto ReadUserWithFields(Guid userId)
        {
            return Mapper.Map<User, UserWithFieldsDto>(this.userFacade.ReadUser(userId));
        }
        public UserWithPositionsDto ReadUserWithPositions(string userName)
        {
            return Mapper.Map<User, UserWithPositionsDto>(this.userFacade.ReadUser(userName));
        }
        public UserWithFieldsDto ReadUserWithFields(string userName)
        {
            return Mapper.Map<User, UserWithFieldsDto>(this.userFacade.ReadUser(userName));
        }
        public UserWithNodesDto ReadUserWithNodes(Guid userId)
        {
            return Mapper.Map<User, UserWithNodesDto>(this.userFacade.ReadUser(userId));
        }
        public UserWithNodesDto ReadUserWithNodes(string userName)
        {
            return Mapper.Map<User, UserWithNodesDto>(this.userFacade.ReadUser(userName));
        }
        public UserWithRolesDto ReadUserWithRoles(Guid userId)
        {
            return Mapper.Map<User, UserWithRolesDto>(this.userFacade.ReadUser(userId));
        }
        public UserWithOwnersDto ReadUserWithOwners(Guid userId)
        {
            return Mapper.Map<User, UserWithOwnersDto>(this.userFacade.ReadUser(userId));
        }
        public UserWithOwnersDto ReadUserWithOwners(string userName)
        {
            return Mapper.Map<User, UserWithOwnersDto>(this.userFacade.ReadUser(userName));
        }
        public UserWithPositionsDto ReadUserWithPositions(Guid userId)
        {
            return Mapper.Map<User, UserWithPositionsDto>(this.userFacade.ReadUser(userId));
        }


        public IEnumerable<UserBaseDto> GetAllUsers()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.userFacade.GetAllUsers());
        }

        public IEnumerable<UserWithFieldsDto> GetUsersWithFields()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithFieldsDto>>(this.userFacade.GetUsersWithFields(t=>true));
        }
        public IEnumerable<UserWithFieldsDto> GetUsersWithFields(int pageNumber, int pageSize)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithFieldsDto>>(this.userFacade.GetUsersWithFields((t=>true),pageNumber, pageSize));
        }

        public IEnumerable<UserWithFieldsDto> GetUsersWithFields(System.Linq.Expressions.Expression<Func<User, bool>> express)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithFieldsDto>>(this.userFacade.GetUsersWithFields(express));
        }
        public IEnumerable<UserWithFieldsDto> GetUsersWithFields(System.Linq.Expressions.Expression<Func<User, bool>> express, int pageNumber, int pageSize)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithFieldsDto>>(this.userFacade.GetUsersWithFields(express, pageNumber, pageSize));
        }

        public IEnumerable<UserWithRelationshipsDto> GetUsersWithRelationships()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetUsersWithRelationships(t => true));
        }

        public IEnumerable<UserWithRelationshipsDto> GetUsersWithRelationships(int pageNumber, int pageSize)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetUsersWithRelationships((t => true), pageNumber, pageSize));
        }

        public IEnumerable<UserWithRelationshipsDto> GetUsersWithRelationships(System.Linq.Expressions.Expression<Func<User, bool>> express, int pageNumber, int pageSize)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetUsersWithRelationships(express, pageNumber, pageSize));
        }

        public IEnumerable<UserBaseDto> GetUsersReportFrom(string userName)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.userFacade.GetUsersReportFrom(userName));
        }

        public IEnumerable<UserBaseDto> GetUsersReportFrom(Guid userId)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.userFacade.GetUsersReportFrom(userId));
        }

        public IEnumerable<OrgNodeWithRelationshipsDto> GetUserNodes(Guid userId)
        {
            User user = this.userFacade.ReadUser(userId);
            if (user != null)
            { 
                return  Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithRelationshipsDto>>(user.Nodes);
            }
            return null;
        }
        public IEnumerable<OrgNodeWithRelationshipsDto> GetUserNodes(string username)
        {
            User user = this.userFacade.ReadUser(username);
            if (user != null)
            {
                return Mapper.Map<IEnumerable<OrgNode>, IEnumerable<OrgNodeWithRelationshipsDto>>(user.Nodes);
            }
            return null;
        }

        public IEnumerable<UserBaseDto> GetNonReferenceUsers()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.userFacade.GetNonReferenceUsers()); 
        }

        public int QueryUserCount(System.Linq.Expressions.Expression<Func<User, bool>> express)
        {
            return this.userFacade.QueryCount(express);
        }
        public int GetUserCount()
        {
            return this.userFacade.Count();
        }
        public  bool IsOnPosition(UserWithPositionsDto user, string positionName)
        {
            user.NullThrowArgumentEx("user is not assigned.");
            positionName.NullOrEmptyThrowArgumentEx("position name is not assigned.");

            return user.Positions.Any(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));
        }

        public FieldBase[] GetUserExtendFieldsDefition()
        {
            return this.userFacade.GetExtendFiledsDefintion();
        }

        public void SaveUserExtendFieldsDefition(FieldBase[] fields)
        {
            this.userFacade.SaveExtendFiledsDefintion(fields);
        }
        public bool UserRoleExists(Guid roleId, Guid userId)
        {
            return this.userFacade.UserRoleExists(roleId, userId);
        }

        public int JobRank(UserWithFieldsDto user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //ExtensionFieldDto field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobRank"));

            //return field.Null() ? 0 : (int)(field.ValueNumber ?? 0);
            return 0;
        }

        public string JobTitle(UserWithFieldsDto user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //ExtensionFieldDto field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobTitle"));

            //return field.Null() ? string.Empty : field.ValueString;
            return "";
        }

        public string JobClass(UserWithFieldsDto user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //ExtensionFieldDto field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("JobClass"));

            //return field.Null() ? string.Empty : field.ValueString;
            return "";
        }

        public string Department(UserWithFieldsDto user)
        {
            //user.NullThrowArgumentEx("user is not assigned.");
            //user.ExFields.NullOrEmptyThrowArgumentEx("user does not contains any extension fields.");

            //ExtensionFieldDto field = user.ExFields.FirstOrDefault(f => f.PropertyName.Equals("Department"));

            //return field.Null() ? string.Empty : field.ValueString;
            return "";
        }

        public string GetUserMailAddress(string userName)
        {
            var userInfo = this.userFacade.ReadUser(userName);

            return userInfo.Email;
        }


        public List<string> GetUsersDisplayName(List<string> userids)
        {
            List<string> displayNames = new List<string>();
            foreach (var id in userids)
            {
                var user = Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(Guid.Parse(id)));
                if (user != null)
                {
                    displayNames.Add(user.FullName);
                }
            }
            return displayNames;
        }

        public List<string> GetUsersDisplayNameByUserName(List<string> userNames)
        {
            List<string> displayNames = new List<string>();
            foreach (var userName in userNames)
            {
                var user = Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(userName));
                if (user != null)
                {
                    displayNames.Add(user.FullName);
                }
            }
            return displayNames;
        }
        public List<UserBaseDto> GetUsersInfoByUserName(List<string> userNames)
        {
            List<UserBaseDto> userinfos = new List<UserBaseDto>();
            foreach (var userName in userNames)
            {
                var user = Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(userName));
                if (user != null)
                {
                    userinfos.Add(user);
                }
            }
            return userinfos;
        }


        //-- Search form 1st reviewer logic

        public UserBaseDto GetUserOwnerContainsJobClass(string userName, string jobClass)
        {
            var satisfiedOwnerList = new List<UserBaseDto>();

            userName.NullOrEmptyThrowArgumentEx("user name is not assigned.");
            jobClass.NullOrEmptyThrowArgumentEx("job class is not assigned.");

            RecursiveOwnersContainsClass(ref satisfiedOwnerList, userName, jobClass);

            return satisfiedOwnerList.FirstOrDefault();
        }

        private void RecursiveOwnersContainsClass(ref List<UserBaseDto> expectedOwnerList, string userName, string requiredJobClass)
        {
            var currentUserWithOwners = this.ReadUserWithOwners(userName);
            currentUserWithOwners.NullThrowArgumentEx("can not found the specified user info.");

            var currentUserOwners = currentUserWithOwners.ReportTo ?? new List<UserBaseDto>();

            foreach (UserBaseDto currentOwner in currentUserOwners)
            {
                var ownerWithFields = this.ReadUserWithFields(currentOwner.SysID);

                string currentOwnerJobClass = string.Format("{0},", this.JobClass(ownerWithFields));
                bool isActivedMan = ownerWithFields.Status.Equals("True", StringComparison.OrdinalIgnoreCase);

                if (requiredJobClass.Contains(currentOwnerJobClass) && isActivedMan)
                {
                    expectedOwnerList.Add(currentOwner);
                    break;//We would stop searching once we got the satisfied owner. 
                }
                else
                {
                    RecursiveOwnersContainsClass(ref expectedOwnerList, currentOwner.UserName, requiredJobClass);
                }
            }

        }




    }
}

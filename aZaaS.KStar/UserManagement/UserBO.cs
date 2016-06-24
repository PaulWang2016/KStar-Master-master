using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoMapper;
using aZaaS.Framework;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Organization.UserManagement;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.Facade;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Authentication;
using System.Transactions;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Facades;


namespace aZaaS.KStar
{
    /// <summary>
    /// User business object.
    /// </summary>
    public class UserBO : AbstractBO
    {
        private readonly UserFacade userFacade;
        private readonly IAuthentication _authManager;
        private readonly ISuperUserAdministration _authAdministration;

        private PositionBO positionBO;
        private OrgChartBO chartBO;

        public UserBO()
        {
            this.userFacade = new UserFacade();
            positionBO = new PositionBO();
            chartBO = new OrgChartBO();
            aZaaS.Framework.Configuration.Config.Initialize(new aZaaS.KStar.UserManagement.TenantDbConfig());
            this._authManager = new DefaultAuthentication(aZaaS.Framework.Configuration.Config.Default);
            this._authAdministration = new SuperUserAdministration(aZaaS.Framework.Configuration.Config.Default);
        }

        /// <summary>
        /// user login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>result</returns>
        public string Login(string userName, string password)
        {
            var result = this._authManager.Verify(userName, password);
            //if (result == VerifyResult.UserIsNotexist && this.UserNameExists(userName))
            //{
            //    this.InitUserPassword(userName, "123456");
            //    result = this._authManager.Verify(userName, password);
            //}
            return result.KeyName();
        }

        /// <summary>
        /// change password
        /// </summary>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns>result</returns>
        public string ChangePassword(string userName, string oldPwd, string newPwd)
        {
            var result = this._authManager.ChangePassword(userName, oldPwd, newPwd);
            return result.KeyName();
        }

        /// <summary>
        /// admin rest password
        /// </summary>
        /// <param name="oldPwd"></param>
        /// <param name="newPwd"></param>
        /// <returns>result</returns>
        public void RestPasswordByAdmin(string userName, string newPwd)
        {
            this._authAdministration.RestPassword(userName, newPwd);
        }

        public void InitUserPassword(string userName, string pwd)
        {
            this._authAdministration.Register(userName, pwd);
        }
        public void DeleteUserPassword(string userName)
        {
            this._authAdministration.Delete(userName);
        }

        ///<summary>
        ///Creates a new user.
        ///</summary>
        ///<param name="user">The new user instance</param>
        ///<returns>The new user id</returns>
        public Guid CreateUser(UserBaseDto userDTO)
        {
            userDTO.NullThrowArgumentEx("user dto is not assigned.");

            return this.userFacade.CreateUser(Mapper.Map<UserBaseDto, User>(userDTO));
        }

        public Guid CreateStaff(UserBaseDto userDTO, AuthenticationType AuthType, List<string> ReportToidlist, List<string> Positionidlist, List<string> Roleidlist, List<string> Departmentidlist, IList<UserExFieldDTO> ExFields)
        {
            Guid userID = new Guid();
            using (TransactionScope ts = new TransactionScope())
            {
                //添加员工
                userID = CreateUser(userDTO);
                if (userID != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    //添加Everyone角色
                    AppendRole(userID, Guid.Parse("22d229b7-e5ad-4b5c-8b89-199a2dc2cbd8"));                    
                }

                //如果为form验证登陆，添加用户时则同时更新form验证初始化密码
               
                if (ReportToidlist != null)
                {
                    foreach (var item in ReportToidlist)
                    {
                        AppendOwner(userID, Guid.Parse(item));
                    }
                }
                if (Positionidlist != null)
                {
                    foreach (var item in Positionidlist)
                    {
                        positionBO.AppendUser(Guid.Parse(item), userID);
                    }
                }
                if (Roleidlist != null)
                {
                    foreach (var item in Roleidlist)
                    {
                        AppendRole(userID, Guid.Parse(item));
                    }
                }
                if (Departmentidlist != null)
                {
                    foreach (var item in Departmentidlist)
                    {
                        chartBO.AppendUser(Guid.Parse(item), userID);
                    }
                }

                #region exfield
                if (ExFields != null && userID != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    foreach (var item in ExFields)
                    {
                        AppendExField(userID, item);
                    }
                }
                #endregion
                ts.Complete();
            }
            if (AuthType == AuthenticationType.Form)
            {
                InitUserPassword(userDTO.UserName, PortalEnvironment.FormPassWord);
            }

            return userID;
        }

        ///<summary>
        ///Checks whether the specified user name exists.
        ///</summary>
        ///<param name="userName">The specified user name</param>
        ///<returns>True or false</returns>
        public bool UserNameExists(string userName)
        {
            return this.userFacade.UserNameExists(userName);
        }

        ///<summary>
        ///Retrieves user according to the specified user id.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<returns>The matching user instance</returns>
        public UserBaseDto ReadUser(Guid userId)
        {
            return Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(userId));
        }

        ///<summary>
        ///Retrieves user according to the specified user name.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<returns>The matching user instance</returns>
        public UserBaseDto ReadUserBase(string userName)
        {
            return Mapper.Map<User, UserBaseDto>(this.userFacade.ReadUser(userName));
        }

        ///<summary>
        ///Retrieves user according to the specified user name.
        ///</summary>
        ///<param name="userName">The specified user name</param>
        ///<returns>The matching user instance</returns>
        public UserWithRelationshipsDto ReadUser(string userName)
        {
            return Mapper.Map<User, UserWithRelationshipsDto>(this.userFacade.ReadUser(userName));
        }

        ///<summary>
        ///Updates the specified user.
        ///</summary>
        ///<param name="user">The specified user instance</param>
        public void UpdateUser(UserBaseDto userDTO)
        {
            userDTO.NullThrowArgumentEx("user dto is not assigned.");

            this.userFacade.UpdateUser(Mapper.Map<UserBaseDto, User>(userDTO));
        }

        ///<summary>
        ///Deletes a specified user.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        public void DeleteUser(Guid userId)
        {
            this.userFacade.DeleteUser(userId);
        }

        ///<summary>
        ///Checks whether the specified field is already assigned to specified user.
        ///</summary>
        ///<param name="propertyName">The specified field name</param>
        ///<param name="userId">The specified user id</param>
        ///<returns>True or false</returns>
        public bool UserFieldExists(string propertyName, Guid userId)
        {
            return this.userFacade.UserFieldExists(propertyName, userId);
        }

        ///<summary>
        ///Checks whether the specified owner is already assigned to specified user.
        ///</summary>
        ///<param name="ownerId">The specified owner id</param>
        ///<param name="userId">The specified user id</param>
        ///<returns>True or false</returns>
        public bool UserOwnerExists(Guid ownerId, Guid userId)
        {
            return this.userFacade.UserOwnerExists(ownerId, userId);
        }

        ///<summary>
        ///Checks whether the specified role is already assigned to specified user.
        ///</summary>
        ///<param name="roleId">The specified role id</param>
        ///<param name="userId">The specified user id</param>
        ///<returns>True or false</returns>
        public bool UserRoleExists(Guid roleId, Guid userId)
        {
            return this.userFacade.UserRoleExists(roleId, userId);
        }

        ///<summary>
        ///Assigns a role to the specified user.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<param name="roleId">The specified role id</param>
        public void AppendRole(Guid userId, Guid roleId)
        {
            this.userFacade.AppendRole(userId, roleId);
        }

        ///<summary>
        ///Removes a role from the specified user. 
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<param name="roleId">The specified role id</param>
        public void RemoveRole(Guid userId, Guid roleId)
        {
            this.userFacade.RemoveRole(userId, roleId);
        }

        /////<summary>
        /////Assigns a field to the spcified user.
        /////</summary>
        /////<param name="userId">The specified user id</param>
        /////<param name="field">The specified field instance</param>
        //public void AppendField(Guid userId, UserExFieldDTO fieldDTO)
        //{
        //    fieldDTO.NullThrowArgumentEx("field dto is not assigned.");

        //    this.userFacade.AppendField(userId, Mapper.Map<UserExFieldDTO, UserExtend>(fieldDTO));
        //}

        ///<summary>
        ///Assigns a field to the specified user.
        ///</summary>
        ///<param name="nodeId">The specified user id</param>
        ///<param name="field">The specified field</param>
        public void AppendExField(Guid sysId, UserExFieldDTO fieldDTO)
        {
            fieldDTO.NullThrowArgumentEx("filed is dto is not assigned.");

            this.userFacade.AppendField(sysId, Mapper.Map<UserExFieldDTO, UserExtend>(fieldDTO));
        }

        ///<summary>
        ///Removes a field from the specified user.
        ///</summary>
        ///<param name="nodeId">The specified user id</param>
        ///<param name="fieldId">The specified field id</param>
        public void RemoveExField(Guid sysId, string name)
        {
            this.userFacade.RemoveField(sysId, name);
        }

        ///<summary>
        ///Updates the specified field.
        ///</summary>
        ///<param name="field">The specified field instance</param>
        public void UpdateExField(UserExFieldDTO fieldDTO)
        {
            fieldDTO.NullThrowArgumentEx("field dto is not assigned.");

            this.userFacade.UpdateField(Mapper.Map<UserExFieldDTO, UserExtend>(fieldDTO));
        }
        ///<summary>
        ///Assigns a owner to the specified user.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<param name="ownerId">The specified owner id</param>
        public void AppendOwner(Guid userId, Guid ownerId)
        {
            this.userFacade.AppendOwner(userId, ownerId);
        }

        ///<summary>
        ///Removes a owner from the specified user.
        ///</summary>
        ///<param name="userId">The specified user id</param>
        ///<param name="ownerId">The specified owner id</param>
        public void RemoveOwner(Guid userId, Guid ownerId)
        {
            this.userFacade.RemoveOwner(userId, ownerId);
        }

        /// <summary>
        /// Append a position from the specified user.
        /// </summary>
        /// <param name="userId">The specified user id</param>
        /// <param name="ownerId">The specified owner id</param>
        public void AppendPosition(Guid userId, Guid positionId)
        {
            this.userFacade.AppendPosition(userId, positionId);
        }

        /// <summary>
        /// Removes a position from the specified user.
        /// </summary>   
        /// <param name="userId">The specified user id</param>
        /// <param name="positionId">The specified owner id</param>
        public void RemovePosition(Guid userId, Guid positionId)
        {
            this.userFacade.RemovePosition(userId, positionId);
        }

        public int GetUserCount()
        {
            return this.userFacade.Count();
        }

        ///<summary>
        ///Retrieves all users.
        ///</summary>
        ///<returns>All users list</returns>
        public IEnumerable<UserWithRelationshipsDto> GetAllUsers()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetAllUsers());
        }

        ///<summary>
        ///Retrieves all users.
        ///</summary>
        ///<returns>All users list</returns>
        public IEnumerable<UserBaseDto> GetAllBaseUsers()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserBaseDto>>(this.userFacade.GetAllUsers());
        }

        ///<summary>
        ///Query users according to the specified query expression
        ///</summary>
        ///<param name="expression">The specified query expression</param>
        ///<returns>The matching user list</returns>
        public IEnumerable<UserWithRelationshipsDto> GetUsers(QueryExpression expression)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetUsers(expression));
        }

        ///<summary>
        ///Retrieves all users.
        ///</summary>
        ///<returns>All users list</returns>
        public IEnumerable<UserWithRelationshipsDto> GetAllUsers(int pageNumber, int pageSize)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetAllUsers(pageNumber, pageSize));
        }

        public List<string> GetUsersDisplayName(List<string> userNames)
        {
            List<string> displayNames = new List<string>();
            foreach (var userName in userNames)
            {
                var user = Mapper.Map<User, UserWithRelationshipsDto>(this.userFacade.ReadUser(userName));
                if (user != null)
                {
                    displayNames.Add(user.FullName);
                }
            }
            return displayNames;
        }

        ///<summary>
        ///Query users according to the specified query expression
        ///</summary>
        ///<param name="expression">The specified query expression</param>
        ///<returns>The matching user list</returns>
        public IEnumerable<UserWithRelationshipsDto> GetUsers(QueryExpression expression, int pageNumber, int pageSize)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserWithRelationshipsDto>>(this.userFacade.GetUsers(expression, pageNumber, pageSize));
        }

        public int QueryUserCount(System.Linq.Expressions.Expression<Func<User, bool>> express)
        {
            return this.userFacade.QueryCount(express);
        }

    }
}

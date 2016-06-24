using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IUserProvider
    {
        [OperationContract]
        string GetUserName(string entrypt);
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="tenantID"></param>
        /// <returns></returns>
        [OperationContract]
        List<UserInfo> GetAllUsers(string tenantID);
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="tenantID"></param>
        /// <returns></returns>
        [OperationContract]
        List<RoleInfo> GetAllRoles(string tenantID);
        /// <summary>
        /// 根据角色id获取单个角色下的所有用户
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        [OperationContract]
        List<UserInfo> GetRoleUsers(string tenantID, string roleID);
        /// <summary>
        /// 通过角色名称获取单个角色下的所有用户
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [OperationContract]
        List<UserInfo> GetRoleUsersByName(string tenantID,string roleName);
        /// <summary>
        /// 根据用户名获取单个用户所属的所有角色
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        List<RoleInfo> GetUserRoles(string tenantID,string userName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        string GetDefaultProfileID(string tenantID, string userName);
        /// <summary>
        /// 根据用户名获取用户Email
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [OperationContract]
        string GetUserMail(string tenantID, string userName);
        /// <summary>
        /// 根据用户名获取单个用户信息
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <param name="profileID"></param>
        /// <returns></returns>
        [OperationContract]
        UserInfo GetUserInfo(string tenantID, string userName, string profileID);
        /// <summary>
        /// 根据用户名获取用户上级信息
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <param name="profileID"></param>
        /// <returns></returns>
        [OperationContract]
        UserInfo GetUserManager(string tenantID, string userName, string profileID);
        [OperationContract]
        List<UserInfo> GetDepartmentManagers(string tenantID, string userName, string profileID);
        [OperationContract]
        List<UserInfo> GetDepartmentUsers(string tenantID, string deptID);
        [OperationContract]
        List<UserInfo> GetCompanyUsers(string tenantID,string compID);
        [OperationContract]
        List<UserInfo> GetPositionUsers(string tenantID, string postID);
        /*
        /// <summary>
        /// for Workflow authentication
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        LoginResult Authenticate(string tenantID, string userName, string pwd);
        */

        /// <summary>
        /// for Portal login
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [OperationContract]
        LoginResult Login(string userName, string pwd);
        
        [OperationContract]
        void InitUserService(string tenantID);
 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Autofac;
using Autofac.Integration.Wcf;
using aZaaS.KStar.MgmtServices;
using aZaaS.Framework.Authentication;
using aZaaS.Framework.Organization.Extensions;
using aZaaS.KStar.MgmtDtos;
using System.Web;
using aZaaS.KStar.Wcf.TenantDbService;

namespace aZaaS.KStar.Wcf
{    
    [ServiceErrorHandlerBehaviour]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class UserProvider : IUserProvider
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly IAuthentication _authManager;
        private readonly PositionService _positionService;
        private readonly OrgChartService _chartService;
        private readonly UserBO _userBO;
        

        public UserProvider()
        {
            _userService = new UserService();
            _roleService = new RoleService();
            _authManager = new DefaultAuthentication(aZaaS.Framework.Configuration.Config.Default);
            _positionService = new PositionService();
            _chartService = new OrgChartService();
            _userBO = new UserBO();
        }
        public void InitUserService(string tenantID)
        {
            TenantDatabaseServiceClient service = new TenantDatabaseServiceClient();
            
            var aZaaSKStarDB = service.GetConnectionString(tenantID, SystemCode.KStarWorkSpace);
            var KSTARServiceDB = service.GetConnectionString(tenantID, SystemCode.KStarService);
            var aZaaSFrameworkDB = service.GetConnectionString(tenantID, SystemCode.KStarFramework);

            if (aZaaSKStarDB.Length > 0 && KSTARServiceDB.Length > 0 && aZaaSFrameworkDB.Length > 0)
            {
                var connectionSets = new Dictionary<string, string>()
                {
                   { "aZaaSKStar",aZaaSKStarDB},
                   { "KSTARService",KSTARServiceDB},
                   { "aZaaSFramework",aZaaSFrameworkDB}
                };              
                System.Web.HttpContext.Current.Session["_TENANT_CONNSET_"] = connectionSets;               
            }            
        }

        public string GetUserName(string entrypt)
        {
            string _username = string.Empty;
            double min =5.0;
            string timeoutSpan = System.Configuration.ConfigurationManager.AppSettings["TimeoutSpan"];
            if (!string.IsNullOrEmpty(timeoutSpan))
            {
                double.TryParse(timeoutSpan, out min);
            }
            entrypt = System.Web.HttpUtility.UrlDecode(entrypt);
            string decryptstr = EncryptHelper.DecryptString(entrypt);
            string[] arr = decryptstr.Split('_');
            if (arr.Length > 1)
            {
                string _t = System.Text.RegularExpressions.Regex.Replace(arr[1], "\0", "");
                try
                {
                    DateTime entryptTime = DateTime.ParseExact(_t, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                    TimeSpan ts = DateTime.Now - entryptTime;

                    if (ts.TotalMinutes <= min)
                    {
                        _username = System.Text.RegularExpressions.Regex.Replace(arr[0], "\0", "");
                    }
                }
                catch (Exception ex) { }
            }
            return _username;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <param name="tenantID"></param>
        /// <returns></returns>
        public List<UserInfo> GetAllUsers(string tenantID)
        {
            List<UserInfo> userlist=new List<UserInfo>();

            IEnumerable<UserBaseDto> users=_userService.GetAllUsers();

            foreach (UserBaseDto user in users)
            {
                userlist.Add(new UserInfo()
                {
                    UserID=user.SysID.ToString(), 
                    UserName =user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID       
                });
            }
            return userlist;
        }
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="tenantID"></param>
        /// <returns></returns>
        public List<RoleInfo> GetAllRoles(string tenantID)
        {
            List<RoleInfo> rolelist = new List<RoleInfo>();

            IEnumerable<RoleBaseDto> roles = _roleService.GetAllRoles();

            foreach (RoleBaseDto role in roles)
            {
                rolelist.Add(new RoleInfo()
                {
                   RoleID=role.SysID.ToString(),
                   RoleName=role.Name,
                   TenantID = tenantID     
                });
            }
            return rolelist;
        }
        /// <summary>
        /// 根据角色id获取单个角色下的所有用户
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public List<UserInfo> GetRoleUsers(string tenantID, string roleID)
        {
            List<UserInfo> userlist = new List<UserInfo>();

            IEnumerable<UserBaseDto> users = _roleService.GetRoleUsers(new Guid(roleID));

            foreach (UserBaseDto user in users)
            {
                userlist.Add(new UserInfo()
                {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                });
            }
            return userlist;
        }
        /// <summary>
        /// 通过角色名称获取单个角色下的所有用户
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public List<UserInfo> GetRoleUsersByName(string tenantID, string roleName)
        {
            List<UserInfo> userlist = new List<UserInfo>();

            IEnumerable<UserBaseDto> users = _roleService.GetRoleUsers(roleName);

            foreach (UserBaseDto user in users)
            {
                userlist.Add(new UserInfo()
                {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                });
            }
            return userlist;
        }
        /// <summary>
        /// 根据用户名获取单个用户所属的所有角色
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<RoleInfo> GetUserRoles(string tenantID, string userName)
        {
            List<RoleInfo> rolelist = new List<RoleInfo>();

            IEnumerable<RoleBaseDto> roles = _roleService.GetUserRoles(userName);

            foreach (RoleBaseDto role in roles)
            {
                rolelist.Add(new RoleInfo()
                {
                    RoleID = role.SysID.ToString(),
                    RoleName = role.Name,
                    TenantID = tenantID  
                });
            }
            return rolelist;
        }

        public string GetDefaultProfileID(string tenantID, string userName)
        {
            return string.Empty;
        }
        /// <summary>
        /// 根据用户名获取用户Email
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserMail(string tenantID, string userName)
        {
            UserWithRelationshipsDto user = _userService.ReadUser(userName);
            if (user != null)
            {
                return user.Email;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 根据用户名获取单个用户信息
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <param name="profileID"></param>
        /// <returns></returns>
        public UserInfo GetUserInfo(string tenantID, string userName, string profileID)
        {
            UserWithRelationshipsDto user = _userService.ReadUser(userName);
            if (user != null)
            {
                return new UserInfo() {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                };
            }
            else
            {
                return new UserInfo();
            }
        }
        /// <summary>
        /// 根据用户名获取用户上级信息
        /// </summary>
        /// <param name="tenantID"></param>
        /// <param name="userName"></param>
        /// <param name="profileID"></param>
        /// <returns></returns>
        public UserInfo GetUserManager(string tenantID, string userName, string profileID)
        {
            List<UserInfo> userlist = new List<UserInfo>();
            List<UserBaseDto> userbaselist = new List<UserBaseDto>();
            UserWithRelationshipsDto userdto = _userService.ReadUser(userName);
            List<OrgNodeWithRelationshipsDto> usernodes = _userService.GetUserNodes(userName).ToList();
            List<PositionBaseDto> positions = userdto.Positions.ToList();

            foreach (PositionBaseDto position in positions)
            {
                var nodes = _positionService.GetPositionNodes(position.SysID);
                foreach (OrgNodeWithRelationshipsDto orgnode in nodes)
                {
                    userbaselist.AddRange(GetPreOrgNodeUser(orgnode));
                }
            }
            foreach (OrgNodeWithRelationshipsDto orgnode in usernodes)
            {
                userbaselist.AddRange(GetPreOrgNodeUser(orgnode));
            }

            foreach (UserBaseDto user in userbaselist)
            {
                userlist.Add(new UserInfo()
                {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                });
            }
            return userlist.FirstOrDefault();            
        }

        public List<UserInfo> GetDepartmentManagers(string tenantID, string userName, string profileID)
        {
            List<UserInfo> userlist = new List<UserInfo>();
            List<UserBaseDto> userbaselist = new List<UserBaseDto>();
            UserWithRelationshipsDto userdto = _userService.ReadUser(userName);
            List<OrgNodeWithRelationshipsDto> usernodes = _userService.GetUserNodes(userName).ToList();
            List<PositionBaseDto> positions = userdto.Positions.ToList();

            foreach (PositionBaseDto position in positions)
            {
                var nodes = _positionService.GetPositionNodes(position.SysID);
                foreach (OrgNodeWithRelationshipsDto orgnode in nodes)
                {
                    userbaselist.AddRange(GetPreOrgNodeUser(orgnode));
                }
            }
            foreach (OrgNodeWithRelationshipsDto orgnode in usernodes)
            {
                userbaselist.AddRange(GetPreOrgNodeUser(orgnode));
            }

            foreach (UserBaseDto user in userbaselist)
            {
                userlist.Add(new UserInfo()
                {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                });
            }
            return userlist;  
        }

        public List<UserInfo> GetDepartmentUsers(string tenantID, string deptID)
        {
            List<UserInfo> userlist = new List<UserInfo>();
            List<UserBaseDto> nodeUsers = new List<UserBaseDto>();
            var node = _chartService.ReadNode(Guid.Parse(deptID));
            if (node != null)
            {
                GetRecursiveUser(node.SysID, nodeUsers);
                foreach (UserBaseDto user in nodeUsers)
                {
                    userlist.Add(new UserInfo()
                    {
                        UserID = user.SysID.ToString(),
                        UserName = user.UserName,
                        DisplayName = user.FullName,
                        FirstName = user.FullName,
                        LastName = user.LastName,
                        Email = user.Email,
                        MobilePhone = user.Phone,
                        TenantID = tenantID  
                    });
                }
            }
            return userlist;            
        }

        public List<UserInfo> GetCompanyUsers(string tenantID, string compID)
        {
            List<UserInfo> userlist = new List<UserInfo>();
            List<UserBaseDto> nodeUsers = new List<UserBaseDto>();
            OrgChartWithRelationshipsDto chart = _chartService.GetChartWithNodes(Guid.Parse(compID));
            foreach (var childnode in chart.Nodes)
            {
                GetRecursiveUser(childnode.SysID, nodeUsers);
            }

            foreach (UserBaseDto user in nodeUsers)
            {
                userlist.Add(new UserInfo()
                {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                });
            }
            return userlist;
        }

        public List<UserInfo> GetPositionUsers(string tenantID, string postID)
        {
            List<UserInfo> userlist = new List<UserInfo>();

            IEnumerable<UserBaseDto> users = _positionService.GetPositionUsersBase(new Guid(postID));

            foreach (UserBaseDto user in users)
            {
                userlist.Add(new UserInfo()
                {
                    UserID = user.SysID.ToString(),
                    UserName = user.UserName,
                    DisplayName = user.FullName,
                    FirstName = user.FullName,
                    LastName = user.LastName,
                    Email = user.Email,
                    MobilePhone = user.Phone,
                    TenantID = tenantID  
                });
            }
            return userlist;
        }

        public LoginResult Login(string userName, string pwd)
        {
            LoginResult result = new LoginResult();

            VerifyResult s = _authManager.Verify(userName, pwd);
            if (s == VerifyResult.Successful)
            {
                UserWithRelationshipsDto user = _userService.ReadUser(userName);
                if (user != null)
                {
                    result.UserID = user.SysID.ToString();
                    result.TenantID = s.KeyName();
                }
            }
            else
            {
                result.ErrorMsg = s.KeyName();
            }
            return result;
        }

        #region  私有方法
        private void GetRecursiveUser(Guid id, List<UserBaseDto> users)
        {
            if (users == null)
            {
                return;
            }
            var node = _chartService.ReadNode(id);
            var nodeUsers = (node == null ? new List<UserBaseDto>() : (node.Users ?? new List<UserBaseDto>()));
            foreach (var user in nodeUsers)
            {
                users.Add(user);
            }
            if (node.ChildNodes.Count > 0)
            {
                foreach (var child in node.ChildNodes)
                {
                    GetRecursiveUser(child.SysID, users);
                }
            }
        }

        private List<UserBaseDto> GetPreOrgNodeUser(OrgNodeWithRelationshipsDto node)
        {
            List<UserBaseDto> userlist = new List<UserBaseDto>();
            if (node.Parent != null)
            {
                var orgnode = _chartService.ReadNode(node.Parent.SysID);
                if (orgnode != null)
                {
                    userlist = orgnode.Users.ToList();
                }
            }
            return userlist;          
        }

        //完善用户其他信息
        private UserInfo GetExtendUserInfo(UserInfo user)
        {
            if (user != null)
            { 
                //更新password
                user.Password = "";
            }
            return user;
        }
        #endregion

    }
}

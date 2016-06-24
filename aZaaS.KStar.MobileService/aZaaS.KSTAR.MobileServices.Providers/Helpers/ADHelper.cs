using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.DirectoryServices;

namespace aZaaS.KSTAR.MobileServices.Helpers
{
    /// 
    /// 活动目录辅助类。封装一系列活动目录操作相关的方法。
    /// 
    public sealed class ADHelper
    {
        /// 
        /// 域名,比如：Denallix
        /// 
        public static string DomainName = ConfigurationManager.AppSettings["DefaultDomain"];

        /// 
        /// LDAP绑定路径,比如：LDAP://DENALLIX.COM
        /// 
        public static string ADPath = ConfigurationManager.AppSettings["ADConnString"];
        /// 
        /// 登录帐号,比如：TestUser
        /// 
        public static string ADUser = ConfigurationManager.AppSettings["DefaultAdminUser"];
        /// 
        /// 登录密码,比如：123456
        /// 
        public static string ADPassword = ConfigurationManager.AppSettings["AdminPassword"];
        /// 
        /// 扮演类实例
        /// 
        private static IdentityImpersonation impersonate = new IdentityImpersonation(ADUser, ADPassword, DomainName);

        /// 
        /// 用户登录验证结果
        /// 
        public enum LoginResult
        {
            /// 
            /// 正常登录
            /// 
            LOGIN_USER_OK = 0,
            /// 
            /// 用户不存在
            /// 
            LOGIN_USER_DOESNT_EXIST,
            /// 
            /// 用户帐号被禁用
            /// 
            LOGIN_USER_ACCOUNT_INACTIVE,
            /// 
            /// 用户密码不正确
            /// 
            LOGIN_USER_PASSWORD_INCORRECT
        }

        /// 
        /// 用户属性定义标志
        /// 
        public enum ADS_USER_FLAG_ENUM
        {
            /// 
            /// 登录脚本标志。如果通过 ADSI LDAP 进行读或写操作时，该标志失效。如果通过 ADSI WINNT，该标志为只读。
            /// 
            ADS_UF_SCRIPT = 0X0001,
            /// 
            /// 用户帐号禁用标志
            /// 
            ADS_UF_ACCOUNTDISABLE = 0X0002,
            /// 
            /// 主文件夹标志
            /// 
            ADS_UF_HOMEDIR_REQUIRED = 0X0008,
            /// 
            /// 过期标志
            /// 
            ADS_UF_LOCKOUT = 0X0010,
            /// 
            /// 用户密码不是必须的
            /// 
            ADS_UF_PASSWD_NOTREQD = 0X0020,
            /// 
            /// 密码不能更改标志
            /// 
            ADS_UF_PASSWD_CANT_CHANGE = 0X0040,
            /// 
            /// 使用可逆的加密保存密码
            /// 
            ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
            /// 
            /// 本地帐号标志
            /// 
            ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0X0100,
            /// 
            /// 普通用户的默认帐号类型
            /// 
            ADS_UF_NORMAL_ACCOUNT = 0X0200,
            /// 
            /// 跨域的信任帐号标志
            /// 
            ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0X0800,
            /// 
            /// 工作站信任帐号标志
            /// 
            ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
            /// 
            /// 服务器信任帐号标志
            /// 
            ADS_UF_SERVER_TRUST_ACCOUNT = 0X2000,
            /// 
            /// 密码永不过期标志
            /// 
            ADS_UF_DONT_EXPIRE_PASSWD = 0X10000,
            /// 
            /// MNS 帐号标志
            /// 
            ADS_UF_MNS_LOGON_ACCOUNT = 0X20000,
            /// 
            /// 交互式登录必须使用智能卡
            /// 
            ADS_UF_SMARTCARD_REQUIRED = 0X40000,
            /// 
            /// 当设置该标志时，服务帐号（用户或计算机帐号）将通过 Kerberos 委托信任
            /// 
            ADS_UF_TRUSTED_FOR_DELEGATION = 0X80000,
            /// 
            /// 当设置该标志时，即使服务帐号是通过 Kerberos 委托信任的，敏感帐号不能被委托
            /// 
            ADS_UF_NOT_DELEGATED = 0X100000,
            /// 
            /// 此帐号需要 DES 加密类型
            /// 
            ADS_UF_USE_DES_KEY_ONLY = 0X200000,
            /// 
            /// 不要进行 Kerberos 预身份验证
            /// 
            ADS_UF_DONT_REQUIRE_PREAUTH = 0X4000000,
            /// 
            /// 用户密码过期标志
            /// 
            ADS_UF_PASSWORD_EXPIRED = 0X800000,
            /// 
            /// 用户帐号可委托标志
            /// 
            ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0X1000000
        }

        public ADHelper()
        {
            //
        }

        #region GetDirectoryObject

        /// 
        /// 获得DirectoryEntry对象实例,以管理员登陆AD
        /// 
        /// 
        public static DirectoryEntry GetDirectoryObject()
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath, ADUser, ADPassword, AuthenticationTypes.Secure);
            return entry;
        }

        /// 
        /// 根据指定用户名和密码获得相应DirectoryEntry实体
        /// 
        public static DirectoryEntry GetDirectoryObject(string userName, string password)
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath, userName, password, AuthenticationTypes.None);
            return entry;
        }

        /// 
        /// i.e. /CN=Users,DC=creditsights, DC=cyberelves, DC=Com
        /// 
        public static DirectoryEntry GetDirectoryObject(string domainReference)
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath + domainReference, ADUser, ADPassword, AuthenticationTypes.Secure);
            return entry;
        }

        /// 
        /// 获得以UserName,Password创建的DirectoryEntry
        /// 
        public static DirectoryEntry GetDirectoryObject(string domainReference, string userName, string password)
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath + domainReference, userName, password, AuthenticationTypes.Secure);
            return entry;
        }

        #endregion

        #region GetDirectoryEntry

        /// 
        /// 根据用户公共名称取得用户的 对象
        /// 
        /// 用户公共名称
        /// 如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntry(string commonName)
        {
            DirectoryEntry de = GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// 
        /// 根据用户公共名称和密码取得用户的 对象。
        /// 
        /// 用户公共名称
        /// 用户密码
        /// 如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntry(string sAMAccountName, string password, string commonName)
        {
            DirectoryEntry de = GetDirectoryObject(DomainName + "\\" + sAMAccountName, password);
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// 
        /// 根据用户帐号称取得用户的 对象
        /// 
        /// 用户帐号名
        /// 如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntryByAccount(string sAMAccountName)
        {
            DirectoryEntry de = GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + sAMAccountName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// 
        /// 根据用户帐号和密码取得用户的 对象
        /// 
        /// 用户帐号名
        /// 用户密码
        /// 如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntryByAccount(string sAMAccountName, string password)
        {
            DirectoryEntry de = GetDirectoryEntryByAccount(sAMAccountName);
            if (de != null)
            {
                string commonName = de.Properties["cn"][0].ToString();

                var entry = GetDirectoryEntry(sAMAccountName, password, commonName);
                if (entry != null)
                    return entry;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        /// 
        /// 根据组名取得用户组的 对象
        /// 
        /// 组名
        /// 
        public static DirectoryEntry GetDirectoryEntryOfGroup(string groupName)
        {
            DirectoryEntry de = GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(objectClass=group)(cn=" + groupName + "))";
            deSearch.SearchScope = SearchScope.Subtree;

            try
            {
                SearchResult result = deSearch.FindOne();
                de = new DirectoryEntry(result.Path);
                return de;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Get and Set Property

        /// 
        /// 获得指定 指定属性名对应的值
        /// 
        /// 
        /// 属性名称
        /// 属性值
        public static object GetProperty(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0];
            }
            else
            {
                return "";
            }
        }

        /// 
        /// 获得指定搜索结果 中指定属性名对应的值
        /// 
        /// 
        /// 属性名称
        /// 属性值
        public static object GetProperty(SearchResult searchResult, string propertyName)
        {
            if (searchResult.Properties.Contains(propertyName))
            {
                return searchResult.Properties[propertyName][0];
            }
            else
            {
                return "";
            }
        }

        /// 
        /// 设置指定 的属性值
        /// 
        /// 
        /// 属性名称
        /// 属性值
        public void SetProperty(DirectoryEntry entry, string propertyName, string propertyValue)
        {
            if (entry.Properties.Contains(propertyName))
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    object o = entry.Properties[propertyName].Value;
                    entry.Properties[propertyName].Remove(o);
                }
                else
                {
                    entry.Properties[propertyName][0] = propertyValue;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(propertyValue))
                {
                    return;
                }
                entry.Properties[propertyName].Add(propertyValue);
            }

        }


        #endregion


        #region Management

        /// 
        /// 创建新的用户
        /// 
        /// DN 位置。例如：OU=共享平台 或 CN=Users
        /// 公共名称
        /// 帐号
        /// 密码
        /// 
        public static DirectoryEntry CreateNewUser(string ldapDN, string commonName, string sAMAccountName, string password)
        {
            DirectoryEntry entry = GetDirectoryObject();
            DirectoryEntry subEntry = entry.Children.Find(ldapDN);
            DirectoryEntry deUser = subEntry.Children.Add("CN=" + commonName, "user");
            deUser.Properties["sAMAccountName"].Value = sAMAccountName;
            deUser.CommitChanges();
            ADHelper.SetPassword(commonName, password);
            ADHelper.EnableUser(commonName);
            deUser.Close();
            return deUser;
        }

        /// 
        /// 创建新的用户。默认创建在 Users 单元下。
        /// 
        /// 公共名称
        /// 帐号
        /// 密码
        /// 
        public static DirectoryEntry CreateNewUser(string commonName, string sAMAccountName, string password)
        {
            return CreateNewUser("CN=Users", commonName, sAMAccountName, password);
        }

        /// 
        /// 判断指定公共名称的用户是否存在
        /// 
        /// 用户公共名称
        /// 如果存在，返回 true；否则返回 false
        public static bool IsUserExists(string commonName)
        {
            DirectoryEntry de = GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName + "))";		// LDAP 查询串
            SearchResultCollection results = deSearch.FindAll();

            if (results.Count == 0)
                return false;
            else
                return true;
        }

        /// 
        /// 判断用户帐号是否激活
        /// 
        /// 用户帐号属性控制器
        /// 如果用户帐号已经激活，返回 true；否则返回 false
        public static bool IsAccountActive(int userAccountControl)
        {
            int userAccountControl_Disabled = Convert.ToInt32(ADS_USER_FLAG_ENUM.ADS_UF_ACCOUNTDISABLE);
            int flagExists = userAccountControl & userAccountControl_Disabled;

            if (flagExists > 0)
                return false;
            else
                return true;
        }

        /// 
        /// 判断用户与密码是否足够以满足身份验证进而登录
        /// 
        /// 用户公共名称
        /// 密码
        /// 如能可正常登录，则返回 true；否则返回 false
        public static LoginResult Login(string commonName, string password)
        {
            DirectoryEntry de = GetDirectoryEntry(commonName);

            if (de != null)
            {
                // 必须在判断用户密码正确前，对帐号激活属性进行判断；否则将出现异常。
                //int userAccountControl = Convert.ToInt32(de.Properties["userAccountControl"][0]);
                string sAMAccountName = de.Properties["sAMAccountName"].ToString();
                de.Close();

                //if (!IsAccountActive(userAccountControl))
                //    return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;

                if (GetDirectoryEntry(sAMAccountName, password, commonName) != null)
                    return LoginResult.LOGIN_USER_OK;
                else
                    return LoginResult.LOGIN_USER_PASSWORD_INCORRECT;
            }
            else
            {
                return LoginResult.LOGIN_USER_DOESNT_EXIST;
            }
        }

        /// 
        /// 判断用户帐号与密码是否足够以满足身份验证进而登录
        /// 
        /// 用户帐号(不包含域名)
        /// 密码
        /// 如能可正常登录，则返回 true；否则返回 false
        public static LoginResult LoginByAccount(string sAMAccountName, string password)
        {
            if (sAMAccountName.IndexOf('\\') > -1)
            {
                sAMAccountName = sAMAccountName.Split('\\')[1].Trim('\\');
            }
            DirectoryEntry de = GetDirectoryEntryByAccount(sAMAccountName);

            if (de != null)
            {
                // 必须在判断用户密码正确前，对帐号激活属性进行判断；否则将出现异常。
                //int userAccountControl = Convert.ToInt32(de.Properties["userAccountControl"][0]);
                de.Close();

                //if (!IsAccountActive(userAccountControl))
                //    return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
                return LoginResult.LOGIN_USER_OK;
                //if (GetDirectoryEntryByAccount(sAMAccountName, password) != null)
                //    return LoginResult.LOGIN_USER_OK;
                //else
                //    return LoginResult.LOGIN_USER_PASSWORD_INCORRECT;
            }
            else
            {
                return LoginResult.LOGIN_USER_DOESNT_EXIST;
            }
        }

        /// 
        /// 设置用户密码，管理员可以通过它来修改指定用户的密码。
        /// 
        /// 用户公共名称
        /// 用户新密码
        public static void SetPassword(string commonName, string newPassword)
        {
            DirectoryEntry de = GetDirectoryEntry(commonName);

            // 模拟超级管理员，以达到有权限修改用户密码
            impersonate.BeginImpersonate();
            de.Invoke("SetPassword", new object[] { newPassword });
            impersonate.StopImpersonate();

            de.Close();
        }

        /// 
        /// 设置帐号密码，管理员可以通过它来修改指定帐号的密码。
        /// 
        /// 用户帐号
        /// 用户新密码
        public static void SetPasswordByAccount(string sAMAccountName, string newPassword)
        {
            DirectoryEntry de = GetDirectoryEntryByAccount(sAMAccountName);

            // 模拟超级管理员，以达到有权限修改用户密码
            IdentityImpersonation impersonate = new IdentityImpersonation(ADUser, ADPassword, DomainName);
            impersonate.BeginImpersonate();
            de.Invoke("SetPassword", new object[] { newPassword });
            impersonate.StopImpersonate();

            de.Close();
        }

        /// 
        /// 修改用户密码
        /// 
        /// 用户公共名称
        /// 旧密码
        /// 新密码
        public static void ChangeUserPassword(string commonName, string oldPassword, string newPassword)
        {
            // to-do: 需要解决密码策略问题
            DirectoryEntry oUser = GetDirectoryEntry(commonName);
            oUser.Invoke("ChangePassword", new Object[] { oldPassword, newPassword });
            oUser.Close();
        }

        /// 
        /// 启用指定公共名称的用户
        /// 
        /// 用户公共名称
        public static void EnableUser(string commonName)
        {
            EnableUser(GetDirectoryEntry(commonName));
        }

        /// 
        /// 启用指定 的用户
        /// 
        /// 
        public static void EnableUser(DirectoryEntry de)
        {
            impersonate.BeginImpersonate();
            de.Properties["userAccountControl"][0] = ADHelper.ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT | ADHelper.ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD;
            de.CommitChanges();
            impersonate.StopImpersonate();
            de.Close();
        }

        /// 
        /// 禁用指定公共名称的用户
        /// 
        /// 用户公共名称
        public static void DisableUser(string commonName)
        {
            DisableUser(GetDirectoryEntry(commonName));
        }

        /// 
        /// 禁用指定 的用户
        /// 
        /// 
        public static void DisableUser(DirectoryEntry de)
        {
            impersonate.BeginImpersonate();
            de.Properties["userAccountControl"][0] = ADHelper.ADS_USER_FLAG_ENUM.ADS_UF_NORMAL_ACCOUNT | ADHelper.ADS_USER_FLAG_ENUM.ADS_UF_DONT_EXPIRE_PASSWD | ADHelper.ADS_USER_FLAG_ENUM.ADS_UF_ACCOUNTDISABLE;
            de.CommitChanges();
            impersonate.StopImpersonate();
            de.Close();
        }

        /// 
        /// 将指定的用户添加到指定的组中。默认为 Users 下的组和用户。
        /// 
        /// 用户公共名称
        /// 组名
        public static void AddUserToGroup(string userCommonName, string groupName)
        {
            DirectoryEntry oGroup = GetDirectoryEntryOfGroup(groupName);
            DirectoryEntry oUser = GetDirectoryEntry(userCommonName);

            impersonate.BeginImpersonate();
            oGroup.Properties["member"].Add(oUser.Properties["distinguishedName"].Value);
            oGroup.CommitChanges();
            impersonate.StopImpersonate();

            oGroup.Close();
            oUser.Close();
        }

        /// 
        /// 将用户从指定组中移除。默认为 Users 下的组和用户。
        /// 
        /// 用户公共名称
        /// 组名
        public static void RemoveUserFromGroup(string userCommonName, string groupName)
        {
            DirectoryEntry oGroup = GetDirectoryEntryOfGroup(groupName);
            DirectoryEntry oUser = GetDirectoryEntry(userCommonName);

            impersonate.BeginImpersonate();
            oGroup.Properties["member"].Remove(oUser.Properties["distinguishedName"].Value);
            oGroup.CommitChanges();
            impersonate.StopImpersonate();

            oGroup.Close();
            oUser.Close();
        }

        #endregion Management

        #region Client

        private static DirectoryEntry InitEntry(string condition)
        {
            DirectoryEntry entry = null;
            DirectorySearcher searcher = new DirectorySearcher(condition);

            if (ADHelper.ADUser != "")
            {
                searcher.SearchRoot.Username = ADHelper.ADUser;
                searcher.SearchRoot.Password = ADHelper.ADPassword;
            }
            SearchResult result = searcher.FindOne();
            if (result == null)
            {
                return null;
            }
            if (ADHelper.ADUser != "")
            {
                entry = new DirectoryEntry(result.Path, ADHelper.ADUser, ADHelper.ADPassword);
            }
            else
            {
                entry = new DirectoryEntry(result.Path);
            }
            entry.RefreshCache();
            return entry;
        }


        public static string ExtractUserName(string userName)
        {
            int index = userName.IndexOf(@"\");
            string str = userName;
            if (index != -1)
            {
                str = userName.Substring(index + 1, (userName.Length - index) - 1);
            }
            else
            {
                index = userName.IndexOf("@");
                if (index != -1)
                {
                    str = userName.Substring(index + 1, (userName.Length - index) - 1);
                }
            }
            return str;
        }

        public static string[] GetMembersOfGroup(string groupName)
        {
            DirectoryEntry entry = InitEntry("(&(objectClass=group)(cn=" + groupName + "))");
            DirectoryEntry entry2 = new DirectoryEntry();
            string[] strArray = new string[entry.Properties["member"].Count];
            for (int i = 0; i < strArray.Length; i++)
            {
                entry2 = new DirectoryEntry("LDAP://" + entry.Properties["member"][i]);
                strArray[i] = entry2.Properties["sAMAccountName"].Value.ToString();
            }
            return strArray;
        }

        public static string GetDisplayName(string userName)
        {
            DirectoryEntry entry = InitEntry("(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + ExtractUserName(userName) + "))");
            if (entry == null)
            {
                throw new Exception("Could not found the badge number: " + userName);
            }
            string str = "";
            if (entry.Properties.Contains("displayName"))
            {
                str = str + entry.Properties["displayName"].Value.ToString();
            }
            return str;
        }

        public static string GetUserMail(string userName)
        {
            DirectoryEntry entry = InitEntry("(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + ExtractUserName(userName) + "))");
            if (entry == null)
            {
                throw new Exception("Could not found the badge number: " + userName);
            }
            object obj = ADHelper.GetProperty(entry, "mail");
            return (obj == null) ? "" : obj.ToString();
        }

        public static string GetUserEnglishName(string userName)
        {
            DirectoryEntry entry = InitEntry("(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + ExtractUserName(userName) + "))");
            if (entry == null)
            {
                throw new Exception("Could not found the badge number: " + userName);
            }
            string str = "";
            if (entry.Properties.Contains("givenName"))
            {
                str = str + entry.Properties["givenName"].Value.ToString();
            }
            if (entry.Properties.Contains("sn"))
            {
                str = str + " " + entry.Properties["sn"].Value.ToString();
            }
            return str;
        }

        public static string[] GetGroupsOfUser(string userName)
        {
            DirectoryEntry entry = InitEntry("(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + ExtractUserName(userName) + "))");
            if (entry == null)
            {
                return null;
            }
            DirectoryEntry entry2 = new DirectoryEntry();
            string[] strArray = new string[entry.Properties["memberOf"].Count];
            for (int i = 0; i < strArray.Length; i++)
            {
                entry2 = new DirectoryEntry("LDAP://" + entry.Properties["memberOf"][i]);
                strArray[i] = entry2.Properties["name"].Value.ToString();
            }
            return strArray;
        }

        public static string GetUserAccountByEmail(string email)
        {
            DirectoryEntry entry = InitEntry("(&(&(objectCategory=person)(objectClass=user))(mail=" + ExtractUserName(email) + "))");
            if (entry == null)
            {
                throw new Exception("Could not find the email:" + email);
            }
            return entry.Properties["sAMAccountName"].Value.ToString();
        }

        public static string GetUserAccountByEmailORAccount(string emailOrAccount)
        {
            if (IsAccount(emailOrAccount))
            {
                return emailOrAccount;
            }
            return GetUserAccountByEmail(emailOrAccount);
        }

        public static string GetUserNameByEmail(string email)
        {
            DirectoryEntry entry = InitEntry("(&(&(objectCategory=person)(objectClass=user))(mail=" + ExtractUserName(email) + "))");
            if (entry == null)
            {
                throw new Exception("Could not found the email: " + email);
            }
            return DomainName + "\\" + entry.Properties["sAMAccountName"].Value.ToString();
        }

        public static bool IsAccount(string rawAccount)
        {
            if (InitEntry("(&(&(objectCategory=person)(objectClass=user))(sAMAccountName=" + ExtractUserName(rawAccount) + "))") == null)
            {
                return false;
            }
            return true;
        }

        #endregion Client

    }


    /// 
    /// 用户模拟角色类。实现在程序段内进行用户角色模拟。
    /// 
    public class IdentityImpersonation
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        // 要模拟的用户的用户名、密码、域(机器名)
        private String _sImperUsername;
        private String _sImperPassword;
        private String _sImperDomain;
        // 记录模拟上下文
        private WindowsImpersonationContext _imperContext;
        private IntPtr _adminToken;
        private IntPtr _dupeToken;
        // 是否已停止模拟
        private Boolean _bClosed;

        private WindowsIdentity fakeId;
        public WindowsIdentity Identity
        {
            get
            {
                return this.fakeId;
            }
        }

        /// 
        /// 构造函数
        /// 
        /// 所要模拟的用户的用户名
        /// 所要模拟的用户的密码
        /// 所要模拟的用户所在的域
        public IdentityImpersonation(String impersonationUsername, String impersonationPassword, String impersonationDomain)
        {
            _sImperUsername = impersonationUsername;
            _sImperPassword = impersonationPassword;
            _sImperDomain = impersonationDomain;

            _adminToken = IntPtr.Zero;
            _dupeToken = IntPtr.Zero;
            _bClosed = true;
        }

        /// 
        /// 析构函数
        /// 
        ~IdentityImpersonation()
        {
            if (!_bClosed)
            {
                StopImpersonate();
            }
        }

        /// 
        /// 开始身份角色模拟。
        /// 
        /// 
        public Boolean BeginImpersonate()
        {
            Boolean bLogined = LogonUser(_sImperUsername, _sImperDomain, _sImperPassword, 2, 0, ref _adminToken);

            if (!bLogined)
            {
                return false;
            }

            Boolean bDuped = DuplicateToken(_adminToken, 2, ref _dupeToken);

            if (!bDuped)
            {
                return false;
            }

            fakeId = new WindowsIdentity(_dupeToken);
            _imperContext = fakeId.Impersonate();
            _bClosed = false;

            return true;
        }

        /// 
        /// 开始身份角色模拟。
        /// 
        /// 
        public Boolean BeginImpersonate(string type)
        {
            Boolean bLogined = LogonUser(_sImperUsername, _sImperDomain, _sImperPassword, 2, 0, ref _adminToken);

            if (!bLogined)
            {
                return false;
            }

            //Boolean bDuped = DuplicateToken(_adminToken, 2, ref _dupeToken);

            //if (!bDuped)
            //{
            //    return false;
            //}


            fakeId = new WindowsIdentity(_adminToken, type, WindowsAccountType.Normal, true);
            _imperContext = fakeId.Impersonate();

            _bClosed = false;

            return true;
        }

        /// 
        /// 停止身分角色模拟。
        /// 
        public void StopImpersonate()
        {
            _imperContext.Undo();
            CloseHandle(_dupeToken);
            CloseHandle(_adminToken);
            _bClosed = true;
        }
    }

}



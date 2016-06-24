using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using aZaaS.Framework.Authentication;
using System.Data;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Configuration;

using System.Web;
using System.Web.Security;


namespace aZaaS.KStar.UserManagement
{
    public class NeowayUserAuthentication : IAuthentication
    {

        private readonly UserBO userService;

        public NeowayUserAuthentication()
        {
            userService = new UserBO();
        }

        public ChangePasswordResult ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
            //ADHelper.ChangeUserPassword(username, oldPassword, newPassword);
        }

        public VerifyResult Verify(string nusername, string password)
        {

            LdapAuthentication ldap = new LdapAuthentication();

            var IsExist = ldap.IsAuthenticated("neowaydc", nusername, password);
            VerifyResult result;
            if (!IsExist)
            {
                result = VerifyResult.PassworkIncorrect;
            }
            result = VerifyResult.Successful;
            return result;

        }
        public int  ChangeUserPassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                string DomainName = string.Empty;
                string ADDomain = ConfigurationManager.AppSettings["WindowDomain"].ToString();
                string _ADPath = ConfigurationManager.AppSettings["LDAPUrl"].ToString();
                DomainName = _ADPath.Substring(_ADPath.LastIndexOf("/") + 1);
                LdapAuthentication la = new LdapAuthentication();
               
                
                var isexist = la.GetUserByAccount(username);
                if(isexist.Rows.Count==0)
                {
                    return -1;
                }

                if (!la.IsAuthenticated(ADDomain, username, oldPassword))
                {
                    return 0;
                }
                if (la.ChangePassword(ADDomain, username, oldPassword, newPassword))
                {
                    return 1;
                }
                return -2;
            }
            catch (Exception ex)
            {
                
                return -2;
            }
            //return 0;
        }
       
        public bool IsPass(string userAccount, string password)
        {

            string ADDomain = ConfigurationManager.AppSettings["WindowDomain"].ToString();
            string Enviroment = ConfigurationManager.AppSettings["Enviroment"].ToString();
            if (Enviroment == "Development")
            {
                var user = userService.ReadUser(ADDomain+"\\" + userAccount);
                if (user == null)
                {
                    return false;
                }
                return true;
            }
            else
            {

                #region 增加正式开启密码验证
                string _error = string.Empty;
                string DomainName = string.Empty;

                //获得当前域中的路径
                string _ADPath = ConfigurationManager.AppSettings["LDAPUrl"].ToString();
                DomainName = _ADPath.Substring(_ADPath.LastIndexOf("/") + 1);
                LdapAuthentication la = new LdapAuthentication();
                var returnValue = la.IsAuthenticated(ADDomain, userAccount, password);
                return returnValue;
                #endregion
            }
            #region 测试使用已注释
            //string domainAndUsername;
            //bool hasDomain = false;
            //if (userAccount.StartsWith(DomainName, StringComparison.CurrentCultureIgnoreCase))
            //{
            //    hasDomain = true;
            //}
            //if (hasDomain)
            //{
            //    domainAndUsername = userAccount;
            //}
            //else
            //{
            //    domainAndUsername = ADDomain + @"\" + userAccount;
            //}
            //DirectoryEntry entry = new DirectoryEntry(_ADPath, domainAndUsername, password, AuthenticationTypes.Secure);
            //DirectorySearcher search = new DirectorySearcher(entry);

            //if (hasDomain)
            //{
            //    userAccount = userAccount.Substring(DomainName.Length + 1);
            //}
            //search.Filter = "(sAMAccountName=" + userAccount + ")";
            //search.PropertiesToLoad.Add("displayName");
            //SearchResult adUser = null;
            //try
            //{
            //    adUser = search.FindOne();
            //    if (adUser == null)
            //    {
            //        _error = "域认证失败";
            //        isExist = false;
            //    }
            //    else
            //    {
            //        isExist = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _error = ex.Message;
            //    adUser = null;
            //}
            //finally
            //{
            //    entry.Close();
            //    entry = null;
            //    search.Dispose();
            //    search = null;
            //}
            //return isExist;
            #endregion
        }

    }
    /// </summary>
    public class LdapAuthentication
    {
        #region   模擬帳號功能用
        protected const int LOGON32_LOGON_INTERACTIVE = 2;
        protected const int LOGON32_PROVIDER_DEFAULT = 0;
        protected static WindowsImpersonationContext impersonationContext;
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        protected static extern int LogonUser(String lpszUserName, String lpszDomain,
        String lpszPassword, int dwLogonType, int dwLogonProvider,
        ref IntPtr phToken);
        [DllImport("advapi32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto,
        SetLastError = true)]
        protected extern static int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);
        #endregion


        /// <summary>
        /// 连接到域中的组。   例如“LDAP://CN=<组名>, CN =<用户>, DC=<域控制器 1>, DC=<域控制器 2>,...”。 
        ///连接到域中的用户。  例如“LDAP://CN=<完整用户名>, CN=<用户>, DC=<域控制器 1>, DC=<域控制器 2>,...”。 
        ///连接到域中的计算机。例如“LDAP://CN=<计算机名>, CN=<计算机>, DC=<域控制器 1>, DC=<域控制器 2>,...”。 
        /// </summary>
        private string _path;//儲存 Active Directory 的 LDAP 路徑  "LDAP://yourCompanyName.com/DC=yourCompanyName,DC=com"; 
        private string _filterAttribute;//儲存搜尋 Active Directory 的篩選條件屬性。 

        public LdapAuthentication()
        {
            //
            // TODO: 在此加入建構函式的程式碼
            //
            // = MAXFC.ConfigLib.ReadWebConfigLib.ReadWebconfigSetting("LDAPURL");
            _path = ConfigurationManager.AppSettings["LDAPUrl"].ToString();
        }

        public LdapAuthentication(string path)
        {
            _path = path;
        }
        public bool ChangePassword(string domain, string username, string odlpwd, string newPassword)
        {
            try
            {
                string domainAndUsername = "";
                //帳號如果有包括domain，去掉domain
                if (username.Length > 0 && username.LastIndexOf(@"\") != -1)
                {
                    domain = username.Substring(0, username.LastIndexOf(@"\"));
                    username = username.Substring(username.LastIndexOf(@"\") + 1);
                }
                domainAndUsername = domain + @"\" + username;
                DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, odlpwd);
                entry.Invoke("ChangePassword", new object[] { odlpwd, newPassword });
                entry.CommitChanges();
                entry.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region IsAuthenticated
        /// <summary>
        /// 新增下列 IsAuthenticated 方法，將網域名稱、使用者名稱和密碼當作參數，
        /// 然後傳回 bool 以指示含有相符密碼的使用者是否存在於 Active Directory 之中
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool IsAuthenticated(string domain, string username, string pwd, bool bAddAuthToCookie)
        {
            string domainAndUsername = "";
            //帳號如果有包括domain，去掉domain
            if (username.Length > 0 && username.LastIndexOf(@"\") != -1)
            {
                domain = username.Substring(0, username.LastIndexOf(@"\"));
                username = username.Substring(username.LastIndexOf(@"\") + 1);
            }
            domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, pwd);
            

            try
            {
                // Bind to the native AdsObject to force authentication.
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + username + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
                // Update the new path to the user in the directory
                _path = result.Path;
                _filterAttribute = (String)result.Properties["cn"][0];

                if (bAddAuthToCookie)
                    AddAuthToCookie(username);
            }
            catch (Exception ex)
            {
                //throw new Exception("Error authenticating user. " + ex.Message);
                ex.ToString();
                return false;
            }
            return true;
        }

        public bool IsAuthenticated(string domain, string username, string pwd)
        {
            return IsAuthenticated(domain, username, pwd, false);
        }
        #endregion

        #region AddAuthToCookie
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUserName"></param>
        /// <param name="intMinutes"></param>
        public static void AddAuthToCookie(string strUserName, double intMinutes)
        {
            //號如果有包括domain，去掉domain
            if (strUserName.Length > 0 && strUserName.LastIndexOf(@"\") != -1)
            {
                strUserName = strUserName.Substring(strUserName.LastIndexOf(@"\") + 1);
            }
            try
            {
                // Create the authetication ticket
                FormsAuthenticationTicket authTicket =
                    new FormsAuthenticationTicket(1,  // version
                                                  strUserName,
                                                  DateTime.Now,
                                                  DateTime.Now.AddMinutes(intMinutes),
                                                  false, "");
                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie and add the encrypted ticket to the
                // cookie as data.
                HttpCookie authCookie = new HttpCookie(strUserName, encryptedTicket);

                authCookie.Expires = DateTime.Now.AddMinutes(intMinutes);

                // Add the cookie to the outgoing cookies collection.
                HttpContext.Current.Response.Cookies.Add(authCookie);

                System.Web.Security.FormsAuthentication.SetAuthCookie(strUserName, false);

                // Redirect the user to the originally requested page
                //if (strRUL == null || strRUL == "")
                //    HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(HttpContext.Current.User.Identity.Name, false));
                //else
                //    HttpContext.Current.Response.Redirect(strRUL);

            }
            catch (Exception ex)
            {
                ex.ToString();
                //lblError.Text = "Error authenticating. " + ex.Message;
            }
        }


        public static void AddAuthToCookie(string strUserName)
        {
            AddAuthToCookie(strUserName, 60);
        }
        #endregion

        #region GetGroupsByUserAccount    GetGroupsByUserName
        /// <summary>
        /// 這個程序會擴充 LdapAuthentication 類別以提供 GetGroupsByUserAccount 方法，
        /// 擷取現有使用者所屬群組的清單。 GetGroupsByUserAccount 方法會傳回以管道分隔的群組清單，如以下所示。
        /// "Group1|Group2|Group3|"
        /// </summary>
        /// <returns></returns>
        public string GetGroupsByUserAccount(string useraccount)
        {
            //帳號如果有包括domain，去掉domain
            if (useraccount != null && useraccount.Length > 0 && useraccount.LastIndexOf(@"\") != -1)
            {
                useraccount = useraccount.Substring(useraccount.LastIndexOf(@"\") + 1);
            }

            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(SAMAccountName=" + useraccount + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();
            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount;
                     propertyCounter++)
                {
                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1),
                                      (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                //throw new Exception("Error obtaining group names. " +
                //  ex.Message);
                return "";
            }
            return groupNames.ToString();
        }


        /// <summary>
        /// 這個程序會擴充 LdapAuthentication 類別以提供 GetGroupsByUserAccount 方法，
        /// 擷取現有使用者所屬群組的清單。 GetGroupsByUserAccount 方法會傳回以管道分隔的群組清單，如以下所示。
        /// "Group1|Group2|Group3|"
        /// </summary>
        /// <returns></returns>
        public string GetGroupsByUserName(string username)
        {
            //帳號如果有包括domain，去掉domain
            if (username != null && username.Length > 0 && username.LastIndexOf(@"\") != -1)
            {
                username = username.Substring(username.LastIndexOf(@"\") + 1);
            }

            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + username + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder groupNames = new StringBuilder();
            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount;
                     propertyCounter++)
                {
                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    groupNames.Append(dn.Substring((equalsIndex + 1),
                                      (commaIndex - equalsIndex) - 1));
                    groupNames.Append("|");
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                //throw new Exception("Error obtaining group names. " +
                //  ex.Message);
                return "";
            }
            return groupNames.ToString();
        }
        #endregion

        #region GetOUsByUserAccount

        /// <summary>
        /// 這個程序會擴充 LdapAuthentication 類別以提供 GetOUsByUserAccount 方法，
        /// 擷取現有使用者所屬OU的清單。 GetOUsByUserAccount 方法會傳回以管道分隔的群組清單，如以下所示。
        /// "OU1|OU2|OU3|"
        /// </summary>
        /// <param name="useraccount"></param>
        /// <returns></returns>
        public string GetOUsByUserAccount(string useraccount)
        {
            //帳號如果有包括domain，去掉domain
            if (useraccount != null && useraccount.Length > 0 && useraccount.LastIndexOf(@"\") != -1)
            {
                useraccount = useraccount.Substring(useraccount.LastIndexOf(@"\") + 1);
            }

            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(SAMAccountName=" + useraccount + ")";
            search.PropertiesToLoad.Add("memberOf");
            StringBuilder ouNames = new StringBuilder();
            try
            {
                SearchResult result = search.FindOne();

                int propertyCount = result.Properties["adspath"].Count;
                String dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount;
                     propertyCounter++)
                {
                    dn = (String)result.Properties["adspath"][propertyCounter];

                    //Response.Write(dn + "<br>");

                    if (dn.IndexOf("OU") != -1)
                    {
                        equalsIndex = dn.IndexOf("=", dn.IndexOf("OU"));
                        commaIndex = dn.IndexOf(",", dn.IndexOf("OU") + 1);

                        //Response.Write(equalsIndex + "<br>");
                        //Response.Write(commaIndex + "<br>");

                        if (-1 == equalsIndex)
                        {
                            break;
                        }
                        ouNames.Append(dn.Substring((equalsIndex + 1),
                                          (commaIndex - equalsIndex) - 1));
                        ouNames.Append("|");
                    }
                }

                propertyCount = result.Properties["memberOf"].Count;

                for (int propertyCounter = 0; propertyCounter < propertyCount;
                     propertyCounter++)
                {
                    dn = (String)result.Properties["memberOf"][propertyCounter];

                    //Response.Write(dn + "<br>");

                    if (dn.IndexOf("OU") != -1)
                    {
                        equalsIndex = dn.IndexOf("=", dn.IndexOf("OU"));
                        commaIndex = dn.IndexOf(",", dn.IndexOf("OU") + 1);

                        //Response.Write(equalsIndex + "<br>");
                        //Response.Write(commaIndex + "<br>");

                        if (-1 == equalsIndex)
                        {
                            break;
                        }
                        ouNames.Append(dn.Substring((equalsIndex + 1),
                                          (commaIndex - equalsIndex) - 1));
                        ouNames.Append("|");
                    }
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                //throw new Exception("Error obtaining group names. " +
                //  ex.Message);
                return "";
            }
            return ouNames.ToString();
        }
        #endregion

        #region  GetAllGroups
        /// <summary>
        /// GroupID
        /// Name
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllGroups()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("GroupID", Type.GetType("System.String"));
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DirectorySearcher searcher = new DirectorySearcher(_path);
            searcher.SearchRoot = new DirectoryEntry(_path);
            searcher.Filter = "(&(objectClass=group)(objectCategory=group))";
            searcher.Sort = new SortOption("Name", System.DirectoryServices.SortDirection.Ascending);
            searcher.PageSize = 512;
            searcher.PropertiesToLoad.Add("sAMAccountName");
            searcher.PropertiesToLoad.Add("name");
            SearchResultCollection results = searcher.FindAll();

            //foreach(SearchResult sr in results)
            //{
            //    foreach(String prop in sr.Properties.PropertyNames)
            //    {
            //        foreach(object obj in sr.Properties[prop]  )
            //        {
            //            Response.Write("<br>"+prop.ToString()+"-------"+obj.ToString()  );
            //        }
            //    }
            //}

            SearchResult result;
            if (results.Count > 0)
            {
                for (Int32 i = 0; i < results.Count; i++)
                {
                    result = results[i];
                    DataRow row = dt.Rows.Add();
                    if (result.Properties.Contains("sAMAccountName"))
                    {
                        row["GroupID"] = result.Properties["sAMAccountName"][0];
                    }
                    if (result.Properties.Contains("name"))
                    {
                        row["Name"] = result.Properties["name"][0];
                    }

                }
            }
            return dt;
        }
        #endregion

        #region  GetAllOU
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllOU()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", Type.GetType("System.String"));
            DirectorySearcher searcher = new DirectorySearcher(_path);
            searcher.SearchRoot = new DirectoryEntry(_path);
            searcher.Filter = "(&(objectClass=organizationalUnit)(objectCategory=organizationalUnit))";
            searcher.Sort = new SortOption("Name", System.DirectoryServices.SortDirection.Ascending);
            searcher.PageSize = 512;
            searcher.PropertiesToLoad.Add("name");
            SearchResultCollection results = searcher.FindAll();

            //foreach(SearchResult sr in results)
            //{
            //    foreach(String prop in sr.Properties.PropertyNames)
            //    {
            //        foreach(object obj in sr.Properties[prop]  )
            //        {
            //            Response.Write("<br>"+prop.ToString()+"-------"+obj.ToString()  );
            //        }
            //    }
            //}

            SearchResult result;
            if (results.Count > 0)
            {
                for (Int32 i = 0; i < results.Count; i++)
                {
                    result = results[i];
                    DataRow row = dt.Rows.Add();
                    if (result.Properties.Contains("name"))
                    {
                        row["Name"] = result.Properties["name"][0];
                    }

                }
            }
            return dt;
        }
        #endregion

        #region GetUserByAccount

        /// <summary>
        /// Account
        /// Name
        /// Email
        /// Title
        /// Groups  多個用 | 分開
        /// OUs       多個用 | 分開
        /// </summary>
        /// <param name="useraccount"></param>
        /// <returns></returns>
        public DataTable GetUserByAccount(string useraccount)
        {
            //號如果有包括domain，去掉domain
            if (useraccount != null && useraccount.Length > 0 && useraccount.LastIndexOf(@"\") != -1)
            {
                useraccount = useraccount.Substring(useraccount.LastIndexOf(@"\") + 1);
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("department", typeof(string));
            dt.Columns.Add("displayName", typeof(string));
            dt.Columns.Add("mail", typeof(string));
            dt.Columns.Add("company", typeof(string));
            dt.Columns.Add("description", typeof(string));


            DirectorySearcher searcher = new DirectorySearcher(_path);
            searcher.SearchRoot = new DirectoryEntry(_path);
            if (useraccount == null || useraccount.Length == 0)
                searcher.Filter = "(&(objectClass=user)(objectCategory=person))";
            else
                searcher.Filter = "(&(objectClass=user)(objectCategory=person)(sAMAccountName=" + useraccount + "))";
            searcher.Sort = new SortOption("name", System.DirectoryServices.SortDirection.Ascending);
            searcher.PageSize = 512;
            searcher.PropertiesToLoad.Add("department");
            searcher.PropertiesToLoad.Add("displayName");
            searcher.PropertiesToLoad.Add("mail");
            searcher.PropertiesToLoad.Add("company");
            searcher.PropertiesToLoad.Add("description");

            SearchResultCollection results = searcher.FindAll();
            SearchResult result;

            if (results.Count > 0)
            {
                for (Int32 i = 0; i < results.Count; i++)
                {
                    result = results[i];
                    DataRow row = dt.Rows.Add();
                    if (result.Properties.Contains("department"))
                    {
                        row["department"] = result.Properties["department"][0];
                    }
                    if (result.Properties.Contains("displayName"))
                    {
                        row["displayName"] = result.Properties["displayName"][0];
                    }
                    if (result.Properties.Contains("mail"))
                    {
                        row["mail"] = result.Properties["mail"][0];
                    }
                    if (result.Properties.Contains("company"))
                    {
                        row["company"] = result.Properties["company"][0];
                    }
                    if (result.Properties.Contains("description"))
                    {
                        row["description"] = result.Properties["description"][0];
                    }

                }
            }
            return dt;
        }

        #endregion

        #region GetAllUsers
        /// <summary>
        /// Account
        /// Name
        /// Email
        /// Title
        /// Groups  多個用 | 分開
        /// OUs       多個用 | 分開
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllUsers()
        {
            return GetUserByAccount(null);
        }
        #endregion

        #region SignOut

        public static void SignOut()
        {
            HttpContext context = HttpContext.Current;
            context.Session.Clear();
            //context.Session.Abandon();  Abandon後，同頁面再設session值，不起作用。因為abandon是在頁面結束時，才清sesion的。少用。
            FormsAuthentication.SignOut();
        }

        #endregion

   
        #region ImpersonateValidUser(string username, string domain, string password)   模擬給定的帳號執行
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="domain"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ImpersonateValidUser(string username, string domain, string password)
        {
            //帳號如果有包括domain，去掉domain
            if (username.Length > 0 && username.LastIndexOf(@"\") != -1)
            {
                domain = username.Substring(0, username.LastIndexOf(@"\"));
                username = username.Substring(username.LastIndexOf(@"\") + 1);
            }

            WindowsIdentity tempWindowsIdentity;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            if (LogonUser(username, domain, password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
            {
                if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
                {
                    tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
                    impersonationContext = tempWindowsIdentity.Impersonate();
                    if (impersonationContext != null)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion


    }
    public sealed class ADHelper
    {
        ///
        ///域名
        ///
        private static string DomainName = ConfigurationManager.AppSettings["WindowDomain"].ToString();
        ///
        /// LDAP 地址
        ///
        private static string LDAPDomain = ConfigurationManager.AppSettings["LDAPUrl"].ToString().Substring(ConfigurationManager.AppSettings["LDAPUrl"].ToString().LastIndexOf("/"));
        ///
        /// LDAP绑定路径
        ///
        private static string ADPath = ConfigurationManager.AppSettings["LDAPUrl"].ToString();
        ///
        ///登录帐号
        ///
        private static string ADUser = ConfigurationManager.AppSettings["LoginUser"].ToString();
        ///
        ///登录密码
        ///
        private static string ADPassword = ConfigurationManager.AppSettings["LoginPassword"].ToString();
        ///
        ///扮演类实例
        ///
        private static IdentityImpersonation impersonate = new IdentityImpersonation(ADUser, ADPassword, DomainName);

        ///
        ///用户登录验证结果
        ///
        public enum LoginResult
        {
            ///
            ///正常登录
            ///
            LOGIN_USER_OK = 0,
            ///
            ///用户不存在
            ///
            LOGIN_USER_DOESNT_EXIST,
            ///
            ///用户帐号被禁用
            ///
            LOGIN_USER_ACCOUNT_INACTIVE,
            ///
            ///用户密码不正确
            ///
            LOGIN_USER_PASSWORD_INCORRECT
        }

        ///
        ///用户属性定义标志
        ///
        public enum ADS_USER_FLAG_ENUM
        {
            ///
            ///登录脚本标志。如果通过 ADSI LDAP 进行读或写操作时，该标志失效。如果通过 ADSI WINNT，该标志为只读。
            ///
            ADS_UF_SCRIPT = 0X0001,
            ///
            ///用户帐号禁用标志
            ///
            ADS_UF_ACCOUNTDISABLE = 0X0002,
            ///
            ///主文件夹标志
            ///
            ADS_UF_HOMEDIR_REQUIRED = 0X0008,
            ///
            ///过期标志
            ///
            ADS_UF_LOCKOUT = 0X0010,
            ///
            ///用户密码不是必须的
            ///
            ADS_UF_PASSWD_NOTREQD = 0X0020,
            ///
            ///密码不能更改标志
            ///
            ADS_UF_PASSWD_CANT_CHANGE = 0X0040,
            ///
            ///使用可逆的加密保存密码
            ///
            ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
            ///
            ///本地帐号标志
            ///
            ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0X0100,
            ///
            ///普通用户的默认帐号类型
            ///
            ADS_UF_NORMAL_ACCOUNT = 0X0200,
            ///
            ///跨域的信任帐号标志
            ///
            ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0X0800,
            ///
            ///工作站信任帐号标志
            ///
            ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
            ///
            ///服务器信任帐号标志
            ///
            ADS_UF_SERVER_TRUST_ACCOUNT = 0X2000,
            ///
            ///密码永不过期标志
            ///
            ADS_UF_DONT_EXPIRE_PASSWD = 0X10000,
            ///
            /// MNS 帐号标志
            ///
            ADS_UF_MNS_LOGON_ACCOUNT = 0X20000,
            ///
            ///交互式登录必须使用智能卡
            ///
            ADS_UF_SMARTCARD_REQUIRED = 0X40000,
            ///
            ///当设置该标志时，服务帐号（用户或计算机帐号）将通过 Kerberos 委托信任
            ///
            ADS_UF_TRUSTED_FOR_DELEGATION = 0X80000,
            ///
            ///当设置该标志时，即使服务帐号是通过 Kerberos 委托信任的，敏感帐号不能被委托
            ///
            ADS_UF_NOT_DELEGATED = 0X100000,
            ///
            ///此帐号需要 DES 加密类型
            ///
            ADS_UF_USE_DES_KEY_ONLY = 0X200000,
            ///
            ///不要进行 Kerberos 预身份验证
            ///
            ADS_UF_DONT_REQUIRE_PREAUTH = 0X4000000,
            ///
            ///用户密码过期标志
            ///
            ADS_UF_PASSWORD_EXPIRED = 0X800000,
            ///
            ///用户帐号可委托标志
            ///
            ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0X1000000
        }

        public ADHelper()
        {
            //
        }

        #region GetDirectoryObject

        ///
        ///获得DirectoryEntry对象实例,以管理员登陆AD
        ///
        ///
        private static DirectoryEntry GetDirectoryObject()
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath, ADUser, ADPassword, AuthenticationTypes.Secure);
            return entry;
        }

        ///
        ///根据指定用户名和密码获得相应DirectoryEntry实体
        ///
        ///
        ///
        ///
        private static DirectoryEntry GetDirectoryObject(string userName, string password)
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath, userName, password, AuthenticationTypes.None);
            return entry;
        }

        ///
        /// i.e. /CN=Users,DC=creditsights, DC=cyberelves, DC=Com
        ///
        ///
        ///
        private static DirectoryEntry GetDirectoryObject(string domainReference)
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath + domainReference, ADUser, ADPassword, AuthenticationTypes.Secure);
            return entry;
        }

        ///
        ///获得以UserName,Password创建的DirectoryEntry
        ///
        ///
        ///
        ///
        ///
        private static DirectoryEntry GetDirectoryObject(string domainReference, string userName, string password)
        {
            DirectoryEntry entry = new DirectoryEntry(ADPath + domainReference, userName, password, AuthenticationTypes.Secure);
            return entry;
        }

        #endregion

        #region GetDirectoryEntry

        ///
        ///根据用户公共名称取得用户的 对象
        ///
        ///用户公共名称 
        ///如果找到该用户，则返回用户的 对象；否则返回 null
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
            catch
            {
                return null;
            }
        }

        ///
        ///根据用户公共名称和密码取得用户的 对象。
        ///
        ///用户公共名称
        ///用户密码 
        ///如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntry(string commonName, string password)
        {
            DirectoryEntry de = GetDirectoryObject(commonName, password);
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName + "))";
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

        ///
        ///根据用户帐号称取得用户的 对象
        ///
        ///用户帐号名 
        ///如果找到该用户，则返回用户的 对象；否则返回 null
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
            catch
            {
                return null;
            }
        }

        ///
        ///根据用户帐号和密码取得用户的 对象
        ///
        ///用户帐号名
        ///用户密码
        ///如果找到该用户，则返回用户的 对象；否则返回 null
        public static DirectoryEntry GetDirectoryEntryByAccount(string sAMAccountName, string password)
        {
            DirectoryEntry de = GetDirectoryEntryByAccount(sAMAccountName);
            if (de != null)
            {
                string commonName = de.Properties["cn"][0].ToString();

                if (GetDirectoryEntry(commonName, password) != null)
                    return GetDirectoryEntry(commonName, password);
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        ///
        ///根据组名取得用户组的 对象
        ///
        ///组名
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

        #region GetProperty

        ///
        ///获得指定 指定属性名对应的值
        ///
        ///
        ///属性名称
        ///属性值
        public static string GetProperty(DirectoryEntry de, string propertyName)
        {
            if (de.Properties.Contains(propertyName))
            {
                return de.Properties[propertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        ///
        ///获得指定搜索结果 中指定属性名对应的值
        ///
        ///
        ///属性名称 
        ///属性值
        public static string GetProperty(SearchResult searchResult, string propertyName)
        {
            if (searchResult.Properties.Contains(propertyName))
            {
                return searchResult.Properties[propertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        ///
        ///设置指定 的属性值
        ///
        ///
        ///属性名称
        ///属性值 
        public static void SetProperty(DirectoryEntry de, string propertyName, string propertyValue)
        {
            if (propertyValue != string.Empty || propertyValue != "" || propertyValue != null)
            {
                if (de.Properties.Contains(propertyName))
                {
                    de.Properties[propertyName][0] = propertyValue;
                }
                else
                {
                    de.Properties[propertyName].Add(propertyValue);
                }
            }
        }

        ///
        ///创建新的用户
        ///
        ///
        ///DN 位置。例如：OU=共享平台 或 CN=Users
        ///公共名称
        ///帐号
        ///密码
        public static DirectoryEntry CreateNewUser(string ldapDN, string commonName, string sAMAccountName, string password)
        {
            DirectoryEntry entry = GetDirectoryObject();
            DirectoryEntry subEntry = entry.Children.Find(ldapDN);
            DirectoryEntry deUser = subEntry.Children.Add("CN=" + commonName, "user");
            deUser.Properties["sAMAccountName"].Value = sAMAccountName;
            deUser.CommitChanges();
            ADHelper.EnableUser(commonName);
            ADHelper.SetPassword(commonName, password);
            deUser.Close();
            return deUser;
        }

        ///
        ///创建新的用户。默认创建在 Users 单元下。
        ///
        ///
        ///公共名称
        ///帐号
        ///密码
        public static DirectoryEntry CreateNewUser(string commonName, string sAMAccountName, string password)
        {
            return CreateNewUser("CN=Users", commonName, sAMAccountName, password);
        }

        ///
        ///判断指定公共名称的用户是否存在
        ///
        ///用户公共名称 

        ///如果存在，返回 true；否则返回 false
        public static bool IsUserExists(string commonName)
        {
            DirectoryEntry de = GetDirectoryObject();
            DirectorySearcher deSearch = new DirectorySearcher(de);
            deSearch.Filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName + "))";       // LDAP 查询串
            SearchResultCollection results = deSearch.FindAll();

            if (results.Count == 0)
                return false;
            else
                return true;
        }

        ///
        ///判断用户帐号是否激活
        ///
        ///用户帐号属性控制器 

        ///如果用户帐号已经激活，返回 true；否则返回 false
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
        ///判断用户与密码是否足够以满足身份验证进而登录
        ///用户公共名称
        ///

        ///密码

        ///如能可正常登录，则返回 true；否则返回 false
        public static LoginResult Login(string commonName, string password)
        {
            DirectoryEntry de = GetDirectoryEntry(commonName);

            if (de != null)
            {
                // 必须在判断用户密码正确前，对帐号激活属性进行判断；否则将出现异常。
                int userAccountControl = Convert.ToInt32(de.Properties["userAccountControl"][0]);
                de.Close();

                if (!IsAccountActive(userAccountControl))
                    return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;

                if (GetDirectoryEntry(commonName, password) != null)
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
        ///判断用户帐号与密码是否足够以满足身份验证进而登录
        ///用户帐号
        ///

        ///密码 

        ///如能可正常登录，则返回 true；否则返回 false
        public static LoginResult LoginByAccount(string sAMAccountName, string password)
        {
            DirectoryEntry de = GetDirectoryEntryByAccount(sAMAccountName);

            if (de != null)
            {
                // 必须在判断用户密码正确前，对帐号激活属性进行判断；否则将出现异常。
                int userAccountControl = Convert.ToInt32(de.Properties["userAccountControl"][0]);
                de.Close();

                if (!IsAccountActive(userAccountControl))
                    return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;

                if (GetDirectoryEntryByAccount(sAMAccountName, password) != null)
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
        ///设置用户密码，管理员可以通过它来修改指定用户的密码。
        ///
        ///用户公共名称 

        ///用户新密码 

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
        ///设置帐号密码，管理员可以通过它来修改指定帐号的密码。
        ///
        ///用户新密码 

        ///用户帐号

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
        ///修改用户密码
        ///用户公共名称 
        ///旧密码 

        ///新密码 

        ///

        public static void ChangeUserPassword(string commonName, string oldPassword, string newPassword)
        {
            // to-do: 需要解决密码策略问题
            DirectoryEntry oUser = GetDirectoryEntry(commonName);
            oUser.Invoke("ChangePassword", new Object[] { oldPassword, newPassword });
            oUser.Close();
        }

        ///
        ///启用指定公共名称的用户
        ///用户公共名称 
        ///

        public static void EnableUser(string commonName)
        {
            EnableUser(GetDirectoryEntry(commonName));
        }

        ///
        ///启用指定 的用户
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
        ///禁用指定公共名称的用户
        ///用户公共名称
        ///

        public static void DisableUser(string commonName)
        {
            DisableUser(GetDirectoryEntry(commonName));
        }

        ///
        ///禁用指定 的用户
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
        ///将指定的用户添加到指定的组中。默认为 Users 下的组和用户。
        ///用户公共名称 
        ///组名 

        ///

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
        ///将用户从指定组中移除。默认为 Users 下的组和用户。
        ///用户公共名称 
        ///组名

        ///

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

    }

    ///
    ///用户模拟角色类。实现在程序段内进行用户角色模拟。
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

        ///
        ///构造函数
        ///所要模拟的用户的用户名
        ///所要模拟的用户的密码 

        ///所要模拟的用户所在的域 

        ///

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
        ///析构函数
        ///
        ~IdentityImpersonation()
        {
            if (!_bClosed)
            {
                StopImpersonate();
            }
        }

        ///
        ///开始身份角色模拟。
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

            WindowsIdentity fakeId = new WindowsIdentity(_dupeToken);
            _imperContext = fakeId.Impersonate();

            _bClosed = false;

            return true;
        }

        ///
        ///停止身分角色模拟。
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





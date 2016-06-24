using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.Framework.Extensions;
using aZaaS.KStar.MgmtServices;
using aZaaS.Framework.Workflow;
using System.Web.Configuration;
using System.Web.Security;
using System.Linq.Expressions;
using System.Reflection;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Form.Helpers;
using System.Xml;
using System.IO;
using aZaaS.KStar.Helper;
using aZaaS.Framework.Workflow.Util;
using System.Text.RegularExpressions;
using aZaaS.KStar.Facades;

namespace aZaaS.KStar.Web.Controllers
{
    /// <summary>
    /// Base class of all mvc handle controller.
    /// </summary>
    [EnhancedHandleError]
    [System.Web.Mvc.Authorize]
    public class BaseMvcController : Controller
    {
        private readonly UserService _userService;
        public AuthenticationType AuthType { get; protected set; }
        private readonly DelegationService _delegationService;

        public BaseMvcController()
        {
            _userService = new UserService();
            _delegationService = new DelegationService();
        }


        private void InitAuthenticationType()
        {
            if (Session["__AuthType"] == null)
            {
                System.Configuration.Configuration configuration =
                    System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(
                    "/Web");

                // Get the section.
                AuthenticationSection authenticationSection =
                    (AuthenticationSection)configuration.GetSection(
                    "system.web/authentication");

                AuthenticationMode mode = authenticationSection.Mode;


                switch (mode)
                {
                    case AuthenticationMode.Forms:
                        Session["__AuthType"] = AuthenticationType.Form;
                        break;
                    case AuthenticationMode.Windows:
                        Session["__AuthType"] = AuthenticationType.Windows;
                        break;
                }
            }
            AuthType = (AuthenticationType)Session["__AuthType"];
        }

        /// <summary>
        /// Gets the shared user which to be appended to url query string.    
        /// </summary>
        protected string SharedUser
        {
            get
            {
                return this.TrimUserName(Request["SharedUser"]);
            }
        }

        /// <summary>
        /// Gets the current windows logon user.
        /// </summary>
        protected string CurrentUser
        {
            get
            {
                if (Session["CurrentUser"] == null)
                {
                    Session["CurrentUser"] = User.Identity.Name;
                    Session.Remove("IsDisable");
                }
                if (Session["CurrentUser"].ToString() != User.Identity.Name)
                {
                    Session["CurrentUser"] = User.Identity.Name;
                }
                return Session["CurrentUser"].ToString();
            }
        }

        protected UserWithFieldsDto CurrentUserEntry
        {
            get
            {
                UserWithFieldsDto user = null;

                if (Session["CurrentUserEntry"] == null)
                {
                    user = _userService.ReadUserWithFields(CurrentUser);
                    Session["CurrentUserEntry"] = user;
                }
                else
                {
                    user = Session["CurrentUserEntry"] as UserWithFieldsDto;
                }

                return user;
            }
        }

        protected string CurrentWorkflowUser
        {
            get
            {
                return string.Format("{0}:{1}", PortalEnvironment.CurrentWorkflowSecurityLabel, CurrentUser);
            }
        }

        public string GetUserWithOutLabel(string users)
        {
            string temp = string.Empty;
            if (!string.IsNullOrEmpty(users))
            {
                temp = Regex.Replace(users, "k2:|k2sql:", "", RegexOptions.IgnoreCase);
            }
            return temp;
        }
        public List<string> GetUserWithOutLabel(List<string> users)
        {
            string temp = string.Empty;
            List<string> userlist = new List<string>();
            if (users != null && users.Count > 0)
            {
                temp = string.Join(",", users);
                temp = Regex.Replace(temp, "k2:|k2sql:", "", RegexOptions.IgnoreCase);
                string[] arr = temp.Split(',');
                userlist = arr.ToList();
            }
            return userlist;
        }

        public string GetUserWithLabel(string user)
        {
            if (!string.IsNullOrEmpty(user) && !Regex.Match(user, "k2:|k2sql:", RegexOptions.IgnoreCase).Success)
            {
                return string.Format("{0}:{1}", PortalEnvironment.CurrentWorkflowSecurityLabel, user);
            }
            else
            {
                return user;
            }
        }
        public List<string> GetUserWithLabel(List<string> users)
        {
            string temp = string.Empty;
            List<string> userlist = new List<string>();
            if (users != null && users.Count > 0)
            {
                temp = string.Join(",", users);
                if (Regex.Match(temp, "k2:|k2sql:", RegexOptions.IgnoreCase).Success)
                {
                    temp = Regex.Replace(temp, "k2:|k2sql:", "", RegexOptions.IgnoreCase);
                }
                temp = Regex.Replace(temp, ",", "," + PortalEnvironment.CurrentWorkflowSecurityLabel + ":", RegexOptions.IgnoreCase);
                temp = PortalEnvironment.CurrentWorkflowSecurityLabel + ":" + temp;
                string[] arr = temp.Split(',');
                userlist = arr.ToList();
            }
            return userlist;
        }

        public string ConnectionString(string key)
        {
            var connStr = string.Empty;

            if (Session["_TENANT_CONNSET_"] != null && AuthType == AuthenticationType.Form)
            {
                var connSets =Session["_TENANT_CONNSET_"] as Dictionary<string, string>;
                connStr = connSets[key];
            }
            else
            {
                connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings[key].ToString();
            }
            return connStr;
        }
        public string TenantID()
        {
            if (Session != null)
            {
                var tenantid = Session["TenantCode"];
                if (tenantid != null)
                {
                    return tenantid as string;
                }
            }
            return string.Empty;
        }

        protected string SerialNumber
        {
            get
            {
                return Request["SN"] ?? string.Empty;
            }
        }

        [NonAction]
        protected string TrimUserName(string account)
        {
            int index = (account ?? string.Empty).IndexOf(":");
            return index > 0 ? account.Substring(index + 1) : account;
        }
        [NonAction]
        protected string GetViewUrl(string fullName)
        {
            string viewurl = string.Empty;
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var process = svc.GetProcessSetByFullName(this.CurrentUser, fullName, false);
            if (process != null)
            {
                viewurl = process.ViewUrl;
            }
            return viewurl;
        }

        [NonAction]
        protected string GetViewUrl(string fullName, string procInstId)
        {
            string viewurl = string.Empty;
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            var process = svc.GetProcessSetByFullName(this.CurrentUser, fullName, false);
            if (process != null)
            {
                var service = new ViewFlowArgs();

                viewurl = service.FormatViewUrl(process.ViewUrl, procInstId);
            }

            return viewurl;
        }

        /// <summary>
        /// 获取用户姓名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetOriginator(string username)
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["Users"] as Dictionary<string, string>;
            if (dics == null||dics.Count==0)
            {
                dics = new Dictionary<string, string>();
                //缓存中没有，则从数据库中加载
                UserBO ub = new UserBO();
                IEnumerable<UserWithRelationshipsDto> users = ub.GetAllUsers();
                foreach (var user in users)
                {
                    if (!dics.ContainsKey(user.UserName.ToLower()))
                    {
                        dics.Add(user.UserName.ToLower(), user.FullName);
                    }
                }
                HttpRuntime.Cache.Remove("Users");
                HttpRuntime.Cache.Insert("Users", dics);
            }
            if (dics.ContainsKey(username.ToLower()))
            {
                return dics[username.ToLower()];
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取流程名称
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetProcessSetByFullName(string fullname)
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["ProcessSets"] as Dictionary<string, string>;
            if (dics == null || dics.Count == 0)
            {
                dics = new Dictionary<string, string>();
                //缓存中没有，则从数据库中加载
                var svc = new ConfigManager(this.AuthType);
                svc.TenantID = TenantID();
                List<Configuration_ProcessSetDTO> process= svc.GetProcessSets(this.CurrentUser);
                foreach (var item in process)
                {
                    if (!dics.ContainsKey(item.ProcessFullName))
                    {
                        dics.Add(item.ProcessFullName, item.ProcessName);
                    }
                }
                HttpRuntime.Cache.Remove("ProcessSets");
                HttpRuntime.Cache.Insert("ProcessSets", dics);
            }
            if (dics.ContainsKey(fullname))
            {
                return dics[fullname];
            }
            return string.Empty;            
        }
        /// <summary>
        /// 获取代理人用户名
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<string> GetDelegationUserName(string processName,string userName)
        {
            List<string> delegationToUsers = new List<string>();
             List<Delegation> delegations = HttpRuntime.Cache["ProcessDelegations"] as List<Delegation>;
             if (delegations == null || delegations.Count == 0)
             {
                 //缓存中没有，则从数据库中加载                
                 delegations = _delegationService.GetAllDelegations(DateTime.Now, true);
                 HttpRuntime.Cache.Remove("ProcessDelegations");
                 HttpRuntime.Cache.Insert("ProcessDelegations", delegations);
             }
             var tempdelegations=delegations.Where(x => x.FullName == processName&&x.FromUser.ToLower()==userName.ToLower()).ToList();
             tempdelegations.ForEach(x =>
             {
                 delegationToUsers.AddRange(x.ToUsers);
             });
             return delegationToUsers;
        }
        /// <summary>
        /// 获取审批以及代理人姓名
        /// </summary>
        /// <param name="processFullName"></param>
        /// <param name="prevApprovers"></param>
        /// <returns></returns>
        public string GetApproversAndDelegationName(string processFullName, string prevApprovers)
        {
            if (string.IsNullOrEmpty(prevApprovers))
                return string.Empty;
            List<string> approversAndDelegations = new List<string>();
            List<string> delegationUsers = new List<string>();
            string[] prevApproverName = prevApprovers.Split(',');
            for (int i = 0; i < prevApproverName.Length; i++)
            {
                var tempdelegation = GetDelegationUserName(processFullName, prevApproverName[i]);
                if (tempdelegation != null && tempdelegation.Count > 0)
                {
                    tempdelegation = GetUserWithOutLabel(tempdelegation);
                    tempdelegation.ForEach(x =>
                    {
                        delegationUsers.Add(GetOriginator(x));
                    });
                    approversAndDelegations.Add(string.Format("{0}({1})", GetOriginator(GetUserWithOutLabel(prevApproverName[i])), string.Join(",", delegationUsers)));
                }
                else
                {
                    approversAndDelegations.Add(string.Format("{0}", GetOriginator(GetUserWithOutLabel(prevApproverName[i]))));
                }
            }
            return string.Join(",", approversAndDelegations);
        }

        /// <summary>
        /// 初始化 基础数据固有过滤条件
        /// </summary>
        public void InitDataFilter()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string path = System.Threading.Thread.GetDomain().BaseDirectory + "DataFilter.xml";
            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (Stream fs = new FileStream(path, FileMode.Open))
                    {
                        xmlDoc.Load(fs);
                        XmlElement root = xmlDoc.DocumentElement;
                        //用户过滤
                        XmlNode userCollection = root.SelectSingleNode("User");
                        List<ListFilter> userfilter = XMLHelper.Deserialize<List<ListFilter>>(userCollection.InnerXml);
                        if (HttpRuntime.Cache.Get("UserDataFilter") == null)
                            HttpRuntime.Cache.Insert("UserDataFilter", userfilter);
                        else
                        {
                            HttpRuntime.Cache.Remove("UserDataFilter");
                            HttpRuntime.Cache.Insert("UserDataFilter", userfilter);
                        }
                        //职位过滤
                        XmlNode positionCollection = root.SelectSingleNode("Position");
                        List<ListFilter> positionfilter = XMLHelper.Deserialize<List<ListFilter>>(positionCollection.InnerXml);
                        if (HttpRuntime.Cache.Get("PositionDataFilter") == null)
                            HttpRuntime.Cache.Insert("PositionDataFilter", positionfilter);
                        else
                        {
                            HttpRuntime.Cache.Remove("PositionDataFilter");
                            HttpRuntime.Cache.Insert("PositionDataFilter", positionfilter);
                        }

                        //部门过滤
                        XmlNode departCollection = root.SelectSingleNode("Department");
                        List<ListFilter> departfilter = XMLHelper.Deserialize<List<ListFilter>>(departCollection.InnerXml);
                        if (HttpRuntime.Cache.Get("DepartmentDataFilter") == null)
                            HttpRuntime.Cache.Insert("DepartmentDataFilter", departfilter);
                        else
                        {
                            HttpRuntime.Cache.Remove("DepartmentDataFilter");
                            HttpRuntime.Cache.Insert("DepartmentDataFilter", departfilter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HttpRuntime.Cache.Get("PositionDataFilter") == null)
                        HttpRuntime.Cache.Insert("PositionDataFilter", new List<ListFilter>());
                    if (HttpRuntime.Cache.Get("UserDataFilter") == null)
                        HttpRuntime.Cache.Insert("UserDataFilter", new List<ListFilter>());
                    if (HttpRuntime.Cache.Get("DepartmentDataFilter") == null)
                        HttpRuntime.Cache.Insert("DepartmentDataFilter", new List<ListFilter>());
                }
            }
            else {
                if (HttpRuntime.Cache.Get("PositionDataFilter")==null)
                    HttpRuntime.Cache.Insert("PositionDataFilter", new List<ListFilter>());
                if (HttpRuntime.Cache.Get("UserDataFilter") == null)
                    HttpRuntime.Cache.Insert("UserDataFilter", new List<ListFilter>());
                if (HttpRuntime.Cache.Get("DepartmentDataFilter") == null)
                    HttpRuntime.Cache.Insert("DepartmentDataFilter", new List<ListFilter>());
            }
        }                
        [NonAction]
        protected static DateTime InitStartDate(DateTime? startDate)
        {
            if (startDate == null)
            {
                return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            }
            else
            {
                DateTime newStartDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day);
                return newStartDate;
            }
        }
        [NonAction]
        protected static DateTime InitEndDate(DateTime? endDate)
        {
            if (endDate == null)
            {
                return DateTime.MaxValue;
            }
            else
            {
                DateTime newEndDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day);
                return newEndDate.AddDays(1);
            }
        }
        [NonAction]
         public static IQueryable<T> DataSorting<T>(IQueryable<T> source, string sortField, string sortDirection)
         {
             string sortingDir = string.Empty;
             if (sortDirection.ToUpper().Trim() == "ASC")
                 sortingDir = "OrderBy";
             else if (sortDirection.ToUpper().Trim() == "DESC")
                 sortingDir = "OrderByDescending";
             ParameterExpression param = Expression.Parameter(typeof(T), sortField);
             PropertyInfo pi = typeof(T).GetProperty(sortField);
             Type[] types = new Type[2];
             types[0] = typeof(T);
             types[1] = pi.PropertyType;
             Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortField), param));
             IQueryable<T> query = source.AsQueryable().Provider.CreateQuery<T>(expr);
             return query;
         }


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool validaccount = true;
            InitAuthenticationType();

            //if (this.AuthType == AuthenticationType.Form && !User.Identity.IsAuthenticated)
            //{
            //    Response.Redirect(FormsAuthentication.LoginUrl);
            //    return;
            //}
            if (User.Identity.IsAuthenticated)
            {
                if (Session["IsDisable"] == null)
                {
                    UserService userService = new UserService();
                    var user = userService.ReadUserBase(this.CurrentUser);
                    if (user == null)
                    {
                        validaccount = false;
                    }
                    else
                    {
                        Session["IsDisable"] = (string.IsNullOrEmpty(user.Status) ? "false" : user.Status);
                    }
                }
                //无效账号
                if (!validaccount)
                {
                    HttpContext.Session.Abandon();
                    HttpContext.Session.Clear();
                    //Response.Clear();
                    //Response.StatusCode = 401;
                    //Response.End();
                    Response.Redirect("/AccountInvalid.html");                    
                }
                else if (filterContext.ActionDescriptor.ActionName != "Logout" && !Convert.ToBoolean(Session["IsDisable"].ToString().ToLower()))
                {
                    HttpContext.Session.Abandon();
                    HttpContext.Session.Clear();
                    //Response.Clear();
                    //Response.StatusCode = 401;
                    //Response.End();
                    Response.Redirect("/AccountUnavailable.html");                    
                }                
            }            
            base.OnActionExecuting(filterContext);
        }

        public UserDto GetLoginUser()
        {
            UserService userService = new UserService();
            return userService.ReadUserInfo(this.CurrentUser);

        }

        public UserDto GetLoginUserInfo()
        {
            UserService userService = new UserService();
            return  userService.ReadUserInfo(this.CurrentUser);
        }

        public UserDto GetUserInfo(string userName)
        {
            UserService userService = new UserService();
            var user = userService.ReadUserInfo(this.CurrentUser);

            return user;
        }
    }
}

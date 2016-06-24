
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;

using aZaaS.KStar.Authentication;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.Web.AuthProviders
{
    public class DomainUserAuthProvider : IUserAuthProvider
    {
        private readonly UserBO _userBO;
        private const string PARAM_DOMAIN = "Domain";
        private const string PARAM_HOSTSERVER = "HostServer";
        private const string PARAM_SERVICEACCOUNT = "ServiceAccount";
        private const string PARAM_SERVICEPASSWORD = "ServicePassword";

        public DomainUserAuthProvider()
        {
            _userBO = new UserBO();
        }

        public bool Authenticate(string userName, string password)
        {
            //背景设定：
            //KStar运行于（单）域环境当中，用户使用域帐号作为登录帐号
            //特殊说明：我们使用Principalcontext进行域用户认证（非LDAP方式）

            var parameters = this.GetParameterMap();

            var domain = parameters[PARAM_DOMAIN];
            var hostName = parameters[PARAM_HOSTSERVER];
            var serviceAccount = parameters[PARAM_SERVICEACCOUNT];
            var servicePassword = parameters[PARAM_SERVICEPASSWORD];

            var prefix = string.Format(@"{0}\", domain);

            if (!userName.Contains(@"\"))
                userName = string.Format(@"{0}{1}", prefix, userName);

            this.ValidUserExists(userName);

            userName = userName.Replace(prefix, string.Empty);

            return AuthenticateUsingPrincipalcontext(hostName, serviceAccount, servicePassword, userName, password);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var parameters = this.GetParameterMap();

            var domain = parameters[PARAM_DOMAIN]; 
            var hostName = parameters[PARAM_HOSTSERVER];
            var serviceAccount = parameters[PARAM_SERVICEACCOUNT];
            var servicePassword = parameters[PARAM_SERVICEPASSWORD];

            var prefix = string.Format(@"{0}\", domain);

            if (!userName.Contains(@"\"))
                userName = string.Format(@"{0}{1}", prefix, userName);

            this.ValidUserExists(userName);

            userName = userName.Replace(prefix, string.Empty);

            if (!AuthenticateUsingPrincipalcontext(hostName, serviceAccount, servicePassword, userName, oldPassword))
                ExceptionRaiser.PasswordNotMatch(userName);

            return ResetPasswordUsingPrincipalcontext(hostName, serviceAccount, servicePassword, userName, oldPassword, newPassword);
        }

        public void ParameterMapValidator(Dictionary<string, string> parameters)
        {
            if (parameters.NotExistsOrEmpty(PARAM_DOMAIN))
                ExceptionRaiser.InvalidParameter(PARAM_DOMAIN);

            if (parameters.NotExistsOrEmpty(PARAM_HOSTSERVER))
                ExceptionRaiser.InvalidParameter(PARAM_HOSTSERVER);

            if (parameters.NotExistsOrEmpty(PARAM_SERVICEACCOUNT))
                ExceptionRaiser.InvalidParameter(PARAM_SERVICEACCOUNT);

            if (parameters.NotExistsOrEmpty(PARAM_SERVICEPASSWORD))
                ExceptionRaiser.InvalidParameter(PARAM_SERVICEPASSWORD);
        }

        private bool AuthenticateUsingPrincipalcontext(string hostName, string serviceAccount, string servicePassword, string userName, string password)
        {

            using (var ctx = new PrincipalContext(ContextType.Domain, hostName, serviceAccount, servicePassword))
            {
                return ctx.ValidateCredentials(userName, password);
            }
        }

        private bool ResetPasswordUsingPrincipalcontext(string hostName, string serviceAccount, string servicePassword, string userName, string oldPassword, string newPassword)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, hostName, serviceAccount, servicePassword))
            {
                using (var user = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, userName))
                {
                    if (user == null)
                        ExceptionRaiser.UserNotExists(userName);

                    user.ChangePassword(oldPassword, newPassword);
                }
            }

            return true;
        }

        public string LoginName(string userName)
        {
            if (!userName.Contains(@"\"))
                userName = string.Format(@"{0}\{1}", this.GetParameter(PARAM_DOMAIN), userName);

            return userName;
        }
    }
}
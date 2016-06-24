using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.Authentication;

namespace aZaaS.KStar.Web.AuthProviders
{
    public class DevUserAuthProvider : IUserAuthProvider
    {
        private readonly UserBO _userBO;
        private const string PARAM_DOMAIN = "Domain";

        public DevUserAuthProvider()
        {
            _userBO = new UserBO();
        }

        public bool Authenticate(string userName, string password)
        {
            //背景设定：
            //KStar运行于（单）域环境当中，用户使用域帐号作为登录帐号

            var parameters = this.GetParameterMap();

            //登录账号允许忽略域名，后台进行账号有效校验前需要自动补全（非必需）
            if (!userName.Contains(@"\"))
                userName = string.Format(@"{0}\{1}", parameters[PARAM_DOMAIN], userName);

            //在开发模式下，为了方便测试，我们只需验证登录用户帐号有效即可
            this.ValidUserExists(userName);

            return true;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            //TODO:
            return true;
        }

        public void ParameterMapValidator(Dictionary<string, string> parameters)
        {
            if (parameters.NotExistsOrEmpty(PARAM_DOMAIN))
                ExceptionRaiser.InvalidParameter(PARAM_DOMAIN);
        }

        public string LoginName(string userName)
        {
            if (!userName.Contains(@"\"))
                userName = string.Format(@"{0}\{1}", this.GetParameter(PARAM_DOMAIN), userName);

            return userName;
        }
    }
}
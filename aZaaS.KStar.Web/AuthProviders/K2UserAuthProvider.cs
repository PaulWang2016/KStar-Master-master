using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using aZaaS.Framework.Workflow;
using aZaaS.KStar.Authentication;

namespace aZaaS.KStar.Web.AuthProviders
{
    public class K2UserAuthProvider : IUserAuthProvider
    {
        private readonly UserBO _userBO;
        //private const string PARAM_K2_SECURITYLABEL = "K2SecurityLabel";
        //private const string PARAM_K2SQL_SECURITYLABEL = "K2SQLSecurityLabel";

        public K2UserAuthProvider()
        {
            _userBO = new UserBO();
        }

        public string LoginName(string userName)
        {
            return userName;
        }

        public bool Authenticate(string userName, string password)
        {
            //背景设定：
            //KStar运行于K2混合（「多/单」域+SQL）环境中，域用户使用域帐号登录，K2SQL用户使用K2SQL帐号登录
            //特殊说明：为了解决多域/K2SQL登录问题了，我们假定这些用户都可以使用K2，那么我们直接通过K2接口进行身份认证。

            //var parameters = this.GetParameterMap();

            this.ValidUserExists(userName);

            //默认配置：
            //我们默认 域帐号使用SecurityLabel为: K2 (集成认证），K2SQL帐号使用SecurityLabel为: K2SQL （非集成认证）

            return K2User.Test(userName, password);
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void ParameterMapValidator(Dictionary<string, string> parameters)
        {
            //if (parameters.NotExistsOrEmpty(PARAM_K2_SECURITYLABEL))
            //    ExceptionRaiser.InvalidParameter(PARAM_K2_SECURITYLABEL);

            //if (parameters.NotExistsOrEmpty(PARAM_K2SQL_SECURITYLABEL))
            //    ExceptionRaiser.InvalidParameter(PARAM_K2SQL_SECURITYLABEL);
        }
    }
}
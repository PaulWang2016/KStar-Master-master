using K2.Framework.SSO;
using SSOLoginWeb.Code.Interface;
using SSOLoginWeb.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SSOLoginWeb
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“TokenService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 TokenService.svc 或 TokenService.svc.cs，然后开始调试。
    public class TokenService : ITokenService
    {

        public string HasLoginedByToken(string token)
        {
            SSOUserToken _token = SSOUserTokenManager.CreateUserTokenInstance(SSOUserTokenManager.UrlReplaceToBase64(token), CrypterKind.SSO);

            WriteLogHelper.WriteLogger(string.Format("{0} 调用了HasLoginedByToken {1},{2}", DateTime.Now.ToString(), _token.ExpireTime, _token.IsExpire.ToString()));

            if (_token == null) return string.Empty;

            return CacheManager.UserAccountIsExist(_token.UserID.Trim());
        }

        public string HasLoginedByUserAccount(string token)
        {
            SSOUserToken _token = SSOUserTokenManager.CreateUserTokenInstance(SSOUserTokenManager.UrlReplaceToBase64(token), CrypterKind.SSO);

            WriteLogHelper.WriteLogger(string.Format("{0} 调用了HasLoginedByToken {1},{2}", DateTime.Now.ToString(), _token.ExpireTime, _token.IsExpire.ToString()));

            if (_token == null) return string.Empty;
            string str = CacheManager.UserAccountIsExist(_token.UserID.Trim());
             
            return string.IsNullOrEmpty(str) ? "" : _token.UserID;
        }

        public string Login(string systemCode, string userAccount, string tokenID)
        {
            return CacheManager.SystemLogin(systemCode, userAccount, tokenID);
        }

        public void LoginOut(string systemCode, string userAccount)
        {
            CacheManager.OtherSystemLoginOut(systemCode, userAccount);
        }

        public bool LoginOutByToken(string token)
        {
            SSOUserToken _token = SSOUserTokenManager.CreateUserTokenInstance(SSOUserTokenManager.UrlReplaceToBase64(token), CrypterKind.SSO);

            WriteLogHelper.WriteLogger(string.Format("{0} 调用了LoginOutByToken", DateTime.Now.ToString(), _token.ExpireTime, _token.IsExpire.ToString()));

            if (_token == null) return false;

            return CacheManager.CommonLoginOut(_token.UserID.Trim());
        }
    }
}

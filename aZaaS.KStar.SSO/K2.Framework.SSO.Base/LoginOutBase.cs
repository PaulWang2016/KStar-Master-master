using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;

namespace K2.Framework.SSO.Base
{
    public class LoginOutBase 
    {

        private const string SSOUserToken = "token";

        /// <summary>
        /// 清除token 并退出sso
        /// </summary>
        /// <param name="controller"></param>
        public static void ClearTokenLogout(Controller controller)
        {
            try
            {
                //清空主站令牌 
                string _token = controller.Session["Token"].ToString();
                //创建WebService对象
                K2.Framework.SSO.Base.Passport.TokenServiceClient tokenService = new K2.Framework.SSO.Base.Passport.TokenServiceClient();
                tokenService.LoginOutByToken(_token);
                //清空本地凭证
                controller.Session.Remove("Token");
                controller.Session.Abandon();
                controller.Session.Clear();
                ClearTokenCookie(controller);
                controller.Response.Redirect(getClearTokenURL(controller.Request.Url.AbsoluteUri));
            }
            catch { } 
        }
        /// <summary>
        /// 清除token 并退出sso
        /// </summary>
        /// <param name="controller"></param>
        public static void ClearTokenLogout(Page controller)
        {
            try
            {
                //清空主站令牌 
                string _token = controller.Session["Token"].ToString();
                //创建WebService对象
                K2.Framework.SSO.Base.Passport.TokenServiceClient tokenService = new K2.Framework.SSO.Base.Passport.TokenServiceClient();
                tokenService.LoginOutByToken(_token);
                //清空本地凭证
                controller.Session.Remove("Token");
                controller.Session.Abandon();
                controller.Session.Clear();
                ClearTokenCookie(controller);
                controller.Response.Redirect(getClearTokenURL(controller.Request.Url.AbsoluteUri));
            }
            catch { }
        }

        /// <summary>
        /// 只清除token
        /// </summary>
        /// <param name="controller"></param>
        public static void ClearToken(Controller controller)
        {
            try
            {
                //清空主站令牌 
                string _token = controller.Session["Token"].ToString();
                //创建WebService对象
                K2.Framework.SSO.Base.Passport.TokenServiceClient tokenService = new K2.Framework.SSO.Base.Passport.TokenServiceClient();
                tokenService.LoginOutByToken(_token);
                //清空本地凭证
                controller.Session.Remove("Token");
                controller.Session.Abandon();
                controller.Session.Clear();
                ClearTokenCookie(controller);
                
            }
            catch { }
        }


        /// <summary>
        /// 只清除token
        /// </summary>
        /// <param name="controller"></param>
        public static void ClearToken(Page controller)
        {
            try
            {
                //清空主站令牌 
                string _token = controller.Session["Token"].ToString();
                //创建WebService对象
                K2.Framework.SSO.Base.Passport.TokenServiceClient tokenService = new K2.Framework.SSO.Base.Passport.TokenServiceClient();
                tokenService.LoginOutByToken(_token);
                //清空本地凭证
                controller.Session.Remove("Token");
                controller.Session.Abandon();
                controller.Session.Clear();
                ClearTokenCookie(controller);
            }
            catch { }
        }
         

        /// <summary>
        /// 获取清除主站TOKEN的URL
        /// </summary>
        /// <returns></returns>
        public static string getClearTokenURL(string currentUrl)
        {
            //http:// || https://
            int len = currentUrl.IndexOf("/", 8);
            if (len <= 0)
            {
                return string.Empty;
            }
            string baseUrl = currentUrl.Substring(0, len); 
            string ssoUrl = ConfigurationManager.AppSettings["ssoUrl"]; 
            return string.Format("{0}/ClearCookies.aspx?RequestUrl={1}", ssoUrl, Utilities.Base64ReplaceToUrl(baseUrl));
        }

        public static void ClearTokenCookie(Page controller)
        {
            HttpCookie aCookie = new HttpCookie(LoginOutBase.SSOUserToken);
            aCookie.Expires = DateTime.Now.AddDays(-1); 
            controller.Response.Cookies.Add(aCookie);

        }
        public static void ClearTokenCookie(Controller controller)
        {
            HttpCookie aCookie = new HttpCookie(LoginOutBase.SSOUserToken);
            aCookie.Expires = DateTime.Now.AddDays(-1);
            controller.Response.Cookies.Add(aCookie); 
        }
    }
}
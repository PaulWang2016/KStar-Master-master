using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace K2.Framework.SSO
{
    public class SSOUserTokenManager
    {
        /// <summary>
        ///byte64 加密
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string Base64ReplaceToUrl(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(base64);
            string str = "";
            try
            {
                str = Convert.ToBase64String(bytes);
            }
            catch
            {
                str = base64;
            }
            str = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
            return str; 
           // return base64.Replace("+", "-").Replace("/", "(").Replace("=", ")");


        }


        /// <summary>
        /// byte64 解密
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string UrlReplaceToBase64(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString)) return string.Empty;
            string decode = ""; 
            try
            {
                byte[] bytes = Convert.FromBase64String(urlString);
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = urlString;
            }
            return decode;

           // return urlString.Replace("-", "+").Replace("(", "/").Replace(")", "=");
        }


        /// <summary>
        /// Kind提示
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static string GetKindTipInfo(int kind)
        {
            string rtValue = "";
            switch (kind)
            {
                case 1: rtValue = "AppUserToken无效且没有SSOUserToken参数"; break;
                case 2: rtValue = "SSOUserToken Is null"; break;
                case 3: rtValue = "SSOUserToken.UserID=<空白>"; break;
                case 4: rtValue = "AppUserToken无效，有SSOUserToken,但该Cookie已过期"; break;
                case 5: rtValue = "AppUserToken无效，有SSOUserToken,但根据产生时间计算已超过有效期"; break;
            }
            return rtValue;
        }       


        /// <summary>
        /// 创建SSOUserToken的过期实例
        /// </summary>
        /// <returns></returns>
        public static SSOUserToken CreateInValidUserTokenInstance()
        {
            return new SSOUserToken();
        }

        /// <summary>
        /// 根据密文创建SSOUserToken的实例
        /// </summary>
        /// <param name="eData"></param>
        /// <returns></returns>
        public static SSOUserToken CreateUserTokenInstance(string eData, CrypterKind crypter)
        {
            return new SSOUserToken(eData, crypter);
        }

        /// <summary>
        /// 创建SSOUserToken的实例
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SSOUserToken CreateUserTokenInstance(HttpContext context, CrypterKind crypter)
        {

            if (context.Request.Cookies[SSOConst.SSOUserToken] == null)
                return CreateInValidUserTokenInstance();
            else
                return CreateUserTokenInstance(context.Request.Cookies[SSOConst.SSOUserToken].Values["Value"], crypter);
        }

        public static SSOUserToken CreateUserTokenInstance(HttpContextBase context, CrypterKind crypter)
        { 
            if (context.Request.Cookies[SSOConst.SSOUserToken] == null)
                return CreateInValidUserTokenInstance();
            else
                return CreateUserTokenInstance(context.Request.Cookies[SSOConst.SSOUserToken].Values["Value"], crypter);
        }

 
        /// <summary>
        /// 判断UserToken Cookie是否存在
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>

        public static bool UserTokenExists(Page page)
        {
            return (page.Request.Cookies[SSOConst.SSOUserToken] != null);

        }

        public static string GetUserTokenInfo(Page page)
        {
            if (UserTokenExists(page))
                return page.Request.Cookies[SSOConst.SSOUserToken].Values["Value"];
            else
                return "";
        }

        public static bool UserTokenExists(HttpApplication application)
        {
            return (application.Request.Cookies[SSOConst.SSOUserToken] != null);

        }

        public static string GetUserTokenInfo(HttpApplication application)
        {
            if (UserTokenExists(application))
                return application.Request.Cookies[SSOConst.SSOUserToken].Values["Value"];
            else
                return "";
        }

        /// <summary>
        /// Token立刻过期
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>  
        public static DateTime Expire(Page page, CrypterKind crypter)
        {
            if (UserTokenExists(page) == false)
                return System.DateTime.MinValue;

            SSOUserToken Token = CreateUserTokenInstance(page.Request.Cookies[SSOConst.SSOUserToken].Values["Value"].ToString(), crypter);

            DateTime _expireDatetime = Token.Expire();

            Token.ReBuild(page, Token.UserID);

            return _expireDatetime;
        }


        /// <summary>
        /// Token立刻过期
        /// </summary>
        /// <param name="eData"></param>
        /// <param name="crypter"></param>
        /// <returns></returns>
        public static DateTime Expire(string eData, CrypterKind crypter)
        {
            if (string.IsNullOrEmpty(eData))
                return System.DateTime.MinValue;

            SSOUserToken Token = CreateUserTokenInstance(eData, crypter);

            DateTime _expireDatetime= Token.Expire(); 

            return _expireDatetime;
        }
         
        /// <summary>
        /// 清理Cookie
        /// </summary>
        /// <param name="page"></param>
        /// <param name="crypter"></param>
        /// <returns></returns>
        public static DateTime CookieExpire(Page page, CrypterKind crypter)
        {

            SSOUserToken Token = CreateInValidUserTokenInstance();

            DateTime _expireDatetime = Token.Expire();

            return Token.ExpireBuild(page); ;
        }

        /// <summary>
        /// 清理Cookie
        /// </summary>
        /// <param name="page"></param>
        /// <param name="crypter"></param>
        /// <returns></returns>
        public static DateTime CookieExpire(Controller page, CrypterKind crypter)
        {

            SSOUserToken Token = CreateInValidUserTokenInstance();

            DateTime _expireDatetime = Token.Expire();

            return Token.ExpireBuild(page); ;
        }



        /// <summary>
        /// 更新Token的有效时间
        /// </summary>
        /// <param name="application"></param>
        /// <param name="crypter"></param>
        /// <returns></returns>
        public static DateTime RefreshUserTokenExpireTime(HttpApplication application,string eData, CrypterKind crypter)
        {  
            SSOUserToken Token = CreateUserTokenInstance(eData, crypter);
            Token.RefreshBuild(application);
            return Token.ExpireTime;
        }

        /// <summary>
        /// 更新Token的有效时间
        /// </summary>
        /// <param name="page"></param>
        /// <param name="eData"></param>
        /// <param name="crypter"></param>
        /// <returns></returns>
        public static DateTime RefreshUserTokenExpireTime(Page page, string eData, CrypterKind crypter)
        {
            SSOUserToken Token = CreateUserTokenInstance(eData, crypter);
            Token.RefreshBuild(page);
            return Token.ExpireTime;
        }

    }
}

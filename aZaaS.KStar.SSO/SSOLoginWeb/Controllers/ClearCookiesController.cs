
using K2.Framework.SSO;
using SSOLoginWeb.Manage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSOLoginWeb.Controllers
{
    public class ClearCookiesController : Controller
    {
        // GET: ClearCookies
        public ActionResult Index()
        {
            string backURL = string.Empty ;
            string extraJS = string.Empty;
            HttpCookie _cookie = this.Request.Cookies[SSOConst.SSOUserToken];

            string webSiteLogouts = ConfigurationManager.AppSettings["WebSiteLogouts"] + string.Empty;

            string[] urls = webSiteLogouts.Split(',');
            if (_cookie != null)
            {

                WriteLogHelper.WriteLogger(string.Format("{0} 调用了ClearCookies页面,Cookie:{1},Cookie的有效时间时间:{2}", DateTime.Now.ToString(), SSOUserTokenManager.Base64ReplaceToUrl(_cookie.Values["Value"].ToString()), _cookie.Expires.ToString()));
                DateTime _expireDatetime = SSOUserTokenManager.CookieExpire(this, CrypterKind.SSO);

                WriteLogHelper.WriteLogger(string.Format("{0} 清理Cookie之后,Cookie的有效时间时间:{1}", DateTime.Now.ToString(), _expireDatetime));
            } 
            if (Request.QueryString["RequestUrl"] != null)
            {
                backURL = SSOUserTokenManager.UrlReplaceToBase64(Request.QueryString["RequestUrl"]);

                WriteLogHelper.WriteLogger(string.Format(" {0} 清理Cookie之后跳转的到backURL:{1}", DateTime.Now.ToString(), backURL));


                string js = "";
                extraJS = js;
            } 
            foreach (var url in urls)
            {
                if (!url.Contains(backURL))
                {
                    string scriptItem = " <script type=\"text/javascript\" src=\"{0}\" async=\"async\" defer></script>";
                    extraJS += string.Format(scriptItem, url);
                }
            } 
            ViewBag.backURL = backURL;
            ViewBag.extraJS = extraJS;
            return View();
        } 
    }
}
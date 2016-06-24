using K2.Framework.SSO;
using SSOLoginWeb.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSOLoginWeb.Controllers
{
    public class GetTokenController : Controller
    {
        // GET: GetToken
        public ActionResult Index()
        {
            if (Request.QueryString["RequestUrl"] != null)
            {
                string backURL = SSOUserTokenManager.UrlReplaceToBase64(Request.QueryString["RequestUrl"]);

                //获取Cookie
                HttpCookie tokenCookie = Request.Cookies[SSOConst.SSOUserToken];

                if (tokenCookie != null)
                {
                    string eData = tokenCookie.Values["Value"].ToString();

                    SSOUserToken _token = SSOUserTokenManager.CreateUserTokenInstance(eData, CrypterKind.SSO);

                    WriteLogHelper.WriteLogger(string.Format("{0} 调用了GetToken页面,Cookie:{1},Cookie的有效时间:{2}", DateTime.Now.ToString(),
                    SSOUserTokenManager.Base64ReplaceToUrl(eData), tokenCookie.Expires.ToString()));

                    if (!string.IsNullOrEmpty(_token.UserID.Trim()) && !string.IsNullOrEmpty(CacheManager.UserAccountIsExist(_token.UserID.Trim())))
                    {
                        backURL = backURL.Replace("$token$", SSOUserTokenManager.Base64ReplaceToUrl(eData));

                        WriteLogHelper.WriteLogger(string.Format("{0} 调用了GetToken页面，获取有效的Token:{1}", DateTime.Now.ToString(), SSOUserTokenManager.Base64ReplaceToUrl(eData)));
                    }
                    else
                    {
                        WriteLogHelper.WriteLogger(string.Format("{0} 调用了GetToken页面，无效的Token", DateTime.Now.ToString()));
                    } 
                } 
                Response.Redirect(backURL);
            }

            return View();
        }
    }
}
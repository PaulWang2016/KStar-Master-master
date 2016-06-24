using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.Acs;
using aZaaS.KStar.Facades;
using System.Web.Security;
using aZaaS.KStar.Localization;
using System.Threading;
using System.Globalization;
using System.Web.Configuration;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Web.Confing;
using System.Configuration;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public class HomeController : BaseMvcController
    {
        public ActionResult Index()
        {
            //this.DisablePageCaching();
            Response.CacheControl = "no-cache";
            ViewBag.CurrentUser = this.CurrentUser;

            ViewBag.AuthType = this.AuthType;
            ViewBag.Title = SystemCustomSettingUtilities.GetTitle();
            ViewBag.Footer = SystemCustomSettingUtilities.GetFooter();

            //缓存基础数据过滤条件
            InitDataFilter();

            return View();
        }
      

        public JsonResult LangSet()
        {

            var lang = Request["lang"].ToString();
            if (String.IsNullOrEmpty(lang))
            {
                lang = "en-US";
            }

            if (Response.Cookies.AllKeys.Contains("LANG"))
            {

                Response.Cookies["LANG"].Value = lang;
                Response.Cookies["LANG"].Expires = DateTime.MaxValue;
            }
            else
            {

                Response.Cookies.Add(new HttpCookie("LANG", lang));
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;

            JsonResult result = new JsonResult();
            result.Data = lang;
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.ContentType = "application/json";

            return result;

        }
        public ActionResult Localization()
        {

            var msg = LocalizationResxExtend.GetCommon_MsgInfo("Message");

            ViewBag.Message = msg;
            return View();

        }

        public ActionResult Left(string pane = "Dashboard")
        {
            Response.CacheControl = "no-cache";
            ViewBag.CurrentUser = this.CurrentUser;

            return PartialView("~/Views/Parts/_Left.cshtml", pane);
        }

        public ActionResult Right(string pane = "Dashboard")
        {
            Response.CacheControl = "no-cache";
            ViewBag.CurrentUser = this.CurrentUser;

            return PartialView("~/Views/Parts/_Right.cshtml", pane);
        }


        public ActionResult LeftRight(string pane = "Dashboard")
        {
            Response.CacheControl = "no-cache";
            ViewBag.CurrentUser = this.CurrentUser;

            return PartialView("~/Views/Parts/_LeftRight.cshtml", pane);
        }

        public ActionResult NoPermission()
        {
            return Content("<h1> Sorry,you do not have permission to access. </h1>");
        }

        public ViewResult CustomPersonTool(string id)
        {
            var sessionid = Request.QueryString["SessionId"];
            ViewBag.Type = id;
            ViewBag.SessionId = (sessionid ?? "");
            return View();
        }        
        /// <summary>
        /// 登陆其他用户
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="loginasanotheruser"></param>
        /// <returns></returns>
        public ActionResult Logout(string Source, bool loginasanotheruser = false)
        {
            Response.CacheControl = "no-cache";

            #region _authenticationAttempts
            int _authenticationAttempts = 0;
            if (!string.IsNullOrEmpty(string.Format("{0}", Session["AuthenticationAttempts"])))
            {
                if (int.TryParse(Session["AuthenticationAttempts"].ToString(), out _authenticationAttempts))
                {
                    _authenticationAttempts++;
                }
            }
            #endregion

            string SourceUrl = string.Format("{0}", Request.UrlReferrer == null ? "/" : Request.UrlReferrer.AbsoluteUri);
            Session.Remove("CurrentUser");
            var previousUser = Session["PreviousUser"] != null ? Session["PreviousUser"].ToString() : "";
            Session["PreviousUser"] = string.IsNullOrEmpty(previousUser) ? User.Identity.Name : previousUser;

            if (loginasanotheruser)
            {
                SourceUrl = Source;
                Session["AuthenticationAttempts"] = _authenticationAttempts;
                if (_authenticationAttempts <= 1)
                {
                    return new HttpUnauthorizedResult();
                }
                else if (_authenticationAttempts == 2 && User.Identity.Name.Equals(previousUser))
                {
                    return new HttpUnauthorizedResult();
                }
                else
                {
                    //FormsAuthentication.SignOut();
                    //HttpContext.Session.Abandon();
                    HttpContext.Session.Clear();
                    Session["CurrentUser"] = User.Identity.Name;
                    Session.Remove("IsDisable");

                    return Content(String.Format("<script>window.location='{0}'</script>", SourceUrl));
                }
            }
            return Content(String.Format("<script>window.location='/Home/Logout?loginasanotheruser=true&Source={0}'</script>", SourceUrl));
        }

        public ActionResult SignOut()
        {
            HttpContext.Session.Abandon();
            HttpContext.Session.Clear();
            FormsAuthentication.SignOut();
            return Redirect("/AccessDenied.aspx");
        }


        public ActionResult NoSuchPage()
        {
            return Content("找不到页面.");
        }

        public ActionResult NoAccessAllowed()
        {
            return Content("无权限访问.");
        }

        public ActionResult AppErrors()
        {
            return Content("服务器异常，请联系系统管理员.");
        }

        /// <summary>
        /// Make sure the browser does not cache this page
        /// </summary>
        public void DisablePageCaching()
        {
            Response.Expires = 0;
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        public ActionResult Identity()
        {
            //aZaaS.KStar.EWOModel.aZasFkEntities fk=new EWOModel.aZasFkEntities();
            //ViewBag.Pwd = fk.CommonKey.FirstOrDefault().CommonKey1;
            ViewBag.Pwd = "K2pass!";
            ViewBag.CurrentUser = "K2SQL:" + Url.Encode(this.CurrentUser);
            ViewBag.ReturnUrl = ConfigurationManager.AppSettings["ReturnUrl"].ToString();
            ViewBag.LoginUrl = ConfigurationManager.AppSettings["LoginUrl"].ToString();
            return View();
        }
    }
}

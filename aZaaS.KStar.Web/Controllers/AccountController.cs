using aZaaS.KStar.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Security;
using System.Configuration;
using System.Threading;

using Newtonsoft.Json;
using aZaaS.KStar.Web.TenantDbService;
using aZaaS.KStar.UserManagement;
using aZaaS.KStar.Web.Utilities.OuterNetwork;
using aZaaS.KStar.Web.Utilities.Cache;
using aZaaS.KStar.Web.Utilities.Caching;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Confing;
using aZaaS.KStar.Authentication;

namespace aZaaS.KStar.Web.Controllers
{
    [EnhancedHandleError]
    public class AccountController : Controller
    {
        private const string KEY_SMARTFORM_INTEGRATED = "SmartformIntegrated";
        private const string KEY_K2INDENTITY_LOGINURL = "K2IdentityLoginUrl";
        private const string KEY_K2SMARTFORM_LOGOUTURL = "K2SmartformLogoutUrl";
        private const string SSO_LOGOUTURL = "SSOLogoutUrl";
        private const string KEY_MUTILTENANT = "MutilTenant";
        private static readonly char[] DefaultCharNumeric = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'k', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'S', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            var clientid = Request.QueryString["clientid"];
            var clientSecret = Request.QueryString["clientSecret"];


            if (User.Identity.IsAuthenticated)
            {
                var url = GetReturnUrl(User.Identity.Name, clientid, clientSecret, returnUrl);
                return Redirect(url);
            }
            else
            {
                //if (Request.QueryString["Token"] != null)
                //{

                //    string userAccount = K2.Framework.SSO.Base.SSOMvcBase.GetTokenByUserAccount(this);
                //    if (!string.IsNullOrWhiteSpace(userAccount))
                //    {
                //        if (userAccount.IndexOf(@"\") == -1)
                //        {
                //            userAccount = ConfigurationManager.AppSettings["WindowDomain"].ToString() + @"\" + userAccount;
                //        }
                //        FormsAuthentication.SetAuthCookie(userAccount, false);
                //        return Redirect(returnUrl);
                //    }
                //    else
                //    {
                //        return Redirect(K2.Framework.SSO.Base.SSOMvcBase.ReplaceToken(this));
                //    }

                //}
                //else
                //{
                //    return Redirect(K2.Framework.SSO.Base.SSOMvcBase.GetTokenURL(this));
                //} 
            }

            var loginUrl = GetWebConfigValue(KEY_K2INDENTITY_LOGINURL);
            ViewBag.K2IndentityLoginUrl = loginUrl;
            ViewBag.SmartformIntegrated = SmartformIntegrated ? "true" : "false";
            ViewBag.clientid = clientid;
            ViewBag.clientSecret = clientSecret;
            ViewBag.returnUrl = returnUrl;
            ViewBag.Footer = SystemCustomSettingUtilities.GetFooter();
            return View();
        }

        [HttpGet]
        public ActionResult ValidateCode()
        {
            Random r = new Random();
            //图片对象
            Bitmap bitmap = new Bitmap(165, 35);
            Graphics g = Graphics.FromImage(bitmap);
            //图片背景刷子
            Brush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, 0, 0, bitmap.Size.Width, bitmap.Size.Height);
            //生产干扰线条
            Pen pen = new Pen(new SolidBrush(Color.Purple), 0);
            System.IO.MemoryStream mStream = null;
            try
            {

                Font f = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold, GraphicsUnit.Pixel);
                string str = Randomstring(4);
                int leftX = 10;
                foreach (char ch in str)
                {
                    g.ResetTransform();
                    SizeF sf = g.MeasureString(ch.ToString(), f);

                    PointF pointF = new PointF(leftX, 0);

                    g.ScaleTransform(1.5f, 1.5f);
                    g.DrawString(ch.ToString(), f, new SolidBrush(Color.Blue), pointF, StringFormat.GenericTypographic);
                    leftX += (int)sf.Width;
                }
                if (mStream == null) mStream = new System.IO.MemoryStream();
                mStream = new System.IO.MemoryStream();
                bitmap.Save(mStream, ImageFormat.Gif);

                mStream.Position = 0;
                HttpContext.Session.Add("_CheckCode", str);
                return File(mStream, @"image/Gif");

            }
            catch (Exception e)
            {
                //context.Session["code"] = "";
            }
            finally
            {
                g.Dispose();
                bitmap.Dispose();
            }
            return null;
        }

        [NonAction]
        private string Randomstring(int count)
        {
            string Str = "";
            Random r = new Random();
            for (int i = 0; i < count; i++)
            {
                Str += DefaultCharNumeric[r.Next(DefaultCharNumeric.Length)];
            }
            return Str;
        }

        [NonAction]
        private string GetReturnUrl(string username, string clientid, string clientSecret, string returnUrl)
        {
            if (string.IsNullOrEmpty(clientid) || string.IsNullOrEmpty(clientSecret))
            {
                return "/";
            }

            //var provider = new ConsumerDBProvider(new aZaaS.Framework.Configuration.Configuration());
            //var consumer = provider.QueryConsumer(clientid);

            //if (consumer != null)
            //{
            //    var secret = ConsumerUtility.GenerateSecret(clientid, consumer.CallbackUrl);
            //    if (secret == clientSecret)
            //    {
            //        var tenant = "";
            //        if (Session["TenantCode"] != null)
            //        {
            //            tenant = Session["TenantCode"].ToString();
            //        }

            //        var token = TokenUtility.GenerateToken(clientid, clientSecret, string.Join("\\", tenant, username));
            //        FormsAuthentication.SetAuthCookie(username, false);

            //        return consumer.CallbackUrl + "?returnUrl=" + Server.UrlEncode(returnUrl) + "&token=" + Server.UrlEncode(token);
            //    }
            //}
            return "/";
        }

        [AllowAnonymous]
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            //判断是否是外网
            if (OuterNetworkUtilites.IsOuterNetwork(this.Request.UserHostAddress))
            {
                return OuterNetworkLogin(model);
            }
            else
            {
                //内网没有验证码
                ClearCheckErrorCookie();
            }

            //string account;
            //var mutiltenant = GetWebConfigValue(KEY_MUTILTENANT);
            //if (Convert.ToBoolean(string.IsNullOrEmpty(mutiltenant) ? "false" : mutiltenant))
            //{
            //    if (!SetTenantDB(model.Account, out account))
            //    {
            //        return Json(new { IsAuthenticated = false, Error = "Invalid TenantId!" });
            //    }
            //    model.Account = account;
            //}

            if (Request["Token"] != null)
            {
                XElement root = XElement.Parse(Request["Token"]);
                if (root == null)
                {
                    throw new Exception("非法操作");
                }
                else
                {
                    string[] names = root.Attribute("LoginName").Value.Split('|');
                    string loginName = ConfigurationManager.AppSettings["WindowDomain"].ToString() + @"\" + (names.Length == 3 ? names[2] : names[0]);
                    FormsAuthentication.SetAuthCookie(loginName, false);
                    return Redirect(model.ReturnUrl);
                }
            }
            else
            {
                var resultMessage =string.Empty;
                if (!KStarUserAuthenticator.Authenticate(model.Account, model.Password,out resultMessage))
                    return Json(new { IsAuthenticated = false, Error = string.IsNullOrEmpty(resultMessage) ? "Invalid username or password!" : resultMessage });

                //NOTE：根据不同的Provider进行调整，或者不需要对登录名进行二次加工（取决于登录帐号规范）
                model.Account = KStarUserAuthenticator.LoginName(model.Account);

                var clientid = Request.Form["clientid"];
                var clientSecret = Request.Form["clientSecret"];

                var url = GetReturnUrl(model.Account, clientid, clientSecret, model.ReturnUrl);
                FormsAuthentication.SetAuthCookie(model.Account, false);
            }

            return Json(new { IsAuthenticated = true, HostUrl = Request.Headers["Host"] });
        }

        [NonAction]
        private ActionResult OuterNetworkLogin(LoginModel model)
        {

            LoginCheckCodetEntity entity = new LoginCheckCodetEntity();
            entity.address = this.Request.UserHostAddress;
            entity.userID = model.Account;
            entity.errorCount = 1;
            if (Request.Cookies["ValidateCode"] != null && Request.Cookies["ValidateCode"].Value == "true")
                if ((model.code + string.Empty).Trim().ToLower() != (HttpContext.Session["_CheckCode"] + string.Empty).Trim().ToLower())
                {
                    CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                    return Json(new { IsAuthenticated = false, CheckCodeError = true });
                }

            string account;

            var mutiltenant = GetWebConfigValue(KEY_MUTILTENANT);
            if (Convert.ToBoolean(string.IsNullOrEmpty(mutiltenant) ? "false" : mutiltenant))
            {
                if (!SetTenantDB(model.Account, out account))
                {
                    CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                    return Json(new { IsAuthenticated = false, Error = "Invalid TenantId!" });
                }
                model.Account = account;
            }
            if (!LoginIsValid(model))
            {
                CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                return Json(new { IsAuthenticated = false, Error = "Invalid username or password!" });
            }
            if (model.Account.IndexOf(@"\") == -1)
            {
                model.Account = ConfigurationManager.AppSettings["WindowDomain"].ToString() + @"\" + model.Account;
            }

            var clientid = Request.Form["clientid"];
            var clientSecret = Request.Form["clientSecret"];

            var url = GetReturnUrl(model.Account, clientid, clientSecret, model.ReturnUrl);

            bool isNetwork = OuterNetworkUtilites.IsNetworkRole(model.Account);
            if (!isNetwork)
            {
                CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                return Json(new { IsAuthenticated = false, RoleError = true });
            }
            FormsAuthentication.SetAuthCookie(model.Account, false);

            //登陆成功清除对应错误信息
            CacheManage.ClearLoginCheckCode(entity);
            ClearCheckErrorCookie();
            return Json(new { IsAuthenticated = true, HostUrl = Request.Headers["Host"] });
        }

        [NonAction]
        private void CheckErrorEvent(bool thirdlyError)
        {
            HttpCookie cookie = new HttpCookie("ValidateCode", "true");
            cookie.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Add(cookie);
        }

        private void ClearCheckErrorCookie()
        {
            string cookieName = "ValidateCode";
            int limit = Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                if (cookieName == Request.Cookies[i].Name)
                {
                    HttpCookie aCookie = new HttpCookie(cookieName);
                    aCookie.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(aCookie);

                    break;

                }
            }
        }

        private bool LoginIsValid(LoginModel model)
        {
            if (!ModelState.IsValid)
                return false;

            // var user = new UserBO();
            var neowayUser = new NeowayUserAuthentication();
            try
            {
                var result =
                  neowayUser.IsPass(model.Account, model.Password);

                ////var authResults = Enum.Parse(typeof(aZaaS.Framework.Authentication.VerifyResult), result);

                //if (!authResults.Equals(aZaaS.Framework.Authentication.VerifyResult.Successful))
                //{
                //    ModelState.AddModelError("", result);
                //    return false;
                //}

                return result;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return false;
            }
        }

        [System.Web.Mvc.Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            //K2.Framework.SSO.Base.LoginOutBase.ClearToken(this);
            //var clearTokenUR = K2.Framework.SSO.Base.Utilities.GetClearTokenURL(this.Request.Url.AbsoluteUri);
            var logoutUrl = GetWebConfigValue(KEY_K2SMARTFORM_LOGOUTURL);
            var SmartformIntegrated = Convert.ToBoolean(GetWebConfigValue(KEY_SMARTFORM_INTEGRATED));
            //return Json(new { LogoutUrl = logoutUrl, SFIntegrated = true, ClearTokenUR = clearTokenUR }, JsonRequestBehavior.AllowGet);
            if (SmartformIntegrated)
            {
                return Json(new { LogoutUrl = logoutUrl, SFIntegrated = SmartformIntegrated }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { SFIntegrated = SmartformIntegrated }, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            var neowayUser = new NeowayUserAuthentication();
            // var result = neowayUser.ChangePassword()

            return View();
        }

        [AllowAnonymous]
        //public string Token()
        //{
        //    var clientId = Request.QueryString["clientid"];
        //    var clientSecret = Request.QueryString["clientSecret"];
        //    var token = Request.QueryString["token"];

        //    try
        //    {
        //        var username = TokenUtility.QueryUser(clientId, clientSecret, token);
        //        var user = new { AccountName = username };
        //        var json = JsonConvert.SerializeObject(user);
        //        return json;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        public JsonResult ResetPassword(ResetPasswordModel model)
        {
            string account;

            if (!SetTenantDB(model.Account, out account))
            {
                return Json(new { Status = false });
            }

            model.Account = account;
            var reuslt = ResetPasswordIsValid(model);

            //return Json(new { Status = true });


            return Json(new { Status = reuslt });
        }

        private int ResetPasswordIsValid(ResetPasswordModel model)
        {
            #region 不是基于域修改
            //if (!ModelState.IsValid)
            //    return false;

            //var user = new UserBO();
            //try
            //{
            //    var result = user.ChangePassword(model.Account, model.OldPassword, model.NewPassword);
            //    var authResults = Enum.Parse(typeof(aZaaS.Framework.Authentication.VerifyResult), result);

            //    if (!authResults.Equals(aZaaS.Framework.Authentication.VerifyResult.Successful))
            //    {
            //        ModelState.AddModelError("", result);
            //        return false;
            //    }
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    ModelState.AddModelError("", ex.Message);
            //    return false;
            //}
            #endregion
            var neowayUser = new NeowayUserAuthentication();
            var result = neowayUser.ChangeUserPassword(model.Account, model.OldPassword, model.NewPassword);
            return result;

        }

        private string GetWebConfigValue(string key)
        {
            var setValue = ConfigurationManager.AppSettings[key];

            return string.IsNullOrEmpty(setValue) ? string.Empty : setValue;
        }

        private bool SmartformIntegrated
        {
            get
            {
                var value = ConfigurationManager.AppSettings[KEY_SMARTFORM_INTEGRATED];

                bool smartformIntegrated = false;
                bool.TryParse(value, out smartformIntegrated);

                return smartformIntegrated;
            }
        }

        private bool SetTenantDB(string userName, out string account)
        {
            account = userName;
            var tenantFlags = account.Split('\\');

            if (tenantFlags != null && tenantFlags.Length == 2)
            {
                var tenantToken = tenantFlags.First();
                bool isSuccess = SetTenantConnections(tenantToken);

                if (!isSuccess)
                {
                    return false;
                }

                account = tenantFlags.Last();
            }

            return true;
        }

        private bool SetTenantConnections(string tenantToken)
        {
            TenantDatabaseServiceClient service = new TenantDatabaseServiceClient();
            var aZaaSKStarDB = service.GetConnectionString(tenantToken, SystemCode.KStarWorkSpace);
            var KSTARServiceDB = service.GetConnectionString(tenantToken, SystemCode.KStarService);
            var aZaaSFrameworkDB = service.GetConnectionString(tenantToken, SystemCode.KStarFramework);

            if (aZaaSKStarDB.Length > 0 && KSTARServiceDB.Length > 0 && aZaaSFrameworkDB.Length > 0)
            {
                var connectionSets = new Dictionary<string, string>()
                {
                   { "aZaaSKStar",aZaaSKStarDB},
                   { "KSTARService",KSTARServiceDB},
                   { "aZaaSFramework",aZaaSFrameworkDB}
                };

                Session["_TENANT_CONNSET_"] = connectionSets;
                Session["TenantCode"] = tenantToken;
                System.Web.HttpContext.Current.Session["_TENANT_CONNSET_"] = connectionSets;
                System.Web.HttpContext.Current.Session["TenantCode"] = tenantToken;
                return true;
            }

            return false;
        }
    }


}

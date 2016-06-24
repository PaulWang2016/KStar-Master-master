using K2.Framework.SSO.ISSOLogin;
using SSOLoginWeb.Manage.DbContext;
using SSOLoginWeb.Manage.DbContext.Caching;
using SSOLoginWeb.Manage.Helpers;
using SSOLoginWeb.Manage.OuterNetwork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CustomSSOLogin
{
    public class SSOLoginCheck : ISSOLogin
    {
         public Controller CurrentController = null;
        private SSOLoginCheck() { }

        public SSOLoginCheck(Controller controller) { this.CurrentController = controller; }

    
        public LoginUser SSOLogin( LoginModel model)
        {
            LoginUser _user = new LoginUser();

            //空验证
            if (string.IsNullOrWhiteSpace(model.Account) || string.IsNullOrWhiteSpace(model.Password))
            {

                ActionResult jsonResult =Json(new { IsAuthenticated = false, Error = "Invalid Request!" });

                throw new SSOLoginException(jsonResult);
            }
           

            //判断是否是外网
            if (OuterNetworkUtilites.IsOuterNetwork(CurrentController.Request.UserHostAddress))
            {
                ActionResult jsonResult = OuterNetworkLogin(model, _user);

                if (jsonResult != null)
                {
                    throw new SSOLoginException(jsonResult);
                } 
            }
            else
            {
                //内网没有验证码
                ClearCheckErrorCookie();

                using (KStarDbContext dbContext = new KStarDbContext())
                {
                    //01、验证用户账号是否在kstar 里面存在
                    SSOLoginWeb.Manage.DbContext.EntityObject.User userInfo = dbContext.UserSet.Where(x => x.UserId == model.Account).FirstOrDefault();
                    if (userInfo == null)
                    {
                        ActionResult jsonResult =Json(new { IsAuthenticated = false, Error = "Invalid username or password!" });
                        throw new SSOLoginException(jsonResult);
                    }
                    if (ConfigurationManager.AppSettings["ADCheck"]+""== "true")
                    {
                        //02、验证用户 是否在域里面存在且账号密码正确。
                        DirectoryEntry directoryEntry = ADHelper.GetDirectoryEntryByAccount(model.Account, model.Password);
                        if (directoryEntry == null)
                        { 
                            ActionResult jsonResult = Json(new { IsAuthenticated = false, Error = "Invalid AD username or password!" });
                            throw new SSOLoginException(jsonResult);
                        }
                    }
                    _user.Token = userInfo.SysId.ToString();
                }
            }
            return _user;
        }

        [NonAction]
        private JsonResult OuterNetworkLogin(LoginModel model, LoginUser _user)
        {
            LoginCheckCodetEntity entity = new LoginCheckCodetEntity();
            entity.address = CurrentController.Request.UserHostAddress;
            entity.userID = model.Account;
            entity.errorCount = 1;
            if (CurrentController.Request.Cookies["ValidateCode"] != null && CurrentController.Request.Cookies["ValidateCode"].Value == "true")
                if ((model.Code + string.Empty).Trim().ToLower() != (CurrentController.HttpContext.Session["_CheckCode"] + string.Empty).Trim().ToLower())
                {
                    CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                    ActionResult jsonResult =Json(new { IsAuthenticated = false, CheckCodeError = true });
                
                    throw new SSOLoginException(jsonResult); 
                }

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                //01、验证用户账号是否在kstar 里面存在
                SSOLoginWeb.Manage.DbContext.EntityObject.User userInfo = dbContext.UserSet.Where(x => x.UserId == model.Account).FirstOrDefault();
                if (userInfo == null)
                {
                    CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                    JsonResult s = new JsonResult();
                   
                    ActionResult jsonResult =Json(new { IsAuthenticated = false, Error = "Invalid username or password!" });
                    throw new SSOLoginException(jsonResult); 
                }
                if (ConfigurationManager.AppSettings["ADCheck"].ToString() == "true")
                {
                    //02、验证用户 是否在域里面存在且账号密码正确。
                    DirectoryEntry directoryEntry = ADHelper.GetDirectoryEntryByAccount(model.Account, model.Password);
                    if (directoryEntry == null)
                    {
                        CacheManage.SetLoginCheckCode(entity, CheckErrorEvent);
                        return Json(new { IsAuthenticated = false, Error = "Invalid AD username or password!" });
                    }
                }
                _user.Token = userInfo.SysId.ToString();
            }
            var _Account = model.Account;
            //添加域
            if (model.Account.IndexOf(@"\") == -1)
            {
                _Account = ConfigurationManager.AppSettings["DefaultDomain"].ToString() + @"\" + model.Account;
            }

            bool isNetwork = OuterNetworkUtilites.IsNetworkRole(_Account);
            if (!isNetwork)
            {
                CacheManage.SetLoginCheckCode(entity, CheckErrorEvent); 
                ActionResult jsonResult =Json(new { IsAuthenticated = false, RoleError = true });
                throw new SSOLoginException(jsonResult); 
            }

            //登陆成功清除对应错误信息
            CacheManage.ClearLoginCheckCode(entity);
            ClearCheckErrorCookie();
            return null;
        }

        [NonAction]
        private void CheckErrorEvent(bool thirdlyError)
        {
            HttpCookie cookie = new HttpCookie("ValidateCode", "true");
            cookie.Expires = DateTime.Now.AddMinutes(30);
            CurrentController.Response.Cookies.Add(cookie);
        }

        [NonAction]
        private void ClearCheckErrorCookie()
        {
            string cookieName = "ValidateCode";
            int limit = CurrentController.Request.Cookies.Count;
            for (int i = 0; i < limit; i++)
            {
                if (cookieName == CurrentController.Request.Cookies[i].Name)
                {
                    HttpCookie aCookie = new HttpCookie(cookieName);
                    aCookie.Expires = DateTime.Now.AddDays(-1);
                    CurrentController.Response.Cookies.Add(aCookie);
                    break;
                }
            }
        }

        private JsonResult Json(object data)
        {
            return new JsonResult { Data = data, ContentType = null, ContentEncoding = null, JsonRequestBehavior = JsonRequestBehavior.DenyGet };

        }
    }
}

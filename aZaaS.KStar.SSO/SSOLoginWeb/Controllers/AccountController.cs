
using K2.Framework.SSO;
using K2.Framework.SSO.ISSOLogin;
using SSOLoginWeb.Code;
using SSOLoginWeb.Manage;
using SSOLoginWeb.Manage.DbContext;
using SSOLoginWeb.Manage.DbContext.Caching;
using SSOLoginWeb.Manage.Helpers;
using SSOLoginWeb.Manage.OuterNetwork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSOLoginWeb.Controllers
{
    public class AccountController : Controller
    {
        private static readonly char[] DefaultCharNumeric = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'k', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'S', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        //登陆验证事件
        public static event SSOLoginWeb.WebApiApplication.SSOLoginCheckDelegate LoginCheckEvent;
 
        // GET: LoginPage
        public ActionResult Index()
        {
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

        [ActionName("SSOLogin")]
        public ActionResult SSOLogin(LoginModel model)
        {
            if (LoginCheckEvent == null)
            {
                LoginCheckEvent += new WebApiApplication.SSOLoginCheckDelegate(WebApiApplication.CustomSSOLoginCheck);
            }

            LoginUser _user = null;

            try
            {
                _user = LoginCheckEvent.Invoke(this, model);
            }
            catch (SSOLoginException ex)
            {
                return ex.Result;

            }
            catch (Exception ex)
            {
                return Json(new { IsAuthenticated = false, Error = ex.Message });
            }

            //验证成功
            string returnUrl = "";
            SSOUserToken _token = SSOUserTokenManager.CreateUserTokenInstance(base.HttpContext, CrypterKind.SSO);

            string eData = _token.ReBuild(base.HttpContext, model.Account);

            CacheManager.SystemLogin("OA", model.Account, _user.Token);
            ////写入cookie
            //HttpCookie tokenCookie = new HttpCookie(SSOConst.SSOUserToken);
            //tokenCookie.Values["Value"] = eData;
            //tokenCookie.Expires = DateTime.MaxValue;
            //Response.AppendCookie(tokenCookie);

            string _url = SSOUserTokenManager.UrlReplaceToBase64(Request["RequestUrl"].ToString());

            string fullUrl = _url;
            string token = SSOUserTokenManager.Base64ReplaceToUrl(eData);

            if (_url.Contains("?"))
            {
                fullUrl += "&token=" + token;
            }
            else
            {
                fullUrl += "?token=" + token;
            }

            returnUrl = fullUrl;


            return Json(new { IsAuthenticated = true, ReturnUrl = returnUrl });
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

        [NonAction]
        private void CheckErrorEvent(bool thirdlyError)
        {
            HttpCookie cookie = new HttpCookie("ValidateCode", "true");
            cookie.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Add(cookie);
        }
    } 
}

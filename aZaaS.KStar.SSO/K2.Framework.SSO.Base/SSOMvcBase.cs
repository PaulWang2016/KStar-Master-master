using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace K2.Framework.SSO.Base
{
    public class SSOMvcBase : Controller
    {
        public static readonly string TokenID = "TokenID";

        public static readonly string Token = "Token"; 
        /// <summary>
        /// 获取TokenID 不会主动去验证。
        /// </summary>
        /// <param name="controller"></param>
        public static void GetToken(Controller controller)
        {
            if (controller.Session[SSOWebFormBase.Token] == null)
            {
                if (controller.Request.QueryString["token"] != null)
                {
                    if (controller.Request.QueryString["token"] != Utilities.TokenReplace)
                    {
                        string _token = controller.Request.QueryString["token"];
                        //调用WebService获取主站凭证
                        Passport.TokenServiceClient tokenService = new Passport.TokenServiceClient();
                        string _tokenID = tokenService.HasLoginedByToken(_token);
                        if (!string.IsNullOrEmpty(_tokenID))
                        {
                            //Token正确
                            controller.Session[SSOWebFormBase.Token] = _token;

                            //用户的GUID
                            controller.Session[SSOWebFormBase.TokenID] = _tokenID;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取UserAccount 不会主动去验证。
        /// </summary>
        /// <param name="controller"></param>
        public static string GetTokenByUserAccount(Controller controller)
        {
            if (controller.Session[SSOWebFormBase.Token] == null)
            {
                if (controller.Request.QueryString["token"] != null)
                {
                    if (controller.Request.QueryString["token"] != Utilities.TokenReplace)
                    {
                        string _token = controller.Request.QueryString["token"];
                        //调用WebService获取主站凭证
                        Passport.TokenServiceClient tokenService = new Passport.TokenServiceClient();
                        string _tokenID = tokenService.HasLoginedByToken(_token);
                        if (!string.IsNullOrEmpty(_tokenID))
                        {
                            //Token正确
                            controller.Session[SSOWebFormBase.Token] = _token;

                            //用户的GUID
                            controller.Session[SSOWebFormBase.TokenID] = _tokenID;

                            //验证并获取UserAccount
                            return tokenService.HasLoginedByUserAccount(_token);
                        }
                    }
                }
            }
            return null;
        }
         
         
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session[SSOWebFormBase.Token] == null)
            {
                //令牌验证结果
                if (Request.QueryString["token"] != null)
                {
                    if (Request.QueryString["token"] != Utilities.TokenReplace)
                    {
                        //持有令牌
                        string _token = Request.QueryString["token"];
                        //调用WebService获取主站凭证
                        Passport.TokenServiceClient tokenService = new Passport.TokenServiceClient();
                        string _tokenID = tokenService.HasLoginedByToken(_token);
                        if (!string.IsNullOrEmpty(_tokenID))
                        {
                            //Token正确
                            Session[SSOWebFormBase.Token] = _token;

                            //用户的GUID
                            Session[SSOWebFormBase.TokenID] = _tokenID;

                            //该为在LoginPage中处理
                            //根据UID,通知论坛，某用户已登录(通知接收后会自动调回，此时已有session值)
                            //int uid = 516;
                            //filterContext.Result = Redirect(this.getAsyncLoginBBSUrl(uid));
                        }
                        else
                        {
                            //Token错误 
                            filterContext.Result = Redirect(SSOMvcBase.ReplaceToken(this));
                        }
                    }
                    else
                    {
                        //未持有令牌 
                        filterContext.Result = Redirect(SSOMvcBase.ReplaceToken(this));
                    }
                }
                //未进行令牌验证，去主站验证
                else
                {
                    filterContext.Result = Redirect(GetTokenURL(this));
                }
            }


            base.OnActionExecuting(filterContext);

        }

 
         
        /// <summary>
        /// 获取带令牌请求的URL
        /// 在当前URL中附加上令牌请求参数
        /// </summary>
        /// <returns></returns>
        public static string GetTokenURL(Controller controller)
        {
            string url = controller.Request.Url.AbsoluteUri;
            Regex reg = new Regex(@"^.*\?.+=.+$");
            if (reg.IsMatch(url))
                url += "&token=" + Utilities.TokenReplace;
            else
                url += "?token=" + Utilities.TokenReplace;

            string ssoUrl = ConfigurationManager.AppSettings["ssoUrl"];
            return string.Format("{0}/GetToken.aspx?RequestUrl={1}", ssoUrl, Utilities.Base64ReplaceToUrl(url));
        }

        /// <summary>
        /// 去掉URL中的令牌
        /// 在当前URL中去掉令牌参数
        /// </summary>
        /// <returns></returns>
        public static string ReplaceToken(Controller controller)
        {
            string url = controller.Request.Url.AbsoluteUri;
            url = Regex.Replace(url, @"(\?|&)token=.*", "", RegexOptions.IgnoreCase);

            string ssoUrl = ConfigurationManager.AppSettings["ssoUrl"];
            return string.Format("{0}/Account?RequestUrl={1}", ssoUrl, Utilities.Base64ReplaceToUrl(url));
        }

        protected void ClearToken()
        {
            try
            {
                //清空主站令牌 
                string _token = Session[SSOWebFormBase.Token].ToString();
                //创建WebService对象
                Passport.TokenServiceClient tokenService = new Passport.TokenServiceClient();
                tokenService.LoginOutByToken(_token);
                //清空本地凭证
                Session.Remove(SSOWebFormBase.Token);
                Session.Abandon();
                Session.Clear();

                Response.Redirect(LoginOutBase.getClearTokenURL(this.Request.Url.AbsoluteUri));
            }
            catch
            {
            }
        } 
    }
}

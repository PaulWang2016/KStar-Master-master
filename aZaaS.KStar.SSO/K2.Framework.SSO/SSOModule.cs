using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace K2.Framework.SSO
{
    public class SSOModule : System.Web.IHttpModule 
    {
        protected string SSOLoginURL { get { return Config.SSOLoginURL; } }
        protected string SSOLoginADURL { get { return Config.SSOLoginADURL; } }   

        public void Dispose() { }

        public void Init(HttpApplication application)
        {
            application.AcquireRequestState += new EventHandler(application_AcquireRequestState);
        }


        public void WriteLocalTextLog(string message)
        {
            string dir = "d:\\SSOLog\\";
            DirectoryInfo di = new DirectoryInfo(dir);

            if (di.Exists)
            {
                try
                {
                    string filename = dir + "E" + System.DateTime.Now.ToString("yyyyMMdd HHmmss")
                        + " " + System.Guid.NewGuid().ToString() + ".txt";
                    StreamWriter sr = File.CreateText(filename);
                    sr.WriteLine(message); 
                    sr.Close();
                }
                catch (Exception ex)
                {
                    //PageHelper.MessageBox ( "写d:本地日志不成功。"  ) ;            
                }
            }

        }

        /// <summary>
        /// AcquireRequestState事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void application_AcquireRequestState(object sender, EventArgs e)
        { 
            HttpApplication application = (HttpApplication)sender;
            HandleUserState(application);
        }

        /// <summary>
        /// 是否需要检查用户登录状态
        /// 默认所有的apsx页面都需要检查，其它不用
        /// </summary>
        /// <param name="uri">Request.Url</param>
        /// <returns>当前页面是否要检查，True/False</returns>
        protected virtual bool IsPageNeedCheck(Uri uri)
        {

            string url = uri.ToString().Trim().ToUpper();
            bool isAspx = (url.EndsWith(".ASPX") || url.IndexOf(".ASPX?") > 0);   //是否是aspx页面
            if (isAspx == false)
            {
                return false;
            }

            return true;
        }

             

        /// <summary>
        /// 传给SSO Web 的url参数
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public string ToSSOPam(int kind)
        {
            return "?Kind=" + SSOUserTokenManager.Base64ReplaceToUrl(kind.ToString())
                + "&Time=" + SSOUserTokenManager.Base64ReplaceToUrl(System.DateTime.Now.ToString("HHmmss"))
                + "&RequestUrl=" +SSOUserTokenManager.Base64ReplaceToUrl(HttpContext.Current.Request.Url.ToString());
        }


        /// <summary>
        /// 根据url是否有LoginWithCert=1参数来控制进入单点登录时进入什么登录页面
        /// </summary>
        private string SSOLoginPageUrl
        {
            get
            {
                if (HttpContext.Current.Request.Url.ToString().ToUpper().IndexOf("LOGINWITHAD=1") > 0)  //用AD登录
                {
                    return this.SSOLoginADURL;
                }
                else    //普通登录
                {
                    return this.SSOLoginURL;
                }
            }
        }

        /// <summary>
        /// 当存在SSO Token时 进行处理
        /// </summary>
        /// <param name="application"></param>
        /// <param name="eData"></param>
        public void DoWith(HttpApplication application, string eData,int type)
        {
            SSOUserToken token = SSOUserTokenManager.CreateUserTokenInstance(eData, CrypterKind.SSO);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("密文：" + eData);
            sb.Append(";");

            sb.Append("SSOUserToken有效:");
            sb.Append("userID=" + token.UserID);
            sb.Append(";ExpireTime=" + token.ExpireTime.ToString());
            sb.Append(";CreateTime=" + token.CreateTime.ToString());
             

            string loginPage = this.SSOLoginPageUrl;

            if (token == null)
            {  
                HttpContext.Current.Response.Redirect(loginPage + this.ToSSOPam(2), true);
            }
            else if (string.IsNullOrEmpty(token.UserID.Trim()))
            { 
                HttpContext.Current.Response.Redirect(loginPage + this.ToSSOPam(3), true);
                return;
            }
            else if (token.IsExpire)                    //如果SSOUserToken过期重登录
            {  
                HttpContext.Current.Response.Redirect(loginPage + this.ToSSOPam(4), true);
                return;
            }
            else if (type == 2 && token.IsOverByCreateTime)   //重播
            {
                HttpContext.Current.Response.Redirect(loginPage + this.ToSSOPam(5), true);
                return;
            }
            else   //有效，登录成功
            {
                SSOUserTokenManager.RefreshUserTokenExpireTime(application, eData, CrypterKind.SSO);

                //记载AD登录验证
                if (HttpContext.Current.Request.QueryString["ADSN"] != null
                    && HttpContext.Current.Request.Url.ToString().ToLower().IndexOf("adchecked=1") > 0)
                {
                    HttpContext.Current.Session["ADSN"] = HttpContext.Current.Request.QueryString["ADSN"].ToString();
                }
            }
        }      
             
        protected virtual void HandleUserState(HttpApplication application)
        {
            //如果不是aspx页面，直接返回						
            if (IsPageNeedCheck(HttpContext.Current.Request.Url) == false)
            {
                return;
            }


            if (SSOUserTokenManager.UserTokenExists(application))
            {
                string eData = SSOUserTokenManager.GetUserTokenInfo(application);

                if (!string.IsNullOrEmpty(eData))
                { 
                    this.DoWith(application, eData,1);
                }
            }
            else if (application.Request["token"] != null && !string.IsNullOrEmpty(application.Request["token"].ToString()))
            {
                string eData = SSOUserTokenManager.UrlReplaceToBase64(application.Request["token"].ToString());
                if (!string.IsNullOrEmpty(eData))
                {
                    this.DoWith(application, eData,2);
                }
            }
            else
            {
                string toUrl = this.SSOLoginPageUrl + this.ToSSOPam(1);
                HttpContext.Current.Response.Redirect(toUrl, true);
                return;
            } 
        }
           
    }
}

using aZaaS.Framework.Workflow;
using aZaaS.KStar.Helper;
using aZaaS.KStar.MgmtDtos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace aZaaS.KStar.Web.Helper
{
    public class HttpHelper
    {
        public static string GetTemplateContent(string url, AuthenticationType authType)
        {
            string pageHtml = string.Empty;            
            string requestHost = "";
            string httpRequestUser = ConfigurationManager.AppSettings["HttpRequestUser"];
            string httpRequestPassword = ConfigurationManager.AppSettings["HttpRequestPassword"];
            if (url.ToLower().StartsWith("http"))
            {
                string tempurl = url.Remove(0, 7);
                requestHost = tempurl.Split('/')[0];
                requestHost = "http://" + requestHost;
            }
            else
            {
                url = "http://" + HttpContext.Current.Request.Headers["Host"] + url;
                requestHost = "http://" + HttpContext.Current.Request.Headers["Host"];
            }
            CookieContainer cc = new CookieContainer();
            string postData ="";
            try
            {
                if (authType == AuthenticationType.Form)
                {
                    postData = "account=" + httpRequestUser + "&password=" + httpRequestPassword;
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    HttpWebRequest weblogin = (HttpWebRequest)WebRequest.Create(new Uri(requestHost+"/Account/Login"));
                    weblogin.CookieContainer = cc;
                    weblogin.Method = "POST";
                    weblogin.ContentType = "application/x-www-form-urlencoded";
                    weblogin.ContentLength = byteArray.Length;
                    Stream streamlogin = weblogin.GetRequestStream();
                    streamlogin.Write(byteArray, 0, byteArray.Length);
                    streamlogin.Close();
                    HttpWebResponse responselogin = (HttpWebResponse)weblogin.GetResponse();

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    webRequest.CookieContainer = cc;
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    pageHtml = sr.ReadToEnd();
                }
                else
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    webRequest.CookieContainer = cc;
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.ContentLength = 0;
                    webRequest.PreAuthenticate = true;
                    NetworkCredential netCreden = new NetworkCredential(httpRequestUser, httpRequestPassword); //登入的帳號密碼
                    webRequest.Credentials = netCreden.GetCredential(new Uri(url), "authType");
                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    pageHtml = sr.ReadToEnd();
                }
            }
            catch (Exception ex){
                LogRequestDto logentity = new LogRequestDto("GetTemplateContent", HttpContext.Current.Request.Url.AbsoluteUri, "GET", "", "Error", ex.Message, CustomHelper.GetClientAddress(), HttpContext.Current.User.Identity.Name, DateTime.Now);
                LogHelper.LogInfoMsg(logentity);
            }
            return pageHtml;
        }

        /// <summary>
        /// 给url增加params
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AddOrEditUrlParams(string url, string key, string value)
        {
            StringBuilder _url = new StringBuilder();
            if (url.Contains("?"))
            {
                string[] separateURL = url.Split('?');
                _url.Append(separateURL[0]);
                System.Collections.Specialized.NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(separateURL[1]);
                //不存在则添加
                if (queryString.Get(key) == null)
                {
                    queryString.Add(key, value);
                }
                else
                {
                    queryString[key] = value;
                }
                _url.Append("?" + queryString.ToString());
            }
            else
            {
                _url.Append(url + "?" + key + "=" + value);
            }
            return _url.ToString();
        }

        /// <summary>
        /// 给url增加params
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string QueryString(string url, string key)
        {
            string value = string.Empty;
            if (url.Contains("?"))
            {
                string[] separateURL = url.Split('?');                
                System.Collections.Specialized.NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(separateURL[1]);                
                if (queryString.Get(key) != null)
                {
                    value = queryString[key];
                }                
            }
            return value;
        }
    }
}
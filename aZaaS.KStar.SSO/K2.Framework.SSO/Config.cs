using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace K2.Framework.SSO
{
    public class Config
    {

        #region 通用方法
        public static void MessageBox(string msg)
        {
            try
            {
                HttpContext.Current.Response.Write("<script>window.alert('" + msg + "')</script>");
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        } 

        #endregion 通用方法

        /// <summary>
        /// CookieDomain
        /// </summary>
        public static string CookieDomain
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Application["CookieDomain"] == null)
                    {
                        string domain = System.Configuration.ConfigurationManager.AppSettings["CookieDomain"].ToString();
                        HttpContext.Current.Application["CookieDomain"] = domain;
                    }
                    return HttpContext.Current.Application["CookieDomain"].ToString();
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string SSOLoginURL
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Application["SSOLoginURL"] == null)
                    {
                        string domain = System.Configuration.ConfigurationManager.AppSettings["SSOLoginURL"].ToString();
                        HttpContext.Current.Application["SSOLoginURL"] = domain;
                    }
                    return HttpContext.Current.Application["SSOLoginURL"].ToString();
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string SSOLoginADURL
        {
            get
            {
                try
                {
                    if (HttpContext.Current.Application["SSOLoginADURL"] == null)
                    {
                        string domain = System.Configuration.ConfigurationManager.AppSettings["SSOLoginADURL"].ToString();
                        HttpContext.Current.Application["SSOLoginADURL"] = domain;
                    }
                    return HttpContext.Current.Application["SSOLoginADURL"].ToString();
                }
                catch
                {
                    return "";
                }
            }
        }



        /// <summary>
        /// SSO 所建立的 SSOUserToken Cookie的有效时间 
        /// </summary>
        public static int UserTokenValidPeriod
        {
            get
            {
                string name = "UserTokenValidPeriod";
                try
                {

                    if (HttpContext.Current.Application[name] == null)
                    {
                        HttpContext.Current.Application[name] =
                              int.Parse(System.Configuration.ConfigurationManager.AppSettings[name]);
                    }
                    return int.Parse(HttpContext.Current.Application[name].ToString());
                }
                catch
                {
                    HttpContext.Current.Application[name] = SSOConst.DEFAULT_VALID_PERIOD;
                    return SSOConst.DEFAULT_VALID_PERIOD;
                }
            }

        }

        /// <summary>
        /// 登录通过url返回标识的有效时长（分）
        /// </summary>
        public static int UserTokenCreateTimeValidPeriod
        {
            get
            {
                string name = "UserTokenCreateTimeValidPeriod";
                try
                {
                    if (HttpContext.Current.Application[name] == null)
                    {
                        HttpContext.Current.Application[name] =
                              int.Parse(System.Configuration.ConfigurationManager.AppSettings[name]);
                    }
                    return int.Parse(HttpContext.Current.Application[name].ToString());
                }
                catch
                {
                    HttpContext.Current.Application[name] = SSOConst.DEFAULT_CREATETIME_VALIDPERIOD;

                    return SSOConst.DEFAULT_CREATETIME_VALIDPERIOD;
                }
            }
        }
    }
}

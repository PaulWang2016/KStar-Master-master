using K2.Framework.SSO.ISSOLogin;
using SSOLoginWeb.Code;
using SSOLoginWeb.Code.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SSOLoginWeb
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public delegate LoginUser SSOLoginCheckDelegate(Controller controller, LoginModel model);


        public static SSOLoginCheckDelegate CustomSSOLoginCheck = null;
       
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CustomSSOLoginCheck = new SSOLoginCheckDelegate(WebApiApplication_LoginCheckEvent);
        }

        LoginUser WebApiApplication_LoginCheckEvent(Controller controller, LoginModel model)
        {
            
            string SSOLoginCheckDLL = ConfigurationManager.AppSettings["SSOLoginCheckDLL"] + string.Empty;
            Assembly loginChecAssembly = Assembly.LoadFrom(SSOLoginCheckDLL);

            foreach (var from in loginChecAssembly.GetTypes())
            {
                if (from.GetInterfaces().Contains(typeof(K2.Framework.SSO.ISSOLogin.ISSOLogin)))
                {
                    var plugin = Activator.CreateInstance(from, new Controller[] { controller });
                    MethodInfo method = from.GetMethod("SSOLogin", new Type[] { typeof(LoginModel) });
                    if (method != null)
                    {
                        //
                        object loginUserObject = method.Invoke(plugin, new object[] { model });
                        return (LoginUser)loginUserObject;
                    }
                }
            } 
            var date = new { IsAuthenticated = false, Error = "登陆集成异常！" }; 

            JsonResult jsonResult = new JsonResult { Data = date, ContentType = null, ContentEncoding = null, JsonRequestBehavior = JsonRequestBehavior.DenyGet };

            throw new SSOLoginException(jsonResult);
        }


    }
}

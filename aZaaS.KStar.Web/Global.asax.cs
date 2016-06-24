using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Localization;
using System.Globalization;
using System.Threading;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.Web.Confing;
using System.Web.SessionState;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;

namespace aZaaS.KStar.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //System.Data.Entity.Database.SetInitializer<MasterDbContext>(null);
            //System.Data.Entity.Database.SetInitializer<aZaaS.KStar.Web.Utilities.AMSDbContext>(null);
            //System.Data.Entity.Database.SetInitializer<AMSDbContext>(null);
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FrameworkConfig.RegisterComponents();

         
        }

        public override void Init()
        {
            this.PreRequestHandlerExecute += MvcApplication_PreRequestHandlerExecute;
            base.Init();
        }
        /// <summary>
        /// 开启web api  session
        /// </summary>
        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        protected void Application_End(object sender, EventArgs e)
        {
         
        }

        void MvcApplication_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            var lang = "";
            if (Request.Cookies.AllKeys.Contains("LANG"))
            {
                lang = Request.Cookies["LANG"].Value;
            }

            if (string.IsNullOrEmpty(lang))
            {
                lang = Request.UserLanguages == null ? "zh-CN" : Request.UserLanguages[0];
            }

            lang = ResxService.LangAdaptor(lang);

            Request.Cookies.Add(new HttpCookie("LANG", lang));
            Response.Cookies.Add(new HttpCookie("LANG", lang));

            //CacheHelper.InitUserCache();

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;


        }
 
    }
}
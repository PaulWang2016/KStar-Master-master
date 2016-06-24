using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace aZaaS.KStar.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("fonts/glyphicons-halflings-regular.woff");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new Route("Transfer", new TransferHandler()));

            routes.MapRoute(
                name: "AppErrors",
                url: "AppErrors",
                defaults: new { controller = "Home", action = "AppErrors" },
                namespaces: new string[] { "aZaaS.KStar.Web.Controllers" }
            );
            routes.MapRoute(
                name: "NoSuchPage",
                url: "NoSuchPage",
                defaults: new { controller = "Home", action = "NoSuchPage" },
                namespaces: new string[] { "aZaaS.KStar.Web.Controllers" }
            );
            routes.MapRoute(
                name: "NoAccessAllowed",
                url: "NoAccessAllowed",
                defaults: new { controller = "Home", action = "NoAccessAllowed" },
                namespaces: new string[] { "aZaaS.KStar.Web.Controllers" }
            );
            routes.MapRoute(
                name: "DynamicWidget_Default",
                url: "DynamicWidget/{key}",
                defaults: new { controller = "DynamicWidget", action = "RenderWidget" },
                namespaces: new string[] { "aZaaS.KStar.Web.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "aZaaS.KStar.Web.Controllers" }
            );

          
          

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.KstarMobile
{
    public class KstarMobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "KstarMobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "KstarMobile_home",
                "KstarMobile/{action}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "KstarMobile_item",
                "KstarMobile/{controller}/{action}/{*parms}",
                new { controller = "Home", action = "Index" }
            );

            context.MapRoute(
                "KstarMobile_default",
                "KstarMobile/{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );


        }
    }
}
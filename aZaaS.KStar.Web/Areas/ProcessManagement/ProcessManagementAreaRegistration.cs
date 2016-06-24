using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.ProcessManagement
{
    public class ProcessManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "ProcessManagement"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProcessManagement_home",
                "ProcessManagement/{action}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "ProcessManagement_default",
                "ProcessManagement/{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );
        }
    }
}
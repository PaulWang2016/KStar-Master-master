using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Dashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Dashboard_Home_default",
                "Dashboard/{action}",
                new { controller = "Home", action = "Index" }
            );
            context.MapRoute(
                "Dashboard_default",
                "Dashboard/{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );
        }
    }
}

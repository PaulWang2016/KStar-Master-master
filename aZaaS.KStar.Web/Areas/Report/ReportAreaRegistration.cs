using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Report
{
    public class ReportAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Report";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Report_home",
                "Report/{action}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Report_item",
                "Report/{controller}/{action}/{*parms}",
                new { controller = "Home", action = "Index" }
            );

            context.MapRoute(
                "Report_default",
                "Report/{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );


        }
    }
}

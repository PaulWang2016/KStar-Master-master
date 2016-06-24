using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.WorkDaily
{
    public class WorkDailyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WorkDaily";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WorkDaily_default",
                "WorkDaily/{controller}/{action}/{id}",
                new { controller = "TodayWorkDaily", action = "Index", id = UrlParameter.Optional },
                new string[] { "aZaaS.KStar.Web.Areas.WorkDaily.Controllers" }
            );
        }
    }
}

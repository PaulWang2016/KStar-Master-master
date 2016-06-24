using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.WorkDaily
{
    public class WeeklyNewspapersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WeeklyNewspapers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WeeklyNewspapers_default",
                "WeeklyNewspapers/{controller}/{action}/{id}",
                new { controller = "WeeklyDaily", action = "Index", id = UrlParameter.Optional },
                new string[] { "aZaaS.KStar.Web.Areas.WeeklyNewspapers.Controllers" }
            );
        }
    }
}

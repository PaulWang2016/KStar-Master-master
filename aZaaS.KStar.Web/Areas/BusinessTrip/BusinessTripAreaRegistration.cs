using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.BusinessTrip
{
    public class BusinessTripAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "BusinessTrip";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "BusinessTrip_default",
                "BusinessTrip/{controller}/{action}/{id}",
                new { controller="Travel", action = "Index", id = UrlParameter.Optional },
                new string[]{"aZaaS.KStar.Web.Areas.BusinessTrip.Controllers"}
            );
        }
    }
}

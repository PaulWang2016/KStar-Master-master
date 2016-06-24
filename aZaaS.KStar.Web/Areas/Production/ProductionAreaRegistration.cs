using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Production
{
    public class ProductionAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Production";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Production_default",
                "Production/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

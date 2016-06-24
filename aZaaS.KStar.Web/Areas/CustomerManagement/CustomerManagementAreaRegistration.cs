using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.CustomerManagement
{
    public class CustomerManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CustomerManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CustomerManagement_default",
                "CustomerManagement/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "aZaaS.KStar.Web.Areas.CustomerManagement.Controllers" }
            );
        }
    }
}

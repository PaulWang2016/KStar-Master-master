using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Depository
{
    public class DepositoryAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Depository";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Depository_default",
                "Depository/{controller}/{action}/{id}",
                new { controller="Deposit", action = "Index", id = UrlParameter.Optional },
                new string[] {"aZaaS.KStar.Web.Areas.Depository.Controllers"}
            );
        }
    }
}

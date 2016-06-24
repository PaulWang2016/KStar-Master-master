using System.Web;
using System.Web.Mvc;

namespace aZaaS.KSTAR.MobileServices
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
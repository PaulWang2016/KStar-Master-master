using aZaaS.KStar.Form.Mvc.ActionFilters;
using aZaaS.KStar.Web.ActionFilters;
using System;
using System.Web;
using System.Web.Mvc;
//using aZaaS.KStar.SqlDB;

namespace aZaaS.KStar.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new KstarWebActionFilter());
            //Register KStarForm ActionFilter
            filters.Add(new KStarFormActionFilter());
        }
    }
}
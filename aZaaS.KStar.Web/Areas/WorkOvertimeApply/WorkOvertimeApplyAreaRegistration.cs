using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.WorkOvertimeApply
{
    public class WorkOvertimeApplyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WorkOvertimeApply";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WorkOvertimeApply_default",
                "WorkOvertimeApply/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

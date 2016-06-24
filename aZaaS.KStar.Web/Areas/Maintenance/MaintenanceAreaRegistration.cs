using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance
{
    public class MaintenanceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Maintenance";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Maintenance_WorklistData_default",
                "Maintenance/WorklistData-{key}",
                new { controller = "Home", action = "WorklistData" }
            );
            context.MapRoute(
                "Maintenance_Key_default",
                "Maintenance/{action}-{key}",
                new { controller = "Home", action = "Index" }
            );
            context.MapRoute(
                "Maintenance_ID_default",
                "Maintenance/Document/{ID}",
                new { controller = "Home", action = "Document" }
            );
            context.MapRoute(
                "Maintenance_ResourcePermission_default",
               "Maintenance/EditResourcePermission/{ID}",
                new { controller = "Home", action = "EditResourcePermission" }
            );
            context.MapRoute(
                "Maintenance_Authorization_default",
               "Maintenance/EditAuthorization/{ID}",
                new { controller = "Home", action = "EditAuthorization" }
            );
            context.MapRoute(
                "Maintenance_Home_default",
                "Maintenance/{action}",
                new { controller = "Home", action = "Index" }
            );
            context.MapRoute(
                "Maintenance_default",
                "Maintenance/{controller}/{action}",
                new { controller = "Home", action = "Index" }
            );
        }
    }
}

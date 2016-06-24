using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Security;
using System.Web.SessionState;

using Autofac;
using Autofac.Integration.Wcf;
using aZaaS.Framework.Facade;
using aZaaS.Framework.Data;
using aZaaS.Framework.EntityFramework;
using aZaaS.KStar.CustomRole;
using aZaaS.KStar.Form;

namespace aZaaS.KStar.Wcf
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            InitializeComponents();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
                
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            CustomRoleManager.Current.StopRoleLoader();
            CustomRoleManager.Current.StopRoleWatch();
        }

        private static void InitializeComponents()
        {
            //
            FrameworkInitializer.Initialize(
                "AutofacWebRequest",
                builder =>
                {
                    builder.RegisterType<aZaaS.KStar.Wcf.ProcessService>().AsSelf();
                    builder.RegisterType<aZaaS.KStar.Wcf.TemplateService>().AsSelf();
                    builder.RegisterType<aZaaS.KStar.Wcf.EchoService>().AsSelf();
                    builder.RegisterType<aZaaS.KStar.Wcf.TaskService>().AsSelf();
                    builder.RegisterType<aZaaS.KStar.Wcf.UserProvider>().AsSelf();
                    builder.RegisterType<aZaaS.KStar.Wcf.WorkflowService>().AsSelf();
                    builder.RegisterType<aZaaS.KStar.Wcf.QuartzMailService>().AsSelf();
                },
                container =>
                {
                    AutofacHostFactory.Container = container;                   
                });

            //aZaaS.Framework.Organization.Facade.FrameworkInitializer.Initialize();

            aZaaS.Framework.ServiceContext.ConnectionSetter = () =>
            {
                var connStr = GetConnectionString("aZaaSFramework");

                return (connStr == "aZaaSFramework" ? string.Empty : connStr);
            };

            aZaaS.KStar.Form.Repositories.aZaaSKStarFormContext.ConnectionSetter = () =>
            {
                var connStr = GetConnectionString("aZaaSFramework");

                return connStr;
            };

            aZaaS.KStar.Repositories.KStarDbContext.ConnectionSetter = () =>
            {
                return GetConnectionString("aZaaSKStar");
            };

            aZaaS.KStar.Repositories.KStarFramekWorkDbContext.ConnectionSetter = () =>
            {
                return GetConnectionString("aZaaSFramework");
            };

            aZaaS.Framework.Organization.Facade.FrameworkInitializer.Initialize(
                "AutofacWebRequest",
                builder => {

                    aZaaS.Framework.Organization.ExtendDbConfig.ExtendConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSFramework");

                        return connStr;
                    };

                    aZaaS.Framework.Organization.ExtendDbConfig.AuthConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSKStar");

                        return connStr;
                    };

                    aZaaS.Framework.Organization.OrganizationDbContext.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSFramework");
                        return connStr;
                    };

                    aZaaS.KStar.UserManagement.TenantDbConfig.ExtendConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSFramework");

                        return connStr;
                    };

                    aZaaS.KStar.UserManagement.TenantDbConfig.AuthConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSKStar");

                        return connStr;
                    };

                    builder.RegisterType<EntityFrameworkRepositoryContext>().As<IRepositoryContext>().InstancePerLifetimeScope();
                },
                container =>
                {
                    //AutofacHostFactory.Container = container;                       
                });


            SQLQueryConfiguration.Initialize();
            AutoMapperConfiguration.Initialize();
            ModelMapperConfig.Initialize();

            CustomRoleManager.Current.StartRoleLoader();
            CustomRoleManager.Current.StartRoleWatch();

        }
        private static string GetConnectionString(string key)
        {            
            var connStr = string.Empty;

            if (HttpContext.Current.Session!=null && HttpContext.Current.Session["_TENANT_CONNSET_"] != null)
            {
                var connSets = HttpContext.Current.Session["_TENANT_CONNSET_"] as Dictionary<string, string>;
                connStr = connSets[key];
            }
            else
            {
                connStr = key;
            }
            return connStr;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Autofac;
using Autofac.Integration.Mvc;
using aZaaS.Framework;
using aZaaS.Framework.Data;
using aZaaS.Framework.EntityFramework;
using aZaaS.Framework.Facade;
using aZaaS.KStar;
using aZaaS.KStar.Form;
using aZaaS.KStar.Authentication;


namespace aZaaS.KStar.Web
{
    public class FrameworkConfig
    {
        public static void RegisterComponents()
        {
            FrameworkInitializer.Initialize(
                "AutofacWebRequest",
                builder =>
                {
                    WFServiceConfig.RegisterWorkflowServices(builder);
                    aZaaS.Framework.ServiceContext.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSFramework");

                        return (connStr == "aZaaSFramework" ? string.Empty : connStr);
                    };

                    builder.RegisterType<EntityFrameworkRepositoryContext>().As<IRepositoryContext>().InstancePerHttpRequest();
                },
                container =>
                {
                    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                });


            aZaaS.Framework.Organization.Facade.FrameworkInitializer.Initialize(
                    "AutofacWebRequest",
                builder =>
                {                    
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

                    aZaaS.KStar.Repositories.KStarDbContext.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSKStar");

                        return connStr;
                    };

                    aZaaS.KStar.Repositories.KStarFramekWorkDbContext.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSFramework");
                        
                        return connStr;
                    };

                    aZaaS.KStar.Repositories.KSTARServiceDBContext.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("KSTARService");
                        
                        return connStr;
                    };

                    aZaaS.KStar.Acs.AcsManager.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSKStar");

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

                    aZaaS.KStar.Form.Repositories.aZaaSKStarFormContext.ConnectionSetter = () =>
                    {
                        var connStr = GetConnectionString("aZaaSFramework");

                        return connStr;
                    };

                    builder.RegisterType<EntityFrameworkRepositoryContext>().As<IRepositoryContext>().InstancePerHttpRequest();
                },
                container =>
                {
                    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                });

            AutoMapperConfiguration.Initialize();
            ModelMapperConfig.Initialize();

            SQLQueryConfiguration.Initialize();
            KStarUserAuthenticator.Initialize();
        }

        private static string GetConnectionString(string key)
        {
            var connStr = string.Empty;

            if (HttpContext.Current != null
                && HttpContext.Current.Session!=null 
                && HttpContext.Current.Session["_TENANT_CONNSET_"] != null)
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
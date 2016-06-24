using aZaaS.Framework;
using aZaaS.Framework.Organization;
using aZaaS.KStar;
using aZaaS.KStar.Acs;
using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSync
{
    public static class Framework
    {
        public static void Initialize()
        {
            ServiceContext.ConnectionSetter = (() => "aZaaSFramework");
            ExtendDbConfig.ExtendConnectionSetter = (() => "aZaaSFramework");
            ExtendDbConfig.AuthConnectionSetter = (() => "aZaaSKStar");
            OrganizationDbContext.ConnectionSetter = (() => "aZaaSFramework");
            KStarDbContext.ConnectionSetter = (() => "aZaaSKStar");
            KStarFramekWorkDbContext.ConnectionSetter = (() => "aZaaSFramework");
            KSTARServiceDBContext.ConnectionSetter = (() => "KSTARService");
            AcsManager.ConnectionSetter = (() => "aZaaSKStar");
            aZaaS.Framework.Facade.FrameworkInitializer.Initialize();
            aZaaS.Framework.Organization.Facade.FrameworkInitializer.Initialize();
            AutoMapperConfiguration.Initialize();
        }
    }
}

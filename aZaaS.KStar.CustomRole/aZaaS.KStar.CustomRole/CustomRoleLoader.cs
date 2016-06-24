using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using aZaaS.KStar;
using aZaaS.Framework.Facade;

namespace aZaaS.KStar.CustomRole
{
    public class CustomRoleLoader : MarshalByRefObject
    {
        public CompositionContainer PartContainer { get; private set; }
        public DirectoryCatalog AssemblyCatalog { get; private set; }

        [ImportMany(typeof(ICustomRole), AllowRecomposition = true)]
        public IEnumerable<ICustomRole> AttachedCustomRoles { get; private set; }

        public CustomRoleLoader()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(CustomRoleLoader).Assembly));

            AssemblyCatalog = new DirectoryCatalog(CustomRoleCfg.AssemblyDirectory);
            catalog.Catalogs.Add(AssemblyCatalog);

            PartContainer = new CompositionContainer(catalog);

            //IMPORTANT:We should init framework environment before compose the parts.
            aZaaS.Framework.ServiceContext.ConnectionSetter = () => "aZaaSFramework";
            aZaaS.Framework.Organization.OrganizationDbContext.ConnectionSetter = () => "aZaaSFramework";
            FrameworkInitializer.Initialize();
            aZaaS.Framework.Organization.Facade.FrameworkInitializer.Initialize();
            AutoMapperConfiguration.Initialize();

            PartContainer.ComposeParts(this);

        }

        public void RefreshRoleParts()
        {
            AssemblyCatalog.Refresh();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}

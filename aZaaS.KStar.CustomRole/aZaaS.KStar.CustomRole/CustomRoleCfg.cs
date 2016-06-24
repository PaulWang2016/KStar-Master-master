using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole
{
    public static class CustomRoleCfg
    {
        public const string CUSTOMROLE_FOLDER = "CustomRoles";
        public const string CUSTOMROLE_CACHE_FOLDER = "__CRCACHE__";
        public const string CUSTOMROLE_APPNAME = "aZaaS.KStar.CustomRole.Host";
        public const string CUSTOMROLE_DOMAINNAME = "aZaaS.KStar.CustomRole.Host.AppDomain";

        private static string _assemblyDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CUSTOMROLE_FOLDER);
        private static string _assemblyCacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CUSTOMROLE_CACHE_FOLDER);

        public static string AssemblyDirectory
        {
            get
            {
                return _assemblyDirectory;
            }
        }

        public static string AssemblyCacheDirectory
        {
            get
            {
                return _assemblyCacheDirectory;
            }
        }
    }
}

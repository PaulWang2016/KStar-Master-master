using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.Framework.Configuration;

namespace aZaaS.KStar.UserManagement
{
    public class TenantDbConfig : IConfiguration
    {
        public static Func<string> ExtendConnectionSetter;
        public static Func<string> AuthConnectionSetter;

        public string GetConfigValue(string key)
        {
            if (key == "FxDB_Extend")
            {
                return ExtendConnectionSetter();
            }
            else
            {
                return AuthConnectionSetter();
            }
        }

        public string GetConnectionString(string key)
        {
            if (key == "FxDB_Extend")
            {
                return ExtendConnectionSetter();
            }
            else
            {
                return AuthConnectionSetter();
            }
        }
    }
}

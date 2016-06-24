using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UserSync.Models;
using LinqToLdap;
using System.Configuration;

namespace UserSync
{
    public static class SimpleLDAPClient
    {
        public static IDirectoryContext Connect(LDAPSetting setting)
        {
            var config = new LdapConfiguration()
                         .MaxPageSizeIs(1000);

            //Mappings
            config.AddMapping(new UserMapping(setting.NamingContext));

            int port = 0;
            int.TryParse(ConfigurationManager.AppSettings["LDAPPort"], out port);

            //configure connecting to the directory
            config.ConfigureFactory(setting.Server)
                .AuthenticateBy(AuthType.Negotiate)
                .AuthenticateAs(new NetworkCredential(setting.UserName, setting.Password))
                .ProtocolVersion(3)
                .UsePort(port);

            return new DirectoryContext(config);
        }

    }
}

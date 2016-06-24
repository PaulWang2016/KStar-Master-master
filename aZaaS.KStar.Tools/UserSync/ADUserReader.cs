using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UserSync.Models;

namespace UserSync
{
    public static class ADUserReader
    {
        public static HashSet<User> GetUsers(IList<LDAPSetting> serverSets)
        {
            var allUsers = new HashSet<User>();

            Logging log = new Logging();
           
            log.ProcessLog("Connecting to LDAP server ...");
            try
            {
                serverSets.ToList().ForEach(setting =>
                {
                    using (var context = SimpleLDAPClient.Connect(setting))
                    {
                        log.ProcessLog(string.Format("Fetching AD users from server:{0} ...", setting.Server));
                        log.ProcessLog(string.Format("Target naming context is {0}", setting.NamingContext));

                        var users = context.Query<User>().ToList();
                        users.ForEach(user => allUsers.Add(user));
                    }
                });
            }
            catch (Exception ex)
            {
                log.ProcessLog(string.Format("** ADUserReader Exception ** {0}", ex.Message));
            }

            return allUsers;
        }
    }
}

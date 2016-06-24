using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserSync
{
    public class LDAPSetting
    {
        public string Server { get; set; }
        public string NamingContext { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public LDAPSetting(string server,string namingContext,string userName,string password)
        {
            Server = server;
            NamingContext = namingContext;
            UserName = userName;
            Password = password;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Authentication
{
    public class UserNotExistsException : AuthenticateException
    {
        public string UserName { get; set; }

        public UserNotExistsException(string userName)
        {
            UserName = userName;
        }
    }
}

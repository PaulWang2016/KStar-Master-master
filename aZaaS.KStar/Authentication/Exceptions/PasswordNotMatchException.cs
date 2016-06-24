using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Authentication
{
    public class PasswordNotMatchException : AuthenticateException
    {
        public string UserName { get; set; }

        public PasswordNotMatchException(string userName)
        {
            UserName = userName;
        }
    }
}

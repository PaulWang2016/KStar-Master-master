using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Wcf
{
    public static class StringUtil
    {
        public static string TrimUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return string.Empty;

            string[] rest = userName.Split(':');
            return rest.Length > 1 ? rest[1] : userName;
        }
    }
}
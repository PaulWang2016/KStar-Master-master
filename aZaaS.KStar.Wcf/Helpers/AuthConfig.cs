using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Wcf
{
    public static class AuthConfig
    {
        private const string AUTHTYPE_CONFIG = "AuthType";

        public static AuthenticationType AuthType
        {
            get
            {
                var authType = ConfigurationManager.AppSettings[AUTHTYPE_CONFIG];

                if (string.IsNullOrEmpty(authType))
                    return AuthenticationType.Windows;

               return (AuthenticationType)Enum.Parse(typeof(AuthenticationType), authType);
            }
        }
    }
}
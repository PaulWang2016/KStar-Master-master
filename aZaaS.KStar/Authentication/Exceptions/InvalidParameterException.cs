using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Authentication
{
    public class InvalidParameterException : AuthenticateException
    {
        public string ParameterName { get; set; }

        public InvalidParameterException(string parameterName)
        {
            ParameterName = parameterName;
        }
    }
}

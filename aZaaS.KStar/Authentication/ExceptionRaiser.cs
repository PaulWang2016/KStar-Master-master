using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Authentication
{
    public static class ExceptionRaiser
    {


        public static void UserNotExists(string userName)
        {
            throw new UserNotExistsException(userName);
        }

        public static void InvalidParameter(string parameterName)
        {
            throw new InvalidParameterException(parameterName);
        }

        public static void PasswordNotMatch(string userName)
        {
            throw new PasswordNotMatchException(userName);
        }


    }
}

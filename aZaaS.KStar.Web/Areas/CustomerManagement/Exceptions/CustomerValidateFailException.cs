using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Exceptions
{
    public class CustomerValidateFailException : ApplicationException
    {
        public CustomerValidateFailException(string message)
            : base(message)
        {
        }
    }
}
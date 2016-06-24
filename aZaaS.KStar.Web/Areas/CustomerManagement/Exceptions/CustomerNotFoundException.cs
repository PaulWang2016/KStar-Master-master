using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Exceptions
{
    public class CustomerNotFoundException : ApplicationException
    {
        public CustomerNotFoundException(string message)
            : base(message)
        {

        }

    }
}
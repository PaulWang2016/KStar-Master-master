using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Models
{
    public class CustomerSearchCriteria : GridViewCriteria<CustomerModel>
    {
        public string City { get; set; }
        public string Country { get; set; }
    }
}
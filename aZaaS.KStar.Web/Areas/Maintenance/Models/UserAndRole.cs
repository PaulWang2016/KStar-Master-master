using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Maintenance.Models
{
    public class UserAndRole
    {
        public Guid Id { get; set; }
        public string Displayname { get; set; }
        public string Type { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.Maintenance.Models
{
    public class AuthorizationMatrices
    {
        public Guid SysId { get; set; }
        public Guid AuthorityId { get; set; }
        public string UserOrRole { get; set; }
        public Guid ResourcePermissionID { get; set; }
        public string Resource { get; set; }
        public string ResourceType { get; set; }
        public string Permission { get; set; }
        public bool Granted { get; set; }

    }
}
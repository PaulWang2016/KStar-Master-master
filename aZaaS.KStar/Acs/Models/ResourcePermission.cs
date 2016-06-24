using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Acs.Models
{
    public class ResourcePermissionView
    {
        public Guid PermissionSysId { get; set; }
        public Guid SysId { get; set; }
        public string ResourceId { get; set; }
        public string ResourceType { get; set; }
        public string Resource { get; set; }
        public string Permission { get; set; }
    }
}
using System;
using System.Collections.Generic;
using aZaaS.Framework.Organization.OrgChart;
using aZaaS.Framework.Organization.UserManagement;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserDto
    {
        public Guid SysID { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public DateTime? CreateDate { get; set; }
        //public IList<User> ReportTo { get; set; }
        //public IList<Role> Roles { get; set; }
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
        public IList<PositionBaseDto> Positions { get; set; }
        public IList<OrgNodeBaseDto> Nodes { get; set; }
    }
}

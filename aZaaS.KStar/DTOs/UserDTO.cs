using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class UserDTO : AbstractDTO
    {
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
        public IList<UserDTO> ReportTo { get; set; }
        public IList<UserExFieldDTO> ExFields { get; set; }
        public  IList<RoleDTO> Roles { get; set; }
        public  IList<PositionDTO> Positions { get; set; }
        public  IList<OrgNodeDTO> Nodes { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }
    }
}

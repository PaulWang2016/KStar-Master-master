using aZaaS.KStar.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserBaseDto : BaseClassDto
    {
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
        public string FullName
        {
            get
            {
                return CustomHelper.UserNameFormat(this.LastName, this.FirstName, this.UserName);                
            }
        }
    }
}

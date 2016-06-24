using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithUsersDto : OrgNodeBaseDto
    {
        public IList<UserBaseDto> Users { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserWithOwnersDto : UserBaseDto
    {
        public IList<UserBaseDto> ReportTo { get; set; }
    }
}

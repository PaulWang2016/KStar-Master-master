using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionWithUsersDto : PositionBaseDto
    {
        public IList<UserBaseDto> Users { get; set; }
    }
}

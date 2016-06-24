using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserWithPositionsDto : UserBaseDto
    {
        public IList<PositionWithFieldsDto> Positions { get; set; }
    }
}

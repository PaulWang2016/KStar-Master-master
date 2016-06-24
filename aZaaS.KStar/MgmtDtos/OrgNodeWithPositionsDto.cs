using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithPositionsDto : OrgNodeBaseDto
    {
        public IList<PositionBaseDto> Positions { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionWithNodesDto : PositionBaseDto
    {
        public IList<OrgNodeBaseDto> Nodes { get; set; }
    }
}

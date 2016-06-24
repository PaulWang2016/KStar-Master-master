using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgChartWithNodesDto : OrgChartBaseDto
    {
        public IList<OrgNodeBaseDto> Nodes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgChartWithRelationshipsDto : OrgChartBaseDto
    {
        public OrgNodeBaseDto Root { get; set; }
        public IList<OrgNodeBaseDto> Nodes { get; set; }
    }
}

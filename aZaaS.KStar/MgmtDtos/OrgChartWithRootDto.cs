using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgChartWithRootDto : OrgChartBaseDto
    {
        public OrgNodeBaseDto Root { get; set; }
    }
}

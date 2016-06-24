using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithChartParentDto : OrgNodeBaseDto
    {
        public OrgChartBaseDto Chart { get; set; }
        public OrgNodeBaseDto Parent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithChartDto : OrgNodeBaseDto
    {
        public OrgChartBaseDto Chart { get; set; }
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserWithNodesDto : UserBaseDto
    {
        public IList<OrgNodeWithChartDto> Nodes { get; set; }
    }
}

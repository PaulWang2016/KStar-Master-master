using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithChildNodesDto : OrgNodeBaseDto
    {
        public IList<OrgNodeBaseDto> ChildNodes { get; set; }
    }
}

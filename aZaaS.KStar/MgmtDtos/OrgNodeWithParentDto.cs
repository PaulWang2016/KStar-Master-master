using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithParentDto : OrgNodeBaseDto
    {
        public OrgNodeBaseDto Parent { get; set; }
    }
}

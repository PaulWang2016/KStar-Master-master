using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionWithRelationshipsDto : PositionBaseDto
    {
        public IList<UserBaseDto> Users { get; set; }
        public IList<OrgNodeBaseDto> Nodes { get; set; }
        public PositionCategoryBaseDto Category { get; set; }
    }
}

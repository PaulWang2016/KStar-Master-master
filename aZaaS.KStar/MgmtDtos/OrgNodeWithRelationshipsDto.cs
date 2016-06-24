using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithRelationshipsDto : OrgNodeBaseDto
    {
        public OrgChartBaseDto Chart { get; set; }
        public OrgNodeBaseDto Parent { get; set; }
        public IList<OrgNodeBaseDto> ChildNodes { get; set; }
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
        public IList<UserBaseDto> Users { get; set; }
        public IList<PositionBaseDto> Positions { get; set; }
    }
}

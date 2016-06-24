using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserWithRelationshipsDto : UserBaseDto
    {
        public IList<UserBaseDto> ReportTo { get; set; }        
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
        public IList<RoleBaseDto> Roles { get; set; }
        public IList<PositionBaseDto> Positions { get; set; }
        public IList<OrgNodeWithChartDto> Nodes { get; set; }
    }
}

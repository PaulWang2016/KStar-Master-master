using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class RoleWithRelationshipsDto : RoleBaseDto
    {
        public IList<UserBaseDto> Users { get; set; }
        public RoleCategoryBaseDto Category { get; set; }
    }
}

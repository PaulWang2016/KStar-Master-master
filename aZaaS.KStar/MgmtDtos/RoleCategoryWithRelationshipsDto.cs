using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class RoleCategoryWithRelationshipsDto:RoleCategoryBaseDto
    {
        public RoleCategoryBaseDto Parent { get; set; }
        public IList<RoleCategoryBaseDto> ChildCategories { get; set; }
        public IList<RoleBaseDto> Roles { get; set; }
    }
}

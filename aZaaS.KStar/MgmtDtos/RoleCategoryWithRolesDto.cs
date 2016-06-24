using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class RoleCategoryWithRolesDto:RoleCategoryBaseDto
    {
        public IList<RoleBaseDto> Roles { get; set; }
    }
}

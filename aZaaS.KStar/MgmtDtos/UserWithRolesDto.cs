﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserWithRolesDto : UserBaseDto
    {
        public IList<RoleBaseDto> Roles { get; set; }
    }
}

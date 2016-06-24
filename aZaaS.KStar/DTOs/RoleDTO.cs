using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class RoleDTO : AbstractDTO
    {
        public string Name { get; set; }
        public IList<UserDTO> Users { get; set; }

    }
}

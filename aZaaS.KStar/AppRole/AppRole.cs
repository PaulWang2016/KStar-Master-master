using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using aZaaS.KStar.Menus;

namespace aZaaS.KStar.AppRole
{
    public class AppRoleEntity
    {
        public virtual Guid RoleId { get; set; }
        [ForeignKey("Menu")]
        public virtual Guid MenuId { get; set; }

        public virtual MenuEntity Menu { get; set; }
    }
}

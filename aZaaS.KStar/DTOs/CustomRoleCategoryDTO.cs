using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.DTOs
{
    public class CustomRoleCategoryDTO : AbstractDTO
    {
        public string Name { get; set; }
        public string RoleType { get; set; }
        public CustomRoleCategoryDTO Parent { get; set; }
        public IList<CustomRoleCategoryDTO> ChildCategories { get; set; }
    }
}

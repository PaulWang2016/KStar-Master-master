using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class RoleCategoryWithChildCategoriesDto:RoleCategoryBaseDto
    {
        public IList<RoleCategoryBaseDto> ChildCategories { get; set; }
    }
}

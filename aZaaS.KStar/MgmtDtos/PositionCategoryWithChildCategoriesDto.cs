using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionCategoryWithChildCategoriesDto : PositionCategoryBaseDto
    {
        public IList<PositionCategoryBaseDto> ChildCategories { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionCategoryWithRelationshipsDto : PositionCategoryBaseDto
    {
        public PositionCategoryBaseDto Parent { get; set; }
        public IList<PositionCategoryBaseDto> ChildCategories { get; set; }
        public IList<PositionBaseDto> Positions { get; set; }
    }
}

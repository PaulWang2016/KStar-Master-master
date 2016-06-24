using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionCategoryWithParentDto : PositionCategoryBaseDto
    {
        public PositionCategoryBaseDto Parent { get; set; }
    }
}

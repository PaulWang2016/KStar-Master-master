using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionWithCategoryDto : PositionBaseDto
    {
        public PositionCategoryBaseDto Category { get; set; }
    }
}

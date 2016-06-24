using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class PositionCategoryDTO : AbstractDTO
    {
        public string Name { get; set; }
        public PositionCategoryDTO Parent { get; set; }
        public IList<PositionCategoryDTO> ChildCategories { get; set; }
        public IList<PositionDTO> Positions { get; set; }
    }
}

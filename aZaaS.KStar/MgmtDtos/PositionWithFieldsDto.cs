using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class PositionWithFieldsDto:PositionBaseDto
    {
        public PositionCategoryBaseDto Category { get; set; }
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
    }
}

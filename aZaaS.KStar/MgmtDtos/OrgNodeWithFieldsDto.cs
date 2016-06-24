using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class OrgNodeWithFieldsDto : OrgNodeBaseDto
    {
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
    }
}

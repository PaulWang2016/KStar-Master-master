using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class UserWithFieldsDto : UserBaseDto
    {
        public IList<ExtensionFieldDto> ExtendItems { get; set; }
    }
}

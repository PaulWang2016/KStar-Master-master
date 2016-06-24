using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.MgmtDtos
{
    public class ExtensionFieldDto : BaseClassDto
    {                
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public string DisplayName { get; set; }
    }
}

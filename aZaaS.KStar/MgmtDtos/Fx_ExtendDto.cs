using aZaaS.Framework.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class Fx_ExtendDto : BaseClassDto
    {
        public string Name { get; set; }
        public FieldBase[] fields { get; set; }      
    }
}

using aZaaS.Framework.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class FieldExtendBaseDto
    {
        public object DefalutValue { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public ValidatorBase[] Validators { get; set; }
        public string Value { get; set; }
        public string FieldType { get; set; }
        public string[] Source { get; set; } 
    }
}

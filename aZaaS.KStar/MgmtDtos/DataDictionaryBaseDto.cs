using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.MgmtDtos
{
    public class DataDictionaryBaseDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
    }
}

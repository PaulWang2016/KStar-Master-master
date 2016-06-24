using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.DTOs
{
    [Serializable]
    public class DataDictionaryDTO
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public int Order{get;set;}
        public string Remark { get; set; }
        public List<DataDictionaryDTO> Childs { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class DataDictionaryTreeItem
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public int Order { get; set; }
        public string Remark { get; set; }
        public bool HasChildren { get; set; }
    }
}
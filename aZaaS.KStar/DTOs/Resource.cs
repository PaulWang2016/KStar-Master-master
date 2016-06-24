using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs
{
    public class Resource
    {
        public Guid ID { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string Links { get; set; }
        public Guid? ParentID { get; set; }
        public string Target { get; set; }
        public string IconKey { get; set; }
        public string Kind { get; set; }
        public string IconPath { get; set; }
        public string Key { get; set; }
        public string OrderBy { get; set; }
    }
}

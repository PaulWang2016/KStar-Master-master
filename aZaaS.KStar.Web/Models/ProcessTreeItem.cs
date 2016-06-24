using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class ProcessTreeItem
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }        
        public int? ParentID { get; set; }
        public string ChildType { get; set; }
        public int ChildID { get; set; }
        public int OrderNo { get; set; }
        public string ConnectionString { get; set; }
        public string Mapping { get; set; }
        public string WhereString { get; set; }
        public bool HasChildren { get; set; }
    }
}
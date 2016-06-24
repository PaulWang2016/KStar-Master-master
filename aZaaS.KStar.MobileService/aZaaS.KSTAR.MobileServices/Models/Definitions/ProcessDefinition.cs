using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class ProcessDefinition
    {
        [Key]
        public int ID { get; set; }
        public string ProcessFullName { get; set; }
        public int ParentID { get; set; }
        public string ChildType { get; set; }
        public int ChildID { get; set; }
        public int OrderNo { get; set; }
        public string ConnectionString { get; set; }
        public string Mapping { get; set; }
        public string WhereString { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class GroupDefinition
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid? LabelID { get; set; }
        public string Type { get; set; }
        public bool Collapsed { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class ItemDefinition
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid? LabelID { get; set; }
        public bool Visible { get; set; }
        public bool Editable { get; set; }
        public string Format { get; set; }
    }
}
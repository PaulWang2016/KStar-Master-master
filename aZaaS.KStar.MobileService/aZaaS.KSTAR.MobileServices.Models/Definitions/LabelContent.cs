using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class LabelContent
    {
        [Key]
        public int ID { get; set; }
        public Guid LabelID { get; set; }
        public string Language { get; set; }
        public string Content { get; set; }
    }
}
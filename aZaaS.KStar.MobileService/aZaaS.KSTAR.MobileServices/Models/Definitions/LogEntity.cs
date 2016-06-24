using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class LogEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string RequestUrl { get; set; }
        public string Parameters { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
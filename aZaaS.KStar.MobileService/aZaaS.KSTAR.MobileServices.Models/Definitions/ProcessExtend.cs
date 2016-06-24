using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KSTAR.MobileServices.Models.Definitions
{
    public class ProcessExtend
    {
        [Key]
        public string ProcessFullName { get; set; }

        public string ControllerFullName { get; set; }
    }
}

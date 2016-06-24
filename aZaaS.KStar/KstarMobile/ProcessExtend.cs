using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.KstarMobile
{
    public class ProcessExtend
    {
        [Key]
        public string ProcessFullName { get; set; }

        public string ControllerFullName { get; set; }
    }
}

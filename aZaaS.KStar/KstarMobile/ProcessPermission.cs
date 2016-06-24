using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.KstarMobile
{
    public class ProcessPermission
    {
        [Key]
        public int ID { set; get; }

        public string ActivityName { set; get; }

        public string ProcessFullName { set; get; }
    }
}

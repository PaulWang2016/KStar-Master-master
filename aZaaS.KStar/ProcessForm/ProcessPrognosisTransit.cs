using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.ProcessForm
{
    public class ProcessPrognosisTask
    {
        [Key]
        public System.Guid SysID { get; set; }

        [Required]
        public int ProcInstID { get; set; }
    }
}

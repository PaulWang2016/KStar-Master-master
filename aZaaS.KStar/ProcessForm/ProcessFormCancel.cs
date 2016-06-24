using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.ProcessForm
{
    public class ProcessFormCancel
    {
        [Key]
        public Guid Id { get; set; }

        public int ProcInstId { get; set; }

        public int Status { get; set; }
    }
}

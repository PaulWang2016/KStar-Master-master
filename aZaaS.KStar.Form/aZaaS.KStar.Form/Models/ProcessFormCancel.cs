using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ProcessFormCancel
    {
        public Guid Id { get; set; }

        public int ProcInstId { get; set; }

        public int Status { get; set; }
    }
}

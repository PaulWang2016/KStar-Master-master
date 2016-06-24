using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ProcessFormSigner
    {
        public int ProcessInstId { get; set; }
        public string ActivityName { get; set; }
        public string UserName { get; set; }
        public string SignerName { get; set; }
        public DateTime? SignerDate { get; set; }
        public bool IsApproval { get; set; }
        public string ActionName { get; set; }
        public DateTime? ApprovalDate { get; set; }                
    }
}

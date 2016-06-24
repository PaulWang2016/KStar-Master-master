using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ProcessFormCC
    {
        public Guid SysId { get; set; }
        public int FormId { get; set; }
        public string Originator { get; set; }
        public string OriginatorName { get; set; }
        public string Receiver { get; set; }
        public string ReceiverName { get; set; }
        public string FormViewUrl { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool ReceiverStatus { get; set; }
        public string ReviewComment { get; set; }
        public Nullable<System.DateTime> ReceiverDate { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Comment { get; set; }
    }
}

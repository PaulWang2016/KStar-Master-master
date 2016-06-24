using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.DTOs
{
    public class WorkflowTask
    {
        public string ActivityName { get; set; }
        public int  Procinstid { get; set; }
        public string ProcInstNo { get; set; }
        public string ProcSubject { get; set; }
        public int Priority { get; set; }
        public Byte Status { get; set; }
        public string StatusDesc { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string Originator { get; set; }
        public string PrevApprovers { get; set; }
        public string Folio { get; set; }
        public string FullName { get; set; }
        public string ProcessName { get; set; }
        public string ViewFlowUrl { get; set; }
        public string ViewUrl { get; set; }
        public string Submitter { get; set; }
    }
}

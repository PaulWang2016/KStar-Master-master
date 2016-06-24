using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class FormCCModel
    {
        public Guid SysId { get; set; }
        public string ProcessFolio { get; set; }
        public string ProcInstNo { get; set; }
        public string ProcessName { get; set; }
        public string FormViewUrl { get; set; }
        public string Originator { get; set; }
        public string OriginatorName { get; set; }
        public string Receiver { get; set; }
        public string ReceiverName { get; set; }
        public string Applicant { get; set; }
        public string ApplicantName { get; set; }
        public Nullable<System.DateTime> ApplicationDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public bool ReceiverStatus { get; set; }
        public Nullable<System.DateTime> ReceiverDate { get; set; }
        public string Comment { get; set; }
    }
}

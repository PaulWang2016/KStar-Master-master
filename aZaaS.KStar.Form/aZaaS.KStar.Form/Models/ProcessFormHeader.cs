using System;
using System.Collections.Generic;

namespace aZaaS.KStar.Form.Models
{
    public partial class ProcessFormHeader
    {
        public int FormID { get; set; }
        public string FormSubject { get; set; }
        public Nullable<int> ProcInstID { get; set; }
        public string ProcessFolio { get; set; }
        public Nullable<int> Priority { get; set; }
        public string SubmitterAccount { get; set; }
        public string SubmitterDisplayName { get; set; }
        public Nullable<System.DateTime> SubmitDate { get; set; }
        public string ApplicantAccount { get; set; }
        public string ApplicantDisplayName { get; set; }
        public string ApplicantTelNo { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPositionID { get; set; }
        public string ApplicantPositionName { get; set; }
        public string ApplicantOrgNodeID { get; set; }
        public string ApplicantOrgNodeName { get; set; }
        public string SubmitComment { get; set; }
        public Nullable<bool> IsDraft { get; set; }
        public string DraftUrl { get; set; }
    }
}

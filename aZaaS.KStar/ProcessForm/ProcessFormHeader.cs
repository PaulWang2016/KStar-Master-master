using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.ProcessForm
{
    public class ProcessFormHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FormID { get; set; }

        public string FormSubject { get; set; }

        public int ProcInstID { get; set; }

        public string ProcessFolio { get; set; }

        public int Priority { get; set; }

        public string SubmitterAccount { get; set; }

        public string SubmitterDisplayName { get; set; }

        public DateTime SubmitDate { get; set; }

        public string ApplicantAccount { get; set; }

        public string ApplicantDisplayName { get; set; }

        public string ApplicantTelNo { get; set; }

        public string ApplicantEmail { get; set; }

        public string ApplicantPositionID { get; set; }

        public string ApplicantPositionName { get; set; }

        public string ApplicantOrgNodeID { get; set; }

        public string ApplicantOrgNodeName { get; set; }

        public string SubmitComment { get; set; }

        public string DraftUrl { get; set; }
    }
     

    public class ProcessFormContent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SysId { get; set; }
        public Nullable<int> FormID { get; set; }
        public string JsonData { get; set; }
        public string XmlData { get; set; }
    }
}

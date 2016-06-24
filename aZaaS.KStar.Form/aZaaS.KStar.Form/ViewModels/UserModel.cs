using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.Models;

namespace aZaaS.KStar.Form.ViewModels
{
    public class UserModel
    {
        public string ApplicantAccount { get; set; }
        public string ApplicantDisplayName { get; set; }
        public string ApplicantTelNo { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPositionID { get; set; }
        public string ApplicantPositionName { get; set; }
        public string ApplicantOrgNodeID { get; set; }
        public string ApplicantOrgNodeName { get; set; }

        public IList<ComboxContext> Positions { get; set; }
        public IList<ComboxContext> Departments { get; set; }
    }
}

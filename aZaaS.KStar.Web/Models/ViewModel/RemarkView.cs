using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class RemarkView
    {
        public DateTime IssuingInstructionDate { get; set; }
        public string AmountArrears { get; set; }

        public DateTime StartArrearsDate { get; set; }
        public DateTime EndArrearsDate { get; set; }

        public string FirstAttemptDate { get; set; }
        public string SecondAttemptDate { get; set; }
        public string ThirdAttemptDate { get; set; }

        public bool IsSeizure { get; set; }

        public string SettlingDate { get; set; }

        public Decimal LegalCost { get; set; }

        public bool IsFree { get; set; }

        public DateTime CostRecoveryDate { get; set; }

        public string Remark { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
    public class ProcDealDurationItemDTO
    {
        public string ActivityName { get; set; }

        public DateTime Arrivewith { get; set; }

        public DateTime Submitwith { get; set; }

        public int TotalConsumingSecond { get; set; }

        public string TotalConsumingSecondStr { get; set; }
    }
}

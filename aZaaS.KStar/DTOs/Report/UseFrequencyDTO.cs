using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DTOs.Report
{
    public class UseFrequencyDTO
    {
        public string ProcessName { get; set; }

        public int UseCount { get; set; }

        public string Avg_Consuming_SecondStr { get; set; }

        public int Avg_Consuming_Second { get; set; }

        public int UseFrequency { get; set; }

        public string FrequencyType { get; set; }

        public Int64 RowNumber { get; set; }
    }
}

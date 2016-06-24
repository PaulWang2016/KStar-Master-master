using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Report
{
    public class FeedbackEntity
    {

        public Guid ID { get; set; }
        public DateTime CommitDate { get; set; }
        public Guid UserId { get; set; }
        public Int32 Rate { get; set; }
        public string Comment { get; set; }
        public Guid ReportInfoID { get; set; }
    }
}

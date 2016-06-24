using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Report
{
    public class ReportStatistics
    {
        [Key]
        public Guid ID { set; get; }

        public Guid ReportID { set; get; }

        public Guid SysID { set; get; }
        [MaxLength(500)]
        public String UserId { set; get; }
        [MaxLength(500)]
        public String FirstName { set; get; }
        [MaxLength(500)]
        public String UserHostAddress { set; get; }

        [MaxLength(500)]
        public String Browser { set; get; }

        public DateTime CreateTime { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Report
{
    public class ReportPermission
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ReportID { get; set; }
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleType { get; set; }
    }
}

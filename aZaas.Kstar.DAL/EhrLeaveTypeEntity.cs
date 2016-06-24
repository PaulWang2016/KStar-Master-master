using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaas.Kstar.DAL
{
    public class EhrLeaveTypeEntity
    {
        public string LeaveType { get; set; }

        public bool IsAudit { get; set; }

        public bool IsContinuous { get; set; }

        public bool IsOnlyRestOneTime { get; set; }

        public int MaxLeaveCount { get; set; }

        public double MinLeaveTime { get; set; }

        public string Caption { get; set; }
    }
}

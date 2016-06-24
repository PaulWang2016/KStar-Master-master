using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaas.Kstar.DAL
{
    public class EhrShiftEntity
    {
        /// <summary>
        /// 班次类型
        /// </summary>
        public string ClassType { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        public Nullable<DateTime> Start { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        public Nullable<DateTime> End { get; set; }

        /// <summary>
        /// 休息开始时间
        /// </summary>
        public Nullable<DateTime> RestStart { get; set; }

        /// <summary>
        /// 休息结束时间
        /// </summary>
        public Nullable<DateTime> RestEnd { get; set; }

        /// <summary>
        /// 加班开始时间
        /// </summary>
        public Nullable<DateTime> OvertimeStart { get; set; }

        /// <summary>
        /// 工作时长
        /// </summary>
        public Nullable<double> WorkHour { get; set; }
    }
}

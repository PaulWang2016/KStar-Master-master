using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.WorkOvertimeApply.Models
{
    public class WorkOvertimeModel
    {
        public WorkOvertimeModel()
        {
            Detail = new List<WorkOvertimeDetail>();
        }
        public string BillNO { set; get; }

        public DateTime CreateTime { set; get; }

        public List<WorkOvertimeDetail> Detail { set; get; }
        /// <summary>
        /// 抄送
        /// </summary>
        public SimpleUser CCMan { set; get; }
        
    }

    public class WorkOvertimeDetail
    {
        public string UserId { set; get; } 

        public string RealName { set; get; }

        public string UserName { set; get; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { set; get; }
        /// <summary>
        /// 科室  administrative or technical offices
        /// </summary>
        public string AOTOffices { set; get; }

        public string Level { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { set; get; }

        public DateTime EndDate { set; get; }

        public DateTime BeginTime { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { set; get; }

        public string Description { set; get; }

        public string IsRepeater { set; get; }
      
    }
}
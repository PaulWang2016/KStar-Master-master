using aZaaS.KStar.Web.Models.BasisEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.WeeklyNewspapers.Models
{
    public class WeeklyModel
    {
        public WeeklyModel()
        {
            this.WorkPlanList = new List<WeeklyDailyPlan>();
            this.OtherMessageList = new List<OtherMessage>(); 
        }
        /// <summary>
        /// 表单编号
        /// </summary>
        public int FormNo { get; set; }

        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public SimpleUser UserInfo { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentsFirst { get; set; }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string Departments { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostName { get; set; }
        /// <summary>
        /// 导师
        /// </summary>
        public SimpleUser Advisor { get; set; }
        /// <summary>
        ///审核人
        /// </summary>
        public SimpleUser Verifier { get; set; }
        /// <summary>
        /// 抄送
        /// </summary>
        public SimpleUser CCMan { get; set; }
        public IList<WeeklyDailyPlan> WorkPlanList { get; set; }
        public IList<OtherMessage> OtherMessageList { get; set; }
    }
    public class WeeklyDailyPlan
    {
        public int ID { get; set; }
        /// <summary>
        /// 本日工作计划
        /// </summary>
        public string TodayDaily { get; set; }
        /// <summary>
        /// 本日工作总结
        /// </summary>
        public string TodaySummary { get; set; }
        /// <summary>
        /// 明日工作计划
        /// </summary>
        public string TomorrowDaily { get; set; }
        /// <summary>
        /// 重要等级
        /// </summary>
        public string TaskLevel { get; set; }
        /// <summary>
        /// 是否关联
        /// </summary>
        public string IsRelate { get; set; }
 
    }
    /// <summary>
    /// 其他消息
    /// </summary>
    public class OtherMessage
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string WantHelp { get; set; }
    } 
 
}
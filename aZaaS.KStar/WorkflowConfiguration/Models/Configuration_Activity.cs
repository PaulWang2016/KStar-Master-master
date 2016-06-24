using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    /// <summary>
    /// 流程配置的节点
    /// </summary>
    public class Configuration_Activity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// Configuration_ProcessVersion ID
        /// </summary>
        public int Configuration_ProcessVersionID { get; set; }
        /// <summary>
        /// 流程节点ID
        /// </summary>
        public int ActivityID { get; set; }
        /// <summary>
        /// 流程节点名称
        /// </summary>
        [NotMapped]
        public string Name { get; set; }

        /// <summary>
        /// 流程节点处理时效
        /// </summary>
        public int? ProcessTime { get; set; }

        [ForeignKey("Configuration_ActivityID")]
        public virtual List<Configuration_RefActivity> ReworkActivityList { get; set; }

        [ForeignKey("RefID")]
        public virtual List<Configuration_User> OperateUserList { get; set; }
    }    
}

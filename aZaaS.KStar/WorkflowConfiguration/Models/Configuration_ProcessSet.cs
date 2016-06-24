using aZaaS.Framework.UserManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    /// <summary>
    /// 流程配置的流程集
    /// </summary>
    public class Configuration_ProcessSet 
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public int ProcessSetID { get; set; }
        /// <summary>
        /// Configuration_Category ID
        /// </summary>
        public int Configuration_CategoryID { get; set; }
        /// <summary>
        /// 流程集编号
        /// </summary>
        public string ProcessSetNo { get; set; }
        /// <summary>
        /// 流程全名
        /// </summary>
        public string ProcessFullName { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 排序编号
        /// </summary>
        public int OrderNo { get; set; }
        /// <summary>
        /// 发起Url
        /// </summary>
        public string StartUrl { get; set; }
        /// <summary>
        ///查看Url
        /// </summary>
        public string ViewUrl { get; set; }
        /// <summary>
        /// 审批Url
        /// </summary>
        public string ApproveUrl { get; set; }
        /// <summary>
        /// 是否跳过重复审批
        /// </summary>
        public bool NotAssignIfApproved { get; set; }
        /// <summary>
        /// 是否开启流程预判
        /// </summary>
        
        public bool? ProcessPredict { get; set; }
        /// <summary>
        /// 自循环备注
        /// </summary>
        public string LoopRemark { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        [ForeignKey("Configuration_ProcessSetID")]
        public virtual List<Configuration_ProcessVersion> ProcessVersionList { get; set; }
    }
}

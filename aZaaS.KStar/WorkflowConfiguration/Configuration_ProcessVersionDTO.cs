using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    /// 流程配置的流程版本
    /// </summary>
    public class Configuration_ProcessVersionDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Configuration_ProcessSet ID
        /// </summary>
        public int Configuration_ProcessSetID { get; set; }
        /// <summary>
        /// 流程版本ID
        /// </summary>
        public int ProcessVersionID { get; set; }

        /// <summary>
        /// 版本编号
        /// </summary>
        public string VersionNo { get; set; }
        /// <summary>
        /// 部署时间
        /// </summary>
        public DateTime DeployDate { get; set; }
        /// <summary>
        /// 是否当前使用
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 环节列表
        /// </summary>
        public List<Configuration_ActivityDTO> ActivityList { get; set; }
    }
}

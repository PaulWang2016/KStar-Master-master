using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    /// 流程配置的节点
    /// </summary>
    public class Configuration_ActivityDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
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
        /// 流程节点处理时效
        /// </summary>
        public int? ProcessTime { get; set; }
        /// <summary>
        /// 节点编号
        /// </summary>
        public string ActivityNo { get; set; }
        /// <summary>
        /// 元数据
        /// </summary>
        public string MetaData { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 可退回节点
        /// </summary>
        public List<int> ReworkActivityList { get; set; }

        /// <summary>
        /// 审批用户
        /// </summary>
        public List<Configuration_UserDTO> OperateUserList { get; set; }
    }
}

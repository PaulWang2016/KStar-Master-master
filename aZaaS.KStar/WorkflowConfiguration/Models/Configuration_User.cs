using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    /// <summary>
    /// 流程配置的用户
    /// </summary>
    public class Configuration_User
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 流程配置的关联ID(Configuration_ProcessSet ID/Configuration_Activity ID)
        /// </summary>
        public int RefID { get; set; }
        /// <summary>
        /// 流程配置的用户类型
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 流程配置的关联数据类型
        /// </summary>
        public string RefType { get; set; }
        /// <summary>
        /// 流程配置的操作类型
        /// </summary>
        public string OperateType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 值的唯一标识
        /// </summary>
        public string Key { get; set; }
    }
}

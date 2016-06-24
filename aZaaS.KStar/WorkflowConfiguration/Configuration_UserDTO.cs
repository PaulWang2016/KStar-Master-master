
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    /// 流程配置的用户
    /// </summary>
    public class Configuration_UserDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 流程配置的关联ID(Configuration_ProcessSet ID/Configuration_Activity ID)
        /// </summary>
        public int RefID { get; set; }
        /// <summary>
        /// 流程配置的用户类型
        /// </summary>
        public Configuration_UserType UserType { get; set; }
        /// <summary>
        /// 流程配置的关联数据类型
        /// </summary>
        public Configuration_RefType RefType { get; set; }
        /// <summary>
        /// 流程配置的操作类型
        /// </summary>
        public Configuration_OperationType OperateType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 值的唯一标识
        /// </summary>
        public string Key { get; set; }
    }


    public enum Configuration_UserType
    {
        User,
        OrgNode,
        Position,
        CustomType,
        Role
    }

    public enum Configuration_OperationType
    {
        StartProcess,
        OperateProcess,
        EndCc,
        ReworkCc
    }

    public enum Configuration_RefType
    {
        ProcessSet,
        Activity
    }
}

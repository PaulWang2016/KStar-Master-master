using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Wcf.DataContracts
{
    /// <summary>
    /// 流程配置的流程集
    /// </summary>
    public class ProcessSetInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
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
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 是否是常用流程
        /// </summary>
        public bool IsCommon { get; set; } 
    }
}
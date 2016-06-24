using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    /// <summary>
    /// 流程配置的节点与流程节点的关联
    /// </summary>
    public class Configuration_RefActivity
    {
        /// <summary>
        /// Configuration_Activity ID
        /// </summary>
        [Key,Column(Order=0)]
        public int Configuration_ActivityID { get; set; }
        /// <summary>
        /// 节点ID
        /// </summary>
        [Key, Column(Order = 1)]
        public int ActivityID { get; set; }
    }
}

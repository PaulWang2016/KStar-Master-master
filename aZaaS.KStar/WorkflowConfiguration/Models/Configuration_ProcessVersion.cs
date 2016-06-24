using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    /// <summary>
    /// 流程配置的流程版本
    /// </summary>
    public class Configuration_ProcessVersion
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// Configuration_ProcessSet ID
        /// </summary>
        public int Configuration_ProcessSetID { get; set; }
        /// <summary>
        /// 流程版本ID
        /// </summary>
        public int ProcessVersionID { get; set; }

        [ForeignKey("Configuration_ProcessVersionID")]
        public virtual List<Configuration_Activity> ActivityList { get; set; }
    }
}

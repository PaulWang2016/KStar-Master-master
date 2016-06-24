using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.WorkflowConfiguration.Models
{

    public class Configuration_LineRule
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key] 
        public Guid SysID { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        [Required]
        public string FullName { get; set; }

        /// <summary>
        /// 源ActivityName
        /// </summary>
        [Required]
        public string SourceActivityName { get; set; }


        /// <summary>
        /// 规则
        /// </summary>
        public string RuleString { get; set; }


        /// <summary>
        /// 目标ActivityName
        /// </summary>
       [Required]
        public string TargetActivityName { get; set; }
 
    }
}

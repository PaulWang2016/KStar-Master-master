using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    /// <summary>
    /// 用户常用流程
    /// </summary>
    public class Configuration_ProcCommon
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Key, Column(Order = 0)]
        public string UserName { get; set; }
        /// <summary>
        /// 流程配置ID
        /// </summary>
        [Key, Column(Order = 1)]
        public int ConfigProcSetID { get; set; }
    }
}

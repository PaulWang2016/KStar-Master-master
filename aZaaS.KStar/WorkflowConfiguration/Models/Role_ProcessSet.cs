using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    public class Role_ProcessSet
    {
        /// <summary>
        /// 角色guid
        /// </summary>
        [Key, Column(Order = 0)]
        public Guid Role_SysId { get; set; }

        /// <summary>
        /// 流程全称
        /// </summary>
        [Key, Column(Order = 1)]
        public string ProcessFullName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole.Models
{
    public class CustomRoleCategory
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid SysID { get; set; }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 上层ID
        /// </summary>
        public Guid Parent_SysId { get; set; }
    }
}

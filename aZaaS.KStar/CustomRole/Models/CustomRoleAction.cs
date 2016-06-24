using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole.Models
{
    public class CustomRoleAction
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 角色Key
        /// </summary>
        public string RoleKey { get; set; }
        /// <summary>
        /// 上层程序集ID
        /// </summary>
        public Guid Assemble_SysId { get; set; }
    }
}

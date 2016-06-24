using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole.Models
{
    public class CustomRoleClassify
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Guid SysID { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 角色KEY
        /// </summary>
        public Guid RoleKey { get; set; }

        /// <summary>
        /// 类名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 程序集名称
        /// </summary>
        public string AssembleName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 上层ID
        /// </summary>
        public Guid Category_SysId { get; set; }
    }
}

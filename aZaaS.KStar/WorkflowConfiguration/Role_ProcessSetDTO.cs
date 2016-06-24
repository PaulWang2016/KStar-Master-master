using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Workflow.Configuration
{
    public class Role_ProcessSetDTO
    {
        /// <summary>
        /// 角色guid
        /// </summary>
        public Guid Role_SysId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 流程全称
        /// </summary>
        public string ProcessFullName { get; set; }

        /// <summary>
        /// 流程显示名称
        /// </summary>
        public string ProcessDispalyName { get; set; }
    }
}

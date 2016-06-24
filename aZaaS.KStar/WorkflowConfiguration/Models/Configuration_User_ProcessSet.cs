using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Workflow.Configuration.Models
{
    public class Configuration_User_ProcessSet
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 与用户有关的流程k2 id 
        /// </summary>
        public int ProcessSetID { get; set; }
        public string ProcessName { get; set; }
        /// <summary>
        /// 流程全称
        /// </summary>
        public string ProcessFullName { get; set; }
        /// <summary>
        /// 流程版本k2 id  如果是当前用户与流程级相关，则版本id为0
        /// </summary>
        public int ProcessVersionID { get; set; }
        /// <summary>
        /// 流程环节k2 id   如果是当前用户与流程级相关，则环节id为0
        /// </summary>
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        /// <summary>
        /// 用户职位id
        /// </summary>
        public string PositionSysId { get; set; }
    }
}

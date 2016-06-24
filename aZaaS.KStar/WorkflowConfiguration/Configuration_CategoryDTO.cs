using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    /// 流程配置的类别
    /// </summary>
    public class Configuration_CategoryDTO
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 流程集列表
        /// </summary>
        public List<Configuration_ProcessSetDTO> ProcessSetList { get; set; }
    }
}

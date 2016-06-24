using aZaaS.Framework.UserManagement;
using aZaaS.KStar.WorkflowConfiguration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Workflow.Configuration
{
    /// <summary>
    /// 流程配置的流程集
    /// </summary>
    public class Configuration_ProcessConfigDTO:Configuration_ProcessSetDTO
    {
        public string DbConnectionString { get; set; }
        public string DataTable { get; set; }
        public string WhereQuery { get; set; }
        public List<Process_ControlSettingDTO> settings { get; set; }

        public List<Configuration_LineRule> lineRules { get;set; }
    }
}

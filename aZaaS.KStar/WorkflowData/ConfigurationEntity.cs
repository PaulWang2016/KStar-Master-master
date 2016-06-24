using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.WorkflowData
{
    public class ConfigurationEntity
    {
        [Key]
        public string ConfigKey { get; set; }
        public string ConfigValue { get; set; }
    }
}

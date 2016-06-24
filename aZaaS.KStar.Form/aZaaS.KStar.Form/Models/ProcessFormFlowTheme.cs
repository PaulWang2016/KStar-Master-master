
namespace aZaaS.KStar.Form.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
     
    public partial class ProcessFormFlowTheme
    {
        public int ID { get; set; }
        public string ProcessFullName { get; set; }
        public string ModelFullName { get; set; }
        public string RuleString { get; set; }
        public string Name { get; set; }
    }
}

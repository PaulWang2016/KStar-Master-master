using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    [Serializable]
    public class DynamicWidget
    {
        public Guid ID { get; set; }

        public string Key { get; set; }

        public string DisplayName { get; set; }

        public string RazorContent { get; set; }

        public string Description { get; set; }

        public Guid MenuID { get; set; }
    }
}

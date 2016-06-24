using System;
using System.Collections.Generic;

namespace aZaaS.KStar.Form.Models
{
    public partial class ProcessFormContent
    {
        public int SysId { get; set; }
        public Nullable<int> FormID { get; set; }
        public string JsonData { get; set; }
        public string XmlData { get; set; }
    }
}

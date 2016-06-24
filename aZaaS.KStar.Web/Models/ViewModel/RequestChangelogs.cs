using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models.ViewModel
{
    public class RequestChangelogs
    {
        public string FieldName { get; set; }
        public string OriginalValue { get; set; }
        public string NewValue { get; set; }
        public string ModifiedBy { get; set; }
        public string OrderBy { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}
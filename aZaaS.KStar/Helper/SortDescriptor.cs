using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Helper
{
    public class SortDescriptor
    {
        public string field { get; set; }

        // asc or desc
        public string dir { get; set; }
        public string compare { get; set; }
    }
}
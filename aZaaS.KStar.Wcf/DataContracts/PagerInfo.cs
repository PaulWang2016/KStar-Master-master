using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace aZaaS.KStar.Wcf.DataContracts
{
    [DataContract]
    public class PagerInfo
    {
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int PageIndex { get; set; }
        [DataMember]
        public int Total { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace aZaaS.KStar.Wcf
{
    [DataContract]
    public class EmailData
    {
        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Content { get; set; }
    }
}
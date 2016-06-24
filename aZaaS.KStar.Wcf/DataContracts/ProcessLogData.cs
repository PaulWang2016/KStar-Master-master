using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace aZaaS.KStar.Wcf
{
    [DataContract]
    public class ProcessLogData
    {
        [DataMember]
        public int ProcInstID { get; set; }

        [DataMember]
        public string ProcessName { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public string TaskOwner { get; set; }
        
        [DataMember]
        public string TaskOwnerName { get; set; }
        
        [DataMember]
        public string ActionTaker { get; set; }
        
        [DataMember]
        public string ActionTakerName { get; set; }

        [DataMember]
        public string ActivityName { get; set; }

        [DataMember]
        public string ActionName { get; set; }

        [DataMember(IsRequired=false)]
        public string Comment { get; set; }
    }
}
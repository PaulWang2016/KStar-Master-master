using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace aZaaS.KStar.Wcf.DataContracts
{
    [DataContract]
    public class SubmitForm
    {
        public SubmitForm()
        {

        }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Folio { get; set; }

        [DataMember]
        public string Submitter { get; set; }

        [DataMember]
        public string Applicant { get; set; }

        [DataMember]
        public Guid? PositionID { get; set; }

        [DataMember]
        public Guid? DepartmentID { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public string JsonData { get; set; }

    }
}
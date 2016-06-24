using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITemplateService" in both code and config file together.
    [ServiceContract]
    public interface ITemplateService
    {
        [OperationContract]
        EmailData ParseGeneralEmailTemplate(int procInstId, string templateName);

        [OperationContract]
        EmailData ParseParticipantEmailTemplate(string serialNumber, string participant, string templateName);

        [OperationContract]
        EmailData TransformProcessTemplate(int procInstID, string activityName, string templateName);

        [OperationContract]
        EmailData TransformWorklistTemplate(string serialNumber, string participant, string activityName, string templateName);
    }
}

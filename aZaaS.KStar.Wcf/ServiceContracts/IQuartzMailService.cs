using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IQuartzMailService" in both code and config file together.
    [ServiceContract]
    public interface IQuartzMailService
    {
        [OperationContract]
        void SendMail(string recipients,string subject,string message);
    }
}

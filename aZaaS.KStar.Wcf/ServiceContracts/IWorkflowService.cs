using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using aZaaS.KStar.Wcf.DataContracts;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWorkflowService" in both code and config file together.
    [ServiceContract]
    public interface IWorkflowService
    {
        [OperationContract]
        int StartWorkflow(SubmitForm formData, string processName, Dictionary<string, object> datafields);

        [OperationContract]
        int StartProcessInstance(string userName, string processName, string folio, Dictionary<string, object> datafields);


    }
}

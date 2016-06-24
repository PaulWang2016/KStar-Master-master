using aZaaS.KStar.Wcf.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWorkflowSignerService" in both code and config file together.
    [ServiceContract]
    public interface IWorkflowSignerService
    {
        [OperationContract]
        IEnumerable<SignerInfo> GetSigners();

        [OperationContract]
        void UpdateApprovalAction(int processInstId, string signerName, string actionName);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITaskService" in both code and config file together.
    [ServiceContract]
    public interface ITaskService
    {
        [OperationContract]
        IEnumerable<string> GetTaskActions(string userName, string serialNumber);

        [OperationContract]
        void ActionTask(string userName, string serialNumber, string sDatafield, string sActDatafield, string actionName);
    }
}

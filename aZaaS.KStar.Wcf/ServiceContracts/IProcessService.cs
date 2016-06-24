using aZaaS.KStar.Wcf.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IProcessLogService" in both code and config file together.
    [ServiceContract]
    public interface IProcessService
    {
        [OperationContract]
        void RecordProcessLog(int procInstId, string processName, string taskOwner, string actionTaker, string activityName, string actionName, string comment = "", string post = "");

        [OperationContract]
        IEnumerable<ProcessLogData> GetProcessLogList(int procInstId);

        [OperationContract]
        IEnumerable<string> GetActivityParticipants(int procInstId, string activityName);

        [OperationContract]
        IEnumerable<string> GetActivityNotPopParticipants(int procInstId, string activityName);

        [OperationContract]
        bool HasAttachActivityParticipant(int procInstId, string activityName);

        [OperationContract]
        string GetActivityParticipantEmail(string userName);

        [OperationContract]
        string GetActivityParticipantEmails(int procInstId, string activityName);

        [OperationContract]
        string GetActivityParticipantCCEmails(int procInstId, string activityName);

        [OperationContract]
        ProcessSetInfo GetProcessConfigByFullName(string fullname);

        [OperationContract]
        ProcessSetInfo GetProcessConfigByProcInstId(int procInstId);

        [OperationContract]
        void SaveKStarFormEndCCUser(int procInstId, string activityName, string comment);

        [OperationContract]
        void SaveKStarFormReworkCCUser(int procInstId, string activityName, string comment);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procInstId">实例ID</param>
        /// <param name="activityName">当前环节</param>
        /// <param name="comment">动作</param>
        /// <returns></returns>
        [OperationContract]
        bool LienRule(int procInstId, string LienName, string activityName, string action);

        /// <summary>
        /// 流程结束
        /// </summary>
        /// <param name="procInstId">实例ID</param>
        [OperationContract]
        void ProcessFinish(int procInstId);
    }
}

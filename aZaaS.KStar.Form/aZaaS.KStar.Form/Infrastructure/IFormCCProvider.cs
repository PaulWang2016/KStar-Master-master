using aZaaS.KStar.Form.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IFormCCProvider
    {
        void Save(IList<ProcessFormCC> ccList);

        void Save(IList<ProcessFormCC> ccList,string viewAddress);

        void UpdateReceiverStatus(Guid sysId, string comment);

        void UpdateReceiverStatus(int formId, int activityId, string receiver, string comment);

        void Delete(Guid sysId);

        void Delete(int formId, string receiver);

        void DeleteAll(int formId);

        IList<FormCCModel> SendFormCC(string userName);

        IList<FormCCModel> SendFormCC(string userName, DateTime? startDate, DateTime? endDate, string receiveStatus, int pageIndex, int pageSize, List<aZaaS.KStar.Helper.SortDescriptor> sort, out int total);

        IList<FormCCModel> ReceiveFormCC(string userName);

        IList<ProcessFormCC> ReceiveFormCC(string userName, bool viewStatus);

        IList<FormCCModel> ReceiveFormCC(string userName, DateTime? startDate, DateTime? endDate, string receiveStatus, int pageIndex, int pageSize, List<aZaaS.KStar.Helper.SortDescriptor> sort, out int total);

        bool IsReceiver(int formId, string receiver);

        bool IsAlreadyReview(Guid sysId, string receiver);

        bool IsAlreadyReview(int formId, int activityId, string receiver);
    }
}

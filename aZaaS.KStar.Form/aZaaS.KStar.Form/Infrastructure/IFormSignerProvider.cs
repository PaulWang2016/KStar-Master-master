using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.Models;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IFormSignerProvider
    {
        void AddSigner(List<ProcessFormSigner> signer);

        List<ProcessFormSigner> GetPendingSigners(int procInstId, string activityName);

        ProcessFormSigner GetPendingSigners(int procInstId, string activityName, string signerName);

        List<ProcessFormSigner> GetSigners();

        void UpdateApprovalAction(int processInstId, string signerName, string actionName);

        bool IsSigner(int procInstId, string signer);
    }
}

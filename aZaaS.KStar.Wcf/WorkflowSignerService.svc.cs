using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Wcf.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace aZaaS.KStar.Wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WorkflowSignerService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WorkflowSignerService.svc or WorkflowSignerService.svc.cs at the Solution Explorer and start debugging.
    public class WorkflowSignerService : IWorkflowSignerService
    {

        private readonly SignerRepository _signerRepository;

        public WorkflowSignerService()
        {
            _signerRepository = new SignerRepository();
        }

        public IEnumerable<SignerInfo> GetSigners()
        {
            List<SignerInfo> signerInfos = new List<SignerInfo>();
            List<ProcessFormSigner> signers=_signerRepository.GetSigners();
            if(signers!=null)
            {
                foreach (ProcessFormSigner item in signers)
                {
                    signerInfos.Add(new SignerInfo()
                    {
                        ProcessInstId=item.ProcessInstId,
                        ActivityName=item.ActivityName,
                        UserName=item.UserName,
                        SignerName=item.SignerName,
                        SignerDate=item.SignerDate,
                        IsApproval=item.IsApproval,
                        ActionName=item.ActionName,
                        ApprovalDate=item.ApprovalDate
                    });
                }
            }
            return signerInfos;
        }

        public void UpdateApprovalAction(int processInstId, string signerName, string actionName)
        {
            _signerRepository.UpdateApprovalAction(processInstId, signerName, actionName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;

namespace aZaaS.KStar.Form
{
    public class KStarFormSignerProvider : IFormSignerProvider
    {
        private readonly SignerRepository _signerRepository;

        public KStarFormSignerProvider()
        {
            _signerRepository = new SignerRepository();
        }

        public void AddSigner(List<ProcessFormSigner> signer)
        {
            _signerRepository.AddSigner(signer);
        }

        public List<ProcessFormSigner> GetPendingSigners(int procInstId, string activityName)
        {
            return _signerRepository.GetPendingSigners(procInstId, activityName);
        }

        public ProcessFormSigner GetPendingSigners(int procInstId, string activityName, string signerName)
        {
            return _signerRepository.GetPendingSigners(procInstId, activityName, signerName);
        }

        public List<ProcessFormSigner> GetSigners()
        {
            return _signerRepository.GetSigners();
        }

        public void UpdateApprovalAction(int processInstId, string signerName, string actionName)
        {
            _signerRepository.UpdateApprovalAction(processInstId, signerName, actionName);
        }

        public bool IsSigner(int procInstId, string signer)
        {
            return _signerRepository.IsSigner(procInstId, signer);
        }
    }
}

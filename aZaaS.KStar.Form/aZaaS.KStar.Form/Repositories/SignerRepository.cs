using aZaaS.KStar.Form.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Repositories
{
    public class SignerRepository
    {
        public void AddSigner(List<ProcessFormSigner> signer)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                context.ProcessFormSigners.AddRange(signer);
                context.SaveChanges();
            }
        }

        public List<ProcessFormSigner> GetPendingSigners(int procInstId, string activityName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var query = context.ProcessFormSigners.Where(s => s.ProcessInstId == procInstId 
                                                    && s.ActivityName.Equals(activityName)
                                                    && s.IsApproval == false );
                return query.ToList();
            }
        }

        public ProcessFormSigner GetPendingSigners(int procInstId, string activityName, string signerName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormSigners.FirstOrDefault(s => s.ProcessInstId == procInstId
                                                    && s.ActivityName.Equals(activityName)
                                                    && s.SignerName == signerName
                                                    && s.IsApproval == false);
                return item;
            }
        }

        public List<ProcessFormSigner> GetSigners()
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                return context.ProcessFormSigners.ToList();                
            }
        }

        public void UpdateApprovalAction(int processInstId, string signerName, string actionName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var signerList = context.ProcessFormSigners.Where(x => x.ProcessInstId == processInstId && x.SignerName == signerName);

                if (signerList != null)
                {
                    signerList.ToList().ForEach(signer =>
                    {
                        signer.IsApproval = true;
                        signer.ActionName = actionName;
                        signer.ApprovalDate = DateTime.Now;
                    });
                }

                context.SaveChanges();
            }            
        }

        public bool IsSigner(int procInstId, string signer)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormSigners.FirstOrDefault(r => r.ProcessInstId == procInstId && r.SignerName == signer);

                if (item != null)
                {
                    return true;
                }

                return false;
            }
        }
    }
}

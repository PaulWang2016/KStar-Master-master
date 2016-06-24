using aZaaS.KStar.UserProfiles;
using aZaaS.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Facades
{
    public class CandidateFacade
    {
        private CandidateManager _candidateManager;
        public CandidateFacade()
        {
            _candidateManager = new CandidateManager();
        }   

        public List<CandidateEntity> GetCandidates(Guid sysId)
        {
            sysId.EmptyThrowArgumentEx("sysId is Empty");
            
            return _candidateManager.GetCandidates(sysId);
        }

        public void SaveCandidates(List<CandidateEntity> candidates)
        {
            _candidateManager.SaveCandidates(candidates);
        }

        public bool DeleteCandidates(Guid sysId)
        {
            return _candidateManager.DeleteCandidates(sysId);
        }
    }
}

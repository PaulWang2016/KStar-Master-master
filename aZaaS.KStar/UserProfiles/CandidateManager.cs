using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.UserProfiles
{
    public class CandidateManager
    {
        public List<CandidateEntity> GetCandidates(Guid sysId)
        {
            List<CandidateEntity> model;
            using (KStarDbContext context = new KStarDbContext())
            {
                model = context.Candidate.Where(s => s.SysId==sysId).ToList();
            }
            return model;
        }

        public void SaveCandidates(List<CandidateEntity> candidates)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                foreach (var candidate in candidates)
                {
                    context.Candidate.Add(new CandidateEntity() { SysId = candidate.SysId, PersonId = candidate.PersonId, Type = candidate.Type });
                }
                context.SaveChanges();
            }
        }

        public bool DeleteCandidates(Guid sysId)
        {            
            using (KStarDbContext context = new KStarDbContext())
            {
                foreach (var candidate in context.Candidate.Where(x=>x.SysId==sysId))
                    context.Candidate.Remove(candidate);
                context.SaveChanges();
            }
            return true;
        }
    }
}

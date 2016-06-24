using aZaaS.KStar.Facades;
using aZaaS.KStar.UserProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Controllers
{
    public class CustomPersonController : Controller
    {
        //
        // GET: /CustomPerson/
        private readonly CandidateFacade candidateFacade;

        public CustomPersonController()
        {
            this.candidateFacade = new CandidateFacade();            
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SaveCustomPersonData(string sessionid, List<string> personlist, List<string> departlist, List<string> positionlist, List<string> customlist, List<string> rolelist)
        {
            if (!string.IsNullOrEmpty(sessionid))
            {
                Guid sysId = Guid.Parse(sessionid);
                List<CandidateEntity> candidates = new List<CandidateEntity>();
                if (personlist != null)
                {
                    foreach (var item in personlist)
                    {
                        candidates.Add(new CandidateEntity() { SysId = sysId, PersonId = Guid.Parse(item), Type = "Person" });
                    }
                }
                if (departlist != null)
                {
                    foreach (var item in departlist)
                    {
                        candidates.Add(new CandidateEntity() { SysId = sysId, PersonId = Guid.Parse(item), Type = "Department" });
                    }
                }
                if (positionlist != null)
                {
                    foreach (var item in positionlist)
                    {
                        candidates.Add(new CandidateEntity() { SysId = sysId, PersonId = Guid.Parse(item), Type = "Position" });
                    }
                }
                if (customlist != null)
                {
                    foreach (var item in customlist)
                    {
                        candidates.Add(new CandidateEntity() { SysId = sysId, PersonId = Guid.Parse(item), Type = "Custom" });
                    }
                }
                if (rolelist != null)
                {
                    foreach (var item in rolelist)
                    {
                        candidates.Add(new CandidateEntity() { SysId = sysId, PersonId = Guid.Parse(item), Type = "Role" });
                    }
                }
                candidateFacade.SaveCandidates(candidates);
                return Json(sysId, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteCustomPersonData(string sessionid)
        {
            if (!string.IsNullOrEmpty(sessionid))
            {
                bool flag = candidateFacade.DeleteCandidates(Guid.Parse(sessionid));
                return Json(flag, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

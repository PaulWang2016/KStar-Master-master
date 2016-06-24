using aZaaS.KStar;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class UserProfileController : BaseMvcController
    {
        private UserProfileFacade _userProfileFacade;
        UserBO userBO = new UserBO();
        public UserProfileController()
        {
            _userProfileFacade = new UserProfileFacade();
        }

        public JsonResult SaveProfile()
        {
            List<UserProfile> userProfiles = new List<UserProfile>();
            var applicant = userBO.ReadUser(this.CurrentUser);
            var keys = Request.Cookies.AllKeys.ToList();
            Guid CurrentUserId = applicant.SysID;
            foreach (var key in keys)
            {
                userProfiles.Add(new UserProfile() { Key = key, Value = Request.Cookies[key].Value });
            }
            _userProfileFacade.SetUserProfile(userProfiles, CurrentUserId);

            return Json("Save successfully!", JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadProfile()
        {
            var applicant = userBO.ReadUser(this.CurrentUser);
            var userProfiles = _userProfileFacade.GetUserProfile(applicant.SysID);
            int oldcount = Response.Cookies.Count;
            var keys = Response.Cookies.AllKeys.ToList();
            foreach (var userProfile in userProfiles)
            {
                HttpCookie httpCookie = new HttpCookie(userProfile.Key, userProfile.Value);
                if (keys.Contains(userProfile.Key))
                {
                    Response.Cookies.Set(httpCookie);
                    keys.Remove(userProfile.Key);
                }
                else
                {
                    Response.Cookies.Add(httpCookie);
                }
            }
            foreach (var key in keys)
            {
                Response.Cookies.Remove(key);
            }
            int newcount = Response.Cookies.Count;

            return Json("Load successfully!", JsonRequestBehavior.AllowGet);
        }

    }
}

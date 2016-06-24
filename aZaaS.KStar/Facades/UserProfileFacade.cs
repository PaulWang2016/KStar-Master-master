using aZaaS.KStar.UserProfiles;
using aZaaS.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Facades
{
    public class UserProfileFacade
    {
        private UserProfileManager _userProfileManager;
        public UserProfileFacade()
        {
            _userProfileManager = new UserProfileManager();
        }

        public List<UserProfile> GetUserProfile(Guid userId)
        {
            userId.EmptyThrowArgumentEx("userId is Empty");

            return _userProfileManager.GetUserProfile(userId);
        }

        public void SetUserProfile(List<UserProfile> userProfile, Guid userId)
        {
            userId.EmptyThrowArgumentEx("userId is Empty");

            _userProfileManager.SetUserProfile(userProfile, userId);
        }
    }
}

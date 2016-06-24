using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.UserProfiles
{
    public class UserProfileManager
    {
        public List<UserProfile> GetUserProfile(Guid userId)
        {
            List<UserProfile> model;
            using (KStarDbContext context = new KStarDbContext())
            {
                model = context.UserProfile.Where(s => s.UserId == userId).Select(s => new UserProfile() { Key = s.Key, Value = s.Value }).ToList();
            }
            return model;
        }

        public void SetUserProfile(List<UserProfile> userProfiles, Guid userId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var oldUserProfiles = context.UserProfile.Where(s => s.UserId == userId);
                //编辑 旧cookies
                foreach (var userProfile in oldUserProfiles)
                {
                    var model = userProfiles.SingleOrDefault(s => s.Key == userProfile.Key);
                    if (model != null)
                    {
                        userProfile.Value = model.Value;
                        userProfiles.Remove(model);
                    }
                    else
                    {
                        userProfile.Value = "";
                    }
                }
                //添加 新cookies
                foreach (var userProfile in userProfiles)
                {
                    context.UserProfile.Add(new UserProfileEntity() { Key = userProfile.Key, Value = userProfile.Value, UserId = userId });
                }
                context.SaveChanges();
            }
        }
    }
}

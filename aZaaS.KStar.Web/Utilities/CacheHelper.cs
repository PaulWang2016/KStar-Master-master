using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Workflow.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Utilities
{
    public class CacheHelper
    {
        public static void InitUserCache()
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["Users"] as Dictionary<string, string>;
            if (dics == null||dics.Count==0)
            {
                dics = new Dictionary<string, string>();
                //缓存中没有，则从数据库中加载
                UserBO ub = new UserBO();
                IEnumerable<UserWithRelationshipsDto> users = ub.GetAllUsers();
                foreach (var user in users)
                {
                    if (!dics.ContainsKey(user.UserName.ToLower()))
                    {
                        dics.Add(user.UserName.ToLower(), user.FullName);
                    }
                }
                HttpRuntime.Cache.Remove("Users");
                HttpRuntime.Cache.Insert("Users", dics);
            }
        }

        public static void UpdateUserCache(string username, string fullname)
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["Users"] as Dictionary<string, string>;
            if (dics != null && dics.ContainsKey(username.ToLower()))
            {
                HttpRuntime.Cache.Remove("Users");
                dics[username.ToLower()] = fullname;                
                HttpRuntime.Cache.Insert("Users", dics);
            }
        }

        public static void AddUserCache(string username, string fullname)
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["Users"] as Dictionary<string, string>;
            if (dics != null && !dics.ContainsKey(username.ToLower()))
            {
                HttpRuntime.Cache.Remove("Users");
                dics.Add(username.ToLower(), fullname);
                HttpRuntime.Cache.Insert("Users", dics);
            }
        }

        public static void DeleteUserCache(string username)
        {
            Dictionary<string, string> dics = HttpRuntime.Cache["Users"] as Dictionary<string, string>;
            if (dics != null && dics.ContainsKey(username.ToLower()))
            {
                HttpRuntime.Cache.Remove("Users");
                dics.Remove(username.ToLower());
                HttpRuntime.Cache.Insert("Users", dics);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Authentication
{
    public static class Extensions
    {
        public static bool NotExistsOrEmpty(this Dictionary<string, string> dic, string key)
        {
            return !dic.ContainsKey(key) || string.IsNullOrEmpty(dic[key]);
        }

        public static Dictionary<string, string> GetParameterMap(this IUserAuthProvider provider)
        {
            return KStarUserAuthenticator.GetParameterMap(provider.GetType(), provider.ParameterMapValidator);
        }

        public static string GetParameter(this IUserAuthProvider provider,string key)
        {
            var parameters = provider.GetParameterMap();

            return parameters.ContainsKey(key) ? parameters[key] : string.Empty;
        }

        /// <summary>
        /// 校验KStar用户是否存在
        /// </summary>
        /// <exception cref="UserNotExistsException">UserNotExistsException</exception>
        /// <param name="provider"></param>
        /// <param name="userName"></param>
        public static void ValidUserExists(this IUserAuthProvider provider, string userName)
        {
            if (!KStarUserAuthenticator.UserExists(userName)) 
                ExceptionRaiser.UserNotExists(userName);
        }
    }
}

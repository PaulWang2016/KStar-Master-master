using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace aZaaS.KStar.Web.Utilities
{
    public class RegexUtilities
    {
        /// <summary>
        /// 匹配 “{}” {} 里面为大小写字母已经“.”
        /// </summary>
        /// <returns>匹配的数组</returns>
        public static string[] RegexAngle(string str)
        {
            var matchs = Regex.Matches(str, @"({[\w\d|\.]*}+)");
            List<string> matchList = new List<string>();
            foreach (Match item in matchs)
            {
                string keys = item.Value;
                keys = keys.Replace("{", "").Replace("}", "");
                matchList.Add(keys);
            }
            return matchList.ToArray();
        }
    }
}
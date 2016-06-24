using aZaaS.KStar.Facades;
using aZaaS.KStar.Localization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace aZaaS.KStar.Helper
{
    public class CustomHelper
    {
        public static string UserNameFormat(string lastname, string firstname, string username, string appsettings = "UserNameFormatter_")
        {
            Regex r = new Regex("{lastname}");
            string result = string.Empty;
            string formatter = ConfigurationManager.AppSettings[appsettings + ResxService.GetAvailableCulture()];
            if (string.IsNullOrEmpty(formatter))
            {
                result = lastname + firstname;
            }
            else
            {
                //替换lastname
                result = r.Replace(formatter, (lastname??""));
                //替换firstname
                r = new Regex("{firstname}");
                result = r.Replace(result, (firstname??""));
                //替换username
                r = new Regex("{username}");
                result = r.Replace(result, (username??""));
            }
            return result;
        }        

      /// <summary>
      /// 获取客户端IP地址（无视代理）
      /// </summary>
      /// <returns>若失败则返回回送地址</returns>
      public static string GetClientAddress()
      {
         string userHostAddress = HttpContext.Current.Request.UserHostAddress;
  
          if (string.IsNullOrEmpty(userHostAddress))
         {
             userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
         }
 
         //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
         if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
         {
             return userHostAddress;
         }
         return "127.0.0.1";
     }
 
     /// <summary>
     /// 检查IP地址格式
     /// </summary>
     /// <param name="ip"></param>
     /// <returns></returns>
     public static bool IsIP(string ip)
     {
         return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
     }

     public static string GetUserWithOutLabel(string users)
     {
         string temp = string.Empty;
         if (!string.IsNullOrEmpty(users))
         {
             temp = Regex.Replace(users, "k2:|k2sql:", "", RegexOptions.IgnoreCase);
         }
         return temp;
     }
     public static List<string> GetUserWithOutLabel(List<string> users)
     {
         string temp = string.Empty;
         List<string> userlist = new List<string>();
         if (users != null && users.Count > 0)
         {
             temp = string.Join(",", users);
             temp = Regex.Replace(temp, "k2:|k2sql:", "", RegexOptions.IgnoreCase);
             string[] arr = temp.Split(',');
             userlist = arr.ToList();
         }
         return userlist;
     }

     public static string GetUserWithLabel(string user)
     {
         if (!string.IsNullOrEmpty(user) && !Regex.Match(user, "k2:|k2sql:", RegexOptions.IgnoreCase).Success)
         {
             return string.Format("{0}:{1}", PortalEnvironment.CurrentWorkflowSecurityLabel, user);
         }
         else
         {
             return user;
         }
     }
     public static List<string> GetUserWithLabel(List<string> users)
     {
         string temp = string.Empty;
         List<string> userlist = new List<string>();
         if (users != null && users.Count > 0)
         {
             temp = string.Join(",", users);
             if (Regex.Match(temp, "k2:|k2sql:", RegexOptions.IgnoreCase).Success)
             {
                 temp = Regex.Replace(temp, "k2:|k2sql:", "", RegexOptions.IgnoreCase);
             }
             temp = Regex.Replace(temp, ",", "," + PortalEnvironment.CurrentWorkflowSecurityLabel + ":", RegexOptions.IgnoreCase);
             temp = PortalEnvironment.CurrentWorkflowSecurityLabel + ":" + temp;
             string[] arr = temp.Split(',');
             userlist = arr.ToList();
         }
         return userlist;
     }
    }
}

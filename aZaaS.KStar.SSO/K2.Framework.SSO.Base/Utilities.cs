using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2.Framework.SSO.Base
{
  public  class Utilities
    {

        /// <summary>
        /// base64加密
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
      public static string Base64ReplaceToUrl(string base64)
      {
          if (string.IsNullOrWhiteSpace(base64)) return string.Empty;
          byte[] bytes = Encoding.UTF8.GetBytes(base64);
          string str = "";
          try
          {
              str = Convert.ToBase64String(bytes);
          }
          catch
          {
              str = base64;
          }

          str = System.Web.HttpUtility.UrlEncode(str, Encoding.UTF8);
          return str;
          // return base64.Replace("+", "-").Replace("/", "(").Replace("=", ")");
      }


        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string UrlReplaceToBase64(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString)) return string.Empty;
            string decode = ""; 
            try
            {
                byte[] bytes = Convert.FromBase64String(urlString);
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                decode = urlString;
            }
            return decode;
            //return urlString.Replace("-", "+").Replace("(", "/").Replace(")", "=");

        }

        public static readonly string TokenReplace = "$token$";
    }
}

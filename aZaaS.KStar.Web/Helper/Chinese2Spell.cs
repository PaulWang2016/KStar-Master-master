using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace aZaaS.KStar.Web.Helper
{
    /// <summary>
    /// 汉字转拼音静态类,包括功能全拼和缩写，方法全部是静态的
    /// </summary>
   public static class Chinese2Spell
   {
       #region   拼音汉字 数组 
       private static string path = System.Threading.Thread.GetDomain().BaseDirectory + "Helper/ChineseSpellData.xml";              
       #endregion

       public static string[][] GetAllhz()
       {
           try
           {
               using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
               {
                   XmlDocument xmlDoc = new XmlDocument();
                   string xml = "";
                   xmlDoc.LoadXml(reader.ReadToEnd());
                   xml = xmlDoc.InnerXml;
                   return XMLHelper.Deserialize<string[][]>(xml);
               }
           }
           catch (Exception ex)
           {
               return new string[][] { new string[] { "", "" } };
           }
       }

       /// <summary>
       /// 获取拼音
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
       public static string GetSpelling(string str)
       {
           StringBuilder rtnSb = new StringBuilder("");
           char[] charary = str.ToCharArray();
           string[][] _Allhz = GetAllhz();
           for (int i = 0; i < charary.Length; i++)
           {
               var ch = charary[i];
               if (IsCharChinese(ch))
               {
                   for (int j = 0; j < _Allhz.Length; j++)
                   {
                       if (_Allhz[j][1].IndexOf(ch) != -1)
                       {
                           rtnSb.Append(_Allhz[j][0]);
                           break;
                       }
                   }
               }
               else
                   rtnSb.Append(ch);
           }
           return rtnSb.ToString().ToLower();
       }

       /// <summary>
       /// 获取首字母拼音
       /// </summary>
       /// <param name="str"></param>
       /// <returns></returns>
       public static string GetFirstSpelling(string str)
       {
           StringBuilder rtnSb = new StringBuilder("");
           char[] charary = str.ToCharArray();
           string[][] _Allhz = GetAllhz();
           for (int i = 0; i < charary.Length; i++)
           {
               var ch = charary[i];
               if (IsCharChinese(ch))
               {
                   for (int j = 0; j < _Allhz.Length; j++)
                   {
                       if (_Allhz[j][1].IndexOf(ch) != -1)
                       {
                           rtnSb.Append(_Allhz[j][0].Substring(0,1));
                           break;
                       }
                   }
               }
               else
                   rtnSb.Append(ch.ToString().Substring(0, 1));
           }
           return rtnSb.ToString().ToLower();
       }

       /// <summary>
       /// 是否是汉字
       /// </summary>
       /// <param name="c"></param>
       /// <returns></returns>
       public static bool IsCharChinese(char c)
       {
           System.Text.RegularExpressions.Regex regex = new
               System.Text.RegularExpressions.Regex(@"[\u4e00-\u9fa5]", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
           if (regex.IsMatch(c.ToString()))
           {
               return true;
           }
           return false;
       }

       /// <summary>
       /// 返回多音字
       /// </summary>
       /// <param name="c"></param>
       /// <returns></returns>
       public static string[] GetPolyphone(char c)
       {
           List<string> list = new List<string>();
           string[][] _Allhz = GetAllhz();
           for (int j = 0; j < _Allhz.Length; j++)
           {
               if (_Allhz[j][1].IndexOf(c) != -1)
               {
                   list.Add(_Allhz[j][0]);
               }
           }
           return list.ToArray();
       }
    }
}
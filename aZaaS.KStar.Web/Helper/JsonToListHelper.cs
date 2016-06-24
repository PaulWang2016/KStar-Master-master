using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace aZaaS.KStar.Web.Helper
{
    public class JsonToListHelper
    {
        private static string FormatJsonMatch(Match m)
        {
            return string.Format("\"\\/Date({0})\\/\"", m.Groups[2].Value);
        }
        public static string FormatJson(string value)
        {
            string p = @"(new Date)\(+([0-9,-]+)+(\))";
            MatchEvaluator matchEvaluator = new MatchEvaluator(FormatJsonMatch);
            Regex reg = new Regex(p);
            bool isfind = reg.IsMatch(value);
            value = reg.Replace(value, matchEvaluator);
            return value;
        }
        public static string FormatJsonDate(string value)
        {
            string p = @"(new Date)\(+([0-9,-]+)+(\))";
            MatchEvaluator matchEvaluator = new MatchEvaluator(FormatJsonDateMatch);
            Regex reg = new Regex(p);
            value = reg.Replace(value, matchEvaluator);

            p = @"(Date)\(+([0-9,-]+)+(\))";
            matchEvaluator = new MatchEvaluator(FormatJsonDateMatch);
            reg = new Regex(p);
            value = reg.Replace(value, matchEvaluator);

            p = "\"\\\\\\/" + @"Date(\()([0-9,-]+)([+])([0-9,-]+)(\))" + "\\\\\\/\"";
            matchEvaluator = new MatchEvaluator(FormatJsonDateMatch);
            reg = new Regex(p);
            value = reg.Replace(value, matchEvaluator);

            return value;

        }
        public static string AttributeToVariable(string value)
        {
            string p = @"\<([A-Z,a-z,0-9,_]*)\>k__BackingField";
            MatchEvaluator matchEvaluator = new MatchEvaluator(AttributeToVariableMatch);
            Regex reg = new Regex(p);
            bool isfind = reg.IsMatch(value);
            value = reg.Replace(value, matchEvaluator);
            return value;
        }
        private static string AttributeToVariableMatch(Match m)
        {
            return m.Groups[1].Value;
        }
        public static string VariableToAttribute(string value)
        {
            string p = "\\\"([A-Z,a-z,0-9,_]*)\\\"\\:";
            MatchEvaluator matchEvaluator = new MatchEvaluator(VariableToAttributeMatch);
            Regex reg = new Regex(p);
            bool isfind = reg.IsMatch(value);
            value = reg.Replace(value, matchEvaluator);
            return value;
        }
        private static string VariableToAttributeMatch(Match m)
        {
            return string.Format("\"<{0}>k__BackingField\":", m.Groups[1].Value);
        }
        private static string FormatJsonDateMatch(Match m)
        {

            string result = string.Empty;

            DateTime dt = new DateTime(1970, 1, 1);

            dt = dt.AddMilliseconds(long.Parse(m.Groups[2].Value));

            dt = dt.ToLocalTime();

            result = dt.ToString("yyyy-MM-dd HH:mm:ss");

            return result;
        }

        public static IList<T> JsonToList<T>(string html)
        {
            IList<T> result = new List<T>();
            html = FormatJson(html);
            try
            {
                DataContractJsonSerializer _Json = new DataContractJsonSerializer(result.GetType());
                byte[] _Using = System.Text.Encoding.UTF8.GetBytes(html);
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_Using);
                _MemoryStream.Position = 0;
                object obj = _Json.ReadObject(_MemoryStream); ;
                result = (IList<T>)obj;
            }
            catch (Exception)
            {
                html = AttributeToVariable(html);
                DataContractJsonSerializer _Json = new DataContractJsonSerializer(result.GetType());
                byte[] _Using = System.Text.Encoding.UTF8.GetBytes(html);
                System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_Using);
                _MemoryStream.Position = 0;
                object obj = _Json.ReadObject(_MemoryStream); ;
                result = (IList<T>)obj;
            }
            return result;
        }
    }
}
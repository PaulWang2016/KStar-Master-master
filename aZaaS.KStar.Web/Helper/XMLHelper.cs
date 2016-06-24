using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace aZaaS.KStar.Web.Helper
{
    public class XMLHelper
    {
        public static string Serializer(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string s = "";
            using (System.IO.TextWriter tw = new System.IO.StringWriter())
            {
                serializer.Serialize(tw, obj);
                s = tw.ToString();
            }
            return s;
        } 

        public static T Deserialize<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T t;
            using (System.IO.TextReader tw = new System.IO.StringReader(xml))
            {
                t = (T)serializer.Deserialize(tw);
            }
            return t;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace aZaaS.KStar.Form.Helpers
{
    public class XmlHelper
    {
        /// <summary>
        /// 序列化一个对象
        /// </summary>
        /// <param name="xmlobj">被序列化的对象</param>
        /// <returns>序列化后的xml</returns>
        public static string SerializeObject(object xmlobj)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            Type type = xmlobj.GetType();
            XmlSerializer sz = new XmlSerializer(type);
            sz.Serialize(tw, xmlobj);
            tw.Close();
            return sb.ToString();
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {

                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }
    }
}

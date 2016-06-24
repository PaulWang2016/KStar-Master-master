using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace aZaaS.KStar.Web.Confing
{
    public class ConfingUtilities
    {
        /// <summary>
        /// 读取单层配置（ key  value）
        /// </summary>
        /// <param name="confingName"></param>
        /// <returns></returns>
        public static Hashtable GetSingleConfing(string confingName)
        {
            
            System.Collections.Hashtable ht = new Hashtable();
            if (!string.IsNullOrWhiteSpace(confingName))
            {
                string str = System.AppDomain.CurrentDomain.BaseDirectory;
                confingName =str+"Confing\\" + confingName;
                XmlDocument xd = new XmlDocument(); 
                xd.Load(confingName);
                XmlNodeList nodes = xd.SelectNodes("//add");
                foreach (XmlNode node in nodes)
                {
                    string key = node.Attributes["key"].Value;
                    string value = node.Attributes["value"].Value;
                    ht.Add(key, value);
                } 
            } 
            return ht;
        }
    }
}
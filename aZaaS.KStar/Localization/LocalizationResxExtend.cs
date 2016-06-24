using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace aZaaS.KStar.Localization
{
    public static class LocalizationResxExtend
    {

        public static string CSResxFor_aZaaS_KStar_Facades(this LocalizationResxHelper htmlHelper, string resxKey)
        {
            return "ExceptionMsg" + DateTime.Now.ToString();


        }

        public static string CSResxFor_aZaaS_KStar_Web_Models(this LocalizationResxHelper htmlHelper, string resxKey)
        {
            var filePath = ResxService.GetResxFilePathByNamespace("aZaaS_KStar_Widget");
            return ResxService.GetResouces(resxKey, filePath);

        }
        public static string CSResxFor_aZaaS_KStar_Widget(this LocalizationResxHelper htmlHelper, String resxKey)
        {
            var filePath = ResxService.GetResxFilePathByNamespace("aZaaS_KStar_Widget");
            return ResxService.GetResouces(resxKey, filePath);

        }

        public static string CSResxFor_MenuBar_DisplayName(this HtmlHelper htmlHelper, string resxKey)
        {
            var filePath = ResxService.GetResxFilePathByDbInfo("MenuBar_DisplayName");
            return ResxService.GetResouces(resxKey, filePath);

        }
        public static string CSResxFor_TestDb_MenuInfo_MenuResxKey(this LocalizationResxHelper htmlHelper, String resxKey)
        {
            var filePath = ResxService.GetResxFilePathByDbInfo("TestDb_MenuInfo_MenuResxKey");
            return ResxService.GetResouces(resxKey, filePath);


        }
        public static string GetCommon_MsgInfo(string resxKey)
        {
            var filePath = ResxService.GetCommonResx("MsgInfo");
            return ResxService.GetResouces(resxKey, filePath);

        }

        public static string DBResxFor_K2_ProcSet_Fullname(string resxKey)
        {
            var dic = ResxService.LoadResxFromDB("K2", "ProcSet", "Fullname");
            if (dic.Keys.Contains(resxKey))
            {
                return dic[resxKey];
            }
            return resxKey;
        }
        /// <summary>
        /// ActInst
        /// </summary>
        /// <param name="resxKey"></param>
        /// <returns></returns>
        public static string DBResxFor_K2_ActInst_ActivityName(string resxKey)
        {
            var dic = ResxService.LoadResxFromDB("K2", "ActInst", "ActivityName");
            if (dic.Keys.Contains(resxKey))
            {
                return dic[resxKey];
            }
            return resxKey;
        }


        /// <summary>
        /// 从数据库中解析导航菜单的资源文件
        /// </summary>
        /// <param name="resxKey"></param>
        /// <returns></returns>
        public static string DBResxFor_aZaaSKStar_MenuBar_DisplayName(string resxKey)
        {
            var dic = ResxService.LoadResxFromDB("aZaaS.KStar", "MenuItem", "DisplayName");
            if (dic.Keys.Contains(resxKey))
            {
                return dic[resxKey];
            }
            return resxKey;

        }

        /// <summary>
        /// 从数据库中解析导航菜单的资源文件
        /// </summary>
        /// <param name="resxKey"></param>
        /// <returns></returns>
        public static void DBResxFor_aZaaSKStar_MenuBar_DisplayName_Update(string resxKey, Dictionary<string, string> resxValues)
        {
            ResxService.UpdateResxFromDB("aZaaS.KStar", "MenuItem", "DisplayName", resxKey, resxValues);
        }

        /// <summary>
        /// 从数据库中解析导航菜单的资源文件
        /// </summary>
        /// <param name="resxKey"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> DBResxFor_aZaaSKStar_MenuBar_DisplayName_All(string resxKey)
        {
            return ResxService.GetAllResxFromDB("aZaaS.KStar", "MenuItem", "DisplayName", resxKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetPortalLanguage()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string key = "Language";

            var item = ResxService.GetPortalEnvironment(key);
            if (item != null)
            {
                var xml = item.Value;
                XmlDocument dom = new XmlDocument();
                dom.LoadXml(xml);
                var node = dom.GetElementsByTagName("langSettings");
                foreach (XmlElement n in node.Item(0).ChildNodes)
                {
                    dic.Add(n.GetAttribute("key").ToLower(), n.GetAttribute("value"));
                }
            }

            return dic;
        }
    }
}
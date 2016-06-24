using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace aZaaS.KStar.Web.Confing
{
    public class SystemCustomSettingUtilities
    {
        private static Hashtable ht= null;

        public static String GetTitle()
        {
            Init();
            if (ht.ContainsKey("Title"))
            {
                return ht["Title"] + string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }


        public static String GetFooter()
        {
            Init();
            if (ht.ContainsKey("Footer"))
            {
                return ht["Footer"] + string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        private static void Init()
        {
            if (ht == null || ht.Count == 0)
            {
                ht = ConfingUtilities.GetSingleConfing("_SystemCustomSetting.xml");
            }
        }


    
    }
}
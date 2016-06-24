using aZaaS.KStar.Repositories;
using aZaaS.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Portal;
using System.Data;
using System.Web.Mvc;

namespace aZaaS.KStar.Facades
{ 
    public class PortalEnvironment
    {
        public static string PortalTitle
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getPortalTitle();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setPortalTitle(value);
            }
        }
        public static string LogoImageUrl
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getLogoImageUrl();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setLogoImageUrl(value);
            }
        }

        public static string LogoTitle
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getLogoTitle();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setLogoTitle(value);
            }
        }
        public static bool IsLogoHeader
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getIsLogoHeader();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setIsLogoHeader(value);

            }
        }



        public static string SubLogoImageUrl
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getSubLogoImageUrl();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setSubLogoImageUrl(value);
            }
        }
        public static bool IsBannerImage
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getIsBannerImage();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setIsBannerImage(value);

            }
        }
        public static string BannerImageUrl
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getBannerImageUrl();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setBannerImageUrl(value);
            }
        }

        public static string DateTimeFormat
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getDateTimeFormat();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setDateTimeFormat(value);
            }
        }

        public static string CurrentWorkflowSecurityLabel
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.getCurrentWorkflowSecurityLabel();
            }
            set
            {
                PortalManager portal = new PortalManager();
                portal.setCurrentWorkflowSecurityLabel(value);
            }
        }

        public DataTable GetPortals()
        {
            PortalManager portal = new PortalManager();
            return portal.GetPortals();
        }

        public void SetPortals(DataTable dt)
        {
            dt.NullThrowArgumentEx("DataTable is Null");
            PortalManager portal = new PortalManager();
            portal.SetPortals(dt);
        }

        public static IEnumerable<SelectListItem> GetLanguageList()
        {
            PortalManager portal = new PortalManager();
            return portal.getLanguageList();
        }

        //form验证初始化密码
        public static string FormPassWord
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.GetFormPassWord();
            }           
        }

        //流程作废跳转环节
        public static string CancelActivityName
        {
            get
            {
                PortalManager portal = new PortalManager();
                return portal.GetCancelActivityName();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using aZaaS.KStar.Repositories;
using System.Web.Mvc;
using System.Xml;

namespace aZaaS.KStar.Portal 
{
    internal class PortalManager
    {
        public string getPortalTitle()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity portalTitle = context.PortalEnvironment.SingleOrDefault(s => s.Key == "PortalTitle");
                if (portalTitle == null)
                {
                    portalTitle = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "PortalTitle", Value = "Portal Site Page - Kendo UI", Description = "Portal Title" };
                    context.PortalEnvironment.Add(portalTitle);
                    context.SaveChanges();
                }
                return portalTitle.Value;
            }
        }
        public void setPortalTitle(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity portalTitle = context.PortalEnvironment.SingleOrDefault(s => s.Key == "PortalTitle");
                if (portalTitle == null)
                {
                    portalTitle = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "PortalTitle", Value = value, Description = "Portal Title" };
                    context.PortalEnvironment.Add(portalTitle);
                }
                else
                    portalTitle.Value = value;
                context.SaveChanges();
            }

        }

        public string getLogoImageUrl()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity logoImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "LogoImageUrl");
                if (logoImageUrl == null)
                {
                    logoImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "LogoImageUrl", Value = "/images/logo_AMS.gif", Description = "Logo Image Url" };
                    context.PortalEnvironment.Add(logoImageUrl);
                    context.SaveChanges();
                }
                return logoImageUrl.Value;
            }
        }
        public void setLogoImageUrl(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity LogoImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "LogoImageUrl");
                if (LogoImageUrl == null)
                {
                    LogoImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "LogoImageUrl", Value = value, Description = "Logo Image Url" };
                    context.PortalEnvironment.Add(LogoImageUrl);
                }
                else
                    LogoImageUrl.Value = value;
                context.SaveChanges();
            }
        }
        public string getLogoTitle()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity logoTitle = context.PortalEnvironment.SingleOrDefault(s => s.Key == "LogoTitle");
                if (logoTitle == null)
                {
                    logoTitle = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "LogoTitle", Value = "Solution to achieve results", Description = "Logo Title" };
                    context.PortalEnvironment.Add(logoTitle);
                    context.SaveChanges();
                }
                return logoTitle.Value;
            }
        }
        public void setLogoTitle(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity logoTitle = context.PortalEnvironment.SingleOrDefault(s => s.Key == "LogoTitle");
                if (logoTitle == null)
                {
                    logoTitle = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "LogoTitle", Value = value, Description = "Logo Title" };
                    context.PortalEnvironment.Add(logoTitle);
                }
                else
                    logoTitle.Value = value;
                context.SaveChanges();
            }
        }

        public bool getIsLogoHeader()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerLogoHeader = context.PortalEnvironment.SingleOrDefault(s => s.Key == "IsLogoHeader");
                if (bannerLogoHeader == null)
                {
                    bannerLogoHeader = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "IsLogoHeader", Value = true.ToString(), Description = "Is Need Logo Header" };
                    context.PortalEnvironment.Add(bannerLogoHeader);
                    context.SaveChanges();
                }
                return bool.Parse(bannerLogoHeader.Value);
            }
        }
        public void setIsLogoHeader(bool value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerLogoHeader = context.PortalEnvironment.SingleOrDefault(s => s.Key == "IsLogoHeader");
                if (bannerLogoHeader == null)
                {
                    bannerLogoHeader = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "IsLogoHeader", Description = "Is Need Logo Header" };
                    context.PortalEnvironment.Add(bannerLogoHeader);
                }
                bannerLogoHeader.Value = value.ToString();
                context.SaveChanges();
            }
        }
        public string getSubLogoImageUrl()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity logoImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "SubLogoImageUrl");
                if (logoImageUrl == null)
                {
                    logoImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "SubLogoImageUrl", Value = "/images/logo_the_link.gif", Description = "Sub Logo Image Url" };
                    context.PortalEnvironment.Add(logoImageUrl);
                    context.SaveChanges();
                }
                return logoImageUrl.Value;
            }
        }
        public void setSubLogoImageUrl(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity logoImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "SubLogoImageUrl");
                if (logoImageUrl == null)
                {
                    logoImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "SubLogoImageUrl", Description = "Sub Logo Image Url" };
                    context.PortalEnvironment.Add(logoImageUrl);
                }
                logoImageUrl.Value = value;
                context.SaveChanges();
            }
        }

        public bool getIsBannerImage()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "IsBannerImage");
                if (bannerImageUrl == null)
                {
                    bannerImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "IsBannerImage", Value = true.ToString(), Description = "Is Need Banner Image" };
                    context.PortalEnvironment.Add(bannerImageUrl);
                    context.SaveChanges();
                }
                return bool.Parse(bannerImageUrl.Value);
            }
        }
        public void setIsBannerImage(bool value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity isBannerImage = context.PortalEnvironment.SingleOrDefault(s => s.Key == "IsBannerImage");
                if (isBannerImage == null)
                {
                    isBannerImage = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "IsBannerImage", Description = "Is Need Banner Image" };
                    context.PortalEnvironment.Add(isBannerImage);
                }
                isBannerImage.Value = value.ToString();
                context.SaveChanges();
            }
        }

        public string getBannerImageUrl()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "BannerImageUrl");
                if (bannerImageUrl == null)
                {
                    bannerImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "BannerImageUrl", Value = "/images/Banner.jpg", Description = "Banner Image Url" };
                    context.PortalEnvironment.Add(bannerImageUrl);
                    context.SaveChanges();
                }
                return bannerImageUrl.Value;
            }
        }
        public void setBannerImageUrl(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "BannerImageUrl");
                if (bannerImageUrl == null)
                {
                    bannerImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "BannerImageUrl", Value = value, Description = "Banner Image Url" };
                    context.PortalEnvironment.Add(bannerImageUrl);
                }
                else
                    bannerImageUrl.Value = value;
                context.SaveChanges();
            }
        }

        public string getDateTimeFormat()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "DateTimeFormat");
                if (bannerImageUrl == null)
                {
                    bannerImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "DateTimeFormat", Value = "MM/dd/yyyy HH:mm tt", Description = "DateTime Format" };
                    context.PortalEnvironment.Add(bannerImageUrl);
                    context.SaveChanges();
                }
                return bannerImageUrl.Value;
            }
        }
        public void setDateTimeFormat(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity bannerImageUrl = context.PortalEnvironment.SingleOrDefault(s => s.Key == "DateTimeFormat");
                if (bannerImageUrl == null)
                {
                    bannerImageUrl = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "DateTimeFormat", Value = value, Description = "DateTime Format" };
                    context.PortalEnvironment.Add(bannerImageUrl);
                }
                else
                    bannerImageUrl.Value = value;
                context.SaveChanges();
            }
        }

        public DataTable GetPortals()
        {
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            using (KStarDbContext context = new KStarDbContext())
            {
                #region Portal
                List<PortalEnvironmentEntity> portals = new List<PortalEnvironmentEntity>();

                portals = context.PortalEnvironment.ToList();
                return converter.ToDataTable(portals);
                #endregion
            }
        }

        public void SetPortals(DataTable dt)
        {
            DataTabletoListConverter converter = new DataTabletoListConverter();
            using (KStarDbContext context = new KStarDbContext())
            {
                List<PortalEnvironmentEntity> portals = converter.ToList<PortalEnvironmentEntity>(dt);
                foreach (var portal in portals)
                {
                    var oldportal = context.PortalEnvironment.SingleOrDefault(s => s.Id == portal.Id || s.Key == portal.Key);
                    if (oldportal == null)
                    {
                        context.PortalEnvironment.Add(portal);
                    }
                    else
                    {
                        oldportal.Value = portal.Value;
                        oldportal.Description = portal.Description;
                    }
                }
                context.SaveChanges();
            }
        }
        
        public IEnumerable<SelectListItem> getLanguageList()
        {

            using (KStarDbContext context = new KStarDbContext())
            {
                //var languageList = context.PortalEnvironment.SqlQuery("select [Value] from PortalEnvironment where [Key]='Language'","").ToList<PortalEnvironmentEntity>();
                //查询出PortalEnvironment表里面Key=Language的集合
                var languageList = context.PortalEnvironment.Where(q => q.Key == "Language").ToList();
                List<SelectListItem> list = new List<SelectListItem>();
                XmlDocument dom = new XmlDocument();
                dom.LoadXml(languageList[0].Value);
                XmlElement root = dom.DocumentElement;
                XmlNodeList nodeList = root.ChildNodes;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    string text = nodeList.Item(i).Attributes["value"].Value;
                    string value = nodeList.Item(i).Attributes["key"].Value;
                    list.Add(new SelectListItem() { Text = text, Value = value });
                }
                return list;

            }
        }

        public string getCurrentWorkflowSecurityLabel()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity currentWorkflowSecurityLabel = context.PortalEnvironment.SingleOrDefault(s => s.Key == "CurrentWorkflowSecurityLabel");
                if (currentWorkflowSecurityLabel == null)
                {
                    currentWorkflowSecurityLabel = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "CurrentWorkflowSecurityLabel", Value = "K2", Description = "安全验证标签，默认为K2" };
                    context.PortalEnvironment.Add(currentWorkflowSecurityLabel);
                    context.SaveChanges();
                }
                return currentWorkflowSecurityLabel.Value;
            }
        }

        public void setCurrentWorkflowSecurityLabel(string value)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity currentWorkflowSecurityLabel = context.PortalEnvironment.SingleOrDefault(s => s.Key == "CurrentWorkflowSecurityLabel");
                if (currentWorkflowSecurityLabel == null)
                {
                    currentWorkflowSecurityLabel = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "CurrentWorkflowSecurityLabel", Value = "K2", Description = "安全验证标签，默认为K2" };
                    context.PortalEnvironment.Add(currentWorkflowSecurityLabel);
                }
                else
                    currentWorkflowSecurityLabel.Value = value;
                context.SaveChanges();
            }

        }

        /// <summary>
        /// 获取用户 form验证登陆时的 初始化密码
        /// </summary>
        public string GetFormPassWord()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity FormPassWord = context.PortalEnvironment.SingleOrDefault(s => s.Key == "FormPassWord");
                if (FormPassWord == null)
                {
                    FormPassWord = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "FormPassWord", Value = "888888", Description = "Form验证用户初始化密码" };
                    context.PortalEnvironment.Add(FormPassWord);
                    context.SaveChanges();
                }
                return FormPassWord.Value;
            }
        }

        /// <summary>
        /// 获取流程作废环节名称
        /// </summary>
        /// <returns></returns>
        public string GetCancelActivityName()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Portal.PortalEnvironmentEntity cancelActivityName = context.PortalEnvironment.SingleOrDefault(s => s.Key == "CancelActivityName");
                if (cancelActivityName == null)
                {
                    cancelActivityName = new Portal.PortalEnvironmentEntity() { Id = Guid.NewGuid(), Key = "cancelActivityName", Value = "CancelActivityName", Description = "流程作废环节名称" };
                    context.PortalEnvironment.Add(cancelActivityName);
                    context.SaveChanges();
                }
                return cancelActivityName.Value;
            }
        }
    }
}

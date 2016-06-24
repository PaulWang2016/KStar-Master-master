using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class PortalController : BaseMvcController
    {

        //private static readonly PortalEnvironment _portalfacade = new PortalEnvironment();

        public JsonResult Save(string PortalTitle, string LogoImageUrl, string SubLogoImageUrl, string BannerImageUrl, bool IsBannerImage, string DateTimeFormat, string LogoTitle, bool IsLogoHeader)
        {
            PortalEnvironment.BannerImageUrl = BannerImageUrl;
            PortalEnvironment.LogoImageUrl = LogoImageUrl;
            PortalEnvironment.PortalTitle = PortalTitle;
            PortalEnvironment.IsBannerImage = IsBannerImage;
            PortalEnvironment.SubLogoImageUrl = SubLogoImageUrl;
            PortalEnvironment.DateTimeFormat = DateTimeFormat;
            PortalEnvironment.LogoTitle = LogoTitle;
            PortalEnvironment.IsLogoHeader = IsLogoHeader;
            return Json("Success！", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Get()
        {
            string BannerImageUrl = PortalEnvironment.BannerImageUrl;
            string LogoImageUrl = PortalEnvironment.LogoImageUrl;
            string SubLogoImageUrl = PortalEnvironment.SubLogoImageUrl;
            string PortalTitle = PortalEnvironment.PortalTitle;
            bool IsBannerImage = PortalEnvironment.IsBannerImage;
            string DateTimeFormat = PortalEnvironment.DateTimeFormat;
            string LogoTitle = PortalEnvironment.LogoTitle;
            bool IsLogoHeader = PortalEnvironment.IsLogoHeader;
            return Json(new
            {
                PortalTitle = PortalTitle,
                LogoImageUrl = LogoImageUrl,
                SubLogoImageUrl = SubLogoImageUrl,
                IsBannerImage = IsBannerImage,
                BannerImageUrl = BannerImageUrl,
                DateTimeFormat = DateTimeFormat,
                LogoTitle = LogoTitle,
                IsLogoHeader = IsLogoHeader
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveImage(IList<HttpPostedFileBase> files, string Field)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = System.IO.Path.GetFileName(file.FileName);
                    string extension = System.IO.Path.GetExtension(fileName);
                    if (extension == ".jpg" || extension == ".gif" || extension == ".png")
                    {
                        var physicalPath = System.IO.Path.Combine(Server.MapPath("~/images"), fileName);

                        file.SaveAs(physicalPath);

                        switch (Field)
                        {
                            case "LogoImageUrl": PortalEnvironment.LogoImageUrl = "/images/" + fileName; break;
                            case "SubLogoImageUrl": PortalEnvironment.SubLogoImageUrl = "/images/" + fileName; break;
                            case "BannerImageUrl": PortalEnvironment.BannerImageUrl = "/images/" + fileName; break;
                            default:
                                break;
                        }
                        return Content("");
                    }
                    else
                    {
                        return Content("Only .jpg .gif .png files can be uploaded");
                    }

                }
            }
            return Content("files is Null");
        }

    }
}

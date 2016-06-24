using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;

using aZaaS.KStar.Facades;

namespace aZaaS.KStar
{
    public static class VersionGetter
    {
        const string VERSION_FILENAME = "version.txt";

        public static VersionInformation GetPublishVersionInfo()
        {
            var information = new VersionInformation();
           
            try
            {
                var versionText = File.ReadAllText(HttpContext.Current.Server.MapPath(string.Format("~/{0}", VERSION_FILENAME)));

                if (!string.IsNullOrEmpty(versionText))
                {
                    var infoTexts = versionText.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                    if (infoTexts.Length >= 2)
                    {
                        var versionLabel = infoTexts[0];

                        DateTime publishDate = DateTime.MaxValue;
                        DateTime.TryParse(infoTexts[1], out publishDate);

                        information.Version = versionLabel;
                        information.PublishDate = publishDate;
                        //info = string.Format("Version: {0} PublishDate:{1}", versionLabel, publishDate.ToString(PortalEnvironment.DateTimeFormat));
                    }
                }
            }
            catch{ }

            return information;
        }
    }

    public class VersionInformation
    {
        public string Version { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
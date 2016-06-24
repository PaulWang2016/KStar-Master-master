using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Helpers
{
    public static class UploaderOptions
    {
        const string ATTACHMENT_EXT_FILTER = "KSTARFORM:AttachmentExtFilter";
        const string ATTACHMENT_MAX_FILESIZE = "KSTARFORM:AttachmentMaxFileSize";
        const string ATTACHMENT_ALLOWED_TYPES = "KSTARFORM:AttachmentAllowedTypes";

        public static string ExtFilter
        {
            get
            {


                var extFilter = ConfigurationManager.AppSettings[ATTACHMENT_EXT_FILTER];

                return string.IsNullOrEmpty(extFilter) ? null : string.Format("'{0}'", extFilter);
            }
        }

        public static string AllowedTypes
        {
            get
            {
                var defaultTypes = "*";

                var allowedTypes = ConfigurationManager.AppSettings[ATTACHMENT_ALLOWED_TYPES];

                return string.IsNullOrEmpty(allowedTypes) ? defaultTypes : allowedTypes;
            }
        }

        public static int MaxFileSize
        {
            get
            {
                var maxFileSize = 0;

                int.TryParse(ConfigurationManager.AppSettings[ATTACHMENT_MAX_FILESIZE], out maxFileSize);

                return maxFileSize;
            }
        }
    }
}

using aZaaS.KStar.Form.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace aZaaS.KStar.Form.Mvc
{
    public static class ControllerExtensions
    {
        public static int GetFormId(this ControllerBase controller)
        {
            int formId = 0;
            var sFormId = controller.ControllerContext.HttpContext.Request["_FormId"];

            if (sFormId == null)
            {
                sFormId = controller.ControllerContext.HttpContext.Request["_DraftId"];
            }

            int.TryParse(sFormId, out formId);

            return formId;
        }

        public static int GetDraftId(this ControllerBase controller)
        {
            int draftId = 0;
            var sDraftId = controller.ControllerContext.HttpContext.Request["_DraftId"];

            int.TryParse(sDraftId, out draftId);

            return draftId;
        }

        public static string GetShareUser(this ControllerBase controller)
        {
            var shareUser = controller.ControllerContext.HttpContext.Request["SharedUser"];

            return shareUser;
        }

        public static string GetSerialNo(this ControllerBase controller)
        {
            var serialNo = controller.ControllerContext.HttpContext.Request["SerialNo"];

            if (serialNo == null)
            {
                serialNo = controller.ControllerContext.HttpContext.Request["SN"];
            }

            if (!string.IsNullOrEmpty(serialNo))
            {
                var serialParts = serialNo.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (serialParts.Length != 2) { throw new FormatException("Invalid SerailNo format!"); }
            }

            return serialNo;
        }

        public static string GeProcSerialNo(this ControllerBase controller)
        {
            var serialNo = controller.ControllerContext.HttpContext.Request["_ProcSN"];

            if (!string.IsNullOrEmpty(serialNo))
            {
                var serialParts = serialNo.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                if (serialParts.Length != 2) { throw new FormatException("Invalid _FormSN format!"); }
            }

            return serialNo;
        }

        public static int GetActivityId(this ControllerBase controller)
        {
            int activityId = 0;
            var sActivityId = controller.ControllerContext.HttpContext.Request["ActivityId"];

            int.TryParse(sActivityId, out activityId);

            return activityId;
        }

        public static Guid GetFormCcId(this ControllerBase controller)
        {
            var ccid = controller.ControllerContext.HttpContext.Request["CcId"];

            Guid ccGuid;
            Guid.TryParse(ccid, out ccGuid);

            return ccGuid;
        }

        public static bool GetReviewStatus(this ControllerBase controller)
        {
            var sActivityId = controller.ControllerContext.HttpContext.Request["ActivityId"];

            if (!string.IsNullOrWhiteSpace(sActivityId))
            {
                return true;
            }

            return false;
        }

        public static int GetDestFormId(this ControllerBase controller)
        {
            int destFormId = 0;
            var sDestFormId = controller.ControllerContext.HttpContext.Request["_DestFormId"];

            int.TryParse(sDestFormId, out destFormId);

            return destFormId;
        }

        public static WorkMode GetWorkMode(this ControllerBase controller)
        {
            var workMode = WorkMode.View;

            if (!string.IsNullOrEmpty(controller.GetSerialNo()))
                workMode = WorkMode.Approval;
            else if (controller.GetDraftId() > 0)
                workMode = WorkMode.Draft;
            else if (controller.GetFormId() == 0 && string.IsNullOrEmpty(controller.GetSerialNo()))
                workMode = WorkMode.Startup;
            else if (controller.GetFormId() > 0 && controller.GetReviewStatus())
                workMode = WorkMode.Review;
            return workMode;
        }

        public static bool IsControlSetting(this ControllerBase controller)
        {
            bool flag = false;

            var _iscontrolsetting = controller.ControllerContext.HttpContext.Request["IsControlSetting"];
            if (!string.IsNullOrEmpty(_iscontrolsetting))
            {
                bool.TryParse(_iscontrolsetting, out flag);
            }
            return flag;
        }

        public static WorkMode ControlSettingMode(this ControllerBase controller)
        {
            int mode = 0;
            var sMode = controller.ControllerContext.HttpContext.Request["ControlSettingMode"];

            int.TryParse(sMode, out mode);

            return (WorkMode)mode;
        }

        public static string GetTemplateContent(this ControllerBase controller)
        {
            var _activityid = controller.ControllerContext.HttpContext.Request["ActivityId"];

            if (!string.IsNullOrEmpty(_activityid))
            {
            }

            string url = "http://127.0.0.1:86/BusinessTrip/Travel/?IsControlSetting=true";
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
            string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
            return pageHtml;
        }

    }
}

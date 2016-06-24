
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Controllers;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [Obsolete("该模块目前在平台中已经不使用，标记为废弃模块！")]
    [EnhancedHandleError]
    public class TDConfigController : BaseMvcController
    {
        //private readonly WorkflowDataService tenantDatabaseService;
        //public TDConfigController()
        //{
        //    tenantDatabaseService = new WorkflowDataService(this.AuthType);
        //}

        //public JsonResult Save([ModelBinder(typeof(JsonListBinder<ConfigurationEntity>))]List<ConfigurationEntity> configs)
        //{
        //    tenantDatabaseService.SetConfigurationList(configs);

        //    return Json("Success！", JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult Get()
        //{
        //    var items = tenantDatabaseService.GetConfigurationList();

        //    return Json(items, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetUploadFileMaxSize()
        //{
        //    return Json(tenantDatabaseService.GetConfigurationList().Where(s => s.ConfigKey == "UploadFileMaxSize").Select(s => s.ConfigValue).FirstOrDefault(), JsonRequestBehavior.AllowGet);
        //}
    }
}

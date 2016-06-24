using aZaaS.KStar;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Organization.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Models.ViewModel;
using aZaaS.KStar.Web.Models;
using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Web.Utilities;
using aZaaS.KStar.MgmtServices;
using aZaaS.KStar.MgmtDtos;
using aZaaS.Framework.Extend;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.Utilities;
using System.Reflection;
using System.Linq.Expressions;
using aZaaS.Framework.Workflow;
using aZaaS.KStar.Facades;
using aZaaS.KStar.Web.TenantDbService;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class TenantController : BaseMvcController
    {

        TenantManager.TenantManagerClient tenantservice;

        protected string Take
        {
            get
            {
                return Request["take"] ?? string.Empty;
            }
        }

        protected string Skip
        {
            get
            {
                return Request["skip"] ?? string.Empty;
            }
        }

        protected string Page
        {
            get
            {
                return Request["page"] ?? string.Empty;
            }
        }

        protected string PageSize
        {
            get
            {
                return Request["pageSize"] ?? string.Empty;
            }
        }

        protected string Sort
        {
            get
            {
                return Request["sort"] ?? string.Empty;
            }
        } 
        protected List<string> Filter
        {
            get
            {
                if (HttpContext.Request.Cookies.Get("TenantManaViewArrayInFilters") != null)
                {
                    string UserManaViewArrayInFilters = Server.UrlDecode(HttpContext.Request.Cookies.Get("TenantManaViewArrayInFilters").Value);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(UserManaViewArrayInFilters);
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        public TenantController()
        {
            tenantservice = new TenantManager.TenantManagerClient();
        }

        #region 获取 Tenant Management    获取租户列表
        public JsonResult GetTenants()
        {
            List<TenantManager.TenantEntity> tenant = new List<TenantManager.TenantEntity>();
            tenant = tenantservice.TenantInfoAllQuery().ToList();
            return Json(tenant, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 增加租户
        /// <summary>
        /// 增加租户
        /// </summary>
        /// <param name="staffItem"></param>
        /// <returns></returns> 
        [HttpPost]
        public JsonResult AddTenant(string tenantId,string tenantName,DateTime expireDate)
        {            
            TenantManager.TenantEntity tenant=new TenantManager.TenantEntity();
            tenant.TenantID = tenantId;
            tenant.TenantName = tenantName;
            tenant.ExpireDate = expireDate;
            tenant.Status = true;
            tenantservice.TenantInfoAdd(tenant);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 判断租户编号是否已经使用
        public JsonResult ExistTenantId(string tenantId)
        { 
            bool flag = false;
            TenantManager.TenantEntity tenant = tenantservice.TenantInfoQuery(tenantId);
            if (tenant != null)
            {
                flag = true;
            }
            return Json(flag, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 批量停用 租户
        /// <summary>
        /// 批量停用 租户
        /// </summary>
        /// <param name="idList">租户idlist</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoDisableTenant(List<string> idList)
        {
            TenantManager.TenantEntity tenant;
            foreach (var item in idList)
            {
                tenant = tenantservice.TenantInfoQuery(item);
                if (tenant != null)
                {
                    tenant.Status = false;
                }
                tenantservice.TenantInfoModify(tenant);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 批量启用 租户
        /// <summary>
        /// 批量启用 租户
        /// </summary>
        /// <param name="idList">租户idlist</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoActiveTenant(List<string> idList)
        {
            TenantManager.TenantEntity tenant;
            foreach (var item in idList)
            {
                tenant = tenantservice.TenantInfoQuery(item);
                if (tenant != null)
                {
                    tenant.Status = true;
                }
                tenantservice.TenantInfoModify(tenant);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #endregion        
       
    }

 
}

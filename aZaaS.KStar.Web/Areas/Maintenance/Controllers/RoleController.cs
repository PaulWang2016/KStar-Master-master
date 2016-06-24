using aZaaS.KStar;
using aZaaS.Framework.Facade;
using aZaaS.KStar.Web.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aZaaS.KStar.Web.Controllers;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Workflow.Configuration;

namespace aZaaS.KStar.Web.Areas.Maintenance.Controllers
{
    [EnhancedHandleError]
    public class RoleController : BaseMvcController
    {
        RoleBO roleBO = new RoleBO();

        #region RoleManagement
        public JsonResult DelRole(List<string> idList)
        {
            var svc = new ConfigManager(this.AuthType);
            svc.TenantID = TenantID();
            foreach (var item in idList)
            {
                roleBO.DeleteRole(Guid.Parse(item));                
                try
                {
                    svc.DeleteStartUserBySysId(Guid.Parse(item), Configuration_UserType.Role);
                }
                catch (Exception ex) { }
            }
            foreach (var item in idList)
            {
                if (roleBO.ReadRole(Guid.Parse(item)) != null)
                {
                    return Json(new List<string>(), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(idList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRole(Role item)
        {
            var roleitem = roleBO.ReadRole(Guid.Parse(item.ID));
            roleitem.Name = item.Name;
            roleBO.UpdateRole(roleitem);
            var afterupdaterole = roleBO.ReadRole(Guid.Parse(item.ID));
            return Json(new Role { ID = afterupdaterole.SysID.ToString(), Name = afterupdaterole.Name }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CreateRole(Role item)
        {
            var newRoleID = roleBO.CreateRole(new RoleWithRelationshipsDto { SysID = Guid.NewGuid(), Name = item.Name });
            var newRole = roleBO.ReadRole(newRoleID);
            return Json(new Role { ID = newRole.SysID.ToString(), Name = newRole.Name }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRole()
        {
            List<Role> roleList = new List<Role>();
            foreach (var item in roleBO.GetAllRoles())
            {
                roleList.Add(new Role { ID = item.SysID.ToString(), Name = item.Name });
            }
            return Json(roleList, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}

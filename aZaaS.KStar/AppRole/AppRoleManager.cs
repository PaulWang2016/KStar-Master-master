using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Acs;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Repositories;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.AppRole
{
    public class AppRoleManager
    {
        public List<AppRoleDTO> GetRoleByPane(string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<AppRoleDTO> appRoleList = new List<AppRoleDTO>();
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuid != null)
                {

                    var roles = context.AppRole.Where(m => m.MenuId == menuid).ToList();
                    foreach (var role in roles)
                    {
                        //RoleBO roleBO = new RoleBO();
                        RoleService roleService = new RoleService();
                        var roleItem = roleService.ReadRoleBase(role.RoleId);
                        appRoleList.Add(new AppRoleDTO() { DisplayName = roleItem.Name, RoleID = roleItem.SysID });
                    }
                }
                return appRoleList;
            }
        }


        public void AddAppRole(Guid id, string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                var appRole = context.AppRole.Where(m => m.MenuId == menuid && m.RoleId == id).FirstOrDefault();
                if (appRole != null)
                    return;//已经存在
                if (menuid != null)
                {
                    context.AppRole.Add(new AppRoleEntity() { RoleId = id, MenuId = menuid });
                    context.SaveChanges();
                }
            }
        }

        public void DelAppRoleByListID(List<Guid> ListID, string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuid = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuid != null)
                    foreach (var id in ListID)
                    {
                        DelAuthorByRoleInPane(id, menuid);   //删除权限

                        var appRoleItem = context.AppRole.Where(m => m.RoleId == id && m.MenuId == menuid).FirstOrDefault();
                        if (appRoleItem != null)
                        {
                            context.AppRole.Remove(appRoleItem);
                        }
                    }
                context.SaveChanges();
            }
        }

        public void DelAuthorByRoleInPane(Guid roleID, Guid menuID)
        {

            IAccessControlService acs = new AccessControlService("aZaaSKStar") { AuthorityProvider = new AuthorityProvider() };
            using (KStarDbContext context = new KStarDbContext())
            {
                var menu = context.Menu.Where(m => m.Id == menuID).Select(s => s.Id).ToList();
                var menuItem = context.MenuItem.Where(m => m.MenuID == menuID).Select(s => s.Id).ToList();
                var documentLibrary = context.DocumentLibrary.Where(m => m.MenuID == menuID).Select(s => s.ID).ToList();
                var documentItem = context.DocumentItem.Where(m => documentLibrary.Contains(m.DocumentLibraryID)).Select(s => s.Id).ToList();
                var resourceIds = menu
                   .Union(menuItem)
                   .Union(documentLibrary)
                   .Union(documentItem).ToList();
                var resourcePermissions = acs.ResourcePermissions().Where(m => resourceIds.Contains(Guid.Parse(m.ResourceId))).ToList();
                foreach (var item in resourcePermissions)
                {
                    var authorization = acs.AuthorizationMatrixes().Where(m => m.ResourcePermissionSysId == item.SysId && m.AuthorityId == roleID).FirstOrDefault();
                    if (authorization != null)
                        acs.RemoveAuthorizationMatrixes(authorization);
                    //if (acs.AuthorizationMatrixes().Where(m => m.ResourcePermissionSysId == item.SysId).FirstOrDefault() == null)
                    //    acs.RemoveResourcePermissions(item);
                }

            }

        }
    }

}

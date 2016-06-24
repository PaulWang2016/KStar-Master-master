using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Acs.Models;
using aZaaS.KStar.DTOs;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.Resources;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Menus;
using aZaaS.KStar.Documents;
using aZaaS.KStar.MgmtServices;

namespace aZaaS.KStar.Acs
{
    public class AcsManager
    {
        public static Func<string> ConnectionSetter;
        private IAccessControlService acs = new AccessControlService(ConnectionSetter()) { AuthorityProvider = new AuthorityProvider() };
        private readonly PositionService positionService;
        private readonly UserService userService;
        private readonly RoleService roleService;
        #region AuthorizationController
        public AcsManager()
        {
            this.positionService = new PositionService();
            this.userService = new UserService();
            this.roleService = new RoleService();
        }
        public List<AuthorizationMatrices> GetAllAuthorization()
        {

            List<AuthorizationMatrices> items;

            items = acs.AuthorizationMatrixes().Select(s => new AuthorizationMatrices()
            {
                SysId = s.SysId,
                AuthorityId = s.AuthorityId,
                Granted = s.Granted,
                ResourcePermissionID = s.ResourcePermissionSysId,
            }).ToList();
            //UserBO user = new UserBO();
            //RoleBO role = new RoleBO();
            foreach (var item in items)
            {
                var temp = GetResourcePermission(item.ResourcePermissionID);
                item.Resource = temp.Resource;
                item.Permission = temp.Permission;
                item.ResourceType = temp.ResourceType;
                item.UserOrRole = userService.GetAllUsers().Where(m => m.SysID == item.AuthorityId).Select(m => new { Name = m.FirstName + " " + m.LastName })
                                        .Union(roleService.GetAllRoles().Where(m => m.SysID == item.AuthorityId).Select(m => new { Name = m.Name })).FirstOrDefault().Name;
                //user.GetAllUsers().Where(m => m.Id == item.AuthorityId).Select(m => new { Name = m.FirstName + " " + m.LastName })
                //                           .Union(role.GetAllRoles().Where(m => m.Id == item.AuthorityId).Select(m => new { Name = m.Name })).FirstOrDefault().Name;
            }

            return items;
        }
        public AuthorizationMatrices GetAuthorization(Guid id)
        {
            AuthorizationMatrices item;

            item = acs.AuthorizationMatrixes().Where(m => m.SysId == id).Select(s => new AuthorizationMatrices()
            {
                AuthorityId = s.AuthorityId,
                Granted = s.Granted,
                ResourcePermissionID = s.ResourcePermissionSysId,
            }).FirstOrDefault();
            UserBO user = new UserBO();
            //RoleBO role = new RoleBO();
            //var temp = context.ResourcePermissions.Where(m => m.SysId == item.ResourcePermissionID).FirstOrDefault();
            item.Resource = GetResourcePermission(item.ResourcePermissionID).Resource;
            item.Permission = GetResourcePermission(item.ResourcePermissionID).Permission;
            item.UserOrRole = userService.GetAllUsers().Where(m => m.SysID == item.AuthorityId).Select(m => new { Name = m.FirstName + " " + m.LastName })
                   .Union(roleService.GetAllRoles().Where(m => m.SysID == item.AuthorityId).Select(m => new { Name = m.Name })).FirstOrDefault().Name;
            //user.GetAllUsers().Where(m => m.Id == item.AuthorityId).Select(m => new { Name = m.FirstName + " " + m.LastName })
            //                    .Union(role.GetAllRoles().Where(m => m.Id == item.AuthorityId).Select(m => new { Name = m.Name })).FirstOrDefault().Name;


            return item;
        }
        //private ResourcePermissionView _GetResourcePermission(Guid id)
        //{
        //    ResourcePermissionView item = new ResourcePermissionView();

        //    var temp = acs.ResourcePermissions().Where(s => s.SysId == id).FirstOrDefault();

        //    item.PermissionSysId = temp.PermissionSysId;
        //    item.ResourceId = temp.ResourceId.ToString();
        //    item.SysId = temp.SysId;
        //    item.ResourceType = temp.ResourceType;
        //    MenuFacade menu = new MenuFacade();
        //    DocumentFacade Doc = new DocumentFacade();
        //    WidgetFacade widget = new WidgetFacade();
        //    switch (item.ResourceType)
        //    {
        //        case "MenuItem":
        //            var MenuItem = menu.GetMenuItem(Guid.Parse(item.ResourceId));
        //            if (MenuItem != null)
        //                item.Resource = MenuItem.DisplayName;
        //            break;
        //        case "Menu":
        //            var Menu = menu.GetMenu(Guid.Parse(item.ResourceId));
        //            if (Menu != null)
        //                item.Resource = Menu.DisplayName;
        //            break;
        //        case "DocumentLibrary":
        //            var DocumentLibrary = Doc.GetDocumentLibrary(Guid.Parse(item.ResourceId));
        //            if (DocumentLibrary != null)
        //                item.Resource = DocumentLibrary.DisplayName;
        //            break;
        //        case "DocumentItem":
        //            var DocumentItem = Doc.GetDocumentItem(Guid.Parse(item.ResourceId));
        //            if (DocumentItem != null)
        //                item.Resource = DocumentItem.DisplayName;
        //            break;
        //        case "Widget":
        //            var Widget = widget.GetWidget(Guid.Parse(item.ResourceId));
        //            if (Widget != null)
        //                item.Resource = Widget.Title;
        //            break;
        //        default: break;
        //    }

        //    item.Permission = acs.Permissions().Where(m => m.SysId == item.PermissionSysId).Select(m => m.Name).FirstOrDefault();

        //    return item;
        //}


        public List<UserAndRole> GetUserAndRole()
        {
            //UserBO user = new UserBO();
            //RoleBO role = new RoleBO();
            return userService.GetAllUsers().Select(m => new UserAndRole { Displayname = m.FirstName + " " + m.LastName, Id = m.SysID, Type = "User" })
               .Union(roleService.GetAllRoles().Select(m => new UserAndRole { Displayname = m.Name, Id = m.SysID, Type = "Role" })).ToList();
            //return user.GetAllUsers().Select(m => new UserAndRole { Displayname = m.FirstName + " " + m.LastName, Id = m.Id, Type = "User" })
            //     .Union(role.GetAllRoles().Select(m => new UserAndRole { Displayname = m.Name, Id = m.Id, Type = "Role" })).ToList();
        }
        public List<UserAndRole> FindUserAndRole(string input)
        {
            //UserBO user = new UserBO();
            //RoleBO role = new RoleBO();
            return userService.GetAllUsers().Where(m => m.LastName.Contains(input) || m.FirstName.Contains(input)).Select(m => new UserAndRole { Displayname = m.FirstName + " " + m.LastName, Id = m.SysID, Type = "User" })
                 .Union(roleService.GetAllRoles().Where(m => m.Name.Contains(input)).Select(m => new UserAndRole { Displayname = m.Name, Id = m.SysID, Type = "Role" })).ToList();
            //return user.GetAllUsers().Where(m => m.LastName.Contains(input) || m.FirstName.Contains(input)).Select(m => new UserAndRole { Displayname = m.FirstName + " " + m.LastName, Id = m.Id, Type = "User" })
            //   .Union(role.GetAllRoles().Where(m => m.Name.Contains(input)).Select(m => new UserAndRole { Displayname = m.Name, Id = m.Id, Type = "Role" })).ToList();
        }
        public List<AuthorizationMatrices> FindAuthori(string input)
        {
            return GetAllAuthorization().Where(m => m.ResourceType.Contains(input) || m.UserOrRole.Contains(input) || m.Resource.Contains(input) || m.Permission.Contains(input)).ToList();

        }
        public List<ResourcePermissionView> FindResourcePermisssion(string input)
        {
            List<ResourcePermissionView> list = new List<ResourcePermissionView>();
            var items = acs.ResourcePermissions().Select(s => new ResourcePermissionView()
            {
                Permission = s.Permission.Name,
                PermissionSysId = s.PermissionSysId,
                ResourceId = s.ResourceId,
                ResourceType = s.ResourceType,
                SysId = s.SysId
            });
            MenuFacade menu = new MenuFacade();
            DocumentFacade Doc = new DocumentFacade();
            WidgetFacade widget = new WidgetFacade();
            foreach (var item in items)
            {
                switch (item.ResourceType)
                {
                    case "MenuItem":
                        item.Resource = menu.GetMenuItem(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "Menu":
                        item.Resource = menu.GetMenu(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "DocumentLibrary":
                        item.Resource = Doc.GetDocumentLibrary(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "DocumentItem":
                        item.Resource = Doc.GetDocumentItem(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "Widget":
                        item.Resource = widget.GetWidget(Guid.Parse(item.ResourceId)).Title;
                        break;
                    case "": break;
                }
                if (item == null)
                {
                    var copy = item;
                }

                list.Add(item);

            }
            return list.Where(m => m.Resource.Contains(input) || m.Permission.Contains(input) || m.ResourceType.Contains(input)).ToList();

        }
        public void CreateAuthorization(Guid AuthorityId, bool Granted, List<Guid> ResourcePermissionIdList)
        {
            //UserBO user = new UserBO();
            foreach (var item in ResourcePermissionIdList)
            {
                acs.AddAuthorizationMatrixes(new AuthorizationMatrix()
                {
                    SysId = Guid.NewGuid(),
                    AuthorityId = AuthorityId,
                    AuthorityType = userService.ReadUser(AuthorityId) == null ? (int)AuthorityType.Role : (int)AuthorityType.User,
                    Granted = Granted,
                    ResourcePermissionSysId = item
                });
            }
        }
        public void UpdateAuthorization(AuthorizationMatrix item)
        {
            //UserBO user = new UserBO();
            acs.UpdateAuthorizationMatrixes(new AuthorizationMatrix()
            {
                SysId = item.SysId,
                AuthorityId = item.AuthorityId,
                AuthorityType = userService.ReadUser(item.AuthorityId) == null ? (int)AuthorityType.Role : (int)AuthorityType.User,
                Granted = item.Granted,
                ResourcePermissionSysId = item.ResourcePermissionSysId
            });
        }
        public void DelAuthorization(List<string> idList)
        {
            foreach (var id in idList)
                acs.RemoveAuthorizationMatrixes(Guid.Parse(id));
        }

        public void DisableAuthorization(List<string> idList)
        {
            foreach (var id in idList)
            {
                acs.UpdateAuthorizationMatrix(Guid.Parse(id), false);
            }
        }
        public void EnableAuthorization(List<string> idList)
        {
            foreach (var id in idList)
            {
                acs.UpdateAuthorizationMatrix(Guid.Parse(id), true);
            }
        }




        #endregion

        public List<string> GetResourceNameById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Menu.Where(m => m.Id == id).Select(s => s.DisplayName).ToList()
                    .Union(context.MenuItem.Where(m => m.Id == id).Select(s => s.DisplayName).ToList())
                    .Union(context.DocumentItem.Where(m => m.Id == id).Select(s => s.DisplayName).ToList())
                    .Union(context.DocumentLibrary.Where(m => m.ID == id).Select(s => s.DisplayName).ToList())
                    .Union(context.DynamicWidget.Where(m => m.ID == id).Select(s => s.DisplayName).ToList()).ToList();
            }
        }
        #region ResourcePermission
        public List<ResourcePermissionView> GetAllResourcePermission()
        {

            List<ResourcePermissionView> list = new List<ResourcePermissionView>();
            var items = acs.ResourcePermissions().Select(s => new ResourcePermissionView()
            {
                Permission = GetResourceNameById(Guid.Parse(s.ResourceId)).FirstOrDefault(),
                PermissionSysId = s.PermissionSysId,
                ResourceId = s.ResourceId,
                ResourceType = s.ResourceType,
                SysId = s.SysId
            });
            MenuFacade menu = new MenuFacade();
            DocumentFacade Doc = new DocumentFacade();
            WidgetFacade widget = new WidgetFacade();
            foreach (var item in items)
            {
                switch (item.ResourceType)
                {
                    case "MenuItem":
                        item.Resource = menu.GetMenuItem(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "Menu":
                        item.Resource = menu.GetMenu(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "DocumentLibrary":
                        item.Resource = Doc.GetDocumentLibrary(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "DocumentItem":
                        item.Resource = Doc.GetDocumentItem(Guid.Parse(item.ResourceId)).DisplayName;
                        break;
                    case "Widget":
                        item.Resource = widget.GetWidget(Guid.Parse(item.ResourceId)).Title;
                        break;
                    case "": break;
                }
                list.Add(item);
            }
            return list;

        }
        public ResourcePermissionView GetResourcePermission(Guid id)
        {
            ResourcePermissionView item;

            item = acs.ResourcePermissions().Where(s => s.SysId == id).Select(s => new ResourcePermissionView()
            {
                PermissionSysId = s.PermissionSysId,
                ResourceId = s.ResourceId.ToString(),
                SysId = s.SysId,
                ResourceType = s.ResourceType
            }).FirstOrDefault();
            MenuFacade menu = new MenuFacade();
            DocumentFacade Doc = new DocumentFacade();
            WidgetFacade widget = new WidgetFacade();
            switch (item.ResourceType)
            {
                case "MenuItem":
                    var MenuItem = menu.GetMenuItem(Guid.Parse(item.ResourceId));
                    if (MenuItem != null)
                        item.Resource = MenuItem.DisplayName;
                    break;
                case "Menu":
                    var Menu = menu.GetMenu(Guid.Parse(item.ResourceId));
                    if (Menu != null)
                        item.Resource = Menu.DisplayName;
                    break;
                case "DocumentLibrary":
                    var DocumentLibrary = Doc.GetDocumentLibrary(Guid.Parse(item.ResourceId));
                    if (DocumentLibrary != null)
                        item.Resource = DocumentLibrary.DisplayName;
                    break;
                case "DocumentItem":
                    var DocumentItem = Doc.GetDocumentItem(Guid.Parse(item.ResourceId));
                    if (DocumentItem != null)
                        item.Resource = DocumentItem.DisplayName;
                    break;
                case "Widget":
                    var Widget = widget.GetWidget(Guid.Parse(item.ResourceId));
                    if (Widget != null)
                        item.Resource = Widget.Title;
                    break;
                default: break;
            }
            item.Permission = acs.Permissions().Where(m => m.SysId == item.PermissionSysId).Select(m => m.Name).FirstOrDefault();

            return item;
        }
        public List<Permission> FindPermission(string input)
        {
            return acs.Permissions().Where(m => m.Name.Contains(input)).ToList();
        }
        //SqlMethods.Like(p.Description, "%" + p.Customer.Name + "%"
        public void CreateResourcePermission(Guid PermissionSysId, string ResourceType, List<string> ResourceIdList)
        {
            ResourceManager re = new ResourceManager();
            foreach (var id in ResourceIdList)
            {
                ResourcePermission item = new ResourcePermission();
                item.SysId = Guid.NewGuid();
                item.PermissionSysId = PermissionSysId;
                item.ResourceType = re.GetResourceType(Guid.Parse(id));
                item.ResourceId = id;
                acs.AddResourcePermissions(item);
            }
        }
        public void DelResourcePermission(List<string> idList)
        {
            foreach (var id in idList)
                acs.RemoveResourcePermissions(Guid.Parse(id));
        }


        #endregion
        #region Permission
        public IEnumerable<Permission> GetPermission()
        {
            return acs.Permissions();
        }
        public void CreatePermission(Permission item)
        {
            item.SysId = Guid.NewGuid();
            acs.AddPermission(item);
        }
        public void UpdatePermission(Permission item)
        {
            acs.UpdatePermission(item);
        }
        public void DelPermission(List<string> idList)
        {
            foreach (var id in idList)
                acs.RemovePermission(Guid.Parse(id));
        }
        #endregion

        public List<string> AuthorityResourcesPermissionList(Guid roleID, string type)
        {
            Guid permissionId = acs.Permissions().Where(m => m.Name == type).FirstOrDefault().SysId;
            return acs.ResourcesAccessibleToAuthority(roleID, permissionId) == null ? new List<string>() : acs.ResourcesAccessibleToAuthority(roleID, permissionId).ToList();
        }
        public void ChangeStatus(string RoleID, string id, bool status)// string Type,
        {
            ResourceManager resourceManager = new ResourceManager();
            var permissionId = acs.Permissions().Where(m => m.Name == "Read").FirstOrDefault().SysId;

            var resourcePermission = acs.ResourcePermissions().Where(m => m.ResourceId == id && m.PermissionSysId == permissionId).FirstOrDefault();
            if (resourcePermission == null)
            {
                acs.AddResourcePermissions(new ResourcePermission() { SysId = Guid.NewGuid(), ResourceType = resourceManager.GetResourceType(Guid.Parse(id)), PermissionSysId = permissionId, ResourceId = id });
                resourcePermission = acs.ResourcePermissions().Where(m => m.ResourceId == id && m.PermissionSysId == permissionId).FirstOrDefault();
            }
            var item = acs.AuthorizationMatrixes().Where(m => m.AuthorityId == Guid.Parse(RoleID) && m.ResourcePermissionSysId == resourcePermission.SysId).FirstOrDefault();

            if (item != null && !status)
            {
                acs.RemoveAuthorizationMatrixes(item.SysId);
                return;
            }
            
            if (item == null && status)
            {
                var guidResourcePermission = acs.ResourcePermissions().Where(m => m.PermissionSysId == permissionId && m.ResourceId == id).Select(s => s.SysId).FirstOrDefault();
                acs.AddAuthorizationMatrixes(new AuthorizationMatrix() { Granted = true, AuthorityId = Guid.Parse(RoleID), AuthorityType = (int)AuthorityType.Role, SysId = Guid.NewGuid(), ResourcePermissionSysId = guidResourcePermission });
            }
            //else
            //{
            //    item.Granted = status;
            //    acs.UpdateAuthorizationMatrixes(item);
            //}
        }

        public void DelAuthority(string id)
        {
            var resourcePermission = acs.ResourcePermissions().Where(m => m.ResourceId == id).FirstOrDefault();
            if (resourcePermission != null)
            {
                var authorization = acs.AuthorizationMatrixes().Where(m => m.ResourcePermissionSysId == resourcePermission.SysId).First();
                if (authorization != null)
                    acs.RemoveAuthorizationMatrixes(authorization);
                acs.RemoveResourcePermissions(resourcePermission);
            }

        }
        public void DelAuthority(List<string> idList)
        {
            foreach (var id in idList)
            {
                var resourcePermission = acs.ResourcePermissions().Where(m => m.ResourceId == id).FirstOrDefault();
                if (resourcePermission != null)
                {
                    var authorization = acs.AuthorizationMatrixes().Where(m => m.ResourcePermissionSysId == resourcePermission.SysId).First();
                    if (authorization != null)
                        acs.RemoveAuthorizationMatrixes(authorization);
                    acs.RemoveResourcePermissions(resourcePermission);
                }
            }

        }
        public void DelAuthorityCompletely(List<string> idList)
        {
            var resourcePermission = acs.ResourcePermissions().Where(m => idList.Contains(m.ResourceId)).ToList();
            if (resourcePermission != null)
            {
                var authorization = acs.AuthorizationMatrixes().Where(m => resourcePermission.Select(s => s.SysId).Contains(m.ResourcePermissionSysId)).ToList();
                if (authorization != null)
                    acs.RemoveAuthorizationMatrixes(authorization.ToArray());
                acs.RemoveResourcePermissions(resourcePermission.ToArray());
            }
        }


        public List<string> GetResourcePermissionIdListByUserName(string userName, string permissionName)
        {
            //UserBO user = new UserBO();
            Guid permissionId = acs.Permissions().Where(m => m.Name == permissionName).FirstOrDefault().SysId;
            var user=userService.ReadUserBase(userName);
            if (user != null)
            {
                var resources = acs.ResourcesAccessibleToAuthority(user.SysID,null, permissionId);
                if (resources != null)
                {
                    return resources.ToList();
                }
            }
            return new List<string>();
        }
        public List<Menu> AuthorityTop(string userName)
        {
            List<Menu> list = new List<Menu>();
            MenuManager menuManager = new MenuManager();
            List<string> ResourcePermissionIdList = GetResourcePermissionIdListByUserName(userName, "Read");
            var allMendus = menuManager.GetAllMenus();
            foreach (var item in allMendus)
            {
                if (ResourcePermissionIdList.Contains(item.Id.ToString()))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        public Menu AuthorityMenuBar(string menuKey, string userName)
        {
            MenuManager menuManager = new MenuManager();
            var model = menuManager.GetMenuById(menuManager.GetMenuIdByKey(menuKey));
            List<string> ResourcePermissionIdList = GetResourcePermissionIdListByUserName(userName, "Read");
            if (!ResourcePermissionIdList.Contains(model.Id.ToString()))
            {
                model.Items = new List<MenuItem>();
            }
            else
            {
                List<MenuItem> MenuItems = new List<MenuItem>();
                foreach (var item in model.Items)
                {
                    if (ResourcePermissionIdList.Contains(item.Id.ToString()))
                    {
                        MenuItems.Add(item);
                    }
                }
                model.Items = MenuItems.OrderBy(s => s.MenuItemOrder).ToList(); ;
            }
            return model;
        }
        public DocumentLibrary AuthorityDocumentItem(string userName, string DocLibraryKey)
        {
            DocumentManager docManager = new DocumentManager();
            DocumentLibrary documentLibrary = docManager.GetDocLibraryById(docManager.GetDocLibraryIdByKey(DocLibraryKey));
            List<string> ResourcePermissionIdList = GetResourcePermissionIdListByUserName(userName, "Read");
            List<DocumentItem> items = new List<DocumentItem>();
            foreach (var item in documentLibrary.Items)
            {
                if (ResourcePermissionIdList.Contains(item.SysID.ToString()))
                {
                    items.Add(item);
                }
            }
            documentLibrary.Items = items;
            return documentLibrary;
        }
        public List<string> AuthorityDocumentLibrary(string userName, string MenuKey)
        {
            DocumentManager docManager = new DocumentManager();
            MenuManager menuManager = new MenuManager();
            IList<DocumentLibrary> items = docManager.GetDocLibraryByMenuId(menuManager.GetMenuIdByKey(MenuKey)).ToList();
            List<string> ResourcePermissionIdList = GetResourcePermissionIdListByUserName(userName, "Read");
            List<string> DocumentLibraryKey = new List<string>();

            foreach (var item in items)
            {
                if (ResourcePermissionIdList.Contains(item.ToString()))
                {
                    DocumentLibraryKey.Add(item.Key);
                }
            }

            return DocumentLibraryKey;
        }
        public DynamicWidget AuthorityDynamiWidget(string key, string userName)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<string> ResourcePermissionIdList = GetResourcePermissionIdListByUserName(userName, "Read");
                DynamicWidget item = context.DynamicWidget.Where(m => m.Key == key).Select(s => new DynamicWidget() { ID = s.ID, RazorContent = s.RazorContent, Key = s.Key, DisplayName = s.DisplayName, Description = s.Description, MenuID = s.MenuID }).FirstOrDefault();
                if (item != null && ResourcePermissionIdList.Contains(item.ID.ToString()))
                    return item;
                return null;
            }
        }
    }
}

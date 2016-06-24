using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Acs;
using aZaaS.KStar.Repositories;
using System.Data;
using aZaaS.KStar.Documents;
using aZaaS.KStar.AppRole;
using aZaaS.KStar.Acs.Models;
using aZaaS.Framework.ACS.Core;
using aZaaS.KStar.DynamicWidgets;

namespace aZaaS.KStar.Menus
{
    public class MenuManager
    {
        private AcsManager acsManager = new AcsManager();


        #region 已经使用下面的替代。
        //public Menu Get(string key)
        //{
        //    Menu model;
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var menuID = context.Menu.Where(m => m.Key == key).Select(m => m.Id).FirstOrDefault();
        //        model = context.Menu.Where(s => s.Key == key).Select(s => new Menu { Id = s.Id, Key = s.Key, DisplayName = s.DisplayName }).FirstOrDefault();
        //        var menu = context.MenuItem.Where(m => m.MenuID == menuID).OrderBy(s => s.MenuItemOrder);
        //        model.Items = menu.Where(s => s.KindValue == (int)MenuItemKind.Catelog).Select(s => new MenuItem() { Id = s.Id, DisplayName = s.DisplayName, Hyperlink = s.Hyperlink, IconKey = s.IconKey, Kind = (MenuItemKind)s.KindValue, Target = (MenuTargetType)s.TargetValue }).ToList();

        //        foreach (var menuItem in menu.Where(s => s.KindValue == (int)MenuItemKind.Item))
        //        {
        //            model.Items.Add(new MenuItem { Id = menuItem.Id, Hyperlink = menuItem.Hyperlink, Scope = menuItem.Scope, IconKey = menuItem.IconKey, Kind = menuItem.Kind, Target = menuItem.Target, DisplayName = menuItem.DisplayName, Parent = model.Items.SingleOrDefault(p => p.Id == menuItem.ParentId.Value) });
        //        }
        //    }
        //    return model;
        //}
        //public List<Menu> GetTop()
        //{
        //    List<Menu> model = new List<Menu>();
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        model = context.Menu.Select(s => new Menu { Id = s.Id, Key = s.Key, DisplayName = s.DisplayName, DefaultPage = s.DefaultPage }).ToList();
        //    }

        //    return model;
        //}
        //public List<string> DelMenuByKey(List<string> keyList)  //key
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        AcsManager acsManager = new AcsManager();

        //        List<string> keys = new List<string>();
        //        List<string> resourceIdList = new List<string>();
        //        foreach (var key in keyList)
        //        {
        //            var entity = context.Menu.Where(s => s.Key == key).FirstOrDefault();
        //            if (entity != null)
        //            {

        //                foreach (var item in context.AppRole.Where(s => s.MenuId == entity.Id))
        //                {
        //                    context.AppRole.Remove(item);
        //                }
        //                var items = context.MenuItem.Where(s => s.MenuID == entity.Id);
        //                foreach (var item in items)
        //                {
        //                    resourceIdList.Add(item.Id.ToString());
        //                    context.MenuItem.Remove(item);
        //                }
        //                var DocumentLibrarys = context.DocumentLibrary.Where(m => m.MenuID == entity.Id).ToList();

        //                foreach (var DocumentLibrary in DocumentLibrarys)
        //                {
        //                    var documentItems = context.DocumentItem.Where(s => s.DocumentLibraryID == DocumentLibrary.ID).ToList();
        //                    foreach (var item in documentItems)
        //                    {
        //                        resourceIdList.Add(item.Id.ToString());
        //                        context.DocumentItem.Remove(item);
        //                    }
        //                    resourceIdList.Add(DocumentLibrary.ID.ToString());
        //                    context.DocumentLibrary.Remove(DocumentLibrary);
        //                }
        //                var dynamicWidgets = context.DynamicWidget.Where(m => m.MenuID == entity.Id).ToList();
        //                foreach (var dynamicWidget in dynamicWidgets)
        //                {
        //                    resourceIdList.Add(dynamicWidget.ID.ToString());
        //                    context.DynamicWidget.Remove(dynamicWidget);
        //                }
        //                resourceIdList.Add(entity.Id.ToString());
        //                context.Menu.Remove(entity);
        //                keys.Add(key);
        //            }
        //        }
        //        context.SaveChanges();
        //        acsManager.DelAuthority(resourceIdList);
        //        return keys;
        //    }
        //}
        //public List<string> DelMenuById(List<string> idList)  //id
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        AcsManager acsManager = new AcsManager();

        //        List<string> ids = new List<string>();
        //        List<string> resourceIdList = new List<string>();
        //        foreach (var id in idList)
        //        {
        //            Guid guid = Guid.Parse(id);

        //            var entity = context.Menu.Where(s => s.Id == guid).FirstOrDefault();
        //            if (entity != null)
        //            {
        //                foreach (var item in context.AppRole.Where(s => s.MenuId == entity.Id))
        //                {
        //                    context.AppRole.Remove(item);
        //                }
        //                var items = context.MenuItem.Where(s => s.MenuID == entity.Id);
        //                foreach (var item in items)
        //                {
        //                    resourceIdList.Add(item.Id.ToString());
        //                    context.MenuItem.Remove(item);
        //                }
        //                var DocumentLibrarys = context.DocumentLibrary.Where(m => m.MenuID == entity.Id).ToList();
        //                foreach (var DocumentLibrary in DocumentLibrarys)
        //                {
        //                    var documentItems = context.DocumentItem.Where(s => s.DocumentLibraryID == DocumentLibrary.ID).ToList();
        //                    foreach (var item in documentItems)
        //                    {
        //                        resourceIdList.Add(item.Id.ToString());
        //                        context.DocumentItem.Remove(item);
        //                    }
        //                    resourceIdList.Add(DocumentLibrary.ID.ToString());
        //                    context.DocumentLibrary.Remove(DocumentLibrary);
        //                }
        //                var dynamicWidgets = context.DynamicWidget.Where(m => m.MenuID == entity.Id).ToList();
        //                foreach (var dynamicWidget in dynamicWidgets)
        //                {
        //                    resourceIdList.Add(dynamicWidget.ID.ToString());
        //                    context.DynamicWidget.Remove(dynamicWidget);
        //                }
        //                resourceIdList.Add(entity.Id.ToString());
        //                context.Menu.Remove(entity);
        //                ids.Add(id);
        //            }
        //        }
        //        context.SaveChanges();
        //        acsManager.DelAuthority(resourceIdList);
        //        return ids;
        //    }
        //}
        //public List<MenuItem> GetMenuItem(string key)
        //{
        //    List<MenuItem> model = new List<MenuItem>();
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var menuID = context.Menu.Where(m => m.Key == key).Select(m => m.Id).FirstOrDefault();
        //        model = context.MenuItem.Where(s => s.MenuID == menuID && s.KindValue == (int)MenuItemKind.Catelog)
        //            .Select(s => new MenuItem
        //            {
        //                Id = s.Id,
        //                Scope = s.Scope,
        //                DisplayName = s.DisplayName,
        //                Hyperlink = s.Hyperlink,
        //                IconKey = s.IconKey,
        //                Kind = (MenuItemKind)s.KindValue,
        //                Target = (MenuTargetType)s.TargetValue
        //            }).ToList();
        //    }

        //    return model;
        //}
        //public List<MenuItem> GetMenuItem(string key, string ParentId)
        //{
        //    List<MenuItem> model = new List<MenuItem>();
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var menuID = context.Menu.Where(m => m.Key == key).Select(m => m.Id).FirstOrDefault();
        //        Guid? parentID = null;
        //        if (ParentId != "")
        //            parentID = Guid.Parse(ParentId);
        //        var items = context.MenuItem.Where(s => s.MenuID == menuID && s.ParentId == parentID).ToList();
        //        MenuItem Parent = context.MenuItem.Where(s => s.Id == parentID)
        //            .Select(s => new MenuItem
        //            {
        //                Scope = s.Scope,
        //                Id = s.Id,
        //                DisplayName = s.DisplayName,
        //                Hyperlink = s.Hyperlink,
        //                IconKey = s.IconKey,
        //                Kind = (MenuItemKind)s.KindValue,
        //                Target = (MenuTargetType)s.TargetValue
        //            }).FirstOrDefault();
        //        foreach (var item in items)
        //        {
        //            model.Add(new MenuItem()
        //            {
        //                Id = item.Id,
        //                DisplayName = item.DisplayName,
        //                Hyperlink = item.Hyperlink,
        //                IconKey = item.IconKey,
        //                Kind = item.Kind,
        //                Target = item.Target,
        //                Parent = Parent
        //            });
        //        }
        //    }

        //    return model;
        //}
        //public MenuItem CreateMenuItem(MenuItem menuItem, string MenuKey)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        Guid? paretnid = null;
        //        if (menuItem.Parent != null)
        //            paretnid = menuItem.Parent.Id;
        //        var menuID = context.Menu.Where(m => m.Key == MenuKey).Select(m => m.Id).FirstOrDefault();
        //        var menuitem = context.MenuItem.Add(new MenuItemEntity
        //           {
        //               Id = Guid.NewGuid(),
        //               DisplayName = menuItem.DisplayName,
        //               IconKey = menuItem.IconKey,
        //               Hyperlink = menuItem.Hyperlink,
        //               KindValue = (int)menuItem.Kind,
        //               TargetValue = (int)menuItem.Target,
        //               ParentId = paretnid,
        //               MenuID = menuID
        //           });
        //        context.SaveChanges();


        //        var parentitem = context.MenuItem.Where(m => m.Id == menuitem.ParentId).Select(s =>
        //            new MenuItem
        //            {
        //                DisplayName = s.DisplayName,
        //                Id = s.Id,
        //                Hyperlink = s.Hyperlink,
        //                Kind = (MenuItemKind)s.KindValue,
        //                Target = (MenuTargetType)s.TargetValue,
        //                IconKey = s.IconKey
        //            }).FirstOrDefault();
        //        var result = context.MenuItem.Where(m => m.Id == menuitem.Id).Select(s => new MenuItem
        //        {
        //            DisplayName = s.DisplayName,
        //            Id = s.Id,
        //            Hyperlink = s.Hyperlink,
        //            Kind = (MenuItemKind)s.KindValue,
        //            Target = (MenuTargetType)s.TargetValue,
        //            IconKey = s.IconKey
        //        }).FirstOrDefault();
        //        result.Parent = parentitem;
        //        return result;  //, Parent => parentitem
        //    }

        //}
        //public void DeleteMenuItem(string menuItemID)
        //{
        //    //using (KStarDbContext context = new KStarDbContext())
        //    //{
        //    //    DelMenuItems(context, Guid.Parse(menuItemID));
        //    //    context.SaveChanges();
        //    //}
        //}
        ///// <summary>
        ///// 递归删除Menu Item
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="menuItemID"></param>
        //private void DelMenuItems(KStarDbContext context, Guid menuItemID)
        //{
        //    AcsManager acsManager = new AcsManager();

        //    var model = context.MenuItem.Where(s => s.Id == menuItemID).FirstOrDefault();
        //    if (model != null)
        //    {

        //        var modelsun = context.MenuItem.Where(s => s.ParentId == menuItemID).ToList();
        //        foreach (var sun in modelsun)
        //        {
        //            DelMenuItems(context, sun.Id);
        //        }
        //        acsManager.DelAuthority(model.Id.ToString());
        //        context.MenuItem.Remove(model);
        //    }
        //}
        ////public void DeleteMenuItem(List<int> menuItemID)
        ////{
        ////    using (KStarDbContext context = new KStarDbContext())
        ////    {
        ////        foreach (var item in menuItemID)
        ////        {
        ////            var model = context.MenuItem.Where(s => s.Id == item).FirstOrDefault();
        ////            if (model != null)
        ////            {
        ////                if (model.ParentId == null)
        ////                {
        ////                    var modelsun = context.MenuItem.Where(s => s.ParentId == item).ToList();
        ////                    foreach (var sun in modelsun)
        ////                        context.MenuItem.Remove(sun);
        ////                }
        ////                context.MenuItem.Remove(model);
        ////            }
        ////        }
        ////        context.SaveChanges();

        ////    }
        ////}
        //public Menu GetMenu(string key)
        //{
        //    Menu model = new Menu();
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var menuID = context.Menu.Where(m => m.Key == key).Select(m => m.Id).FirstOrDefault();
        //        var menu = context.MenuItem.Where(m => m.MenuID == menuID).OrderBy(s => s.MenuItemOrder);
        //        model.Items = menu.Where(s => s.KindValue == (int)MenuItemKind.Catelog).Select(s => new MenuItem() { Id = s.Id, DisplayName = s.DisplayName, Hyperlink = s.Hyperlink, IconKey = s.IconKey, Kind = (MenuItemKind)s.KindValue }).ToList();
        //        foreach (var menuItem in menu.Where(s => s.KindValue == (int)MenuItemKind.Item))
        //        {
        //            /////******************id.value.tostring()
        //            model.Items.Add(new MenuItem { Id = menuItem.Id, Hyperlink = menuItem.Hyperlink, IconKey = menuItem.IconKey, Kind = menuItem.Kind, DisplayName = menuItem.DisplayName, Parent = model.Items.SingleOrDefault(p => p.Id == menuItem.ParentId.Value) });
        //        }
        //    }
        //    return model;
        //}
        //#region 用下面的那个替代
        ////public Guid GetMenuIdByKey(string key)
        ////{
        ////    using (KStarDbContext context = new KStarDbContext())
        ////    {
        ////        var menuID = context.Menu.Where(m => m.Key == key).Select(m => m.Id).FirstOrDefault();
        ////        return menuID;
        ////    }
        ////} 
        //#endregion
        //public Menu GetMenu(Guid id)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var item = context.Menu.Where(m => m.Id == id).Select(s => new Menu()
        //        {
        //            Id = s.Id,
        //            DisplayName = s.DisplayName
        //        }).FirstOrDefault();
        //        return item;
        //    }
        //}
        //public MenuItem GetMenuItem(Guid id)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var item = context.MenuItem.Where(m => m.Id == id).Select(s => new MenuItem()
        //        {
        //            Id = s.Id,
        //            DisplayName = s.DisplayName
        //        }).FirstOrDefault();
        //        return item;
        //    }
        //} 
        #endregion


        public DataSet GetApps(string pane)
        {
            DataSet ds = new DataSet();
            ListtoDataTableConverter converter = new ListtoDataTableConverter();
            using (KStarDbContext context = new KStarDbContext())
            {
                #region Menu
                List<MenuEntity> menus = new List<MenuEntity>();

                if (string.IsNullOrWhiteSpace(pane))
                {
                    menus = context.Menu.ToList();
                }
                else
                {
                    menus = context.Menu.Where(s => s.Key == pane).ToList();
                }

                ds.Tables.Add(converter.ToDataTable(menus));
                #endregion

                #region AppRole MenuItem DocumentLibrary And DynamicWidget
                List<AppRoleEntity> appRoles = new List<AppRoleEntity>();
                List<MenuItemEntity> menuItems = new List<MenuItemEntity>();
                List<DocumentLibraryEntity> documentLibrarys = new List<DocumentLibraryEntity>();
                List<DynamicWidgetEntity> dynamicWidgets = new List<DynamicWidgetEntity>();
                foreach (var menu in menus)
                {
                    appRoles.AddRange(context.AppRole.Where(s => s.MenuId == menu.Id).ToList());
                    menuItems.AddRange(context.MenuItem.Where(s => s.MenuID == menu.Id).ToList());
                    documentLibrarys.AddRange(context.DocumentLibrary.Where(s => s.MenuID == menu.Id).ToList());
                    dynamicWidgets.AddRange(context.DynamicWidget.Where(m => m.MenuID == menu.Id).ToList());
                }
                ds.Tables.Add(converter.ToDataTable(appRoles));
                ds.Tables.Add(converter.ToDataTable(menuItems));
                ds.Tables.Add(converter.ToDataTable(documentLibrarys));
                ds.Tables.Add(converter.ToDataTable(dynamicWidgets));
                #endregion

                #region DocumentItem
                List<DocumentItemEntity> documentItems = new List<DocumentItemEntity>();
                foreach (var document in documentLibrarys)
                {
                    documentItems.AddRange(context.DocumentItem.Where(s => s.DocumentLibraryID == document.ID).ToList());
                }
                ds.Tables.Add(converter.ToDataTable(documentItems));
                #endregion

                #region APP权限获取
                IAccessControlService acs = new AccessControlService("aZaaSKStarDB") { AuthorityProvider = new AuthorityProvider() };
                var ResourceIds = menuItems.Select(s => s.Id.ToString()).ToList()
                    .Union(menus.Select(s => s.Id.ToString()).ToList())
                    .Union(documentItems.Select(s => s.Id.ToString()).ToList())
                    .Union(dynamicWidgets.Select(s => s.ID.ToString()).ToList())
                    .Union(documentLibrarys.Select(s => s.ID.ToString()).ToList()).ToList();
                List<Permission> permissions = acs.Permissions().ToList();
                List<ResourcePermission> ResourcePermissionsByApp = acs.ResourcePermissions().Where(m => ResourceIds.Contains(m.ResourceId)).ToList();
                var ResourcePermissionsIdsByApp = ResourcePermissionsByApp.Select(s => s.SysId).ToList();
                var appRolesIds = appRoles.Select(s => s.RoleId).ToList();
                List<AuthorizationMatrix> AuthorizationMatrixesByApp = acs.AuthorizationMatrixes().Where(m => ResourcePermissionsIdsByApp.Contains(m.ResourcePermissionSysId) && appRolesIds.Contains(m.AuthorityId)).ToList();

                ds.Tables.Add(converter.ToDataTable(permissions));
                ds.Tables.Add(converter.ToDataTable(ResourcePermissionsByApp));
                ds.Tables.Add(converter.ToDataTable(AuthorizationMatrixesByApp));
                #endregion
            }

            return ds;
        }
        public void SetApps(DataSet ds)
        {
            DataTabletoListConverter converter = new DataTabletoListConverter();
            using (KStarDbContext context = new KStarDbContext())
            {
                List<MenuEntity> menus = converter.ToList<MenuEntity>(ds.Tables["MenuEntity"]);

                foreach (var menu in menus)
                {
                    this.DelMenu(new List<Guid>() { menu.Id });
                    //this.DelMenuById(new List<string>() { menu.Id.ToString() });
                    context.Menu.Add(menu);
                }

                List<AppRoleEntity> appRoles = converter.ToList<AppRoleEntity>(ds.Tables["AppRoleEntity"]);
                foreach (var appRole in appRoles)
                {
                    context.AppRole.Add(appRole);
                }
                List<MenuItemEntity> menuItems = converter.ToList<MenuItemEntity>(ds.Tables["MenuItemEntity"]);
                foreach (var menuItem in menuItems)
                {
                    context.MenuItem.Add(menuItem);
                }
                List<DocumentLibraryEntity> documentLibrarys = converter.ToList<DocumentLibraryEntity>(ds.Tables["DocumentLibraryEntity"]);
                foreach (var documentLibrary in documentLibrarys)
                {
                    context.DocumentLibrary.Add(documentLibrary);
                }
                List<DocumentItemEntity> documentItems = converter.ToList<DocumentItemEntity>(ds.Tables["DocumentItemEntity"]);
                foreach (var documentItem in documentItems)
                {
                    context.DocumentItem.Add(documentItem);
                }
                List<DynamicWidgetEntity> dynamicWidget = converter.ToList<DynamicWidgetEntity>(ds.Tables["DynamicWidgetEntity"]);
                foreach (var dynamicWidgetItem in dynamicWidget)
                {
                    context.DynamicWidget.Add(dynamicWidgetItem);
                }

                context.SaveChanges();
                IAccessControlService acs = new AccessControlService("aZaaSKStarDB") { AuthorityProvider = new AuthorityProvider() };

                List<Permission> permissions = converter.ToList<Permission>(ds.Tables["Permission"]);
                foreach (var permission in permissions)
                {
                    var p = acs.GetPermission(permission.SysId);
                    if (p == null)
                    {
                        acs.AddPermission(permission);
                    }
                    else
                    {
                        p.Name = permission.Name;
                        p.Code = permission.Code;
                        p.Description = permission.Description;
                        acs.UpdatePermission(p);
                    }
                }
                List<ResourcePermission> ResourcePermissionsByApp = converter.ToList<ResourcePermission>(ds.Tables["ResourcePermission"]);
                foreach (var ResourcePermission in ResourcePermissionsByApp)
                {
                    acs.AddResourcePermissions(ResourcePermission);
                }
                List<AuthorizationMatrix> AuthorizationMatrixesByApp = converter.ToList<AuthorizationMatrix>(ds.Tables["AuthorizationMatrix"]);
                foreach (var AuthorizationMatrixe in AuthorizationMatrixesByApp)
                {
                    acs.AddAuthorizationMatrixes(AuthorizationMatrixe);
                }
            }
        }
        public string GetMenuDefaultPage(string key)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Menu.Where(m => m.Key == key).Select(m => m.DefaultPage).FirstOrDefault();
            }
        }
        public void SetDefaultPage(Guid id, bool statu = true)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var dywidget = context.DynamicWidget.Where(m => m.ID == id).FirstOrDefault();
                if (dywidget != null)
                {
                    var item = context.Menu.Where(m => m.Id == dywidget.MenuID).FirstOrDefault();
                    item.DefaultPage = "/DynamicWidget/" + dywidget.Key;
                    context.SaveChanges();
                }
            }
        }

        public Guid GetMenuIdByKey(string key)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return GetMenuIdByKey(key, context);
            }
        }

        private Guid GetMenuIdByKey(string key, KStarDbContext context)
        {
            return context.Menu.Where(m => m.Key == key).Select(m => m.Id).FirstOrDefault();
        }

        public List<Guid> GetMenuIdsByKeys(List<string> keys)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Menu.Where(m => keys.Contains(m.Key)).Select(m => m.Id).ToList();
            }
        }
        public string GetMenuKeyById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Menu.Where(m => m.Id == id).Select(m => m.Key).FirstOrDefault();
            }
        }
        public Menu GetMenuById(Guid id)//包括各层的MenuItem
        {

            //using (KStarDbContext context = new KStarDbContext())
            //{
            //    var model = context.Menu.Where(m => m.Id == id).Select(s => new Menu()
            //    {
            //        DefaultPage = s.DefaultPage,
            //        Key = s.Key,
            //        MenuOrder = s.MenuOrder,
            //        Id = s.Id,
            //        DisplayName = s.DisplayName
            //    }).FirstOrDefault();
            //    if (includeItems)
            //    {
            //        model.Items = context.MenuItem.Where(m => m.MenuID == model.Id).Select(s => new MenuItem()
            //        {
            //            Hyperlink = s.Hyperlink,
            //            IconKey = s.IconKey,
            //            MenuItemOrder = s.MenuItemOrder,
            //            MenuID = s.MenuID,
            //            Scope = s.Scope,
            //            Target = (MenuTargetType)s.TargetValue,
            //            Kind = (MenuItemKind)s.KindValue,
            //            Id = s.Id,
            //            DisplayName = s.DisplayName
            //        }).ToList();
            //    }
            //    foreach (var item in model.Items)
            //    {
            //        item.Parent = GetParentMenuItemById((Guid)item.Id); //虽然数据一样，但是不是相同的对象。因此没有效果。
            //    }
            //    return model;
            //}
            using (KStarDbContext context = new KStarDbContext())
            {
                var model = context.Menu.Where(m => m.Id == id).Select(s => new Menu { DefaultPage = s.DefaultPage, MenuOrder = s.MenuOrder, Id = s.Id, Key = s.Key, DisplayName = s.DisplayName }).FirstOrDefault();
                List<MenuItemEntity> menu = context.MenuItem.Where(m => m.MenuID == id).OrderBy(s => s.MenuItemOrder).ToList();
                model.Items = menu.Where(s => s.KindValue == (int)MenuItemKind.Catelog).Select(s => new MenuItem() { MenuItemOrder = s.MenuItemOrder, Scope = s.Scope, MenuID = s.MenuID, Id = s.Id, DisplayName = s.DisplayName, Hyperlink = s.Hyperlink, IconKey = s.IconKey, Kind = (MenuItemKind)s.KindValue, Target = (MenuTargetType)s.TargetValue }).ToList();

                foreach (var menuItem in menu.Where(s => s.KindValue == (int)MenuItemKind.Item))
                {
                    model.Items.Add(new MenuItem { MenuItemOrder = menuItem.MenuItemOrder, Id = menuItem.Id, Hyperlink = menuItem.Hyperlink, Scope = menuItem.Scope, IconKey = menuItem.IconKey, Kind = menuItem.Kind, Target = menuItem.Target, DisplayName = menuItem.DisplayName, Parent = model.Items.SingleOrDefault(p => p.Id == menuItem.ParentId.Value) });
                }
                return model;
            }
        }
        public MenuItem GetMenuItemById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var item = context.MenuItem.Where(m => m.Id == id).Select(s => new MenuItem()
                {
                    Hyperlink = s.Hyperlink,
                    IconKey = s.IconKey,
                    MenuItemOrder = s.MenuItemOrder,
                    MenuID = s.MenuID,
                    Scope = s.Scope,
                    Target = (MenuTargetType)s.TargetValue,
                    Kind = (MenuItemKind)s.KindValue,
                    Id = s.Id,
                    DisplayName = s.DisplayName
                }).FirstOrDefault();
                return item;
            }
        }
        public MenuItem GetParentMenuItemById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var parentId = context.MenuItem.Where(m => m.Id == id).Select(s => s.ParentId).FirstOrDefault();
                if (parentId == null) return null;
                var item = context.MenuItem.Where(m => m.Id == parentId).Select(s => new MenuItem()
                {
                    Hyperlink = s.Hyperlink,
                    IconKey = s.IconKey,
                    MenuItemOrder = s.MenuItemOrder,
                    MenuID = s.MenuID,
                    Scope = s.Scope,
                    Target = (MenuTargetType)s.TargetValue,
                    Kind = (MenuItemKind)s.KindValue,
                    Id = s.Id,
                    DisplayName = s.DisplayName
                }).FirstOrDefault();
                return item;
            }
        }
        public List<MenuItem> GetMenuItemsByParentId(Guid ParentId) //首层的时候根据menuid 第二层的时候根据parentid
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var items = context.MenuItem.Where(m => m.ParentId == ParentId || (m.MenuID == ParentId && m.KindValue == (int)MenuItemKind.Catelog)).Select(s => new MenuItem()
                {
                    Hyperlink = s.Hyperlink,
                    IconKey = s.IconKey,
                    MenuItemOrder = s.MenuItemOrder,
                    MenuID = s.MenuID,
                    Scope = s.Scope,
                    Target = (MenuTargetType)s.TargetValue,
                    Kind = (MenuItemKind)s.KindValue,
                    Id = s.Id,
                    DisplayName = s.DisplayName
                }).ToList();
                return items;
            }
        }
        public List<Menu> GetAllMenus()
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.Menu.Select(s => new Menu { Id = s.Id, Key = s.Key, MenuOrder = s.MenuOrder, DisplayName = s.DisplayName, DefaultPage = s.DefaultPage }).OrderBy(m => m.MenuOrder).ToList();
            }
        }

        #region 添加菜单app
        public Menu CreateMenu(Menu menu)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var entity = context.Menu.Where(s => s.Key == menu.Key).FirstOrDefault();
                if (entity != null) return null;
                context.Menu.Add(new MenuEntity() { Id = menu.Id, MenuOrder = menu.MenuOrder, Key = menu.Key, DisplayName = menu.DisplayName, DefaultPage = menu.DefaultPage });
                context.SaveChanges();
                return context.Menu.Where(s => s.Key == menu.Key).Select(s => new Menu() { Id = s.Id, MenuOrder = s.MenuOrder, Key = s.Key, DisplayName = s.DisplayName, DefaultPage = s.DefaultPage }).FirstOrDefault();
            }
        }
        #endregion
        #region 编辑菜单app
        public Menu UpdateMenu(Menu menu)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                MenuEntity entity;
                if (menu.Id != null)
                    entity = context.Menu.Where(s => s.Id == menu.Id).FirstOrDefault();
                else entity = context.Menu.Where(s => s.Key == menu.Key).FirstOrDefault();
                if (entity == null) return null;
                entity.DisplayName = menu.DisplayName;
                entity.DefaultPage = menu.DefaultPage;
                entity.MenuOrder = menu.MenuOrder;
                context.SaveChanges();
                return context.Menu.Where(s => s.Key == menu.Key).Select(s => new Menu() { Id = s.Id, MenuOrder = s.MenuOrder, Key = s.Key, DisplayName = s.DisplayName, DefaultPage = s.DefaultPage }).FirstOrDefault();
            }
        }
        #endregion
        #region 编辑MenuItem
        public MenuItem UpdateMenuItem(MenuItem menuItem)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var entity = context.MenuItem.Where(s => s.Id == menuItem.Id).FirstOrDefault();
                if (entity != null)
                {
                    Guid? parentid = null;
                    if (menuItem.Parent != null)
                        parentid = menuItem.Parent.Id;
                    entity.Hyperlink = menuItem.Hyperlink;
                    entity.IconKey = menuItem.IconKey;
                    entity.ParentId = parentid;
                    entity.DisplayName = menuItem.DisplayName;
                    entity.KindValue = (int)menuItem.Kind;
                    entity.TargetValue = (int)menuItem.Target;
                    context.SaveChanges();
                }
                var result = context.MenuItem.Where(m => m.Id == menuItem.Id).Select(s => new MenuItem
                {
                    DisplayName = s.DisplayName,
                    Id = s.Id,
                    Hyperlink = s.Hyperlink,
                    Kind = (MenuItemKind)s.KindValue,
                    Target = (MenuTargetType)s.TargetValue,
                    IconKey = s.IconKey
                }).FirstOrDefault();
                result.Parent = GetParentMenuItemById(entity.Id);
                return result;
            }
        }
        #endregion
        #region 创建MenuItem
        public MenuItem CreateMenuItem(MenuItem menuItem) // 创建的时候menuid要写入menuitem里面、parentid 也是。返回的时候包含了parentItem
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                Guid? paretnid = null;
                if (menuItem.Parent != null)
                    paretnid = menuItem.Parent.Id;
                var menuitem = context.MenuItem.Add(new MenuItemEntity
                {
                    Id = Guid.NewGuid(),
                    DisplayName = menuItem.DisplayName,
                    IconKey = menuItem.IconKey,
                    Hyperlink = menuItem.Hyperlink,
                    KindValue = (int)menuItem.Kind,
                    TargetValue = (int)menuItem.Target,
                    ParentId = paretnid,
                    MenuItemOrder = menuItem.MenuItemOrder,
                    MenuID = menuItem.MenuID
                });
                context.SaveChanges();


                var result = context.MenuItem.Where(m => m.Id == menuitem.Id).Select(s => new MenuItem
                {
                    MenuID = s.MenuID,
                    Scope = s.Scope,
                    MenuItemOrder = s.MenuItemOrder,
                    DisplayName = s.DisplayName,
                    Id = s.Id,
                    Hyperlink = s.Hyperlink,
                    Kind = (MenuItemKind)s.KindValue,
                    Target = (MenuTargetType)s.TargetValue,
                    IconKey = s.IconKey
                }).FirstOrDefault();
                if (menuitem.ParentId == null) return result;
                result.Parent = GetParentMenuItemById((Guid)menuitem.ParentId);
                return result;
            }

        }
        #endregion
        #region 删除Menu
        public List<Guid> DelMenu(List<Guid> ids)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<Guid> DelItemIds = new List<Guid>();
                var result = DelMenu(context, ids, ref DelItemIds);
                context.SaveChanges();
                acsManager.DelAuthorityCompletely(DelItemIds.Select(s => s.ToString()).ToList());
                return result;
            }
        }
        internal List<Guid> DelMenu(KStarDbContext context, List<Guid> ids, ref List<Guid> DelItemIds)
        {
            var items = context.Menu.Where(m => ids.Contains(m.Id)).ToList();
            if (items == null) return null;
            DynamicWidgetManager dynamicWidgetManager = new DynamicWidgetManager();
            DocumentManager documentManager = new DocumentManager();
            foreach (var item in items)
            {
                var libraryIds = context.DocumentLibrary.Where(m => m.MenuID == item.Id).Select(s => s.ID).ToList();
                var menuItemIds = context.MenuItem.Where(m => m.MenuID == item.Id).Select(s => s.Id).ToList();
                var dynamicWidgetIds = context.DynamicWidget.Where(m => m.MenuID == item.Id).Select(s => s.ID).ToList();
                documentManager.DelDocLibrarysByIds(context, libraryIds, ref DelItemIds);
                DelItemIds.AddRange(DelMenuItem(context, menuItemIds));
                dynamicWidgetManager.DynamicWidgets(context, dynamicWidgetIds);
                foreach (var menuRole in context.AppRole.Where(m => m.MenuId == item.Id))
                    context.AppRole.Remove(menuRole);
                context.Menu.Remove(item);
            }
            DelItemIds.AddRange(items.Select(s => s.Id).ToList());
            return items.Select(s => s.Id).ToList();
        }
        #endregion
        #region 删除MenuItem
        public List<Guid> DelMenuItem(Guid MenuItemId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<Guid> MenuItemList;
                MenuItemList = DelMenuItem(context, MenuItemId, new List<Guid>());
                context.SaveChanges();
                acsManager.DelAuthorityCompletely(MenuItemList.Select(s => s.ToString()).ToList());
                return MenuItemList;
            }
        }
        internal List<Guid> DelMenuItem(KStarDbContext context, List<Guid> menuItemIds)
        {
            List<Guid> result = new List<Guid>();
            foreach (var id in menuItemIds)
            {
                result.AddRange(DelMenuItem(context, id, new List<Guid>()));
            }
            return result;
        }
        internal List<Guid> DelMenuItem(KStarDbContext context, Guid menuItemId, List<Guid> MenuItemList)
        {
            var model = context.MenuItem.Where(s => s.Id == menuItemId).FirstOrDefault();
            if (model == null) return MenuItemList;

            var modelsun = context.MenuItem.Where(s => s.ParentId == menuItemId).ToList();
            foreach (var sun in modelsun)
            {
                MenuItemList = DelMenuItem(context, sun.Id, MenuItemList);
            }
            context.MenuItem.Remove(model);
            MenuItemList.Add(model.Id);
            return MenuItemList;
        }
        #endregion
    }
}

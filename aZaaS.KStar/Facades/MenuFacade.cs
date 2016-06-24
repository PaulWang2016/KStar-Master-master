using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Menus;
using System.Data;

using aZaaS.Framework.Extensions;

namespace aZaaS.KStar
{
    public class MenuFacade
    {
        private MenuManager _menuManager;

        public MenuFacade()
        {
            _menuManager = new MenuManager();
        }


        public List<Menu> GetTop()
        {
            List<Menu> model = _menuManager.GetAllMenus();
            //List<Menu> model = _menuManager.GetTop();
            return model;
        }

        public List<MenuItem> GetMenuItem(string key)
        {
            key.NullOrEmptyThrowArgumentEx("key is Null");
            var menuItemId = _menuManager.GetMenuIdByKey(key);
            return _menuManager.GetMenuItemsByParentId(menuItemId);
            //return _menuManager.GetMenuItem(key);
        }
        public List<MenuItem> GetMenuItem(string key, string ParentId)
        {
            key.NullOrEmptyThrowArgumentEx("key is Null");
            return _menuManager.GetMenuItemsByParentId(Guid.Parse(ParentId));
            //return _menuManager.GetMenuItem(key, ParentId);
        }
        public Menu GetMenu(Guid id)
        {
            id.EmptyThrowArgumentEx("id is Null");

            return _menuManager.GetMenuById(id);
            //return _menuManager.GetMenu(id);
        }
        public string GetMenuDefaultPage(string key)
        {
            key.NullOrEmptyThrowArgumentEx("key is Null");

            return _menuManager.GetMenuDefaultPage(key);
        }
        public void SetDefaultPage(Guid id, bool statu = true)
        {
            id.EmptyThrowArgumentEx("id is Null");

            _menuManager.SetDefaultPage(id, true);
        }

        public MenuItem GetMenuItem(Guid id)
        {
            id.EmptyThrowArgumentEx("id is Null");

            return _menuManager.GetMenuItemById(id);
            //return _menuManager.GetMenuItem(id);
        }
        public Guid GetMenuIdByKey(string key)
        {
            key.NullOrEmptyThrowArgumentEx("key is Null");

            return _menuManager.GetMenuIdByKey(key);
        }
        public Menu Get(string key)
        {
            key.NullOrEmptyThrowArgumentEx("key is Null");
            var menuId = _menuManager.GetMenuIdByKey(key);
            return _menuManager.GetMenuById(menuId);
            //return _menuManager.Get(key);
        }

        public Menu CreateMenu(Menu menu)
        {
            menu.NullThrowArgumentEx("Menu is Null");

            return _menuManager.CreateMenu(menu);
        }
        public MenuItem CreateMenuItem(MenuItem menuItem, string MenuKey, string ParentId)
        {
            menuItem.NullThrowArgumentEx("MenuItem is Null");
            MenuKey.NullOrEmptyThrowArgumentEx("MenuKey is Null");
            if (ParentId != "")
            {
                menuItem.Parent = new MenuItem() { Id = Guid.Parse(ParentId) };
            }
            menuItem.MenuID = _menuManager.GetMenuIdByKey(MenuKey);
            return _menuManager.CreateMenuItem(menuItem);
            //return _menuManager.CreateMenuItem(menuItem, MenuKey);
        }

        public Menu UpdateMenu(Menu menu)
        {
            menu.NullThrowArgumentEx("Menu is Null");

            return _menuManager.UpdateMenu(menu);
        }
        public MenuItem UpdateMenuItem(MenuItem menu, string ParentId)
        {
            menu.NullThrowArgumentEx("Menu is Null");
            if (ParentId != "")
            {
                menu.Parent = new MenuItem() { Id = Guid.Parse(ParentId) };
            }
            return _menuManager.UpdateMenuItem(menu);
        }


        public List<Guid> DelMenu(List<string> keyList)
        {
            keyList.NullOrEmptyThrowArgumentEx("keyList is Null");
            var menuIds = _menuManager.GetMenuIdsByKeys(keyList);
            return _menuManager.DelMenu(menuIds);
            //return _menuManager.DelMenuByKey(keyList);
        }
        public void DelMenuItem(string id)
        {
            id.NullOrEmptyThrowArgumentEx("MenuItemId is Null");

            _menuManager.DelMenuItem(Guid.Parse(id));
            //_menuManager.DeleteMenuItem(id);
        }


        public DataSet GetApps(string pane)
        {
            return _menuManager.GetApps(pane);
        }

        public void SetApps(DataSet ds)
        {
            ds.NullThrowArgumentEx("DataSet is Null");

            _menuManager.SetApps(ds);
        }
    }
}

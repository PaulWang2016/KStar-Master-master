using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Menus;
using aZaaS.KStar.Documents;
namespace aZaaS.KStar.Resources
{

    public class ResourceManager
    {
        public List<Resource> GetResources()
        {
            using (KStarDbContext context = new KStarDbContext())
            {

                var test = context.Menu.Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu" })
                   .Union(context.MenuItem.Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "MenuItem" })
                   .Union(context.DocumentLibrary.Select(s => new Resource { ID = s.ID, DisplayName = s.DisplayName, Type = "DocumentLibrary" }))
                   .Union(context.DocumentItem.Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "DocumentItem" }))
                   .Union(context.Widget.Select(s => new Resource { ID = s.Id, DisplayName = s.Title, Type = "Widget" }))
                      ).ToList();
                return test;
            }

        }
        public List<Resource> GetTreeResources(Guid? id, string type)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<Resource> test = new List<Resource>();
                if (id == null)
                {
                    test = context.Menu.Where(m => m.Key == type).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu", Target = "", Links = "", ParentID = null }).ToList();
                }
                else
                {
                    List<MenuItem> menuitems = context.MenuItem.Where(m => m.MenuID == id && m.ParentId == null).Select(s => new MenuItem()
                    {
                        Scope = s.Scope,
                        Id = s.Id,
                        DisplayName = s.DisplayName,
                        Hyperlink = s.Hyperlink,
                        IconKey = s.IconKey,
                        Kind = (MenuItemKind)s.KindValue,
                        Target = (MenuTargetType)s.TargetValue
                    }).ToList();

                    var temp = menuitems.Select(s => new Resource
                    {
                        ID = s.Id,
                        Target = s.Target.ToString(),
                        DisplayName = s.DisplayName,
                        Type = "MenuItem",
                        Links = s.Hyperlink,
                        ParentID = id
                    }).ToList();
                    var doc = context.DocumentLibrary.Where(M => M.MenuID == id).Select(s => new Resource
                    {
                        ID = s.ID,
                        DisplayName = s.DisplayName,
                        Type = "DocumentLibrary",
                        Links = "",
                        ParentID = id
                    }).ToList();
                    test = doc.Union(temp).ToList();

                    if (temp.Count == 0 && doc.Count == 0)
                    {
                        menuitems = context.MenuItem.Where(m => m.ParentId == id).Select(s => new MenuItem()
                        {
                            Scope = s.Scope,
                            Id = s.Id,
                            DisplayName = s.DisplayName,
                            Hyperlink = s.Hyperlink,
                            IconKey = s.IconKey,
                            Kind = (MenuItemKind)s.KindValue,
                            Target = (MenuTargetType)s.TargetValue
                        }).ToList();
                        var DocumentItem = context.DocumentItem.Where(m => m.DocumentLibraryID == id).Select(s => new Resource
                        {
                            ID = s.Id,
                            DisplayName = s.DisplayName,
                            Type = "DocumentItem",
                            Links = s.StorageUri,
                            ParentID = id
                        }).ToList();
                        test = menuitems.Select(s => new Resource { ID = s.Id, Target = s.Target.ToString(), DisplayName = s.DisplayName, Type = "MenuItem", Links = s.Hyperlink, ParentID = id }).ToList();
                        test = DocumentItem.Union(test).ToList();
                    }
                }
                return test;
            }
        }
        public List<Resource> GetMenuTreeResources(Guid? id, string type)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<Resource> test = new List<Resource>();
                if (id == null)
                {
                    test = context.Menu.Where(m => m.Key == type).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu", Target = "", Links = "", ParentID = null }).ToList();
                }
                else
                {
                    List<MenuItem> menuitems = context.MenuItem.Where(m => m.MenuID == id && m.ParentId == null).Select(s => new MenuItem()
                    {
                        MenuItemOrder = s.MenuItemOrder,
                        Scope = s.Scope,
                        Id = s.Id,
                        DisplayName = s.DisplayName,
                        Hyperlink = s.Hyperlink,
                        IconKey = s.IconKey,
                        Kind = (MenuItemKind)s.KindValue,
                        Target = (MenuTargetType)s.TargetValue
                    }).ToList();

                    test = menuitems.Select(s => new Resource
                    {
                        OrderBy = s.MenuItemOrder,
                        Kind = s.Kind.ToString(),
                        IconKey = s.IconKey,
                        ID = s.Id,
                        Target = s.Target.ToString(),
                        DisplayName = s.DisplayName,
                        Type = "MenuItem",
                        Links = s.Hyperlink,
                        ParentID = id
                    }).ToList();


                    if (test.Count == 0)
                    {
                        var menuitem = context.MenuItem.Where(m => m.ParentId == id).Select(s => new MenuItem()
                        {
                            MenuItemOrder = s.MenuItemOrder,
                            Scope = s.Scope,
                            Id = s.Id,
                            DisplayName = s.DisplayName,
                            Hyperlink = s.Hyperlink,
                            IconKey = s.IconKey,
                            Kind = (MenuItemKind)s.KindValue,
                            Target = (MenuTargetType)s.TargetValue
                        }).ToList();
                        test = menuitem.Select(s => new Resource
                        {
                            OrderBy = s.MenuItemOrder,
                            Kind = s.Kind.ToString(),
                            IconKey = s.IconKey,
                            ID = s.Id,
                            Target = s.Target.ToString(),
                            DisplayName = s.DisplayName,
                            Type = "MenuItem",
                            Links = s.Hyperlink,
                            ParentID = id
                        }).ToList();

                    }
                }
                return test;
            }
        }
        public List<Resource> GetDocumentTreeResources(Guid? id, string type)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<Resource> test = new List<Resource>();
                if (id == null)
                {
                    test = context.Menu.Where(m => m.Key == type).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu", Target = "", Links = "", ParentID = null }).ToList();
                }
                else
                {

                    test = context.DocumentLibrary.Where(M => M.MenuID == id).Select(s => new Resource
                    {
                        ID = s.ID,
                        DisplayName = s.DisplayName,
                        Type = "DocumentLibrary",
                        Links = "",
                        Key = s.Key,
                        IconPath = s.IconPath,
                        ParentID = id
                    }).ToList();

                    if (test.Count == 0)
                    {

                        test = context.DocumentItem.Where(m => m.DocumentLibraryID == id).Select(s => new Resource
                        {
                            IconPath = s.IconPath,
                            ID = s.Id,
                            DisplayName = s.DisplayName,
                            Type = "DocumentItem",
                            Links = s.StorageUri,
                            ParentID = id
                        }).ToList();
                    }
                }
                return test;
            }
        }


        public void AddResource(Resource item)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                //AuditLogManager audit = new AuditLogManager();

                switch (item.Type)
                {
                    case "Menu":
                        break;
                    case "MenuItem":
                        MenuItemEntity menuItem = new MenuItemEntity();
                        menuItem.DisplayName = item.DisplayName;
                        menuItem.Hyperlink = item.Links;
                        if (!string.IsNullOrEmpty(item.Target))
                        {
                            menuItem.Target = (MenuTargetType)Enum.Parse(typeof(MenuTargetType), item.Target);
                        }
                        menuItem.Id = item.ID;
                        var parent = context.MenuItem.Where(m => m.Id == item.ParentID).FirstOrDefault();
                        if (parent == null)
                        {
                            menuItem.ParentId = null;
                            menuItem.IconKey = item.IconKey;
                            menuItem.MenuItemOrder = item.OrderBy;
                            menuItem.MenuID = (Guid)item.ParentID;
                            menuItem.KindValue = (int)Menus.MenuItemKind.Catelog;
                        }
                        else
                        {
                            menuItem.MenuItemOrder = item.OrderBy;
                            menuItem.IconKey = item.IconKey;
                            menuItem.ParentId = item.ParentID;
                            menuItem.MenuID = parent.MenuID;
                            menuItem.KindValue = (int)Menus.MenuItemKind.Item;

                        }
                        context.MenuItem.Add(menuItem);
                        //menuItem.ToChange("Insert");
                        break;
                    case "DocumentLibrary":
                        DocumentLibraryEntity documentlibrary = new DocumentLibraryEntity();
                        documentlibrary.ID = item.ID;
                        documentlibrary.MenuID = (Guid)item.ParentID;
                        documentlibrary.Key = item.Key;
                        documentlibrary.IconPath = item.IconPath;
                        documentlibrary.DisplayName = item.DisplayName;
                        context.DocumentLibrary.Add(documentlibrary);

                        break;
                    case "DocumentItem":
                        DocumentItemEntity documentItem = new DocumentItemEntity();
                        documentItem.Id = item.ID;
                        documentItem.DisplayName = item.DisplayName;
                        documentItem.StorageUri = item.Links;
                        documentItem.IconPath = item.IconPath;
                        documentItem.CreateTime = DateTime.Now;
                        documentItem.DocumentLibraryID = (Guid)item.ParentID;
                        context.DocumentItem.Add(documentItem);
                        break;
                }
                context.SaveChanges();

                //audit.SaveAuditLogMaster(list);
            }
        }
        public void DeleteResource(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var Type = context.Menu.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu" })
                   .Union(context.MenuItem.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "MenuItem" })
                   .Union(context.DocumentLibrary.Where(m => m.ID == id).Select(s => new Resource { ID = s.ID, DisplayName = s.DisplayName, Type = "DocumentLibrary" }))
                   .Union(context.DocumentItem.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "DocumentItem" }))
                    //.Union(context.Widget.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.Title, Type = "Widget" }))
                      ).FirstOrDefault().Type;
                DocumentManager docManager = new DocumentManager();
                MenuManager menuManager = new MenuManager();
                switch (Type)
                {
                    case "Menu":
                        menuManager.DelMenu(new List<Guid> { id });
                        //menuManager.DelMenuById(new List<string> { id.ToString() });
                        break;
                    case "MenuItem":
                        menuManager.DelMenuItem(id);
                        //menuManager.DeleteMenuItem(id.ToString());
                        break;
                    case "DocumentLibrary":
                        docManager.DelDocLibrarysByIds(new List<Guid> { id });
                        //docManager.DelLibrary(id);
                        break;
                    case "DocumentItem":
                        docManager.DelDocItemsByIds(new List<Guid> { id });
                        //docManager.DelDocumentItem(id);
                        break;
                    //case "Widget":
                    //    var Widget = context.Widget.Where(m => m.Id == id).FirstOrDefault();
                    //    context.Widget.Remove(Widget);
                    //break;
                }
                context.SaveChanges();
            }
        }
        public void EditResource(Resource item)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var Type = context.Menu.Where(m => m.Id == item.ID).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu" })
                   .Union(context.MenuItem.Where(m => m.Id == item.ID).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "MenuItem" })
                   .Union(context.DocumentLibrary.Where(m => m.ID == item.ID).Select(s => new Resource { ID = s.ID, DisplayName = s.DisplayName, Type = "DocumentLibrary" }))
                   .Union(context.DocumentItem.Where(m => m.Id == item.ID).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "DocumentItem" }))
                    //.Union(context.Widget.Where(m => m.Id == item.ID).Select(s => new Resource { ID = s.Id, DisplayName = s.Title, Type = "Widget" }))
                      ).FirstOrDefault().Type;
                switch (Type)
                {
                    case "Menu":
                        var Menu = context.Menu.Where(m => m.Id == item.ID).FirstOrDefault();
                        Menu.DisplayName = item.DisplayName;
                        break;
                    case "MenuItem":
                        var MenuItem = context.MenuItem.Where(m => m.Id == item.ID).FirstOrDefault();
                        MenuItem.MenuItemOrder = item.OrderBy;
                        MenuItem.DisplayName = item.DisplayName;
                        MenuItem.Hyperlink = item.Links;
                        MenuItem.IconKey = item.IconKey;
                        MenuItem.Target = (MenuTargetType)Enum.Parse(typeof(MenuTargetType), item.Target);
                        break;
                    case "DocumentLibrary":
                        var DocumentLibrary = context.DocumentLibrary.Where(m => m.ID == item.ID).FirstOrDefault();
                        DocumentLibrary.DisplayName = item.DisplayName;
                        DocumentLibrary.IconPath = item.IconPath;
                        break;
                    case "DocumentItem":
                        var DocumentItem = context.DocumentItem.Where(m => m.Id == item.ID).FirstOrDefault();
                        DocumentItem.DisplayName = item.DisplayName;
                        DocumentItem.StorageUri = item.Links;
                        DocumentItem.IconPath = item.IconPath;
                        break;
                    //case "Widget":
                    //    var Widget = context.Widget.Where(m => m.Id == item.ID).FirstOrDefault();
                    //    context.Widget.Remove(Widget);
                    //    break;
                }
                context.SaveChanges();
            }
        }
        public string GetResourceType(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var test = context.Menu.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu" })
                   .Union(context.MenuItem.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "MenuItem" })
                   .Union(context.DocumentLibrary.Where(m => m.ID == id).Select(s => new Resource { ID = s.ID, DisplayName = s.DisplayName, Type = "DocumentLibrary" }))
                   .Union(context.DocumentItem.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "DocumentItem" }))
                   .Union(context.Widget.Where(m => m.Id == id).Select(s => new Resource { ID = s.Id, DisplayName = s.Title, Type = "Widget" }))
                   .Union(context.DynamicWidget.Where(m => m.ID == id).Select(s => new Resource { ID = s.ID, DisplayName = s.DisplayName, Type = "DynamicWidget" }))
                      ).FirstOrDefault().Type;
                return test;
            }

        }
        public List<Resource> FindResources(string input)
        {
            using (KStarDbContext context = new KStarDbContext())
            {

                var test = context.Menu.Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "Menu" })
                   .Union(context.MenuItem.Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "MenuItem" })
                   .Union(context.DocumentLibrary.Select(s => new Resource { ID = s.ID, DisplayName = s.DisplayName, Type = "DocumentLibrary" }))
                   .Union(context.DocumentItem.Select(s => new Resource { ID = s.Id, DisplayName = s.DisplayName, Type = "DocumentItem" }))
                   .Union(context.Widget.Select(s => new Resource { ID = s.Id, DisplayName = s.Title, Type = "Widget" }))
                      ).Where(m => m.Type.Contains(input) || m.DisplayName.Contains(input)).ToList();
                return test;
            }

        }
    }
}

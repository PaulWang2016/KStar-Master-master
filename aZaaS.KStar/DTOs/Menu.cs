using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Menus;

namespace aZaaS.KStar
{
    [Serializable]
    public class Menu
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string DefaultPage { get; set; }
        public string MenuOrder { get; set; }
        public List<MenuItem> Items { get; set; }
    }

    [Serializable]
    public class MenuItem
    {
        public Guid Id { get; set; }
        public Guid MenuID { get; set; }
        public string DisplayName { get; set; }
        public string Hyperlink { get; set; }
        public string MenuItemOrder { get; set; }
        public virtual string IconKey { get; set; }
        public string Scope { get; set; }
        public MenuItemKind Kind { get; set; }
        public MenuTargetType Target { get; set; }
        public MenuItem Parent { get; set; }
    }
}

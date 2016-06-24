using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DynamicWidgets
{
    public sealed class DynamicWidgetManager
    {
        public DynamicWidget GetWidget(string key)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var widget = context.DynamicWidget.SingleOrDefault(s => s.Key == key);
                if (widget == null)
                {
                    return null;
                }
                return widget.ToDTO();
            }
        }

        public DynamicWidget GetWidgetByID(Guid Id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DynamicWidget.Where(m => m.ID == Id).Select(s => new DynamicWidget()
                {
                    Description = s.Description,
                    DisplayName = s.DisplayName,
                    ID = s.ID,
                    Key = s.Key,
                    MenuID = s.MenuID,
                    RazorContent = s.RazorContent
                }).FirstOrDefault();
            }
        }
        public List<DynamicWidget> GetAllWidget(string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuID = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuID == null) return new List<DynamicWidget>();
                return context.DynamicWidget.Where(m => m.MenuID == menuID).Select(s => new DynamicWidget()
                     {
                         Description = s.Description,
                         DisplayName = s.DisplayName,
                         ID = s.ID,
                         Key = s.Key,
                         MenuID = s.MenuID,
                         RazorContent = s.RazorContent
                     }).ToList();
            }
        }
        public void AddWidget(DynamicWidget dyWidget, string pane)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menuID = context.Menu.Where(m => m.Key == pane).Select(s => s.Id).FirstOrDefault();
                if (menuID == null)
                {
                    return;
                }
                context.DynamicWidget.Add(new DynamicWidgetEntity()
                {
                    RazorContent = dyWidget.RazorContent,
                    MenuID = menuID,
                    Key = dyWidget.Key,
                    ID = dyWidget.ID,
                    DisplayName = dyWidget.DisplayName,
                    Description = dyWidget.Description,
                });
                context.SaveChanges();
            }
        }
        public void DelWidget(List<Guid> IdList)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                foreach (var id in IdList)
                {
                    var model = context.DynamicWidget.Where(m => m.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        context.DynamicWidget.Remove(model);
                        context.SaveChanges();
                    }
                }
            }
        }
        public void EditWidget(DynamicWidget dyWidge)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var model = context.DynamicWidget.Where(m => m.ID == dyWidge.ID).FirstOrDefault();
                if (model != null)
                {
                    model.Key = dyWidge.Key;
                    model.RazorContent = dyWidge.RazorContent;
                    model.Description = dyWidge.Description;
                    model.DisplayName = dyWidge.DisplayName;
                    context.SaveChanges();
                }
            }
        }

        #region 删除 Menu 下的 DynamicWidget
        /// <summary>
        /// 删除 Menu 下的 DynamicWidget
        /// </summary>
        internal List<Guid> DelWidget(KStarDbContext context, Guid menuItemID)
        {
            List<Guid> idList = new List<Guid>();
            var dynamicWidgets = context.DynamicWidget.Where(s => s.MenuID == menuItemID);

            foreach (var item in dynamicWidgets)
            {
                idList.Add(item.ID);
                context.DynamicWidget.Remove(item);
            }

            return idList;
        }
        #endregion


        #region 删除DynamicWidgets
        internal List<Guid> DynamicWidgets(KStarDbContext context, List<Guid> ids)
        {
            List<Guid> idList = new List<Guid>();
            var dynamicWidgets = context.DynamicWidget.Where(s => ids.Contains(s.MenuID));
            foreach (var item in dynamicWidgets)
            {
                idList.Add(item.ID);
                context.DynamicWidget.Remove(item);
            }
            return idList;
        }
        #endregion

    }
}

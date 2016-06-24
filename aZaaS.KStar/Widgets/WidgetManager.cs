using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Widgets
{
    internal class WidgetManager
    {
        private static readonly object slock = new object();

        private static bool isInitialized = false;
        private static Dictionary<string, Widget> widgets;

        public WidgetManager()
        {
            Initialize();
        }

        public Widget Get(string key)
        {
            return widgets[key];
        }

        // Widget 的注册过程在可预见的时间不会通过Web前端来实现。
        // 更可能的方案是通过数据库、配置文件乃至Assembly自发现的
        // 方式。因此此方法不应该对框架使用者公开。在 ServiceFacade 
        // 中包装时需要注意。
        //
        // y2mao@outlook.com, 2013/10/6
        public void Register(Widget widget)
        {
            // 设置初始化标签为False，
            // 以确保下一次对Widget的访问会触发初始化过程。
            isInitialized = false;

            using (KStarDbContext context = new KStarDbContext())
            {
                context.Widget.Add(widget.ToEntity());
                context.SaveChanges();
            }
        }

        public Widget GetWidget(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var item = context.Widget.Where(m => m.Id == id).Select(s => new Widget()
                {
                    ID = s.Id,
                    Title = s.Title
                }).FirstOrDefault();
                return item;
            }
        }

        private void Initialize()
        {
            lock (slock)
            {
                if (isInitialized) return;

                var repo = new WidgetRepository();
                widgets = repo.GetAll()
                    .Select(we => we.ToDTO()).ToDictionary(w => w.Key);
                isInitialized = true;
            }
        }
    }
}

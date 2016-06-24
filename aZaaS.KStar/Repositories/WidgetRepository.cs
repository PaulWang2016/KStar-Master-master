using aZaaS.KStar.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Repositories
{
    internal class WidgetRepository : AbstractRepository
    {
        public WidgetEntity[] GetAll()
        {
            using (KStarDbContext context = new KStarDbContext( ))
            {
                if (context.Widget.Count() == 0)
                {
                    context.Widget.Add(new WidgetEntity() { Key = "page", Url = "", RenderModeValue = (int)WidgetRenderMode.IFrame, });

                    context.Widget.Add(new WidgetEntity() { Key = "doc", Url = "Document/RenderWidget", RenderModeValue = (int)WidgetRenderMode.HtmlFragment });

                    context.Widget.Add(new WidgetEntity() { Key = "dynamic", Url = "DynamicWidget/RenderWidget", RenderModeValue = (int)WidgetRenderMode.HtmlFragment });
                    
                    context.Configuration.ValidateOnSaveEnabled = false;
                    int count = context.SaveChanges();
                    context.Configuration.ValidateOnSaveEnabled = true;
                }

                return context.Widget.ToArray();
            }
        }
    }
}

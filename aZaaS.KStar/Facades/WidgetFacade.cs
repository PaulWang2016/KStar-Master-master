using aZaaS.KStar.Repositories;
using aZaaS.KStar.Widgets;
using aZaaS.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class WidgetFacade
    {
        private WidgetManager _widgetManager;

        public WidgetFacade()
        {
            _widgetManager = new WidgetManager();
        }
        public Widget GetWidget(Guid id)
        {
            id.EmptyThrowArgumentEx("id");

            return _widgetManager.GetWidget(id);
        }
        public Widget Get(string key)
        {
            key.NullOrEmptyThrowArgumentEx("key");

            return _widgetManager.Get(key);
        }
    }
}

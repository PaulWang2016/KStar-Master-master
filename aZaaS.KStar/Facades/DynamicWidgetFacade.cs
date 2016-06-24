using aZaaS.KStar.DynamicWidgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework.Extensions;

namespace aZaaS.KStar
{
    public class DynamicWidgetFacade
    {
        private DynamicWidgetManager _dynamicWidgetManager;

        public DynamicWidgetFacade()
        {
            _dynamicWidgetManager = new DynamicWidgetManager();
        }

        public DynamicWidget GetWidget(string key)
        {
            key.NullOrEmptyThrowArgumentEx("key");

            return _dynamicWidgetManager.GetWidget(key);
        }

        public DynamicWidget GetWidgetByID(Guid Id)
        {
            Id.EmptyThrowArgumentEx("Id");

            return _dynamicWidgetManager.GetWidgetByID(Id);
        }
        public List<DynamicWidget> GetAllWidget(string pane)
        {
            if (string.IsNullOrWhiteSpace(pane))
            {
                throw new ArgumentNullException("pane");
            }

            pane.NullOrEmptyThrowArgumentEx("pane");

            return _dynamicWidgetManager.GetAllWidget(pane);
        }
        public void AddWidget(DynamicWidget dyWidget, string pane)
        {
            dyWidget.NullThrowArgumentEx("DynamicWidget is null");
            pane.NullOrEmptyThrowArgumentEx("pane");

            _dynamicWidgetManager.AddWidget(dyWidget, pane);
        }
        public void DelWidget(List<Guid> IdList)
        {
            IdList.NullOrEmptyThrowArgumentEx("IdList is Empty");

            _dynamicWidgetManager.DelWidget(IdList);
        }
        public void EditWidget(DynamicWidget dyWidge)
        {
            dyWidge.NullThrowArgumentEx("DynamicWidget is  null");

            _dynamicWidgetManager.EditWidget(dyWidge);
        }
    }
}

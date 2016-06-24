using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public enum WidgetRenderMode
    {
        IFrame = 0,
        HtmlFragment = 1,
    }

    [Serializable]
    public class Widget
    {
        public Guid ID { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public WidgetRenderMode RenderMode { get; set; }

        public Dictionary<string, string> HtmlAttributes { get; set; }
    }
}

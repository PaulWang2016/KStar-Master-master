using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web;

namespace aZaaS.KStar.Html
{
    public static class WidgetExtensions
    {
        public static MvcHtmlString Widget(this HtmlHelper htmlHelper, string widgetKey, object widgetAttributes = null, object htmlAttributes = null )
        {
            var unValidatedCustomizedWidgetAttributes = 
                HtmlHelper.AnonymousObjectToHtmlAttributes(widgetAttributes)
                as IDictionary<string, object>;

            var htmlAttributesDic = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes)
                as IDictionary<string, object>;

            var facade = new WidgetFacade();
            var definition = facade.Get(widgetKey);

            var customizedWidgetAttributesDic = new Dictionary<string, object>();
            foreach (var kv in unValidatedCustomizedWidgetAttributes)
            {
                if (kv.Key.StartsWith("widget-", StringComparison.InvariantCultureIgnoreCase))
                {
                    customizedWidgetAttributesDic.Add(
                        kv.Key.ToLowerInvariant(), 
                        kv.Value);
                }
                else
                {
                    customizedWidgetAttributesDic.Add(
                        "data-" + kv.Key.ToLowerInvariant(), 
                        kv.Value);
                }
            }

            var builder = new TagBuilder("div");
            var httpContextWrapper = new HttpContextWrapper(HttpContext.Current);

            // 设置基础样式，应用规则如下：
            // 1. 首先应用插件定义的样式
            // 2. 应用传入的自定义样式（会覆盖同名样式）
            // 3. 最后应用Widget专用的数据源样式
            builder.MergeAttribute("data-title", definition.Title);
            builder.MergeAttribute("widget-mode", definition.RenderMode.ToString().ToLowerInvariant());
            builder.MergeAttribute("widget-url", UrlHelper.GenerateContentUrl("~/" + definition.Url, httpContextWrapper));

            builder.MergeAttributes(definition.HtmlAttributes);
            builder.MergeAttributes(htmlAttributesDic, true);
            builder.MergeAttributes(customizedWidgetAttributesDic, true);
            builder.AddCssClass("widget-container");
            

            if (definition.RenderMode == WidgetRenderMode.IFrame)
            {
                builder.InnerHtml = string.Format(
                    "<iframe id=\"{0}\" name=\"{0}\" width=\"100%\" height=\"600\" scrolling=\"auto\" onload=\"this.height={0}.document.body.scrollHeight\" frameborder=\"0\" src=\"{1}\"></iframe>",
                    "ifm_" + Guid.NewGuid().ToString(),
                    definition.Url);
            }
            else
            {
                builder.InnerHtml = "<p>Loading ...</p>";
            }

            return MvcHtmlString.Create(builder.ToString());
        }
    }
}

using aZaaS.KStar.Facades;
using aZaaS.KStar.MgmtDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace aZaaS.KStar.Form.Mvc
{
    public static class ControlExtensions
    {
        private static readonly DataDictionaryFacade _dataDictionaryFacade = new DataDictionaryFacade();
        private const string CHECK_BOX = "checkbox";
        private const string RADIO_BUTTON = "radio";

        #region DropDownList

        public static HtmlString KStarDictionary(this HtmlHelper htmlHelper, string name, string codeCategory)
        {
            var selectList = ControlExtensions.GetDataDictionary(codeCategory);

            return htmlHelper.DropDownList(name, selectList);
        }

        public static HtmlString KStarDictionary(this HtmlHelper htmlHelper, string name, string codeCategory, object htmlAttributes)
        {
            var selectList = ControlExtensions.GetDataDictionary(codeCategory);

            return htmlHelper.DropDownList(name, selectList, htmlAttributes);
        }

        public static HtmlString KStarDictionary(this HtmlHelper htmlHelper, string name, string codeCategory, IDictionary<string, object> htmlAttributes)
        {
            var selectList = ControlExtensions.GetDataDictionary(codeCategory);

            return htmlHelper.DropDownList(name, selectList, htmlAttributes);
        }

        public static HtmlString KStarDictionary(this HtmlHelper htmlHelper, string name, string codeCategory, string optionLabel)
        {
            var selectList = ControlExtensions.GetDataDictionary(codeCategory);

            return htmlHelper.DropDownList(name, selectList, optionLabel);
        }

        public static HtmlString KStarDictionary(this HtmlHelper htmlHelper, string name, string codeCategory, string optionLabel, object htmlAttributes)
        {
            var selectList = ControlExtensions.GetDataDictionary(codeCategory);

            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }

        public static HtmlString KStarDictionary(this HtmlHelper htmlHelper, string name, string codeCategory, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            var selectList = ControlExtensions.GetDataDictionary(codeCategory);

            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }

        #endregion

        #region CheckboxList

        //public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection = RepeatDirection.Horizontal)
        //{
        //    return htmlHelper.KStarCheckBoxList(name, codeCategory, 0,
        //        ((IDictionary<string, object>)null));
        //}

        //public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, int split)
        //{
        //    return htmlHelper.KStarCheckBoxList(name, codeCategory, split,
        //        ((IDictionary<string, object>)null));
        //}

        //public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, object htmlAttributes)
        //{
        //    return htmlHelper.KStarCheckBoxList(name, codeCategory, 0,
        //        ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
        //}

        //public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, int split, object htmlAttributes)
        //{
        //    return htmlHelper.KStarCheckBoxList(name, codeCategory, split,
        //        ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
        //}

        //public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, int split,
        //    IDictionary<string, object> htmlAttributes)
        //{
        //    if (String.IsNullOrEmpty(name))
        //        throw new ArgumentException("The argument must have a value", "name");

        //    var listInfo = ControlExtensions.GetDataDictionary(codeCategory);

        //    if (listInfo == null)
        //        throw new ArgumentNullException("listInfo");
        //    if (listInfo.Count < 1)
        //        throw new ArgumentException("The list must contain at least one value", "listInfo");
        //    StringBuilder sb = new StringBuilder();
        //    int colCount = 0;
        //    int count = 0;

        //    int maxItems = listInfo.Count();
        //    if (split > 1)
        //    {
        //        // Begin Table
        //        sb.AppendLine("<table width='100%' border='0'><tr><td>");
        //    }
        //    foreach (SelectListItem info in listInfo)
        //    {
        //        TagBuilder builder = new TagBuilder("input");
        //        if (info.Selected) builder.MergeAttribute("checked", "checked");
        //        builder.MergeAttributes<string, object>(htmlAttributes);
        //        builder.MergeAttribute("type", "checkbox");
        //        builder.MergeAttribute("value", info.Value);
        //        builder.MergeAttribute("name", name);
        //        builder.InnerHtml = info.Text;
        //        sb.Append(builder.ToString(TagRenderMode.Normal));
        //        if (split <= 1)
        //        {
        //            // Skip Table all together....
        //        }
        //        else
        //        {
        //            count++;
        //            colCount++;
        //            if (split == colCount)
        //            {
        //                colCount = 0;
        //                sb.Append("</td></tr>");
        //                if (count != maxItems)
        //                {
        //                    // Need another row
        //                    sb.Append("<tr><td>");
        //                }
        //            }
        //            else
        //            {
        //                sb.Append("</td><td>");
        //            }
        //        }
        //    }
        //    if (split > 1)
        //        sb.Append("</table>");
        //    return new HtmlString(sb.ToString());
        //}

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory)
        {
            return htmlHelper.KStarCheckBoxList(name, codeCategory, RepeatDirection.Horizontal);
        }

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, object htmlAttributes)
        {
            return htmlHelper.KStarCheckBoxList(name, codeCategory, RepeatDirection.Horizontal, null, htmlAttributes);
        }

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.KStarCheckBoxList(name, codeCategory, RepeatDirection.Horizontal, null, htmlAttributes);
        }

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection)
        {
            return htmlHelper.KStarCheckBoxList(name, codeCategory, repeatDirection, null);
        }

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection
            , object defaultCheckValue)
        {
            return htmlHelper.KStarCheckBoxList(name, codeCategory, repeatDirection, defaultCheckValue, null);
        }

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection,
            object defaultCheckValue, object htmlAttributes)
        {
            //var dicAttributes = ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
            return htmlHelper.KStarCheckBoxList(name, codeCategory, repeatDirection, defaultCheckValue, ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
        }

        public static HtmlString KStarCheckBoxList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection,
            object defaultCheckValue, IDictionary<string, object> htmlAttributes)
        {
            var listInfo = ControlExtensions.GetDataDictionary(codeCategory);

            return GenerateHtml(name, codeCategory, repeatDirection, CHECK_BOX, defaultCheckValue, htmlAttributes);
        }

        #endregion

        #region RadioButtonList

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory)
        {
            return htmlHelper.KStarRadioButtonList(name, codeCategory, RepeatDirection.Horizontal);
        }

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory, object htmlAttributes)
        {
            return htmlHelper.KStarRadioButtonList(name, codeCategory, RepeatDirection.Horizontal, null, htmlAttributes);
        }

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.KStarRadioButtonList(name, codeCategory, RepeatDirection.Horizontal, null, htmlAttributes);
        }

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection)
        {
            return htmlHelper.KStarRadioButtonList(name, codeCategory, repeatDirection, null);
        }

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection
            , object defaultCheckValue)
        {
            return htmlHelper.KStarRadioButtonList(name, codeCategory, repeatDirection, defaultCheckValue, null);
        }

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection,
            object defaultCheckValue, object htmlAttributes)
        {
            //var dicAttributes = ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
            return htmlHelper.KStarRadioButtonList(name, codeCategory, repeatDirection, defaultCheckValue, ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
        }

        public static HtmlString KStarRadioButtonList(this HtmlHelper htmlHelper, string name, string codeCategory, RepeatDirection repeatDirection,
            object defaultCheckValue, IDictionary<string, object> htmlAttributes)
        {
            var listInfo = ControlExtensions.GetDataDictionary(codeCategory);

            return GenerateHtml(name, codeCategory, repeatDirection, RADIO_BUTTON, defaultCheckValue, htmlAttributes);
        }

        #endregion

        #region private Method

        private static List<SelectListItem> GetDataDictionary(string dicKey)
        {
            var selectList = new List<SelectListItem>();

            var dataList = _dataDictionaryFacade.GetDataDictionaryByCode(dicKey).ToList().OrderBy(r => r.Order);

            dataList.ToList().ForEach(
                item =>
                {
                    selectList.Add(new SelectListItem() { Text = item.Name, Value = item.Value });
                });

            return selectList;
        }

        private static MvcHtmlString GenerateHtml(string name, string codeCategory, RepeatDirection repeatDirection, string type, object stateValue, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder table = new TagBuilder("table");
            //int i = 0;
            bool isCheckBox = type == "checkbox";
            var listInfo = ControlExtensions.GetDataDictionary(codeCategory);
            TagBuilder span = new TagBuilder("span");
            span.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;";
            if (repeatDirection == RepeatDirection.Horizontal)
            {
                TagBuilder tr = new TagBuilder("tr");
                foreach (var item in listInfo)
                {
                    //i++;
                    //string id = string.Format("{0}_{1}", name, i);
                    TagBuilder td = new TagBuilder("td");

                    bool isChecked = false;
                    if (isCheckBox)
                    {
                        IEnumerable<string> currentValues = stateValue as IEnumerable<string>;
                        isChecked = (null != currentValues && currentValues.Contains(item.Value));
                    }
                    else
                    {
                        string currentValue = stateValue as string;
                        isChecked = (null != currentValue && item.Value == currentValue);
                    }

                    td.InnerHtml = GenerateControlHtml(name, item.Text, item.Value, isChecked, type, htmlAttributes);
                    td.InnerHtml += span.ToString();
                    tr.InnerHtml += td.ToString();
                    
                }
                table.InnerHtml = tr.ToString();
            }
            else
            {
                foreach (var item in listInfo)
                {
                    TagBuilder tr = new TagBuilder("tr");
                    //i++;
                    //string id = string.Format("{0}_{1}", name, i);
                    TagBuilder td = new TagBuilder("td");

                    bool isChecked = false;
                    if (isCheckBox)
                    {
                        IEnumerable<string> currentValues = stateValue as IEnumerable<string>;
                        isChecked = (null != currentValues && currentValues.Contains(item.Value));
                    }
                    else
                    {
                        string currentValue = stateValue as string;
                        isChecked = (null != currentValue && item.Value == currentValue);
                    }

                    td.InnerHtml = GenerateControlHtml(name, item.Text, item.Value, isChecked, type, htmlAttributes);
                    tr.InnerHtml = td.ToString();
                    table.InnerHtml += tr.ToString();
                }
            }
            return new MvcHtmlString(table.ToString());
        }

        private static string GenerateControlHtml(string name, string labelText, string value, bool isChecked, string type, IDictionary<string, object> htmlAttributes)
        {
            StringBuilder sb = new StringBuilder();

            TagBuilder builder = new TagBuilder("input");
            if (isChecked) builder.MergeAttribute("checked", "checked");
            builder.MergeAttributes<string, object>(htmlAttributes);
            builder.MergeAttribute("type", type);
            builder.MergeAttribute("value", value);
            builder.MergeAttribute("name", name);
            builder.InnerHtml = labelText;
            sb.Append(builder.ToString(TagRenderMode.Normal));

            return sb.ToString();
        }

        #endregion
    }
}

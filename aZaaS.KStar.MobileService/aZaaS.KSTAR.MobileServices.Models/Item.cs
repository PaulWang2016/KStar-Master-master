using System.Runtime.Serialization;

namespace aZaaS.KSTAR.MobileServices.Models
{
    public class Item
    {
        /// <summary>
        /// Item的键，必填
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Item的值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Item的显示名称
        /// </summary>
        public string Label { get; set; }

        private bool _visible = true;
        /// <summary>
        /// 是否显示该字段 ，默认值 1
        /// </summary>
        public bool Visible { get { return _visible; } set { _visible = value; } }

        /// <summary>
        /// 是否可编辑：默认值 0
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// 详细信息的连接路径
        /// </summary>
        public string DetailsUrl { get; set; }

        /// <summary>
        /// 显示格式
        /// </summary>
        public string Format { get; set; }
    }
}

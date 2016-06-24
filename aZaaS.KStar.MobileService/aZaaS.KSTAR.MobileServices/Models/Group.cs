using System.Collections.Generic;
using System.Runtime.Serialization;

namespace aZaaS.KSTAR.MobileServices.Models
{
    /// <summary>
    /// 数据分组
    /// </summary>
    public partial class Group
    {
        /// <summary>
        /// Group类型，分为Single和Table
        /// </summary>
        private string _type = "Single";
        public string Type { get { return _type; } set { if (value != null) _type = value; } }

        /// <summary>
        /// 显示标签内容
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 是否折叠的 ，默认值 0
        /// </summary>
        public bool Collapsed { get; set; }

        /// <summary>
        /// Type=Single
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Type=Table
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Type=Table
        /// </summary>
        public List<Row> Rows { get; set; }
    }

    public class Header : BaseEntity
    {
        public List<Item> Items { get; set; }
    }

    public class Row : BaseEntity
    {
        public Data Data { get; set; }
        public More More { get; set; }
    }

    public class Data : BaseEntity
    {
        public List<Item> Items { get; set; }
    }

    public class More : BaseEntity
    {
        public List<Item> Items { get; set; }
    }
}

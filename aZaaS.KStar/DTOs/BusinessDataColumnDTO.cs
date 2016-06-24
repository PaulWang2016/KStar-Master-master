using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class BusinessDataColumnDTO : AbstractDTO
    {
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ValueType { get; set; }
        public bool IsVisible { get; set; }
        public BusinessDataConfigDTO Config { get; set; }

        public ValueType DataType
        {
            get
            {
                return (ValueType)Enum.Parse(typeof(ValueType), this.ValueType);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace aZaaS.KStar
{
    public class ExFieldDTO : AbstractDTO
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public enum FieldTypeCode
    {
        String = 1,
        DateTime = 2,
        Decimal = 4
    }
}

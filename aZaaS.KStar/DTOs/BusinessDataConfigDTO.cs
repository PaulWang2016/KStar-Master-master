using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class BusinessDataConfigDTO : AbstractDTO
    {
        public string ApplicationName { get; set; }
        public string ProcessName { get; set; }
        public string DbConnectionString { get; set; }
        public string DataTable { get; set; }
        public string WhereQuery { get; set; }
        public IList<BusinessDataColumnDTO> ColumnItems { get; set; }
    }
}

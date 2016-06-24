using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    public class OrgChartDTO : AbstractDTO
    {
        public string Name { get; set; }
        public OrgNodeDTO Root { get; set; }
        public IList<OrgNodeDTO> Nodes { get; set; }
    }
}

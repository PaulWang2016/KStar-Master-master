using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace aZaaS.KStar
{
    public class OrgNodeDTO  : AbstractDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public OrgChartDTO Chart { get; set; }
        public OrgNodeDTO Parent { get; set; }
        public IList<OrgNodeDTO> ChildNodes { get; set; }
        public IList<OrgNodeExFieldDTO> ExtendItems { get; set; }
        public IList<UserDTO> Users { get; set; }
        public IList<PositionDTO> Positions { get; set; }

    }
}

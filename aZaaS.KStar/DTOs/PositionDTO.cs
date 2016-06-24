using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace aZaaS.KStar
{
    public class PositionDTO : AbstractDTO
    {
        public string Name { get; set; }
        public  IList<UserDTO> Users { get; set; }
        public  IList<OrgNodeDTO> Nodes { get; set; }
        public IList<PositionExFieldDTO> ExFields { get; set; }
        public  PositionCategoryDTO Category { get; set; }
    }
}

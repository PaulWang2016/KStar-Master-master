using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ControlTemplateCategory
    {
        public Guid SysId { get; set; }
        public Guid? ParentId { get; set; }        
        public string CategoryName { get; set; }        
    }
}

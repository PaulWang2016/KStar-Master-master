using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ControlRenderTemplate
    {
        public Guid SysId { get; set; }
        public string DisplayName { get; set; }
        public string HtmlTemplate { get; set; }
        public Guid CategoryId { get; set; }
    }
}

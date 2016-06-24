using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ActivityFormSetting
    {
        public Guid SysId { get; set; }
        public int ActivityId { get; set; }
        public int WorkMode { get; set; }
        public bool IsCustom { get; set; }
        public bool IsEditable { get; set; }
        public bool IsSettingEnabled { get; set; }
    }
}

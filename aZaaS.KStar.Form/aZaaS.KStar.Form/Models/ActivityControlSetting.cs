using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models
{
    public class ActivityControlSetting
    {
        public Guid SysId { get; set; }
        public int ActivityId { get; set; }
        public int WorkMode { get; set; }
        public string ControlRenderId { get; set; }
        public string ControlName { get; set; }
        public string ControlType { get; set; }
        public bool IsHide { get; set; }
        public bool IsDisable { get; set; }
        public bool IsCustom { get; set; }
        public string RenderTemplateId { get; set; }
        public string ProcessFullName { get; set; }
    }
}

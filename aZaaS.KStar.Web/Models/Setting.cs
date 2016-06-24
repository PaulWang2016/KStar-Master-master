using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Models
{
    public class Setting
    {
        public string    SettingsID  { get; set; }
        public string Category    { get; set; }
        public string SettingName  { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using aZaaS.KStar.Menus;

namespace aZaaS.KStar.AppDelegate
{
    public class AppDelegateEntity
    {
        public virtual Guid Id { get; set;}
        public virtual string ProcessFullName { get; set; }
        [ForeignKey("Menu")]
        public virtual Guid AppId { get; set; }

        public virtual MenuEntity Menu { get; set; }
    }
}

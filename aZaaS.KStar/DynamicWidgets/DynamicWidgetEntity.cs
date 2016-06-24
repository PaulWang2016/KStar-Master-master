using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.DynamicWidgets
{
    public class DynamicWidgetEntity
    {
        [Key]
        public virtual Guid ID { get; set; }
        public virtual string Key { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string RazorContent { get; set; }

        public virtual string Description { get; set; }

        // 通过逗号分隔一组用户或角色。用户或角色需要有前缀标识，举例如下：
        // u:boby, u:squall, r:Administrators, r:PowerUsers (空格可有可无，实现时需要进行Trim操作）
        public virtual string EnabledRoles { get; set; }

        // DynamicWidget 所属的菜单标识
        [ForeignKey("Menu")]
        public virtual Guid MenuID { get; set; }

        public virtual Menus.MenuEntity Menu { get; set; }
    }
}

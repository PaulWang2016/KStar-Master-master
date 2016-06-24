/* 项目：门户垂直导航菜单实体
 * 作者：y2mao@outlook.com
 * 日期：2013/10/6
 * 
 * 【修订记录】
 * 2013/10/06：第一版设计
 * 
 * 【设计要点】
 * 考虑 KStar 的通用性目的， 菜单需要满足如下几类需求：
 * 1. 菜单项需要有访问控制。
 * 2. 菜单在不同的应用范围（Application Scope）下，其表现出的结构也不尽相同。
 * 3. 菜单项需要被分类，且分类的层级在未来可能会扩展为多级（当前一级分类可满足）。
 * 4. 一个系统中可能会出现多组菜单。
 * 
 * 当前的菜单采用了分层设计：
 * 通过 MenuEntity 来记录系统中的所有菜单；通过 MenuItemEntity 来记录每个菜单的菜单项集合。
 * 菜单的排序由 MenuEntity 中的 MenuOrder 负责维护；
 * 菜单的权限则通过 MenuEntity、MenuItemEntity 中的 EnabledRoles 来过滤。
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aZaaS.KStar.Menus
{
    public class MenuEntity
    {
        public static readonly string DEFAULT_KEY = "sys";

        // 菜单的标识，一组用户自定义的字符串（建议进行如下命名约束：[\w_]+[\w\d_]* )
        // 针对默认菜单，使用 DEFAULT_KEY 来命名。
        [Key]
        public virtual Guid Id { get; set; }
        public virtual string Key { get; set; }

        [Required]
        public virtual string DisplayName { get; set; }

        [Required]
        public virtual string DefaultPage { get; set; }
        // IsDefault字段表明在有多组菜单的情况下，默认的菜单。
        // IsDefault字段应该是唯一的，因此实现时需要进行唯一性校验。
        public virtual bool IsDefault { get; set; }

        // MenuOrder通过逗号来分隔 MenuItem的Id 序列。这组序列标明了
        // 当前菜单项的排序。
        // 注意：当有新的菜单项或者删除了某菜单项时，需要联动更新此处。
        public virtual string MenuOrder { get; set; }

        // 通过逗号分隔一组用户或角色。用户或角色需要有前缀标识，举例如下：
        // u:boby, u:squall, r:Administrators, r:PowerUsers (空格可有可无，实现时需要进行Trim操作）
        public virtual string EnabledRoles { get; set; }


        public virtual IList<MenuItemEntity> Items { get; set; }
    }

    public class MenuItemEntity
    {
        [Key]
        public virtual Guid Id { get; set; }

        [Required]
        public virtual string DisplayName { get; set; }

        // Scope通过一组上层自定义的字符串来表明当前菜单应用的范围。
        // Scope字段应该是唯一的，因此实现时需要进行唯一性校验。
        public virtual string Scope { get; set; }
        public virtual string Hyperlink { get; set; }

        // 通过给定一个字符串key，从统一的图标资源存取服务中指定一个图标来显示。
        public virtual string IconKey { get; set; }

        // 通过逗号分隔一组用户或角色。用户或角色需要有前缀标识，举例如下：
        // u:boby, u:squall, r:Administrators, r:PowerUsers (空格可有可无，实现时需要进行Trim操作）
        public virtual string EnabledRoles { get; set; }

        public virtual Guid? ParentId { get; set; }
        // 菜单项类型，可以为：Item 或者 Catelog。
        // 未使用枚举模式是因为 
        // 1）自EF 5.0 +.NET 4.5方可原生支持，框架支持度太低。
        // 2）使用一个 Wrapper Property + Int Property 的方式不够简洁，
        //    此实体对象不会暴露到外部，通过内部规约达到效果。
        [NotMapped]
        public virtual MenuItemKind Kind
        {
            get
            {
                return (MenuItemKind)KindValue;
            }
            set
            {
                this.KindValue = (int)value;
            }
        }

        public virtual int KindValue { get; set; }
        [NotMapped]
        public virtual MenuTargetType Target
        {
            get
            {
                return (MenuTargetType)TargetValue;
            }
            set
            {
                this.TargetValue = (int)value;
            }
        }

        public virtual int TargetValue { get; set; }

        // 菜单所属的菜单标识
        [ForeignKey("Menu")]
        public virtual Guid MenuID { get; set; }

        public virtual MenuEntity Menu { get; set; }
        public virtual string MenuItemOrder { get; set; }
    }

    public enum MenuItemKind
    {
        Item = 0, Catelog = 1
    }

    public enum MenuTargetType
    {
        PopUp = 0, Redirect = 1, Panel = 2, Window = 3
    }
}

/* 项目：注册的插件实体
 * 作者：y2mao@outlook.com
 * 日期：2013/10/6
 * 
 * 【修订记录】
 * 2013/10/06：第一版设计
 * 
 * 【设计要点】
 * 1. 通过在本实体中保存插件的相关基础信息，形成注册的插件库。
 * 2. 后续页面如果期望访问某类插件，只需要提供 Key 即可指定。
 * 3. 在长期规划中，会出现用户自定义页面，通过插件给定的
 * 4. EnabledRoles 属性，管理员可以自定义不同用户可以插入的插件。
 * 5. 通过一系列前端元素的默认值控制，可以配置插件期望的形态。
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace aZaaS.KStar.Widgets
{
    public class WidgetEntity
    {
        // 插件的标识。
        // 在页面中通过 HtmlHelper.RenderWidget 方法指定插件时，通过此属性来标识。
        [Key]
        public virtual Guid Id { get; set; }
        public virtual string Key { get; set; }

        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        // 插件的内容Url，根据 RenderMode 的不同，其被访问和解析的方式也会变化。
        [Required]
        public virtual string Url { get; set; }

        // 插件默认的可访问角色和成员。
        // 其通过逗号分隔一组用户或角色。用户或角色需要有前缀标识，举例如下：
        // u:boby, u:squall, r:Administrators, r:PowerUsers (空格可有可无，实现时需要进行Trim操作）
        // 此属性主要用在后期用户可惜自定义当前页面插件时，进行权限控制。在第一期阶段用途不大。
        public virtual string EnabledRoles { get; set; }

        // 告诉插件渲染框架期望的渲染模式，当前支持：HtmlFragment 和 iFrame 两种。
        // HtmlFragment 将把 TargetUrl 的响应内容作为 Html 片段嵌入到插件容器中；
        // iFrame 将在插件容器中构造一个 iframe 元素并将src属性设置为 TargetUrl。
        // 默认采用 HtmlFragment。
        [NotMapped]
        public virtual WidgetRenderMode RenderMode
        {
            get
            {
                return (WidgetRenderMode)RenderModeValue;
            }
            set
            {
                this.RenderModeValue = (int)value;
            }
        }

        public virtual int RenderModeValue { get; set; }

        //
        [NotMapped]
        public IDictionary<string, string> HtmlAttributes { get; set; }
    }


}

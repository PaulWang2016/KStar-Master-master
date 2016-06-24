/* 项目：门户环境配置实体
 * 作者：y2mao@outlook.com
 * 日期：2013/10/6
 * 
 * 【修订记录】
 * 2013/10/06：第一版设计
 * 
 * 【设计要点】
 * 此实体用以保存门户所有基础配置信息。
 * 由于配置项繁多，通过 web.config 来定义又不利于维护（涉及到CRUD和IIS权限）
 * 因此在此通过一个键值对的方式来定义。
 * 此键值对在框架内部使用时以弱类型的方式提供。但当经过DTO转换后，其对应的DTO
 * 将转为强类型接口以为 Web 层提供更加友好的访问模式。同时，将保留索引器以实现
 * 更为灵活的扩展：
 *     比如动态新加了一个组件，这个组件在门户基础配置中注册了一些参数。
 *     但这些参数并为在DTO中被暴露，则可以通过索引器从前端访问这些扩展的参数。
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Portal
{
    public class PortalEnvironmentEntity
    {
        [Key]
        public virtual Guid Id { get; set; }

        [StringLength(256)]
        public virtual string Key { get; set; }

        public virtual string Value { get; set; }

        // 考虑以后可能会有 Web 界面来管理此实体。加入描述字段以实现管理上的友好性。
        public virtual string Description { get; set; }
    }
}

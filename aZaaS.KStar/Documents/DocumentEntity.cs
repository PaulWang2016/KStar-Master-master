/* 项目：文档库实体定义
 * 作者：y2mao@outlook.com
 * 日期：2013/10/6
 * 
 * 【修订记录】
 * 2013/10/06：第一版设计
 * 
 * 【设计要点】
 * 文档库的主要诉求如下：
 * 1. 能够为客户提供文档索引。
 * 2. 能够作为 KStar 基础组件为其他业务提供文档管理的功能。
 * 3. 能够实现基于角色的权限控制。
 * 4. 文档可能最终会被保存在其他的异构系统中。
 * 
 * 基于以上几点，文档库采用了库 - 项目 的二层结构（One-To-Many）。要点如下：
 * 1. 有一个默认的公开库，用户默认上传的内容会到此处。
 * 2. 库、项目都提供了 EnabledRoles 属性来实现各个级别的权限控制。
 * 3. 考虑不同的存储，使用私有URI模型来标识。
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using aZaaS.KStar.Menus;

namespace aZaaS.KStar.Documents
{
    public class DocumentLibraryEntity
    {
        public static readonly string DEFAULT_KEY = "public";
        public static readonly string DEFAULT_NAME = "Public Documents";

        // 文档库的标识，为用户自定义的字符串（建议进行如下命名约束：[\w_]+[\w\d_]* )
        // 针对默认库，使用 DEFAULT_KEY 来命名。
        [Key]
        public virtual Guid ID { get; set; }
        public virtual string Key { get; set; }

        // 文档库的名称。针对默认库，使用 DEFAULT_NAME 来命名。
        public virtual string DisplayName { get; set; }

        //文档库文档的默认Icon
        public virtual string IconPath { get; set; }

        public virtual string Description { get; set; }

        // 通过逗号分隔一组用户或角色。用户或角色需要有前缀标识，举例如下：
        // u:boby, u:squall, r:Administrators, r:PowerUsers (空格可有可无，实现时需要进行Trim操作）
        public virtual string EnabledRoles { get; set; }

        [ForeignKey("Menu")]
        public virtual Guid MenuID { get; set; }
        public virtual MenuEntity Menu { get; set; }

        public virtual IList<DocumentItemEntity> Items { get; set; }
    }

    public class DocumentItemEntity
    {
        [Key]
        public virtual Guid Id { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual string FileName { get; set; }

        //文档的Icon  默认跟随 Library
        public virtual string IconPath { get; set; }

        // 考虑到后续很可能会出现的文档类型查找，在保存之初就先
        // 行把扩展名提取单独保存，以为后期的需求进行性能保证。
        public virtual string FileExtension { get; set; }

        public virtual string Creator { get; set; }

        // 由业务层在进行 Insert 操作时默认分配
        public virtual DateTime CreateTime { get; set; }

        // 通过逗号分隔一组用户或角色。用户或角色需要有前缀标识，举例如下：
        // u:boby, u:squall, r:Administrators, r:PowerUsers (空格可有可无，实现时需要进行Trim操作）
        public virtual string EnabledRoles { get; set; }

        // 文档所属的库标识
        [ForeignKey("Library")]
        public virtual Guid DocumentLibraryID { get; set; }

        // 存储内容的目标位置，以私有URI的方式记录。
        // KStar 会对内置的所有 StorageProvider 以管道过滤器架构进行
        // 筛选，并最终由给定的 StorageProvider 来提供对应的存储操作。
        // 私有URI的格式举例：
        //      ftp://<file_name>
        //      sps://<doc_lib>/<doc_name>
        public virtual string StorageUri { get; set; }

        public virtual DocumentLibraryEntity Library { get; set; }
        public virtual string DocumentItemOrder { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Localization
{


    /// <summary>
    /// /数据 多语言化
    /// </summary>
    public class LocalizationResourceEntity
    {

        [Key]
        public Guid ID { get; set; }

        public string DataBaseName { get; set; }

        // --表名称 
        public string TableName { get; set; }

        //--列名称
        public string ColumnName { get; set; }


        //--列值
        public string ResxKey { get; set; }


        //--资源值
        public string ResxValue { get; set; }



    }

    public class PortalEnvironmentEntity
    {
        [Key]
        public Guid ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}

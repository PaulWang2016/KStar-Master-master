//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace aZaaS.KStar.Web.Models.BasisEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuartzCacheSendMail
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ReplyTo { get; set; }
        public string Prompt { get; set; }
        public bool Forward { get; set; }
        public Nullable<System.DateTime> ReplyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
    }
}
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
    
    public partial class VW_ErrorLog
    {
        public int ID { get; set; }
        public int ProcID { get; set; }
        public int ProcInstID { get; set; }
        public byte State { get; set; }
        public byte Context { get; set; }
        public int ObjectID { get; set; }
        public string Descr { get; set; }
        public System.DateTime Date { get; set; }
        public System.Guid Code { get; set; }
        public string ItemName { get; set; }
        public string StackTrace { get; set; }
        public int CodeID { get; set; }
        public string Folio { get; set; }
    }
}

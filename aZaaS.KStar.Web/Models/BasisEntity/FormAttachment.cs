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
    
    public partial class FormAttachment
    {
        public int FileId { get; set; }
        public Nullable<System.Guid> FileGuid { get; set; }
        public string FormId { get; set; }
        public Nullable<int> ProcInstId { get; set; }
        public string ProcessName { get; set; }
        public string ActivityName { get; set; }
        public Nullable<long> FileBytes { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string StoragePath { get; set; }
        public string DownloadUrl { get; set; }
        public string Uploader { get; set; }
        public string UploaderName { get; set; }
        public Nullable<System.DateTime> UploadedDate { get; set; }
        public string FileComment { get; set; }
    }
}
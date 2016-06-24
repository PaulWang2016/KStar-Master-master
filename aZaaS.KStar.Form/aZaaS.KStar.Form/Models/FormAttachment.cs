using System;
using System.Collections.Generic;

namespace aZaaS.KStar.Form.Models
{
    public partial class FormAttachment
    {
        public int FileId { get; set; } // FileId (Primary key)
        public Guid? FileGuid { get; set; } // FileGuid
        public string FormId { get; set; } // FormId
        public int? ProcInstId { get; set; } // ProcInstId
        public string ProcessName { get; set; } // ProcessName
        public string ActivityName { get; set; } // ActivityName
        public long? FileBytes { get; set; } // FileBytes
        public string FileType { get; set; } // FileType
        public string FileExtension { get; set; } // FileExtension
        public string OldFileName { get; set; } // OldFileName
        public string NewFileName { get; set; } // NewFileName
        public string StoragePath { get; set; } // StoragePath
        public string DownloadUrl { get; set; } // DownloadUrl
        public string Uploader { get; set; } // Uploader
        public string UploaderName { get; set; } // UploaderName
        public DateTime? UploadedDate { get; set; } // UploadedDate
        public string FileComment { get; set; } // FileComment
    }
}

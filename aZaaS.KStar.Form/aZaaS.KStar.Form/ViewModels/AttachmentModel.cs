using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class AttachmentModel
    {
        public AttachmentModel()
        {
            FileComment = string.Empty;
        }

        public int FileId { get; set; }
        public Guid FileGuid { get; set; }
        public FileState FileState { get; set; }
        public bool FileOwner { get; set; }

        public string FormId { get; set; }
        public int ProcInstId { get; set; }
        public string ProcessName { get; set; }
        public string ActivityName { get; set; }

        public long FileBytes { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public string OldFileName { get; set; }
        public string NewFileName { get; set; }
        public string DownloadUrl { get; set; }

        [JsonIgnore]
        public string StoragePath { get; set; }


        public DateTime UploadedDate { get; set; }
        public string Uploader { get; set; }
        public string UploaderName { get; set; }

        public string FileComment { get; set; }
    }

    public enum FileState
    {
        Default,
        Edited,
        Cached,
        Removed
    }
}

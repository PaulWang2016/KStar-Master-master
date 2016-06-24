using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.ViewModels
{
    public class CacheFileModel
    {
        public Guid FileGuid { get; set; }

        public long FileBytes { get; set; }
        public string FileType { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public string StoragePath { get; set; }
        public string Uploader { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}

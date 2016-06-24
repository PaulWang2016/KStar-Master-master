using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace aZaaS.KStar.Form.Infrastructure
{
    public class FileUploadContext
    {
        private static readonly object _lock = new object();
        private const string UPLOADED_CACHE_NAME = "_KStarForm_CachedUploadedFiles";

        public IEnumerable<AttachmentModel> CachedFiles { get; set; }
        public IEnumerable<AttachmentModel> EditedFiles { get; set; }
        public IEnumerable<AttachmentModel> RemovedFiles { get; set; }

        public FileUploadContext()
        {
            CachedFiles = new List<AttachmentModel>();
            EditedFiles = new List<AttachmentModel>();
            RemovedFiles = new List<AttachmentModel>();
        }

        public string CacheFolder
        {
            get
            {
                var relativeFolder = @"~/Cache/Uploads/";

                return HttpContext.Current.Server.MapPath(relativeFolder);
            }
        }

        public Dictionary<Guid, CacheFileModel> UploadedCache
        {
            get
            {
                lock (_lock)
                {
                    if (HttpContext.Current.Cache[UPLOADED_CACHE_NAME] == null)
                    {
                        HttpContext.Current.Cache[UPLOADED_CACHE_NAME] = new Dictionary<Guid, CacheFileModel>();
                    }

                    return HttpContext.Current.Cache[UPLOADED_CACHE_NAME] as Dictionary<Guid, CacheFileModel>;
                }
            }
        }

        public CacheFileModel GetCachedFile(Guid fileGuid)
        {
            var cache = UploadedCache;
            if (cache.ContainsKey(fileGuid))
                return cache[fileGuid];

            return null;
        }

        public void RemoveCachedFile(Guid fileGuid)
        {
            var cache = UploadedCache;
            if (cache.ContainsKey(fileGuid))
                cache.Remove(fileGuid);
        }

        public void StoreCachedFile(Guid fileGuid, string destFolder, string fileName)
        {
            var cachedFile = GetCachedFile(fileGuid);
            if (cachedFile != null && !string.IsNullOrEmpty(fileName))
            {

                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }

                fileName = Path.Combine(destFolder, fileName);
                if (File.Exists(cachedFile.StoragePath))
                {
                    File.Copy(cachedFile.StoragePath, fileName, true);
                    File.Delete(cachedFile.StoragePath);
                }
            }
        }

        public void CacheUploadedFile(HttpPostedFileBase file, CacheFileModel fileInfo)
        {
            var cache = UploadedCache;
            if (file != null && fileInfo != null)
            {
                if (!Directory.Exists(CacheFolder))
                    Directory.CreateDirectory(CacheFolder);

                file.SaveAs(fileInfo.StoragePath);
                if (!cache.ContainsKey(fileInfo.FileGuid))
                    cache.Add(fileInfo.FileGuid, fileInfo);
            }
        }
    }
}

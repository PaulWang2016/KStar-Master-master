using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.ViewModels;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace aZaaS.KStar.Form
{
    public class KStarFormFileStorage : IFormFileStorage
    {
        private readonly FileUploadContext _uploadContext;
        private const string DEFAULT_PATH_DATEFORMAT = @"{[YMDHS\-\:\s]+}";
        private const string DEFAULT_PATH_STORAGEFOLDER = @"~/Uploads/Forms/";
        private const string ATTACHMENT_PATH_DATEFORMAT_NAME = "KSTARFORM:AttachmentDateFormat";
        private const string ATTACHMENT_PATH_STORAGEFOLDER_NAME = "KSTARFORM:AttachmentStorageFolder";

        public KStarFormFileStorage(FileUploadContext uploadContext)
        {
            _uploadContext = uploadContext;
        }

        protected string AttachmentDateFormat
        {
            get
            {

                var dateFormat = ConfigurationManager.AppSettings[ATTACHMENT_PATH_DATEFORMAT_NAME];

                return string.IsNullOrEmpty(dateFormat) ? DEFAULT_PATH_DATEFORMAT : dateFormat;
            }
        }

        protected string AttachmentStorageFolder
        {
            get
            {

                var storageFolder = ConfigurationManager.AppSettings[ATTACHMENT_PATH_STORAGEFOLDER_NAME];

                return string.IsNullOrEmpty(storageFolder) ? DEFAULT_PATH_STORAGEFOLDER : storageFolder;
            }
        }


        public string StoreFile(AttachmentModel file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            var storageFolder = this.GetStorageFolder(file);
            var cachedFile = _uploadContext.GetCachedFile(file.FileGuid);
            if (cachedFile == null)
                throw new InvalidOperationException("The target cache file was not found!");

            //var fileName = Path.ChangeExtension(file.NewFileName, file.FileExtension);
            var fileName = string.Format("{0}{1}", file.NewFileName, file.FileExtension);
            if (cachedFile != null && !string.IsNullOrEmpty(fileName))
            {
                if (!Directory.Exists(storageFolder))
                    Directory.CreateDirectory(storageFolder);

                fileName = Path.Combine(storageFolder, fileName);
                if (File.Exists(cachedFile.StoragePath))
                {
                    File.Copy(cachedFile.StoragePath, fileName, true);
                    File.Delete(cachedFile.StoragePath);
                }
            }

            return fileName;
        }

        public Stream FetchFile(AttachmentModel file)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (string.IsNullOrEmpty(file.StoragePath))
                throw new InvalidOperationException("The file storage location was not found!");

            return new FileStream(file.StoragePath, FileMode.Open);
        }


        private string GetStorageFolder(object source)
        {
            var storageFolder = string.Empty;

            var dateRegex = new Regex(AttachmentDateFormat,
                                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            storageFolder = dateRegex.Replace(AttachmentStorageFolder, new MatchEvaluator(delegate(Match m)
            {
                var format = m.Value;
                format = format.Substring(1, format.Length - 1 - 1);
                return DateTime.Now.ToString(format);
            }));

            storageFolder = storageFolder.FormatWith(source);

            if (VirtualPathUtility.IsAppRelative(storageFolder))
                storageFolder = HttpContext.Current.Server.MapPath(storageFolder);

            return storageFolder;
        }

    }
}

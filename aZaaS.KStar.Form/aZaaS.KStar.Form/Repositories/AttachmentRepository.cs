using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Repositories
{
    public class AttachmentRepository
    {
        public AttachmentModel Get(Guid fileGuid)
        {
            using (var db = new aZaaSKStarFormContext())
            {
                AttachmentModel fileModel = null;
                var item = db.FormAttachments.FirstOrDefault(f => f.FileGuid == fileGuid);
                if (item != null)
                {
                    fileModel = new AttachmentModel()
                    {
                        FormId = item.FormId,
                        ProcInstId = item.ProcInstId ?? 0,
                        ProcessName = item.ProcessName,
                        ActivityName = item.ActivityName,

                        FileGuid = item.FileGuid ?? Guid.Empty,
                        FileBytes = item.FileBytes ?? 0,
                        FileType = item.FileType,
                        FileExtension = item.FileExtension,
                        OldFileName = item.OldFileName,
                        NewFileName = item.NewFileName,

                        Uploader = item.Uploader,
                        UploaderName = item.UploaderName,
                        UploadedDate = item.UploadedDate ?? DateTime.MinValue,

                        StoragePath = item.StoragePath,
                        DownloadUrl = item.DownloadUrl,
                        FileComment = item.FileComment
                    };
                }

                return fileModel;
            }
        }
        public IEnumerable<AttachmentModel> GetAll()
        {
            var files = new List<AttachmentModel>();
            using (var db = new aZaaSKStarFormContext())
            {
                var attachments = db.FormAttachments.ToList();

                attachments.ForEach(item =>
                {
                    var fileModel = new AttachmentModel()
                    {
                        FormId = item.FormId,
                        ProcInstId = item.ProcInstId ?? 0,
                        ProcessName = item.ProcessName,
                        ActivityName = item.ActivityName,

                        FileGuid = item.FileGuid ?? Guid.Empty,
                        FileBytes = item.FileBytes ?? 0,
                        FileType = item.FileType,
                        FileExtension = item.FileExtension,
                        OldFileName = item.OldFileName,
                        NewFileName = item.NewFileName,

                        Uploader = item.Uploader,
                        UploaderName = item.UploaderName,
                        UploadedDate = item.UploadedDate ?? DateTime.MinValue,

                        StoragePath = item.StoragePath,
                        DownloadUrl = item.DownloadUrl,
                        FileComment = item.FileComment
                    };

                    files.Add(fileModel);
                });
            }

            return files;
        }

        private void Add(aZaaSKStarFormContext db, IEnumerable<AttachmentModel> files)
        {
            if (files == null || !files.Any())
                return;

            foreach (var item in files)
            {
                var attachment = new FormAttachment()
                {
                    FormId = item.FormId,
                    ProcInstId = item.ProcInstId,
                    ProcessName = item.ProcessName,
                    ActivityName = item.ActivityName,

                    FileGuid = item.FileGuid,
                    FileBytes = item.FileBytes,
                    FileType = item.FileType,
                    FileExtension = item.FileExtension,
                    OldFileName = item.OldFileName,
                    NewFileName = item.NewFileName,

                    Uploader = item.Uploader,
                    UploaderName = item.UploaderName,
                    UploadedDate = item.UploadedDate,

                    StoragePath = item.StoragePath,
                    DownloadUrl = item.DownloadUrl,
                    FileComment = item.FileComment
                };

                db.FormAttachments.Add(attachment);
            }
        }

        private void Update(aZaaSKStarFormContext db, IEnumerable<AttachmentModel> files)
        {
            if (files == null || !files.Any())
                return;

            foreach (var item in files)
            {
                var source = db.FormAttachments.FirstOrDefault(f => f.FileGuid == item.FileGuid);
                if (source != null)
                {
                    source.NewFileName = item.NewFileName;
                    source.FileComment = item.FileComment;
                }
            }

        }

        private void Remove(aZaaSKStarFormContext db, IEnumerable<AttachmentModel> files)
        {
            if (files == null || !files.Any())
                return;

            foreach (var item in files)
            {
                var source = db.FormAttachments.FirstOrDefault(f => f.FileGuid == item.FileGuid);
                if (source != null)
                {
                    db.FormAttachments.Remove(source);
                }
            }
        }

        public void Save(IEnumerable<AttachmentModel> addFiles, IEnumerable<AttachmentModel> updateFiles, IEnumerable<AttachmentModel> removeFiles)
        {
            using (var db = new aZaaSKStarFormContext())
            {
                Add(db, addFiles);
                Update(db, updateFiles);
                Remove(db, removeFiles);

                db.SaveChanges();
            }
        }

    }
}

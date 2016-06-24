using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Workflow;
using System.IO;
using System.Web;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Repositories;
using aZaaS.KStar.Helper;
using System.Linq.Expressions;

namespace aZaaS.KStar.Form
{
    public class KStarFormStorageProvider : IStorageProvider
    {
        private FileUploadContext _uploadContext;
        private readonly AttachmentRepository _attachmentRepository;
        private IFormFileStorage _fileStorage;

        public KStarFormStorageProvider()
        {
            _uploadContext = new FileUploadContext();
            _attachmentRepository = new AttachmentRepository();
            _fileStorage = new KStarFormFileStorage(_uploadContext);
        }

        public KStarFormModel ReadForm(int formId, string loggedOnUser)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var header = db.ProcessFormHeaders.FirstOrDefault(h => h.FormID == formId);

                if (header == null)
                {
                    //throw new ArgumentException("Invalid FormId!");
                    return new KStarFormModel();
                }

                header.ProcessFolio = header.ProcessFolio.Split('-').Last();

                var content = db.ProcessFormContents.FirstOrDefault(c => c.FormID == formId);

                var formModel = Mapper.Map<ProcessFormHeader, KStarFormModel>(header);
                Mapper.Map<ProcessFormContent, KStarFormModel>(content, formModel);
                formModel.Attachments = new List<AttachmentModel>();
                formModel.ProcessLogs = new List<ProcessLogModel>();
                formModel.Toolbar = new ToolbarActionModel();

                var attachments = db.FormAttachments.Where(a => a.FormId == formId.ToString()).ToList();
                var attachModels = Mapper.Map<IList<FormAttachment>, IList<AttachmentModel>>(attachments);
                if (attachModels != null)
                {
                    foreach (var attachModel in attachModels)
                    {
                        var isOwner = string.Equals(attachModel.Uploader, loggedOnUser, StringComparison.OrdinalIgnoreCase);
                        attachModel.FileOwner = isOwner;
                        attachModel.FileState = isOwner ? FileState.Edited : FileState.Default;
                    }

                    formModel.Attachments = attachModels;
                }

                if (header.ProcInstID != null && header.ProcInstID > 0)
                {
                    formModel.ProcessLogs = GetProcessLogs(Convert.ToInt32(header.ProcInstID));
                }

                return formModel;
            }
        }

        public int SaveForm(KStarFormModel formModel)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var header = Mapper.Map<KStarFormModel, ProcessFormHeader>(formModel);
                header.SubmitDate = DateTime.Now;
                var content = Mapper.Map<KStarFormModel, ProcessFormContent>(formModel);
                var attachments = formModel.Attachments;
                //var attachments = Mapper.Map<IEnumerable<AttachmentModel>, IEnumerable<FormAttachment>>(formModel.Attachments);

                db.ProcessFormHeaders.Add(header);
                db.SaveChanges();

                content.FormID = header.FormID;
                content.XmlData = JsonHelper.JsonToXml(content.JsonData);
                db.ProcessFormContents.Add(content);
                db.SaveChanges();

                attachments.ToList().ForEach(a => a.FormId = header.FormID.ToString());
                SaveAttachment(attachments);

                //Ignore processlog 

                return header.FormID;
            }
        }

        public void UpdateForm(KStarFormModel formModel)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var header = Mapper.Map<KStarFormModel, ProcessFormHeader>(formModel);
                var content = Mapper.Map<KStarFormModel, ProcessFormContent>(formModel);
                var attachments = formModel.Attachments;
                //var attachments = Mapper.Map<IEnumerable<AttachmentModel>, IEnumerable<FormAttachment>>(formModel.Attachments);

                var sourceHeader = db.ProcessFormHeaders.Find(header.FormID);
                header.ProcInstID = sourceHeader.ProcInstID;
                header.ProcessFolio = sourceHeader.ProcessFolio;
                db.Entry<ProcessFormHeader>(sourceHeader).CurrentValues.SetValues(header);
                db.SaveChanges();

                var sourceContent = db.ProcessFormContents.FirstOrDefault(c => c.FormID == header.FormID);
                sourceContent.JsonData = content.JsonData;
                sourceContent.XmlData = JsonHelper.JsonToXml(content.JsonData);
                db.SaveChanges();

                attachments.ToList().ForEach(a => a.FormId = header.FormID.ToString());

                //var sourceAttachments = db.FormAttachments.Where(fa => fa.FormId == header.FormID.ToString()).ToList();
                //db.FormAttachments.RemoveRange(sourceAttachments);
                //db.FormAttachments.AddRange(attachments);
                SaveAttachment(attachments);

                db.SaveChanges();

                //Ignore processlog 
            }
        }

        public void UpdateForm(int formId, string processFolio)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var form = db.ProcessFormHeaders.FirstOrDefault(f => f.FormID == formId);

                form.ProcessFolio = processFolio;

                db.SaveChanges();
            }
        }

        public void UpdateFormContent(int formId, string contentData)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var sourceContent = db.ProcessFormContents.FirstOrDefault(c => c.FormID == formId);
                sourceContent.JsonData = contentData;
                sourceContent.XmlData = JsonHelper.JsonToXml(contentData);

                db.SaveChanges();
            }
        }

        public void DeleteForm(int formId)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var header = db.ProcessFormHeaders.FirstOrDefault(r => r.FormID == formId);

                if (header == null)
                {
                    //throw new ArgumentException("Invalid draftId");
                    return;
                }

                var content = db.ProcessFormContents.FirstOrDefault(r => r.FormID == formId);
                var attachments = db.FormAttachments.Where(r => r.FormId == formId.ToString());

                db.ProcessFormHeaders.Remove(header);
                db.ProcessFormContents.Remove(content);
                db.FormAttachments.RemoveRange(attachments);

                db.SaveChanges();
            }
        }

        public void UpdateFormDraftStatus(int formId, bool isDraft = true)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var form = db.ProcessFormHeaders.FirstOrDefault(f => f.FormID == formId);
                form.IsDraft = isDraft;
                form.DraftUrl = isDraft ? form.DraftUrl : string.Empty;

                db.SaveChanges();
            }
        }

        public void UpdateFormDraftStatus(int formId, int procInstId, string processName, string processFolio, bool isDraft = true)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var form = db.ProcessFormHeaders.FirstOrDefault(f => f.FormID == formId);
                form.IsDraft = isDraft;
                form.ProcInstID = procInstId;
                form.DraftUrl = isDraft ? form.DraftUrl : string.Empty;
                form.ProcessFolio = processFolio;

                var attachments = db.FormAttachments.Where(f => f.FormId == formId.ToString()).ToList();
                attachments.ForEach(attachment =>
                {
                    attachment.ProcInstId = form.ProcInstID;
                    attachment.ProcessName = processName;
                });

                db.SaveChanges();

                SaveProcArgs(formId, procInstId);
            }
        }

        private void SaveProcArgs(int formId, int procInstId)
        {
            var flowArg = new ViewFlowArgs();
            var args = string.Format("_FormId={0}", formId);
            flowArg.SaveFlowArgs(args, procInstId.ToString());
        }

        public void UpdateFormDraftUrl(int formId, string draftUrl)
        {
            using (aZaaSKStarFormContext db = new aZaaSKStarFormContext())
            {
                var form = db.ProcessFormHeaders.FirstOrDefault(f => f.FormID == formId);

                form.DraftUrl = draftUrl;

                db.SaveChanges();
            }
        }

        public int GetFormId(int procInstId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormHeaders.FirstOrDefault(r => r.ProcInstID == procInstId);

                if (item == null)
                {
                    throw new ArgumentException("Invalid procInstId");
                }

                return item.FormID;
            }
        }

        public ProcessFormHeader GetProcessFormHeaderByProcInstId(int procInstId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormHeaders.FirstOrDefault(r => r.ProcInstID == procInstId);

                if (item == null)
                {
                    throw new ArgumentException("Invalid procInstId");
                }

                return item;
            }
        }

        public ProcessFormHeader GetProcessFormHeaderByFormId(int formId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormHeaders.FirstOrDefault(r => r.FormID == formId);

                return item;
            }
        }

        public IEnumerable<ProcessFormHeader> GetInsteadRequest(string applyAccount)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormHeaders.Where(r => r.ApplicantAccount == applyAccount && r.SubmitterAccount != applyAccount);

                return item;
            }
        }

        public ProcessFormContent GetProcessFormContent(int formId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormContents.FirstOrDefault(r => r.FormID == formId);

                return item;
            }
        }

        public IList<MyDraftModel> GetDrafts(string userName)
        {
            var myDrafts = new List<MyDraftModel>();
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormHeaders.Where(r => r.SubmitterAccount == userName && r.IsDraft == true && !string.IsNullOrEmpty(r.DraftUrl));

                if (items != null)
                {
                    items.ToList().ForEach(item =>
                    {
                        var draft = new MyDraftModel()
                        {
                            FormId = item.FormID,
                            FormSubject = item.FormSubject,
                            DraftUrl = item.DraftUrl,
                            ProcessName = GetProcessNameByStartUrl(item.DraftUrl.Split('?').First()),
                            CreatedDate = item.SubmitDate
                        };

                        myDrafts.Add(draft);
                    });
                }
            }

            return myDrafts;
        }

        public IList<MyDraftModel> GetDraftsByPage(string userName,DateTime? startDate, DateTime? endDate, string folio, int pageIndex, int pageSize, List<aZaaS.KStar.Helper.SortDescriptor> sort, out int total)
        {
            var myDrafts = new List<MyDraftModel>();
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {                
                var items = context.ProcessFormHeaders.Where(r=>r.SubmitterAccount==userName&&r.IsDraft == true && !string.IsNullOrEmpty(r.DraftUrl));
                if (startDate != null && endDate != null)
                {
                    items = items.Where(r => r.SubmitDate>=startDate&&r.SubmitDate<=endDate);    
                }
                if (!string.IsNullOrEmpty(folio))
                {
                    items = items.Where(r => r.FormSubject.Contains(folio));
                }              
                total = items.Count();

                if (sort.Count == 0)
                {
                    items = items.OrderByDescending(r => r.SubmitDate);
                }
                else
                {
                    foreach (var st in sort)
                    {
                        items = DataListSort.DataSorting<ProcessFormHeader>(items.AsQueryable(), st.field, st.dir);
                    }
                }
              
                var result = items.ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (result != null)
                {
                    result.ForEach(item =>
                    {
                        var draft = new MyDraftModel()
                        {
                            FormId = item.FormID,
                            FormSubject = item.FormSubject,
                            DraftUrl = item.DraftUrl,
                            ProcessName = GetProcessNameByStartUrl(item.DraftUrl.Split('?').First()),
                            CreatedDate = item.SubmitDate
                        };

                        myDrafts.Add(draft);
                    });
                }
            }

            return myDrafts;
        }

        private string GetProcessNameByStartUrl(string startUrl)
        {
            var configManager = new ConfigManager(AuthenticationType.Windows);
            return configManager.GetProcessNameByStartUrl(startUrl);
        }

        public void DeleteDraft(int draftId)
        {
            DeleteForm(draftId);
        }

        private IList<ProcessLogModel> GetProcessLogs(int procInstId)
        {
            var wfService = new KStarFormWorkflowService(Framework.Workflow.AuthenticationType.Windows);
            var processlogs = wfService.GetProcessLog(procInstId);

            return processlogs;
        }

        public Activity GetActivityByViewMode(int formId, string userName)
        {
            var formHeader = GetProcessFormHeaderByFormId(formId);
            int procInstId = Convert.ToInt32(formHeader.ProcInstID);

            var processLogs = GetProcessLogs(procInstId);

            var myProcessLogs = processLogs.Where(r => r.ActionName != "撤回" && r.ActionName != "审 阅" && r.ActionName != "作  废");
            //myProcessLogs = processLogs;

            var activityItem = new Activity()
            {
                ID = 0,
                Name = ""
            };

            if (myProcessLogs.Count() > 0)
            {
                var maxDate = myProcessLogs.Max(r => r.CreatedDate);
                var currLog = myProcessLogs.FirstOrDefault(x => x.CreatedDate == maxDate);
                var configManager = new ConfigManager(AuthenticationType.Windows);
                activityItem = configManager.GetActivityInfo(currLog.ProcInstID, currLog.ActivityName);
            }

            return activityItem;
        }

        public HashSet<int> GetOperActivitys(int formId, string userName)
        {
            var formHeader = GetProcessFormHeaderByFormId(formId);
            int procInstId = Convert.ToInt32(formHeader.ProcInstID);

            var processLogs = GetProcessLogs(procInstId);

            var myProcessLogs = processLogs.Where(r => r.ActionName != "撤回" && r.ActionName != "审 阅" && r.ActionName != "作  废");
            //myProcessLogs = processLogs;

            var activityList = new HashSet<int>();
            var configManager = new ConfigManager(AuthenticationType.Windows);

            myProcessLogs.ToList().ForEach(log =>
            {
                var actItem = configManager.GetActivityInfo(procInstId, log.ActivityName);
                activityList.Add(actItem.ID);
            });

            return activityList;
        }

        public IList<AttachmentModel> AttachmentUpload(HttpFileCollectionBase files, string uploader, string activityName, string downloadUrl)
        {
            var uploadResults = new List<AttachmentModel>();

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];

                var userName = uploader;
                var userDisplayName = new KStarFormOrganizationService().GetDisplayName(userName);

                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                var fileGuid = Guid.NewGuid();
                var cacheFileName = Path.ChangeExtension(fileGuid.ToString(), fileExtension);
                var cacheFilePath = Path.Combine(_uploadContext.CacheFolder, cacheFileName);

                var cachedFile = new CacheFileModel()
                {
                    FileGuid = fileGuid,
                    FileName = fileName,
                    FileType = file.ContentType,
                    FileBytes = file.ContentLength,
                    FileExtension = fileExtension,
                    StoragePath = cacheFilePath,
                    Uploader = userName,
                    UploadedDate = DateTime.Now
                };

                _uploadContext.CacheUploadedFile(file, cachedFile);

                var fileModel = new AttachmentModel()
                {
                    FileState = FileState.Cached,
                    FileOwner = true,
                    FileGuid = cachedFile.FileGuid,
                    FileType = cachedFile.FileType,
                    FileBytes = cachedFile.FileBytes,
                    FileExtension = fileExtension,
                    OldFileName = fileNameWithoutExtension,
                    NewFileName = fileNameWithoutExtension,
                    StoragePath = string.Empty,
                    DownloadUrl = GetDownloadUrl(downloadUrl, fileGuid, true),

                    FormId = "",
                    ProcInstId = 0,
                    ProcessName = "",
                    //ProcessName = GetProcessFullNameByStartUrl(),
                    ActivityName = activityName,

                    Uploader = cachedFile.Uploader,
                    UploaderName = userDisplayName,
                    UploadedDate = cachedFile.UploadedDate
                };

                uploadResults.Add(fileModel);
            }


            return uploadResults;
        }

        public AttachmentModel GetDownloadFile(Guid fileGuid)
        {
            var file = _attachmentRepository.Get(fileGuid);

            return file;
        }

        public Stream GetFetchFile(AttachmentModel file)
        {
            return _fileStorage.FetchFile(file);
        }

        public CacheFileModel GetCachedFile(Guid fileGuid)
        {
            return _uploadContext.GetCachedFile(fileGuid);
        }

        public void ProcInstCancel(int procInstId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormCancels.FirstOrDefault(r => r.ProcInstId == procInstId);

                if (item != null)
                {
                    item.Status = 5;
                }
                else
                {
                    item = new ProcessFormCancel() { Id = Guid.NewGuid(), ProcInstId = procInstId, Status = 5 };
                    context.ProcessFormCancels.Add(item);
                }

                context.SaveChanges();
            }
        }

        public bool IsApplicatant(int formId, string userName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormHeaders.FirstOrDefault(r => r.FormID == formId && r.ApplicantAccount == userName && r.IsDraft == false);

                if (item != null)
                {
                    return true;
                }

                return false;
            }
        }

        public void SaveApprovalDraft(string serialNo, string actionName, string actionComment)
        {
            using (var db = new aZaaSKStarFormContext())
            {
                var item = db.ProcessFormApprovalDrafts.FirstOrDefault(r => r.SerialNo == serialNo);

                if (item == null)
                {
                    item = new ProcessFormApprovalDraft()
                    {
                        SerialNo = serialNo,
                        ActionName = actionName,
                        ActionComment = actionComment
                    };

                    db.ProcessFormApprovalDrafts.Add(item);
                }
                else
                {
                    item.ActionName = actionName;
                    item.ActionComment = actionComment;
                }

                db.SaveChanges();
            }
        }

        public ProcessFormApprovalDraft GetProcessFormApprovalDraft(string serialNo)
        {
            using(var db=new aZaaSKStarFormContext())
            {
                var item = db.ProcessFormApprovalDrafts.FirstOrDefault(r => r.SerialNo == serialNo);

                return item;
            }
        }

        private bool SaveAttachment(IEnumerable<AttachmentModel> files)
        {
            if (files == null)
                throw new ArgumentNullException("files");

            var cachedFiles = new List<AttachmentModel>();
            var editedFiles = new List<AttachmentModel>();
            var removedFiles = new List<AttachmentModel>();

            foreach (var item in files)
            {
                item.UploadedDate = DateTime.Now;

                switch (item.FileState)
                {
                    case FileState.Edited:
                        editedFiles.Add(item);
                        break;
                    case FileState.Cached:
                        var cachedFile = _uploadContext.GetCachedFile(item.FileGuid);
                        if (cachedFile == null)
                            throw new InvalidDataException("Cache file was not found!");
                        item.UploadedDate = cachedFile.UploadedDate;
                        item.StoragePath = _fileStorage.StoreFile(item);
                        item.DownloadUrl = item.DownloadUrl.Replace("DownloadCacheFile", "DownloadFile");
                        _uploadContext.RemoveCachedFile(item.FileGuid);
                        cachedFiles.Add(item);
                        break;
                    case FileState.Removed:
                        removedFiles.Add(item);
                        break;
                }
            }

            _uploadContext.CachedFiles = cachedFiles;
            _uploadContext.EditedFiles = editedFiles;
            _uploadContext.RemovedFiles = removedFiles;

            _attachmentRepository.Save(cachedFiles, editedFiles, removedFiles);

            return true;
        }

        private string GetDownloadUrl(string url, Guid fileGuid, bool cached)
        {
            var downloadUrl = string.Format("{0}?fileGuid={1}", url, fileGuid);
            if (!cached)
            {
                downloadUrl = downloadUrl.Replace("DownloadCacheFile", "DownloadFile");
            }

            return downloadUrl;
            //return Url.Action(cached ? "DownloadCacheFile" : "DownloadFile", RouteData.Values["controller"].ToString(), new { fileGuid = fileGuid });
        }
    }
}

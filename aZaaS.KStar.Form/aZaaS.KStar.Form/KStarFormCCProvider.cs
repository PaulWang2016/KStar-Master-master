using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Workflow.Configuration;
using aZaaS.Framework.Workflow;

namespace aZaaS.KStar.Form
{
    public class KStarFormCCProvider : IFormCCProvider
    {
        private readonly FormCCRepository _formCCRepository;
        private readonly ConfigManager _configManager;
        public KStarFormCCProvider()
        {
            _formCCRepository = new FormCCRepository();
            _configManager = new ConfigManager(AuthenticationType.Form);
        }

        public void Save(IList<ProcessFormCC> ccList)
        {
            _formCCRepository.Add(ccList);
        }

        public void Save(IList<ProcessFormCC> ccList, string viewAddress)
        {
            _formCCRepository.Add(ccList, viewAddress);
        }

        public void UpdateReceiverStatus(Guid sysId, string comment)
        {
            _formCCRepository.UpdateStatus(sysId, comment);
        }

        public void UpdateReceiverStatus(int formId, int activityId, string receiver, string comment)
        {
            _formCCRepository.UpdateStatus(formId, activityId, receiver, comment);
        }

        public void Delete(Guid sysId)
        {
            _formCCRepository.Delete(sysId);
        }

        public void Delete(int formId, string receiver)
        {
            _formCCRepository.Delete(formId, receiver);
        }

        public void DeleteAll(int formId)
        {
            _formCCRepository.DeleteAll(formId);
        }

        public IList<FormCCModel> SendFormCC(string userName)
        {
            var formCcItems = new List<FormCCModel>();
            var viewurl = string.Empty;
            var sendItems = _formCCRepository.SendFormCC(userName);
            var header = new ProcessFormHeader();

            if (sendItems != null)
            {
                sendItems.ToList().ForEach(item =>
                {
                    header = GetProcessHeader(item.FormId);

                    if (header != null)
                    {
                        viewurl = item.FormViewUrl.Substring(0, item.FormViewUrl.IndexOf("&"));
                        var ccItem = new FormCCModel()
                        {
                            SysId = item.SysId,
                            ProcessFolio = header.ProcessFolio.Split('-').First(),
                            ProcInstNo = header.ProcessFolio.Split('-').Last(),
                            ProcessName = header.ProcessFolio.Split('-').First(),
                            Originator = item.Originator,
                            OriginatorName = item.OriginatorName,
                            Receiver = item.Receiver,
                            ReceiverName = item.ReceiverName,
                            ReceiverStatus = item.ReceiverStatus,
                            Applicant = header.SubmitterAccount,
                            ApplicantName = header.SubmitterDisplayName,
                            ApplicationDate = header.SubmitDate,
                            FormViewUrl = viewurl,
                            CreatedDate = item.CreatedDate,
                            ReceiverDate = item.ReceiverDate,
                            Comment = item.ReviewComment
                        };

                    formCcItems.Add(ccItem);
                    }
                });
            }

            return formCcItems;
        }

        public IList<FormCCModel> ReceiveFormCC(string userName)
        {
            var formCcItems = new List<FormCCModel>();
            var viewurl = string.Empty;
            var sendItems = _formCCRepository.ReceiveFormCC(userName);
            var header = new ProcessFormHeader();

            if (sendItems != null)
            {
                sendItems.ToList().ForEach(item =>
                {
                    header = GetProcessHeader(item.FormId);

                    if (header != null)
                    {
                        viewurl = item.ReceiverStatus ? item.FormViewUrl.Substring(0, item.FormViewUrl.IndexOf("&")) : item.FormViewUrl;
                        var ccItem = new FormCCModel()
                        {
                            SysId = item.SysId,
                            ProcessFolio = header.ProcessFolio.Split('-').First(),
                            ProcInstNo = header.ProcessFolio.Split('-').Last(),
                            Originator = item.Originator,
                            OriginatorName = item.OriginatorName,
                            ProcessName = header.ProcessFolio.Split('-').First(),
                            Receiver = item.Receiver,
                            ReceiverName = item.ReceiverName,
                            ReceiverStatus = item.ReceiverStatus,
                            Applicant = header.SubmitterAccount,
                            ApplicantName = header.SubmitterDisplayName,
                            ApplicationDate = header.SubmitDate,
                            FormViewUrl =viewurl,
                            CreatedDate = item.CreatedDate,
                            ReceiverDate = item.ReceiverDate,
                            Comment = item.ReviewComment
                        };

                        formCcItems.Add(ccItem);
                    }
                });
            }

            return formCcItems;
        }

        public IList<ProcessFormCC> ReceiveFormCC(string userName, bool viewStatus)
        {
            return _formCCRepository.ReceiveFormCC(userName, viewStatus);
        }

        private ProcessFormHeader GetProcessHeader(int formId)
        {
            var storageProvider = new KStarFormStorageProvider();
            var header = storageProvider.GetProcessFormHeaderByFormId(formId);
            if (header.ProcessFolio == null)
            {
                header.ProcessFolio = "";
            }

            return header;
        }

        public bool IsReceiver(int formId, string receiver)
        {
            var identity = _formCCRepository.IsReceiver(formId, receiver);

            return identity;
        }

        private string GetProcessFolio(int formId)
        {
            var processFolio = string.Empty;
            var storageProvider = new KStarFormStorageProvider();
            var header = storageProvider.GetProcessFormHeaderByFormId(formId);

            if (header != null)
            {
                processFolio = header.ProcessFolio;
            }

            return processFolio;
        }



        public IList<FormCCModel> SendFormCC(string userName, DateTime? startDate, DateTime? endDate, string receiveStatus, int pageIndex, int pageSize, List<Helper.SortDescriptor> sort, out int total)
        {
            var formCcItems = new List<FormCCModel>();

            var sendItems = _formCCRepository.SendFormCC(userName,startDate,endDate,receiveStatus,pageIndex,pageSize,sort,out total);
            var header = new ProcessFormHeader();

            if (sendItems != null)
            {
                sendItems.ToList().ForEach(item =>
                {
                    header = GetProcessHeader(item.FormId);

                    if (header != null)
                    {
                        var ccItem = new FormCCModel()
                        {
                            SysId = item.SysId,
                            ProcessFolio = header.ProcessFolio.Split('-').First(),
                            ProcInstNo = header.ProcessFolio.Split('-').Last(),
                            Originator = item.Originator,
                            OriginatorName = item.OriginatorName,
                            Receiver = item.Receiver,
                            ReceiverName = item.ReceiverName,
                            ReceiverStatus = item.ReceiverStatus,
                            Applicant = header.SubmitterAccount,
                            ApplicantName = header.SubmitterDisplayName,
                            ApplicationDate = header.SubmitDate,
                            FormViewUrl = item.FormViewUrl.Substring(0, item.FormViewUrl.IndexOf("&")),
                            CreatedDate = item.CreatedDate,
                            ReceiverDate = item.ReceiverDate,
                            Comment = item.ReviewComment
                        };

                        formCcItems.Add(ccItem);
                    }
                });
            }

            return formCcItems;
        }

        public IList<FormCCModel> ReceiveFormCC(string userName, DateTime? startDate, DateTime? endDate, string receiveStatus, int pageIndex, int pageSize, List<Helper.SortDescriptor> sort, out int total)
        {
            var formCcItems = new List<FormCCModel>();

            var sendItems = _formCCRepository.ReceiveFormCC(userName, startDate, endDate, receiveStatus, pageIndex, pageSize, sort, out total);
            var header = new ProcessFormHeader();

            if (sendItems != null)
            {
                sendItems.ToList().ForEach(item =>
                {
                    header = GetProcessHeader(item.FormId);

                    if (header != null)
                    {
                        var ccItem = new FormCCModel()
                        {
                            SysId = item.SysId,
                            ProcessFolio = header.ProcessFolio.Split('-').First(),
                            ProcInstNo = header.ProcessFolio.Split('-').Last(),
                            Originator = item.Originator,
                            OriginatorName = item.OriginatorName,
                            Receiver = item.Receiver,
                            ReceiverName = item.ReceiverName,
                            ReceiverStatus = item.ReceiverStatus,
                            Applicant = header.SubmitterAccount,
                            ApplicantName = header.SubmitterDisplayName,
                            ApplicationDate = header.SubmitDate,
                            FormViewUrl = item.ReceiverStatus ? item.FormViewUrl.Substring(0, item.FormViewUrl.IndexOf("&")) : item.FormViewUrl,
                            CreatedDate = item.CreatedDate,
                            ReceiverDate = item.ReceiverDate,
                            Comment = item.ReviewComment
                        };

                        formCcItems.Add(ccItem);
                    }
                });
            }

            return formCcItems;
        }

        public bool IsAlreadyReview(Guid sysId, string receiver)
        {
            var status = _formCCRepository.IsAlreadyReview(sysId, receiver);

            return status;
        }

        public bool IsAlreadyReview(int formId, int activityId, string receiver)
        {
            var status = _formCCRepository.IsAlreadyReview(formId, activityId, receiver);

            return status;
        }
    }
}

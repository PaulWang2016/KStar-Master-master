using aZaaS.KStar.Form.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Repositories
{
    public class FormCCRepository
    {
        public void Add(IList<ProcessFormCC> ccList)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ccList.ToList().ForEach(item =>
                {
                    item.SysId = Guid.NewGuid();
                    item.CreatedDate = DateTime.Now;
                });
                context.ProcessFormCCs.AddRange(ccList);

                context.SaveChanges();
            }
        }

        public void Add(IList<ProcessFormCC> ccList, string viewAddress)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ccList.ToList().ForEach(item =>
                {
                    item.SysId = Guid.NewGuid();
                    item.FormViewUrl = string.Format("{0}?_FormId={1}&ActivityId={2}&CcId={3}", viewAddress, item.FormId, item.ActivityId, item.SysId);
                    item.CreatedDate = DateTime.Now;
                });
                context.ProcessFormCCs.AddRange(ccList);

                context.SaveChanges();
            }
        }

        public void UpdateStatus(Guid sysId, string comment)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormCCs.FirstOrDefault(r => r.SysId == sysId);

                if (item == null)
                {
                    throw new ArgumentException("Invalid SysId");
                }

                item.ReceiverStatus = true;
                item.ReceiverDate = DateTime.Now;
                item.ReviewComment = comment;

                context.SaveChanges();
            }
        }

        public void UpdateStatus(int formId, int activityId, string receiver, string comment)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                //var items = context.ProcessFormCCs.Where(r => r.FormId == formId && r.ActivityId == activityId && r.Receiver == receiver && r.ReceiverStatus == false);

                //if (items == null)
                //{
                //    throw new ArgumentException("Invalid SysId");
                //}

                //items.ToList().ForEach(item =>
                //{
                //    item.ReceiverStatus = true;
                //    item.ReceiverDate = DateTime.Now;
                //    item.ReviewComment = comment;
                //});

                var item = context.ProcessFormCCs.FirstOrDefault(r => r.FormId == formId && r.ActivityId == activityId && r.Receiver == receiver && r.ReceiverStatus == false);

                if (item == null)
                {
                    return;
                    //throw new ArgumentException("Invalid SysId");
                }

                item.ReceiverStatus = true;
                item.ReceiverDate = DateTime.Now;
                item.ReviewComment = comment;
                context.SaveChanges();
            }
        }

        public void Delete(Guid sysId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormCCs.FirstOrDefault(r => r.SysId == sysId);

                if (item == null)
                {
                    throw new ArgumentException("Invalid SysId");
                }

                context.ProcessFormCCs.Remove(item);

                context.SaveChanges();
            }
        }

        public void Delete(int formId, string receiver)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.FormId == formId && r.Receiver == receiver);

                if (items == null)
                {
                    throw new ArgumentException("Invalid formId or receiver");
                }

                context.ProcessFormCCs.RemoveRange(items);

                context.SaveChanges();
            }
        }

        public void DeleteAll(int formId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.FormId == formId);

                if (items == null)
                {
                    throw new ArgumentException("Invalid FormId");
                }

                context.ProcessFormCCs.RemoveRange(items);

                context.SaveChanges();
            }
        }

        public IList<ProcessFormCC> SendFormCC(string userName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.Originator == userName).ToList();

                return items;
            }
        }

        public IList<ProcessFormCC> SendFormCC(string userName, DateTime? startDate, DateTime? endDate, string receiveStatus, int pageIndex, int pageSize, List<Helper.SortDescriptor> sort, out int total)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.Originator == userName).ToList();

                if (startDate != null)
                {
                    items = items.Where(r => r.CreatedDate >= startDate).ToList();
                }

                if (endDate != null)
                {
                    items = items.Where(r => r.CreatedDate <= endDate).ToList();
                }
                if (!string.IsNullOrWhiteSpace(receiveStatus))
                {
                    items = items.Where(r => r.ReceiverStatus == (receiveStatus == "1" ? true : false)).ToList();
                }
                total=items.Count();
                items=items.ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();                
                return items;
            }
        }

        public IList<ProcessFormCC> ReceiveFormCC(string userName)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.Receiver == userName).ToList();

                return items;
            }
        }

        public IList<ProcessFormCC> ReceiveFormCC(string userName, bool viewStatus)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.Receiver == userName && r.ReceiverStatus == viewStatus).ToList();

                return items;
            }
        }

        public IList<ProcessFormCC> ReceiveFormCC(string userName, DateTime? startDate, DateTime? endDate, string receiveStatus, int pageIndex, int pageSize, List<Helper.SortDescriptor> sort, out int total)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var items = context.ProcessFormCCs.Where(r => r.Receiver == userName).ToList();
                if (startDate != null)
                {
                    items = items.Where(r => r.CreatedDate >= startDate).ToList();
                }
                if (endDate != null)
                {
                    items = items.Where(r => r.CreatedDate <= endDate).ToList();
                }
                if (!string.IsNullOrWhiteSpace(receiveStatus))
                {
                    items = items.Where(r => r.ReceiverStatus == (receiveStatus == "1" ? true : false)).ToList();
                }
                total = items.Count();
                items = items.ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();    
                return items;
            }
        }

        public bool IsReceiver(int formId, string receiver)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormCCs.FirstOrDefault(r => r.FormId == formId && (r.Receiver == receiver || r.Originator == receiver));

                if (item != null)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsAlreadyReview( Guid sysId, string receiver)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormCCs.FirstOrDefault(r => r.SysId == sysId && r.Receiver == receiver);

                var status = false;

                if (item != null)
                {
                    status = item.ReceiverStatus;
                }

                return status;
            }
        }

        public bool IsAlreadyReview(int formId, int activityId, string receiver)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                var item = context.ProcessFormCCs.FirstOrDefault(r => r.FormId == formId && r.ActivityId == activityId && r.Receiver == receiver && r.ReceiverStatus == false);

                var status = false;

                if (item == null)
                {
                    status = true;
                }

                return status;
            }
        }
    }
}

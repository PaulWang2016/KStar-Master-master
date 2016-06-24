using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar.ParticipantSetService
{
    public class ActivityParticipantSetService
    {
        /// <summary>
        /// 添加加签队列
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddActivityParticipantSet(ProcessActivityParticipantSet model)
        {
            System.Guid guid = System.Guid.NewGuid();
            try
            {
                using (KStarFramekWorkDbContext enity = new KStarFramekWorkDbContext())
                {
                   
                    model.SetID = guid;
                    enity.ProcessActivityParticipantSet.Add(model);
                    enity.SaveChanges();
                    return guid.ToString();
                }
            }
            catch (Exception ex)
            {
 
            }
            return null; 

            
        }
        /// <summary>
        /// 添加加签组
        /// </summary>
        /// <param name="SetID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddActivityParticipantEntrySet(Guid SetID,ProcessActivityParticipantSetEntry model)
        {
            var isSuccess = false;
            try
            {
                using (KStarFramekWorkDbContext enity = new KStarFramekWorkDbContext())
                {

                    model.SetID = SetID;
                    enity.ProcessActivityParticipantSetEntry.Add(model);
                   
                    enity.SaveChanges();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
 
            }
            return isSuccess;
        }
        public bool DeleteActivityParticipantSet(Guid guid)
        {
            var isSuccess = false;
            try
            {
                using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
                {
                    var model = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == guid);
                    if (model != null)
                    {
                        var list = entity.ProcessActivityParticipantSetEntry.Where(p => p.SetID == guid).ToList();
                        entity.ProcessActivityParticipantSet.Remove(model);
                        entity.ProcessActivityParticipantSetEntry.RemoveRange(list);
                        entity.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return isSuccess;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProcessFullName"></param>
        /// <param name="ActivityName"></param>
        /// <returns></returns>
        public List<ProcessActivityParticipantSet> GetActivityParticipantsSet(string ProcessFullName, string ActivityName)
        {
            List<ProcessActivityParticipantSet> model = null;

            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                model = entity.ProcessActivityParticipantSet.Where(p => p.ProcessFullName == ProcessFullName && p.ActivityName == ActivityName).ToList();
            }
            return model;
        }
        public List<ProcessActivityParticipantSet> GetActivityParticipantsSet(int ProcInstId, string ActivityName)
        {
            List<ProcessActivityParticipantSet> model = null;

            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                model = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID==ProcInstId && p.ActivityName == ActivityName).ToList();
            }
            return model;
        }
        public ProcessActivityParticipantSet GetActivityParticipantsSet(Guid guid)
        {
            ProcessActivityParticipantSet model = null;

            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                model = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == guid);
            }
            return model;
        }

        /// <summary>
        /// 出站
        /// </summary>
        /// <param name="ProcInstId"></param>
        /// <param name="ActivityName"></param>
        /// <returns></returns>
        public List<ProcessActivityParticipantSetEntry> Pop(int ProcInstId, string ActivityName)
        {
            List<ProcessActivityParticipantSetEntry> model = null;

            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var task = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstId && p.ActivityName == ActivityName && p.IsPeeked == false).OrderBy(x => x.Priority).FirstOrDefault();
                if (task != null)
                {
                    task.IsPeeked = true;
                    entity.SaveChanges();

                    model = entity.ProcessActivityParticipantSetEntry.Where(x => x.SetID == task.SetID).ToList();
                }
            }
            return model;
        }

        public List<string> PopParticipants(int ProcInstId, string ActivityName, Action<int, string,List<ProcessActivityParticipantSetEntry>, List<string>> resolveAction)
        {
            List<string> participants = new List<string>(); 
            try
            { 
                using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
                {
                    var task = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstId && p.ActivityName == ActivityName && p.IsPeeked == false).OrderBy(x => x.Priority).FirstOrDefault();
                    if (task != null)
                    {
                        task.IsPeeked = true;
                        List<ProcessActivityParticipantSetEntry> model = entity.ProcessActivityParticipantSetEntry.Where(x => x.SetID == task.SetID).ToList();

                        //解析
                        if (resolveAction != null)
                        {
                            resolveAction(ProcInstId, ActivityName, model, participants);
                        }

                        entity.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return participants;
        }



        public List<string> GetParticipants(int ProcInstId, string ActivityName, Action<int, string, List<ProcessActivityParticipantSetEntry>, List<string>> resolveAction)
        {
            List<string> participants = new List<string>();
            try
            {
                using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
                {
                    var task = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstId && p.ActivityName == ActivityName && p.IsPeeked == true).OrderBy(x => x.Priority).ToList();
                    if (task != null)
                    {
                        List<ProcessActivityParticipantSetEntry> model = (from set in task join setEntry in entity.ProcessActivityParticipantSetEntry on set.SetID equals setEntry.SetID select setEntry).ToList();

                        //解析
                        if (resolveAction != null)
                        {
                            resolveAction(ProcInstId, ActivityName, model, participants);
                        }  
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return participants;
        }

      
        
        public List<ProcessActivityParticipantSetEntry> GetActivityParticipantsSetEntry(Guid guid)
        {
            List<ProcessActivityParticipantSetEntry> model = null;

            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                model = entity.ProcessActivityParticipantSetEntry.Where(p => p.SetID == guid).ToList();
            }
            return model;
        }
        public List<ProcessActivityParticipantSetEntry> GetActivityParticipantsSetEntry(string guid)
        {
            List<ProcessActivityParticipantSetEntry> list = new List<ProcessActivityParticipantSetEntry>();
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var linq = from t in entity.ProcessActivityParticipantSetEntry
                           select t;

                if (!string.IsNullOrEmpty(guid))
                {
                    Guid SetID = new Guid(guid);
                    linq = linq.Where(p => p.SetID == SetID);
                }
                else 
                {
                    Guid SetID = new Guid();
                    linq = linq.Where(p => p.SetID == SetID);
                }
                
                list = linq.ToList();
            }
            return list;
        }
        public void DownActivityParticipantSet(Guid SetID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result  = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == SetID);
                if (result != null)
                {
                    int priority = (int)result.Priority;
                    //priority = priority + 1;
                    int temp = priority;
                    var desnation = entity.ProcessActivityParticipantSet.OrderBy(p => p.Priority).FirstOrDefault(p => p.ProcessFullName == result.ProcessFullName && p.ActivityName == result.ActivityName && p.Priority > priority);
                    if (desnation == null)
                    {
                        desnation = entity.ProcessActivityParticipantSet.OrderBy(p => p.Priority).FirstOrDefault(p => p.ProcInstID == result.ProcInstID && p.ActivityName == result.ActivityName && p.Priority > priority);
                    }
                    if (desnation != null)
                    {
                        priority = (int)desnation.Priority;
                    }
                    desnation.Priority = temp;
                    result.Priority = priority;
                    entity.Entry(desnation).State = System.Data.Entity.EntityState.Modified;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    entity.SaveChanges();

                }
            } 
        }
        public void UpperActivityParticipantSet(Guid SetID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == SetID);
                if (result != null)
                {
                    int priority = (int)result.Priority;
                     int temp =priority;
                   // priority = priority -1 ;
                    var desnation = entity.ProcessActivityParticipantSet.OrderByDescending(p=>p.Priority).FirstOrDefault(p => p.ProcessFullName == result.ProcessFullName  && p.ActivityName == result.ActivityName&& p.Priority < priority);
                    if (desnation == null)
                    {
                        desnation = entity.ProcessActivityParticipantSet.OrderBy(p => p.Priority).FirstOrDefault(p => p.ProcInstID == result.ProcInstID && p.ActivityName == result.ActivityName && p.Priority < priority);
                    }
                    if (desnation != null) {
                        priority = (int)desnation.Priority;
                    }

                    desnation.Priority = temp;
                    result.Priority = priority;
                    entity.Entry(desnation).State = System.Data.Entity.EntityState.Modified;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;


                    entity.SaveChanges();

                }
            }
        }
        /// <summary>
        /// 重置加签队列
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <param name="ActivityName"></param>
        public void ResetProcessActivityParticipantSet(int ProcInstID,string ActivityName)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID && p.ActivityName == ActivityName).ToList();
                if (result != null)
                {
                    foreach (ProcessActivityParticipantSet set in result)
                    {
                        set.IsPeeked = false;
                        entity.Entry(set).State = System.Data.Entity.EntityState.Modified;
                        entity.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 重置所有加签队列
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <param name="ActivityName"></param>
        public void ResetProcessActivityParticipantSet(int ProcInstID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID ).ToList();
                if (result != null)
                {
                    foreach (ProcessActivityParticipantSet set in result)
                    {
                        set.IsPeeked = false;
                        entity.Entry(set).State = System.Data.Entity.EntityState.Modified;
                        entity.SaveChanges();
                    }
                }
            }

        }
        /// <summary>
        /// 判断当前节点是否还有未走的加签队列
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <param name="ActivityName"></param>
        /// <returns></returns>
        public bool HasUnPeekedProcessActivityParticipantSet(int ProcInstID, string ActivityName)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID&& p.ActivityName== ActivityName&& p.IsPeeked==false).ToList();
                if (result != null)
                {
                    if (result.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 标识该加签已出队列
        /// </summary>
        /// <param name="guid"></param>
        public void MarkProcessActivityParticipantSetPeeked(Guid guid)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == guid);
                if (result != null)
                {
                    result.IsPeeked = true;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    entity.SaveChanges();
                }
            }
        }
        public ProcessActivityParticipantSet PeekActivityParticipantSet(Guid guid)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == guid);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public void RemoveActivityParticipantSet(Guid guid)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == guid);
                if (result != null)
                {
                    entity.ProcessActivityParticipantSet.Remove(result);
                    entity.SaveChanges();
                }
            }
        }
        public void RemoveActivityParticipantSetEntry(Guid SetID,int ID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSetEntry.FirstOrDefault(p => p.SetID == SetID&&p.ID==ID);
                if (result != null)
                {
                    entity.ProcessActivityParticipantSetEntry.Remove(result);
                    entity.SaveChanges();
                }
            }
        }
        
        public void RemoveActivityParticipantSetEntry(int ID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSetEntry.FirstOrDefault(p=>p.ID==ID);
                if (result != null)
                {
                    entity.ProcessActivityParticipantSetEntry.Remove(result);
                    entity.SaveChanges();
                }
            }
        }
        public void SkipAssignerActivityParticipantSets(Guid SetID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == SetID);
                if (result != null)
                {
                    result.SkipAssigner = true;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    entity.SaveChanges();
                }
            }
        }
        public void SkipSetForActivityParticipantSets(Guid SetID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == SetID);
                if (result != null)
                {
                    result.SkipSet = true;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    entity.SaveChanges();
                }
            }
        }

        public void UnSkipAssignerActivityParticipantSets(Guid SetID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == SetID);
                if (result != null)
                {
                    result.SkipAssigner = false;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    entity.SaveChanges();
                }
            }
        }
        public void UnSkipSetForActivityParticipantSets(Guid SetID)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var result = entity.ProcessActivityParticipantSet.FirstOrDefault(p => p.SetID == SetID);
                if (result != null)
                {
                    result.SkipSet = false;
                    entity.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    entity.SaveChanges();
                }
            }
        }
        /// <summary>
        /// 应用流程模板
        /// </summary>
        /// <param name="FullName"></param>
        /// <param name="ProcInstID"></param>
        /// <param name="ActivityName"></param>
        public void ApplyProcessActivity(string FullName, int ProcInstID, string ActivityName, Action<int, string, KStarFramekWorkDbContext, ProcessActivityParticipantSet> addParticipants, bool isPop=true)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                //当前环节已经应用了模板
                var count = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID && p.ActivityName == ActivityName).Count();
                if (count > 0)
                {
                    //默认出列模式
                    if (isPop)
                    {
                        int peekedCount = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID && p.ActivityName == ActivityName && p.IsPeeked == false).Count();
                        //是否还存在没走完的
                        if (peekedCount > 0)
                        {
                            return;
                        }
                        else
                        {
                            //全部走完 在在访问表示goto 回来了 清除全部标记
                            var peekedEntityList = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID && p.ActivityName == ActivityName).ToList();

                            foreach (var item in peekedEntityList)
                            {
                                item.IsPeeked = false;
                            }
                            entity.SaveChanges();
                        }
                    }
                    
                    return;
                }

                //Apply 模板数据
                var results = entity.ProcessActivityParticipantSet.Where(p => p.ProcessFullName == FullName && p.ProcInstID == 0 && p.ActivityName == ActivityName).ToList();
                if (results != null && results.Count>0)
                {

                    ProcessActivityParticipantSet itemEntity = null;
                    foreach (var item in results)
                    {
                        string itemString = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                        itemEntity = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessActivityParticipantSet>(itemString);

                        itemEntity.SetID = Guid.NewGuid();
                        itemEntity.ProcInstID = ProcInstID;
                        entity.ProcessActivityParticipantSet.Add(itemEntity);
                        //子对象
                        var entryList = entity.ProcessActivityParticipantSetEntry.Where(x => x.SetID == item.SetID).ToList();

                        foreach (var _entry in entryList)
                        {
                            entity.ProcessActivityParticipantSetEntry.Add(new ProcessActivityParticipantSetEntry() { SetID = itemEntity.SetID, EntryType = _entry.EntryType, EntryName = _entry.EntryName, EntryID = _entry.EntryID });
                        }
                    }
                    if (addParticipants != null)
                    {
                        addParticipants(ProcInstID, ActivityName, entity, itemEntity);
                    }
                    entity.SaveChanges();
                }
            }
        }
         
         /// <summary>
        /// clear current Activity Participant
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <param name="ActivityName"></param>
        public void ClearProcessActivity(int ProcInstID, string ActivityName)
        {
            try
            {


                using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
                {
                    var peekedEntityList = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID && p.ActivityName == ActivityName && p.IsPeeked == true).ToList();

                    foreach (var item in peekedEntityList)
                    {
                        item.IsPeeked = false;
                    }
                    entity.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        /// <summary>
        /// ActivityName is Signature  (current activate  Activity）
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <param name="ActivityName"></param>
        /// <returns></returns>
        public bool IsSignatureActivity(int ProcInstID, string ActivityName)
        {
            using (KStarFramekWorkDbContext entity = new KStarFramekWorkDbContext())
            {
                var task = entity.ProcessActivityParticipantSet.Where(p => p.ProcInstID == ProcInstID && p.ActivityName == ActivityName && p.IsPeeked == true).OrderByDescending(x => x.Priority).FirstOrDefault();
                if (task != null)
                {

                    if (task.Priority == int.MaxValue)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                } 
                return false;
            }
          
        }

    }
}

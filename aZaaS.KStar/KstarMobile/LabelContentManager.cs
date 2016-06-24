using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.KstarMobile
{
    public sealed class LabelContentManager
    {
        public LabelContentEntity GetLabelContentById(Guid? id)
        {
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                return ctx.LabelContent.Where(x => x.LabelID==id).SingleOrDefault();                
            }
        }

        public bool AddLabelContent(LabelContentEntity entity)
        {
            var result = true;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    ctx.LabelContent.Add(entity);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }

        public Guid? AddLabelContent(string language,string content)
        {
            Guid? labelid =Guid.NewGuid();
            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    LabelContentEntity entity = new LabelContentEntity();
                    entity.Language = language;
                    entity.LabelID = labelid;
                    entity.Content = content;
                    
                    ctx.LabelContent.Add(entity);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    labelid = null;
                }
            }
            return labelid;
        }


        public bool UpdateLabelContent(int id, LabelContentEntity newEntity)
        {
            var result = true;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    var oldLabelContent = ctx.LabelContent.FirstOrDefault(x => x.ID == id);                    
                    oldLabelContent.LabelID = newEntity.LabelID;
                    oldLabelContent.Content = newEntity.Content;
                    oldLabelContent.Language = newEntity.Language;                    
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }
        public bool DeleteLabelContent(LabelContentEntity entity)
        {
            var result = true;

            using (KSTARServiceDBContext ctx = new KSTARServiceDBContext())
            {
                try
                {
                    ctx.LabelContent.Remove(entity);
                    ctx.SaveChanges();
                }
                catch (Exception)
                {
                    result = false;
                }
            }
            return result;

        }

    }
}

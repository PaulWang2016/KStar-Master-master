using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.DataDictionary
{
    internal class DataDictionaryManager
    {
        /// <summary>
        /// 获取所有字典第一级分类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataDictionaryEntity> GetAllDataDicCategory()
        {
            using (KStarDbContext context = new KStarDbContext())
            {                
                return context.DataDictionary.Include("Childs").Where(m => m.Type == (int)DataDictionaryType.Folder && m.ParentId == null).OrderBy(x=>x.Order).ToList();                       
            }
        }
        /// <summary>
        /// 根据父id获取子分类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataDictionaryEntity> GetDataDicCategoryByParentId(Guid parentid)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DataDictionary.Include("Childs").Where(m => m.Type < (int)DataDictionaryType.Data && m.ParentId == parentid).OrderBy(x => x.Order).ToList();                 
            }
        }

        /// <summary>
        /// 根据父编码获取子分类
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataDictionaryEntity> GetDataDicCategoryByCode(string code)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var data = context.DataDictionary.Where(m => m.Type < (int)DataDictionaryType.Data && m.Code == code).FirstOrDefault();
                if (data != null)
                {
                    return context.DataDictionary.Include("Childs").Where(m => m.Type < (int)DataDictionaryType.Data && m.ParentId == data.Id).OrderBy(x => x.Order).ToList();                    
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataDictionaryEntity> GetDataDictionaryByParentId(Guid parentid)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DataDictionary.Include("Childs").Where(m => m.Type == (int)DataDictionaryType.Data && m.ParentId == parentid).OrderBy(x => x.Order).ToList();                   
            }
        }

        /// <summary>
        /// 根据编码获取字典
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataDictionaryEntity> GetDataDictionaryByCode(string code)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var data=context.DataDictionary.Where(m => m.Type < (int)DataDictionaryType.Data && m.Code == code).FirstOrDefault();
                if (data == null)
                {
                    data = context.DataDictionary.Where(m => m.Type == (int)DataDictionaryType.Data && m.Code == code).FirstOrDefault();
                }                
                Guid parentid=(data==null?Guid.Empty:data.Id);
                return context.DataDictionary.Include("Childs").Where(m => m.Type == (int)DataDictionaryType.Data && m.ParentId == parentid).OrderBy(x => x.Order).ToList();             
            }
        }

        public DataDictionaryEntity GetDataDictionaryById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DataDictionary.Include("Childs").Where(m => m.Id == id).FirstOrDefault();                
            }
        }

        public Guid AddDataDictionary(DataDictionaryBaseDto datadic)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                DataDictionaryEntity dataentity = new DataDictionaryEntity()
                 {
                     Id = Guid.NewGuid(),
                     ParentId = datadic.ParentId,
                     Code = datadic.Code,
                     Name = datadic.Name,
                     Value = datadic.Value,
                     Type = datadic.Type,
                     Order=datadic.Order,
                     Remark = datadic.Remark
                 };
                context.DataDictionary.Add(dataentity);
                context.SaveChanges();
                return dataentity.Id;
            }
        }

        public void EditDataDictionary(DataDictionaryBaseDto datadic)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                DataDictionaryEntity olddataentity = context.DataDictionary.Where(x => x.Id == datadic.Id).FirstOrDefault();                                                    
                olddataentity.Code = datadic.Code;
                olddataentity.Name = datadic.Name;
                olddataentity.Value = datadic.Value;
                olddataentity.Order = datadic.Order;
                olddataentity.Remark = datadic.Remark;               
                context.SaveChanges();                
            }
        }

        public void DelDataDictionary(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var data = context.DataDictionary.Where(m => m.Id == id).FirstOrDefault();
                if (data != null)
                {
                    context.DataDictionary.Remove(data);
                    context.SaveChanges();
                }
            }
        }

        public string GetDataDicValue(string key)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var data = context.DataDictionary.Where(m => m.Code == key && m.Type == (int)DataDictionaryType.Data).FirstOrDefault();
                if (data != null)
                {
                    return data.Value;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public void DelDataDicByParentId(Guid parentId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var data = context.DataDictionary.Where(m => m.ParentId == parentId).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        context.DataDictionary.Remove(item);   
                    }                    
                    context.SaveChanges();
                }
            }
        }

        public bool ExistCode(string code,bool iscategory)
        {
            bool flag = false;
            using (KStarDbContext context = new KStarDbContext())
            {
                DataDictionaryEntity data;
                if(iscategory)
                {
                    data = context.DataDictionary.Where(m => m.Code == code&&m.Type<(int)DataDictionaryType.Data).FirstOrDefault();   
                }
                else
                {
                    data = context.DataDictionary.Where(m => m.Code == code && m.Type==(int)DataDictionaryType.Data).FirstOrDefault();
                }                 
                if (data != null)
                {
                    flag = true;
                }
            }
            return flag;
        }

    }
}

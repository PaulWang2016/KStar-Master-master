using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.DTOs;
using aZaaS.KStar.Acs;

namespace aZaaS.KStar.Documents
{
    public sealed class DocumentManager
    {
        private static readonly DocumentStorageFactory _factory = new DocumentStorageFactory();
        private AcsManager acsManager = new AcsManager();

        #region 下面的取代

        //public string GetDocMenuKeyById(Guid id)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        if (id != null)
        //        {
        //            return context.Menu.Where(m => m.Id == id).Select(s => s.Key).FirstOrDefault();
        //        }
        //        return "";
        //    }
        //}
        //public DocumentLibrary GetLibrary(Guid DocumentLibraryID, bool includeItems = true)
        //{
        //    DocumentLibrary model;
        //    using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
        //    {
        //        model = context.DocumentLibrary.Where(m => m.ID == DocumentLibraryID).Select(s => new DocumentLibrary()
        //        {
        //            MenuID = s.MenuID,
        //            Id = s.ID,
        //            Key = s.Key,
        //            IconPath = s.IconPath,
        //            DisplayName = s.DisplayName
        //        }).FirstOrDefault();
        //        var libraryID = model.Id;

        //        if (includeItems)
        //        {
        //            model.Items = context.DocumentItem.Where(s => s.DocumentLibraryID == libraryID).Select(s => new DocumentItem()
        //             {
        //                 Id = s.Id,
        //                 IconPath = s.IconPath == null ? model.IconPath : s.IconPath,
        //                 DisplayName = s.DisplayName,
        //                 StorageUri = s.StorageUri
        //             }).ToList();
        //        }
        //    }
        //    return model;
        //}
        //public DocumentLibrary GetLibraryByKey(string DocumentLibraryKey, bool includeItems = true)
        //{
        //    DocumentLibrary model;
        //    using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
        //    {
        //        model = context.DocumentLibrary.Where(m => m.Key == DocumentLibraryKey).Select(s => new DocumentLibrary()
        //        {
        //            MenuID = s.MenuID,
        //            Id = s.ID,
        //            Key = s.Key,
        //            IconPath = s.IconPath,
        //            DisplayName = s.DisplayName
        //        }).FirstOrDefault();
        //        var libraryID = model.Id;

        //        if (includeItems)
        //        {
        //            model.Items = context.DocumentItem.Where(s => s.DocumentLibraryID == libraryID).Select(s => new DocumentItem()
        //            {
        //                Id = s.Id,
        //                IconPath = s.IconPath == null ? model.IconPath : s.IconPath,
        //                DisplayName = s.DisplayName,
        //                StorageUri = s.StorageUri
        //            }).ToList();
        //        }
        //    }
        //    return model;
        //}
        //public List<DocumentLibrary> GetAllLibrary()
        //{
        //    List<DocumentLibrary> model;
        //    using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
        //    {
        //        model = context.DocumentLibrary.Select(s => new DocumentLibrary() { MenuID = s.MenuID, Id = s.ID, Key = s.Key, IconPath = s.IconPath, DisplayName = s.DisplayName }).ToList();
        //    }
        //    return model;
        //}
        //public List<Guid> GetDocLibraryIDByMenuKey(string menuKey)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var menuID = context.Menu.Where(m => m.Key == menuKey).Select(s => s.Id).FirstOrDefault();
        //        return context.DocumentLibrary.Where(m => m.MenuID == menuID).Select(s => s.ID).ToList();
        //    }
        //}
        //public List<string> GetDocLibraryKeyByMenuKey(string menuKey)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var menuID = context.Menu.Where(m => m.Key == menuKey).Select(s => s.Id).FirstOrDefault();
        //        return context.DocumentLibrary.Where(m => m.MenuID == menuID).Select(s => s.Key).ToList();
        //    }
        //}
        //public DocumentLibrary GetDocumentLibrary(Guid id)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var item = context.DocumentLibrary.Where(m => m.ID == id).Select(s => new DocumentLibrary()
        //        {
        //            Id = s.ID,
        //            DisplayName = s.DisplayName
        //        }).FirstOrDefault();
        //        return item;
        //    }
        //}
        //public DocumentItem GetDocumentItem(Guid id)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var item = context.DocumentItem.Where(m => m.Id == id).Select(s => new DocumentItem()
        //        {
        //            Id = s.Id,
        //            DisplayName = s.DisplayName
        //        }).FirstOrDefault();
        //        return item;
        //    }
        //}
        //public void CreateLibrary(DocumentLibrary lib, IEnumerable<DocumentItem> items)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var temp = context.DocumentLibrary.Where(m => m.MenuID == lib.MenuID && m.Key == lib.Key).FirstOrDefault();
        //        if (temp != null) return;
        //        var entity = context.DocumentLibrary.Where(s => s.Key == lib.Key).FirstOrDefault();
        //        if (entity == null)
        //        {
        //            List<DocumentItemEntity> Items = null;
        //            if (items != null)
        //            {
        //                Items = new List<DocumentItemEntity>();
        //                foreach (var item in lib.Items)
        //                {
        //                    if (item.Id == null)
        //                        item.Id = Guid.NewGuid();
        //                    Items.Add(new DocumentItemEntity() { Id = item.Id, DisplayName = item.DisplayName, StorageUri = item.StorageUri, IconPath = item.IconPath });
        //                }
        //            }
        //            context.DocumentLibrary.Add(new DocumentLibraryEntity()
        //            {
        //                ID = lib.Id,
        //                MenuID = lib.MenuID,
        //                Key = lib.Key,
        //                DisplayName = lib.DisplayName,
        //                IconPath = lib.IconPath,
        //                Items = Items
        //            });
        //            context.SaveChanges();
        //        }
        //    }
        //}
        //public void CreateDocumentItem(DocumentItem dto, Guid DocumentLibraryID)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var libraryID = context.DocumentLibrary.Where(s => s.ID == DocumentLibraryID).Select(m => m.ID).FirstOrDefault();
        //        if (libraryID != null)
        //        {
        //            context.DocumentItem.Add(new DocumentItemEntity() { Id = dto.Id, DisplayName = dto.DisplayName, IconPath = dto.IconPath, StorageUri = dto.StorageUri, DocumentLibraryID = libraryID, CreateTime = DateTime.Now });
        //            context.SaveChanges();
        //        }
        //    }
        //}
        //public void UpdateDocumentItem(DocumentItem dto)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var entity = context.DocumentItem.Where(s => s.Id == dto.Id).FirstOrDefault();
        //        if (entity != null)
        //        {
        //            entity.DisplayName = dto.DisplayName;
        //            entity.IconPath = dto.IconPath;
        //            entity.StorageUri = dto.StorageUri;
        //            context.SaveChanges();
        //        }
        //    }
        //}
        //public void UpdateLibrary(DocumentLibrary lib)
        //{
        //    using (KStarDbContext context = new KStarDbContext())
        //    {
        //        var temp = context.DocumentLibrary.Where(m => m.ID != lib.Id && m.MenuID == lib.MenuID && m.Key == lib.Key).FirstOrDefault();
        //        if (temp != null) return;
        //        var entity = context.DocumentLibrary.Where(s => s.ID == lib.Id).FirstOrDefault();
        //        if (entity != null)
        //        {
        //            entity.MenuID = lib.MenuID;
        //            entity.DisplayName = lib.DisplayName;
        //            entity.IconPath = lib.IconPath;
        //        }
        //        context.SaveChanges();
        //    }
        //}
        //public List<string> DelLibrary(List<string> idList)
        //{
        //    List<string> ids = new List<string>();
        //    foreach (var id in idList)
        //    {
        //        var libraryID = Guid.Parse(id);
        //        DelLibrary(libraryID);
        //        ids.Add(id);
        //    }
        //    return ids;
        //}
        //public void DelLibrary(Guid id)
        //{
        //    using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
        //    {
        //        AcsManager acsManager = new AcsManager();

        //        var model = context.DocumentLibrary.Where(m => m.ID == id).FirstOrDefault();
        //        if (model != null)
        //        {
        //            var items = context.DocumentItem.Where(s => s.DocumentLibraryID == id);
        //            foreach (var item in items)
        //            {
        //                acsManager.DelAuthority(item.Id.ToString());
        //                context.DocumentItem.Remove(item);
        //            }
        //            acsManager.DelAuthority(model.ID.ToString());
        //            context.DocumentLibrary.Remove(model);
        //        }
        //        context.SaveChanges();
        //    }
        //}
        //public List<string> DelDocumentItem(List<string> idlist)
        //{
        //    using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
        //    {
        //        List<string> ids = new List<string>();
        //        foreach (var id in idlist)
        //        {
        //            var model = context.DocumentItem.Where(m => m.Id == Guid.Parse(id)).FirstOrDefault();
        //            if (model != null)
        //            {
        //                context.DocumentItem.Remove(model);
        //                ids.Add(id);
        //            }
        //        }
        //        context.SaveChanges();
        //        return ids;
        //    }
        //}
        //public void DelDocumentItem(Guid id)
        //{
        //    using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
        //    {
        //        var model = context.DocumentItem.Where(m => m.Id == id).FirstOrDefault();
        //        if (model != null)
        //        {
        //            AcsManager acsManager = new AcsManager();
        //            acsManager.DelAuthority(model.Id.ToString());
        //            context.DocumentItem.Remove(model);
        //            context.SaveChanges();
        //        }
        //    }
        //} 
        #endregion



        #region 是否已经存在DocumentLibrary
        public bool IsExistDocLibrary(DocumentLibrary item)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                if (context.DocumentLibrary.Where(m => m.ID == item.Id || m.Key == item.Key).FirstOrDefault() != null)
                    return true;
                return false;
            }
        }
        #endregion
        #region 是否已经存在DocumentItem
        public bool IsExistDocItem(DocumentLibrary item)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                if (context.DocumentItem.Where(m => m.Id == item.Id).FirstOrDefault() != null)
                    return true;
                return false;
            }
        }
        #endregion
        #region 获取所有的DocLibrary
        public List<DocumentLibrary> GetAllDocLibrary(bool includeItems)
        {
            List<DocumentLibrary> models;
            using (Repositories.KStarDbContext context = new Repositories.KStarDbContext())
            {
                models = context.DocumentLibrary.Select(s => new DocumentLibrary() { MenuID = s.MenuID, Id = s.ID, Key = s.Key, IconPath = s.IconPath, DisplayName = s.DisplayName }).ToList();
                if (includeItems)
                {
                    foreach (var item in models)
                    {
                        item.Items = context.DocumentItem.Where(m => m.DocumentLibraryID == item.Id).ToArray().Select(s => s.ToDTO()).ToList();
                    }
                }
            }
            return models;
        }
        #endregion
        #region 根据DocumentLibrary的Id获取DocumentLibrary的Key
        public string GetDocLibraryKeyById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DocumentLibrary.Where(m => m.ID == id).Select(s => s.Key).FirstOrDefault();
            }
        }
        #endregion
        #region 根据DocumentLibrary的Key获取DocumentLibrary的Id
        public Guid GetDocLibraryIdByKey(string key)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DocumentLibrary.Where(m => m.Key == key).Select(s => s.ID).FirstOrDefault();
            }
        }
        #endregion
        #region 获取菜单Id下的DocumentLibrary列表
        public IEnumerable<DocumentLibrary> GetDocLibraryByMenuId(Guid menuId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DocumentLibrary.Where(m => m.MenuID == menuId).ToList().Select(s => s.ToDTO());
            }
        }
        #endregion
        #region 获取LibraryId下的对应DocumentItem列表
        public IEnumerable<DocumentItem> GetDocItemsByLibraryId(Guid libraryId)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                return context.DocumentItem.Where(m => m.DocumentLibraryID == libraryId).ToArray().Select(s => s.ToDTO());
            }
        }
        #endregion
        #region 根据Id获取对应的DocumentLibrary
        public DocumentLibrary GetDocLibraryById(Guid id, bool includeItems = true)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var item = context.DocumentLibrary.Where(m => m.ID == id).Select(s => new DocumentLibrary()
                {
                    MenuID = s.MenuID,
                    IconPath = s.IconPath,
                    Key = s.Key,
                    Id = s.ID,
                    DisplayName = s.DisplayName
                }).FirstOrDefault();
                if (includeItems)
                    item.Items = context.DocumentItem.Where(m => m.DocumentLibraryID == id).ToArray().Select(s => s.ToDTO()).ToList();
                return item;
            }
        }
        #endregion
        #region 根据Id获取DocumentItem
        public DocumentItem GetDocItemById(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var item = context.DocumentItem.Where(m => m.Id == id).Select(s => new DocumentItem()
                {
                    DocumentLibraryID = s.DocumentLibraryID,
                    DocumentItemOrder = s.DocumentItemOrder,
                    StorageUri = s.StorageUri,
                    IconPath = s.IconPath,
                    SysID = s.Id,
                    DisplayName = s.DisplayName
                }).FirstOrDefault();
                return item;
            }
        }
        #endregion
        #region  添加DocumentLibrary
        public DocumentLibrary CreateDocLibrary(DocumentLibrary lib)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var menu = context.Menu.Where(m => m.Id == lib.MenuID).FirstOrDefault();
                if (menu == null) return null;
                if (lib.Id == null) lib.Id = Guid.NewGuid();
                context.DocumentLibrary.Add(new DocumentLibraryEntity()
                {
                    ID = lib.Id,
                    MenuID = lib.MenuID,
                    Key = lib.Key,
                    DisplayName = lib.DisplayName,
                    IconPath = lib.IconPath,
                });
                if (lib.Items != null)
                {
                    foreach (var item in lib.Items)
                    {
                        if (item.SysID == null) item.SysID = Guid.NewGuid();
                        CreateDocItem(context, item);
                    }
                }
                context.SaveChanges();
                return lib;
            }
        }
        #endregion
        #region 添加DocumentItem
        public DocumentItem CreateDocItem(DocumentItem dto)
        {
            using (KStarDbContext context = new KStarDbContext())
            {

                return CreateDocItem(context, dto);
                //new DocumentItemEntity()   不确定ToEntity()是否满足
                //{
                //    Id = dto.Id,
                //    DisplayName = dto.DisplayName,
                //    IconPath = dto.IconPath,
                //    StorageUri = dto.StorageUri,
                //    DocumentLibraryID = libraryID,
                //    CreateTime = DateTime.Now
                //}
            }
        }
        internal DocumentItem CreateDocItem(KStarDbContext context, DocumentItem dto)
        {
            var libraryID = context.DocumentLibrary.Where(s => s.ID == dto.DocumentLibraryID).Select(m => m.ID).FirstOrDefault();
            if (libraryID == null) return null;
            context.DocumentItem.Add(dto.ToEntity());
            context.SaveChanges();
            return dto;
        }
        #endregion
        #region 编辑DocumentLibrary
        public DocumentLibrary UpdateDocLibrary(DocumentLibrary lib)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var entity = context.DocumentLibrary.Where(s => s.ID == lib.Id).FirstOrDefault();
                if (entity == null) return null;

                entity.MenuID = lib.MenuID;
                entity.DisplayName = lib.DisplayName;
                entity.IconPath = lib.IconPath;
                context.SaveChanges();
                return entity.ToDTO();
            }
        }
        #endregion
        #region 编辑DocumentItem
        public DocumentItem UpdateDocItem(DocumentItem Item)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var entity = context.DocumentItem.Where(s => s.Id == Item.SysID).FirstOrDefault();
                if (entity == null) return null;
                entity.DisplayName = Item.DisplayName;
                entity.IconPath = Item.IconPath;
                entity.StorageUri = Item.StorageUri;
                context.SaveChanges();
                return entity.ToDTO();
            }
        }
        #endregion
        #region 根据id列 批量删除DocumentLibrary
        public List<Guid> DelDocLibrarysByIds(List<Guid> ids)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                List<Guid> DelItemIds = new List<Guid>();
                var result = DelDocLibrarysByIds(context, ids, ref DelItemIds);
                context.SaveChanges();
                acsManager.DelAuthorityCompletely(DelItemIds.Select(s => s.ToString()).ToList());
                return result;
            }
        }
        internal List<Guid> DelDocLibrarysByIds(KStarDbContext context, List<Guid> ids, ref List<Guid> DelItemIds)
        {
            var items = context.DocumentLibrary.Where(m => ids.Contains(m.ID)).ToList();
            if (items == null) return null;
            foreach (var item in items)
            {
                DelItemIds.AddRange(DelDocItemsByLibraryId(context, item.ID));
                context.DocumentLibrary.Remove(item);
            }
            DelItemIds.AddRange(items.Select(s => s.ID).ToList());
            return items.Select(s => s.ID).ToList();
        }
        #endregion
        #region 根据id列 批量删除DocumentItem
        public List<Guid> DelDocItemsByIds(List<Guid> ids)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var result = DelDocItemsByIds(context, ids);
                context.SaveChanges();
                acsManager.DelAuthorityCompletely(result.Select(s => s.ToString()).ToList());
                return result;
            }
        }
        internal List<Guid> DelDocItemsByIds(KStarDbContext context, List<Guid> ids)
        {
            var items = context.DocumentItem.Where(m => ids.Contains(m.Id)).ToList();
            if (items == null) return null;

            foreach (var item in items)
            {
                context.DocumentItem.Remove(item);
            }
            return items.Select(s => s.Id).ToList();
        }
        #endregion
        #region 删除LibraryId下所有DocumentItem
        public List<Guid> DelDocItemsByLibraryId(Guid id)
        {
            using (KStarDbContext context = new KStarDbContext())
            {
                var result = DelDocItemsByLibraryId(context, id);
                context.SaveChanges();
                acsManager.DelAuthorityCompletely(result.Select(s => s.ToString()).ToList());
                return result;
            }
        }
        internal List<Guid> DelDocItemsByLibraryId(KStarDbContext context, Guid id)
        {
            var items = context.DocumentItem.Where(m => m.DocumentLibraryID == id).ToList();
            if (items == null) return null;

            foreach (var item in items)
            {
                context.DocumentItem.Remove(item);
            }
            return items.Select(s => s.Id).ToList();
        }
        #endregion

    }
}

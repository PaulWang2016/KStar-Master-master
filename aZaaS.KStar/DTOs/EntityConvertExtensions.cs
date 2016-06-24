using aZaaS.KStar.Documents;
using aZaaS.KStar.DynamicWidgets;
using aZaaS.KStar.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.KStar.Menus;

namespace aZaaS.KStar
{
    // 此处有两种方案：
    // 1. 使用AutoMapper 或 StructureMap 来实现对象间的映射
    // 2. 手工转换
    // 考虑到对象不会太多， 建议使用手工转换的模式以降低复杂度。
    // 后续如果发现 KStar 的实体对象呈现爆发性增长，在当期的实现后
    // 整体迁移到方案一中去。
    //
    // y2mao@outlook.com, 2013/10/6
    internal static class EntityConvertExtensions
    {
        public static DocumentLibrary ToDTO(this DocumentLibraryEntity entity)
        {
            DocumentLibrary model = new DocumentLibrary()
            {
                Id = entity.ID,
                IconPath = entity.IconPath,
                MenuID = entity.MenuID,
                Key = entity.Key,
                DisplayName = entity.DisplayName
            };
            if (entity.Items != null)
            {
                model.Items = new List<DocumentItem>();
                foreach (var item in entity.Items)
                {
                    model.Items.Add(new DocumentItem()
                    {
                        DocumentLibraryID = item.DocumentLibraryID,
                        SysID = item.Id,
                        DisplayName = item.DisplayName,
                        IconPath = item.IconPath,
                        DocumentItemOrder = item.DocumentItemOrder,
                        StorageUri = item.StorageUri
                    });
                }
            }
            return model;
        }

        public static DocumentLibraryEntity ToEntity(this DocumentLibrary dto)
        {
            DocumentLibraryEntity model = new DocumentLibraryEntity()
            {
                MenuID = dto.MenuID,
                ID = dto.Id,
                IconPath = dto.IconPath,
                Key = dto.Key,
                DisplayName = dto.DisplayName
            };

            model.Items = new List<DocumentItemEntity>();
            foreach (var item in dto.Items)
            {
                model.Items.Add(new DocumentItemEntity()
                {
                    DocumentLibraryID = item.DocumentLibraryID,
                    Id = item.SysID,
                    DisplayName = item.DisplayName,
                    IconPath = item.IconPath,
                    DocumentItemOrder = item.DocumentItemOrder,
                    StorageUri = item.StorageUri
                });
            }
            return model;
        }

        public static Widget ToDTO(this WidgetEntity entity)
        {
            Widget model = new Widget()
            {
                Key = entity.Key,
                Title = entity.Title,
                Url = entity.Url,
                RenderMode = entity.RenderMode,
                HtmlAttributes = entity.HtmlAttributes as Dictionary<string, string>
            };

            return model;
        }

        public static WidgetEntity ToEntity(this Widget dto)
        {
            WidgetEntity model = new WidgetEntity()
            {
                Key = dto.Key,
                Title = dto.Title,
                Url = dto.Url,
                RenderMode = dto.RenderMode,
                HtmlAttributes = dto.HtmlAttributes
            };
            return model;
        }

        public static DynamicWidget ToDTO(this DynamicWidgetEntity entity)
        {
            DynamicWidget model = new DynamicWidget()
            {
                ID = entity.ID,
                Key = entity.Key,
                DisplayName = entity.DisplayName,
                RazorContent = entity.RazorContent,
                Description = entity.Description,
                MenuID = entity.MenuID
            };

            return model;
        }

        public static DynamicWidgetEntity ToEntity(this DynamicWidget dto)
        {
            DynamicWidgetEntity model = new DynamicWidgetEntity()
            {
                ID = dto.ID,
                Key = dto.Key,
                DisplayName = dto.DisplayName,
                RazorContent = dto.RazorContent,
                Description = dto.Description,
                MenuID = dto.MenuID
            };
            return model;
        }


        public static DocumentItem ToDTO(this DocumentItemEntity entity)
        {
            DocumentItem model = new DocumentItem()
            {
                DocumentItemOrder = entity.DocumentItemOrder,
                DocumentLibraryID = entity.DocumentLibraryID,
                IconPath = entity.IconPath,
                SysID = entity.Id,
                StorageUri = entity.StorageUri,
                DisplayName = entity.DisplayName
            };
            return model;
        }

        public static DocumentItemEntity ToEntity(this DocumentItem dto)
        {
            DocumentItemEntity model = new DocumentItemEntity()
            {
                DocumentItemOrder = dto.DocumentItemOrder,
                DocumentLibraryID = dto.DocumentLibraryID,
                IconPath = dto.IconPath,
                Id = dto.SysID,
                StorageUri = dto.StorageUri,
                DisplayName = dto.DisplayName,
                CreateTime = DateTime.UtcNow
            };

            return model;
        }

        //public static AttachmentInfo ToDTO(this AttachmentInfoEntity entity)
        //{
        //    AttachmentInfo model = new AttachmentInfo()
        //    {
        //        ID = entity.ID,
        //        FileName = entity.FileName,
        //        FileID = entity.FileID,
        //        CreateDate = entity.CreateDate,
        //        Creator = entity.Creator,
        //        Remark = entity.Remark,
        //        Form_ID = entity.Form_ID
        //    };

        //    return model;
        //}

        //public static AttachmentInfoEntity ToEntity(this AttachmentInfo dto)
        //{
        //    AttachmentInfoEntity model = new AttachmentInfoEntity()
        //    {
        //        ID = dto.ID,
        //        FileName = dto.FileName,
        //        FileID = dto.FileID,
        //        CreateDate = dto.CreateDate,
        //        Creator = dto.Creator,
        //        Remark = dto.Remark,
        //        Form_ID = dto.Form_ID
        //    };

        //    return model;
        //}

    }
}

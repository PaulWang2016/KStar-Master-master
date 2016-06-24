using AutoMapper;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Repositories
{
    public class ControlTmplRepository
    {
        public Guid AddCategory(TemplateCagetoryModel category)
        {
            Guid sysId = Guid.NewGuid();
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlTemplateCategory templateCategory = Mapper.Map<TemplateCagetoryModel, ControlTemplateCategory>(category);
                templateCategory.SysId = sysId;
                context.ControlTemplateCategorys.Add(templateCategory);
                context.SaveChanges();
            }
            return sysId;
        }

        public void EditCategory(TemplateCagetoryModel category)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlTemplateCategory templateCategory = Mapper.Map<TemplateCagetoryModel, ControlTemplateCategory>(category);
                ControlTemplateCategory oldtemplateCategory = context.ControlTemplateCategorys.Where(x => x.SysId == templateCategory.SysId).FirstOrDefault();
                oldtemplateCategory.ParentId = templateCategory.ParentId;                
                oldtemplateCategory.CategoryName = templateCategory.CategoryName;
                context.SaveChanges();
            }
        }

        public void DeleteCategory(Guid categroyId)
        {
            List<Guid> childs = new List<Guid>();
            IList<TemplateCagetoryModel> categorys = GetAllCategories();
            GetCategoryChilds(categorys, categroyId, childs);
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                List<ControlRenderTemplate> controlTemplates = context.ControlRenderTemplates.Where(x => childs.Contains(x.CategoryId)).ToList();
                List<ControlTemplateCategory> templateCategorys = context.ControlTemplateCategorys.Where(x => childs.Contains(x.SysId)).ToList();
                context.ControlRenderTemplates.RemoveRange(controlTemplates);
                context.ControlTemplateCategorys.RemoveRange(templateCategorys);
                context.SaveChanges();
            }
        }

        public void AddTemplate(ControlTemplateModel template)
        {
            Guid sysId = Guid.NewGuid();
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlRenderTemplate controlTemplate = Mapper.Map<ControlTemplateModel, ControlRenderTemplate>(template);
                controlTemplate.SysId = sysId;
                context.ControlRenderTemplates.Add(controlTemplate);
                context.SaveChanges();
            }
        }

        public void EditTemplate(ControlTemplateModel template)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlRenderTemplate controlTemplate = Mapper.Map<ControlTemplateModel, ControlRenderTemplate>(template);
                ControlRenderTemplate oldcontrolTemplate = context.ControlRenderTemplates.Where(x => x.SysId == controlTemplate.SysId).FirstOrDefault();
                oldcontrolTemplate.CategoryId = controlTemplate.CategoryId;
                oldcontrolTemplate.DisplayName = controlTemplate.DisplayName;
                oldcontrolTemplate.HtmlTemplate = controlTemplate.HtmlTemplate;
                context.SaveChanges();
            }
        }

        public void DeleteTemplate(Guid templateId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlRenderTemplate controlTemplate = context.ControlRenderTemplates.Where(x => x.SysId == templateId).FirstOrDefault();
                context.ControlRenderTemplates.Remove(controlTemplate);
                context.SaveChanges();
            }
        }

        public IList<ControlTemplateModel> GetTemplate()
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ControlRenderTemplate> controlTemplate = context.ControlRenderTemplates.ToList();

                return Mapper.Map<IList<ControlRenderTemplate>, IList<ControlTemplateModel>>(controlTemplate);
            }
        }

        public ControlTemplateModel GetTemplate(Guid templatedId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlRenderTemplate controlTemplate = context.ControlRenderTemplates.Where(x => x.SysId == templatedId).FirstOrDefault();

                return Mapper.Map<ControlRenderTemplate, ControlTemplateModel>(controlTemplate);
            }
        }

        public IList<ControlTemplateModel> GetTemplate(int pagesize, int pageindex, System.Linq.Expressions.Expression<Func<ControlRenderTemplate,bool>> searchexpress, out int total)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ControlRenderTemplate> controlTemplates = context.ControlRenderTemplates.Where(searchexpress).ToList();
                total = controlTemplates.Count;
                IList<ControlRenderTemplate> controlTemplate = controlTemplates.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                return Mapper.Map<IList<ControlRenderTemplate>, IList<ControlTemplateModel>>(controlTemplate);
            }
        }

        public TemplateCagetoryModel GetCategory(Guid? categoryId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                ControlTemplateCategory templateCategory = context.ControlTemplateCategorys.Where(x => x.SysId == categoryId).FirstOrDefault();

                return Mapper.Map<ControlTemplateCategory, TemplateCagetoryModel>(templateCategory);
            }
        }

        public IList<TemplateCagetoryModel> GetAllCategories()
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ControlTemplateCategory> templateCategorys = context.ControlTemplateCategorys.ToList();

                return Mapper.Map<IList<ControlTemplateCategory>, IList<TemplateCagetoryModel>>(templateCategorys);
            }
        }

        public IList<TemplateCagetoryModel> GetRootCategories()
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ControlTemplateCategory> templateCategorys = context.ControlTemplateCategorys.Where(x => x.ParentId ==null).ToList();

                return Mapper.Map<IList<ControlTemplateCategory>, IList<TemplateCagetoryModel>>(templateCategorys);
            }
        }

        public IList<TemplateCagetoryModel> GetChildCategories(Guid categoryId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ControlTemplateCategory> childCategories = context.ControlTemplateCategorys.Where(x => x.ParentId == categoryId).ToList();

                return Mapper.Map<IList<ControlTemplateCategory>, IList<TemplateCagetoryModel>>(childCategories);
            }
        }

        public IList<ControlTemplateModel> GetCategoryTemplates(Guid categoryId)
        {
            using (aZaaSKStarFormContext context = new aZaaSKStarFormContext())
            {
                IList<ControlRenderTemplate> controlTemplates = context.ControlRenderTemplates.Where(x => x.CategoryId == categoryId).ToList();

                return Mapper.Map<IList<ControlRenderTemplate>, IList<ControlTemplateModel>>(controlTemplates);
            }
        }

        public void GetCategoryChilds(IList<TemplateCagetoryModel> categorys, Guid sysId, List<Guid> childs)
        {
            if (categorys != null && sysId != Guid.Empty)
            {
                childs.Add(sysId);
                List<TemplateCagetoryModel> categorychilds = categorys.Where(x => x.ParentId == sysId).ToList();
                foreach (var item in categorychilds)
                {
                    childs.Add(item.SysId);
                    GetCategoryChilds(categorys, item.SysId, childs);
                }
            }
        }
    }
}

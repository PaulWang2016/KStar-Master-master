using AutoMapper;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Models;
using aZaaS.KStar.Form.Repositories;
using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form
{
    public class ControlTmplProvider : IControlTmplProvider
    {
        private readonly ControlTmplRepository _controlTmplRepository;
        public ControlTmplProvider()
        {
            _controlTmplRepository = new ControlTmplRepository();
        }
        public Guid AddCategory(TemplateCagetoryModel category)
        {
            return _controlTmplRepository.AddCategory(category);
        }

        public void EditCategory(TemplateCagetoryModel category)
        {
            _controlTmplRepository.EditCategory(category);
        }

        public void DeleteCategory(Guid categroyId)
        {
            _controlTmplRepository.DeleteCategory(categroyId);
        }

        public void AddTemplate(ControlTemplateModel template)
        {
            _controlTmplRepository.AddTemplate(template);
        }

        public void EditTemplate(ControlTemplateModel template)
        {
            _controlTmplRepository.EditTemplate(template);
        }

        public void DeleteTemplate(Guid templateId)
        {
            _controlTmplRepository.DeleteTemplate(templateId);
        }

        public IList<ControlTemplateModel> GetTemplate()
        {
            return _controlTmplRepository.GetTemplate();
        }

        public ControlTemplateModel GetTemplate(Guid templatedId)
        {
            return _controlTmplRepository.GetTemplate(templatedId);
        }

        public IList<ControlTemplateModel> GetTemplate(int pagesize, int pageindex,System.Linq.Expressions.Expression<Func<ControlRenderTemplate,bool>> searchexpress, out int total)
        {
            return _controlTmplRepository.GetTemplate(pagesize, pageindex, searchexpress, out total);
        }

        public TemplateCagetoryModel GetCategory(Guid? categoryId)
        {
            return _controlTmplRepository.GetCategory(categoryId);
        }

        public IList<TemplateCagetoryModel> GetAllCategories()
        {
            return _controlTmplRepository.GetAllCategories();
        }

        public IList<TemplateCagetoryModel> GetRootCategories()
        {
            return _controlTmplRepository.GetRootCategories();
        }        

        public IList<TemplateCagetoryModel> GetChildCategories(Guid categoryId)
        {
            return _controlTmplRepository.GetChildCategories(categoryId);
        }

        public IList<ControlTemplateModel> GetCategoryTemplates(Guid categoryId)
        {
            return _controlTmplRepository.GetCategoryTemplates(categoryId);
        }

        public void GetCategoryChilds(IList<TemplateCagetoryModel> categorys, Guid sysId, List<Guid> childs)
        {
            _controlTmplRepository.GetCategoryChilds(categorys,sysId,childs);
        }
    }
}

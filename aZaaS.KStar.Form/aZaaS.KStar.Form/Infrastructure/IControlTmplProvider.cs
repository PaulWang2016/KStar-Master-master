using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IControlTmplProvider
    {
        Guid AddCategory(TemplateCagetoryModel category);
        void EditCategory(TemplateCagetoryModel category);
        void DeleteCategory(Guid categroyId);

        void AddTemplate(ControlTemplateModel template);
        void EditTemplate(ControlTemplateModel template);
        void DeleteTemplate(Guid templateId);

        ControlTemplateModel GetTemplate(Guid templatedId);
        TemplateCagetoryModel GetCategory(Guid? categoryId);
        IList<TemplateCagetoryModel> GetAllCategories();
        IList<TemplateCagetoryModel> GetChildCategories(Guid categoryId);
        IList<ControlTemplateModel> GetCategoryTemplates(Guid categoryId);
    }
}

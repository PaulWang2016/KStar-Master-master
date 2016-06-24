using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.Helpers;
using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public abstract class ContextBase
    {
        public int FormId { get; protected set; }
        public string UserName { get; protected set; }
        public KStarFormModel FormModel { get; protected set; }

        public ContextBase(KStarFormModel formModel)
        {
            FormModel = formModel;
            FormId = formModel.FormId;
        }

        public TModel DataModel<TModel>() where TModel : class
        {
            if (string.IsNullOrEmpty(FormModel.ContentData))
                throw new InvalidOperationException("Invalid content data!");

            return JsonHelper.ConvertToModel<TModel>(FormModel.ContentData);
        }

        public void SynchronizeFormId(int formId)
        {
            FormId = formId;
            FormModel.FormId = formId;
            if (FormModel.Attachments.Any())
                FormModel.Attachments.ToList().ForEach(a => a.FormId = formId.ToString());
        }
    }
}

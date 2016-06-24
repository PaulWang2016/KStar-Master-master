using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Helpers;

namespace aZaaS.KStar.Form.Infrastructure
{
    public class StorageContext : ContextBase
    {
        private List<DataError> _dataErrors;
        public WorkMode WorkMode { get; set; }

        public StorageContext(KStarFormModel formModel,int formId,string userName,WorkMode mode)
            : base(formModel)
        {
            this.FormId = formId;
            this.UserName = userName;
            this.WorkMode = mode;
            _dataErrors = new List<DataError>();
        }

        
        public void AddError(string errorMessage)
        {
            _dataErrors.Add(new DataError(AlertType.Error, errorMessage));
        }

        public void AddError(AlertType type, string errorMessage)
        {
            _dataErrors.Add(new DataError(type, errorMessage));
        }

        public bool DataIsValid()
        {
           return _dataErrors.Any();
        }

        public void ClearErrors()
        {
           if(_dataErrors != null)
               _dataErrors.Clear();
        }

        public IEnumerable<DataError> DataErrors
        {
            get
            {
                return _dataErrors.AsReadOnly();
            }
        }


    }
}

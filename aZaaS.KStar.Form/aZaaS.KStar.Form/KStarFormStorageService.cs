using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.ViewModels;
using aZaaS.KStar.Form.Infrastructure;
using aZaaS.KStar.Form.Helpers;

namespace aZaaS.KStar.Form
{
    public class KStarFormStorageService : IStorageService
    {
        public StorageContext _context;
        private IStorageProvider _dataProvider;
        private IList<IDataValidator> _validators;
        private IList<IDataHandler> _storingHandlers;
        private IList<IDataHandler> _storedHandlers;
        private IStorageBehavior _storageBehavior;

        public KStarFormStorageService(IStorageProvider dataProvider, IStorageBehavior storageBehavior)
        {
            _dataProvider = dataProvider;
            _validators = new List<IDataValidator>();
            _storingHandlers = new List<IDataHandler>();
            _storedHandlers = new List<IDataHandler>();
            _storageBehavior = storageBehavior;
        }

        public StorageContext Context
        {
            get { return _context; }
        }

        public IStorageProvider DataProvider
        {
            get { return _dataProvider; }
        }


        public bool ProcessData(StorageContext context)
        {
            try
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                _context = context;

                if (!ApplyDataValidators()) { return false; }
                if (!_storageBehavior.OnDataValidating(_context)) { return false; };

                _storageBehavior.OnBeforeDataStored(_context);

                object savedData = null;
                if (_context.FormId == 0)
                {
                    var newFormId = _dataProvider.SaveForm(_context.FormModel);
                    _context.SynchronizeFormId(newFormId);

                    if (_context.WorkMode == WorkMode.Draft)
                        savedData = _storageBehavior.OnDataDrafting(_context);
                    else
                        savedData = _storageBehavior.OnDataStoring(_context);
                }
                else
                {
                    _dataProvider.UpdateForm(_context.FormModel);
                    savedData = _storageBehavior.OnDataUpdating(_context);
                }

                if (savedData != null)
                {
                    var contentData = JsonHelper.SerializeObject(savedData);
                    _dataProvider.UpdateFormContent(_context.FormId, contentData);
                }

                _storageBehavior.OnAfterDataStored(_context);
            }
            catch (Exception ex)
            {
                if (_context.WorkMode == WorkMode.Draft || _context.WorkMode == WorkMode.Startup)
                {
                    _dataProvider.DeleteForm(_context.FormId);
                }
                _storageBehavior.OnDataStoredError(_context, ex);

                throw ex;
            }

            return true;
        }


        private bool ApplyDataValidators()
        {
            var isPass = true;

            if (_validators.Any())
            {
                foreach (var item in _validators)
                {
                    if (!item.Validate(_context))
                    {
                        isPass = false;
                        break;
                    }
                }
            }

            return isPass;
        }


        public void AddValidator(IDataValidator validator)
        {
            _validators.Add(validator);
        }
        public void AddStoringHandler(IDataHandler handler)
        {
            _storingHandlers.Add(handler);
        }
        public void AddStoredHandler(IDataHandler handler)
        {
            _storedHandlers.Add(handler);
        }
    }
}

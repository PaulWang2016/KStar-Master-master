using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.Form.ViewModels;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IStorageService
    {
        StorageContext Context { get; }
        IStorageProvider DataProvider { get; }

        bool ProcessData(StorageContext context);

        void AddValidator(IDataValidator validator);
        void AddStoringHandler(IDataHandler handler);
        void AddStoredHandler(IDataHandler handler);
    }
}

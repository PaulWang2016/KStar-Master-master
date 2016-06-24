using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IStorageBehavior
    {
        bool OnDataValidating(StorageContext context);
        void OnBeforeDataStored(StorageContext context);
        void OnAfterDataStored(StorageContext context);
        object OnDataStoring(StorageContext context);
        object OnDataUpdating(StorageContext context);
        object OnDataDrafting(StorageContext context);
        void OnDataStoredError(StorageContext context,Exception exception);
    }
}

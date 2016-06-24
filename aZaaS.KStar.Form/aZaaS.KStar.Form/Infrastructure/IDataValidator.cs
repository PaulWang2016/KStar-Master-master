using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IDataValidator
    {
        bool Validate(StorageContext context);
    }
}

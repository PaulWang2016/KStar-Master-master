using aZaaS.KStar.Form.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Infrastructure
{
    public interface IFormFileStorage
    {
        string StoreFile(AttachmentModel file);

        Stream FetchFile(AttachmentModel file);
    }
}

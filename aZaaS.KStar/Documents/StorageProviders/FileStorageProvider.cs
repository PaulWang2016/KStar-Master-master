using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Documents.StorageProviders
{
    internal class FileStorageProvider : IStorageProvider
    {
        public string Save(byte[] documentStream)
        {
            throw new NotImplementedException();
        }

        public void Delete(string storageUri)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string stoargeUri)
        {
            throw new NotImplementedException();
        }

        public bool CanHandle(string storageUri)
        {
            throw new NotImplementedException();
        }

        public byte[] Retrieve(string storageUri)
        {
            throw new NotImplementedException();
        }
    }
}

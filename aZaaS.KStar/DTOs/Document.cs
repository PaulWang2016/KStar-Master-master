using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar
{
    [Serializable]
    public class DocumentLibrary
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public string IconPath { get; set; }
        public Guid MenuID { get; set; }
        public List<DocumentItem> Items { get; set; }
    }

    [Serializable]
    public class DocumentItem : AbstractDTO
    {
        public Guid DocumentLibraryID { get; set; }
        public string DocumentItemOrder { get; set; }
        public string DisplayName { get; set; }
        public string IconPath { get; set; }

        public string StorageUri { get; set; }
    }
}

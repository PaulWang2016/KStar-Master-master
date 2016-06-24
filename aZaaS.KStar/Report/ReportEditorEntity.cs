using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Report
{
    class ReportEditorEntity
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public String Department { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public string ReportCode { get; set; }
        public string Status { get; set; }
        public string Rate { get; set; }
        public string ReportUrl { get; set; }
        public string ImageThumbPath { get; set; }
        public string Comment { get; set; }
        public bool IsFavourite { get; set; }
        public Guid ParnentID { get; set; }
    }
}

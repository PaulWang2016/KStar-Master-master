using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Report
{
    /// <summary>
    ///  通过ParnentID 来归结到所属的分类
    /// </summary>

    public class ReportInfoEntity
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public String Department { get; set; }
        /// <summary>
        /// 报表上架时间
        /// </summary>
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

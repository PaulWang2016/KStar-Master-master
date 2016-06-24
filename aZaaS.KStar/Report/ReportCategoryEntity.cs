using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Report
{

    /// <summary>
    ///   报表类别 通过 ParnentID 构成树状结构
    /// </summary>
    public class ReportCategoryEntity
    {
        [Key]
        public virtual Guid ID { get; set; }
        public virtual Guid ParnentID { get; set; }
        public virtual String Category { get; set; }
        public virtual string Comment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Report
{

    /// <summary>
    /// 个人报表收藏  通过PrenetID构成目录结构
    /// </summary>
    public class ReportFavouriteEntity
    {

        public virtual Guid ID { get; set; }

        public virtual Guid UserID { get; set; }

        public virtual string Name { get; set; }

        public virtual Guid ParnentID { get; set; }


        public virtual string Comment { get; set; }

       


    }


    public class Report_FavouriteEntity
    {
        [Key]
        public virtual Guid ID { get; set; }

        public virtual Guid UserID { get; set; }

        public virtual string Name { get; set; }

        public virtual Guid FavouriteID { get; set; }

        public virtual Guid ReportInfoID { get; set; }

        public virtual DateTime FavouriteDate { get; set; }




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using aZaaS.KStar.Report;

namespace aZaaS.KStar.Web.Models
{
    public class Category
    {
        public string CategoryID { get; set; }
        public string DisplayName { get; set; }
        public string ParentID { get; set; }
        public string Comment { get; set; }

    }

    internal static partial class Extensions
    {
        public static void FromData(this ReportCategoryEntity entity, Category category)
        {
            //entity.ID = Guid.Parse(category.CategoryID);
            entity.ParnentID = Guid.Parse(category.ParentID);
            entity.Category = category.DisplayName;
            entity.Comment = category.Comment;
        }
    }
}
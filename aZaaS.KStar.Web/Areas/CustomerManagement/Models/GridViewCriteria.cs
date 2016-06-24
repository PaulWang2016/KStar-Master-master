
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Models
{
    public class GridViewCriteria<TModel>
    {
        public int current { get; set; }
        public int rowCount { get; set; }
        public Dictionary<string, string> sort { get; set; }
        public string searchPhrase { get; set; }

        public IEnumerable<TModel> Apply(IEnumerable<TModel> items, Func< IEnumerable<TModel>> searching, Func<IEnumerable<TModel>,string, string, IOrderedEnumerable<TModel>> ordering = null)
        {

             items = searching();

            if (sort != null && sort.Any() 
                && ordering != null)
            {
                var field = sort.First().Key;
                var dir = sort.First().Value;

                items = ordering(items,field, dir);
            }
            else
                items = items.OrderBy(o => 0);

            var page = current;
            var pageSize = rowCount;
            var skip = rowCount * (current - 1);

            if (pageSize == -1)
                return items.AsEnumerable();

            return items.Skip(skip).Take(pageSize).AsEnumerable();
        }
    }
}
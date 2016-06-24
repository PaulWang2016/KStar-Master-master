using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Models
{
    public class GridViewDataResult<TModel>
    {
        public GridViewDataResult(IEnumerable<TModel> records, GridViewCriteria<TModel> request,int? totalCount)
        {
            this.current = request.current;
            this.rowCount = request.rowCount;
            this.total = totalCount ?? 0;
            this.rows = records;
        }

        public GridViewDataResult(IEnumerable<TModel> records,int? page,int? pageSize,int? totalCount)
        {
            this.current = page ?? 1;
            this.rowCount = pageSize ?? 10;
            this.total = totalCount ?? 0;
            this.rows = records;
        }

        public int current { get; set; }
        public int rowCount { get; set; }
        public int total { get; set; }
        public IEnumerable<TModel> rows { get; set; }        
    }
}
using AutoMapper;
using aZaaS.KStar.Helper;
using aZaaS.KStar.MgmtDtos;
using aZaaS.KStar.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.LogRequestProvider
{
    public class LogRequestManager
    {
        public IEnumerable<LogRequestDto> GetLogRequestByPage(string action, string userName, DateTime? startDate, DateTime? endDate, int pageIndex, int pageSize, List<aZaaS.KStar.Helper.SortDescriptor> sort, out int total)
        {            
            using (KStarFramekWorkDbContext context = new KStarFramekWorkDbContext())
            {
                var items = context.LogRequest.Where(t=>true);                
                if (startDate != null && endDate != null)
                {
                    items = items.Where(r => r.RequestTime >= startDate && r.RequestTime <= endDate);
                }
                if (!string.IsNullOrEmpty(action))
                {
                    items = items.Where(r => r.Name.Contains(action));
                }
                if (!string.IsNullOrEmpty(userName))
                {
                    items = items.Where(r => r.RequestUser.Contains(userName));
                }
                total = items.Count();

                if (sort.Count == 0)
                {
                    items = items.OrderByDescending(r => r.RequestTime);
                }
                else
                {
                    foreach (var st in sort)
                    {
                        items = DataListSort.DataSorting<LogRequest>(items.AsQueryable(), st.field, st.dir);
                    }
                }
                var result = items.ToList().Skip((pageIndex - 1) * pageSize).Take(pageSize);
                if (result != null)
                {
                    return Mapper.Map<IEnumerable<LogRequest>, IEnumerable<LogRequestDto>>(result);
                }
                else
                {
                    return new List<LogRequestDto>();
                }
            }            
        }
    }
}

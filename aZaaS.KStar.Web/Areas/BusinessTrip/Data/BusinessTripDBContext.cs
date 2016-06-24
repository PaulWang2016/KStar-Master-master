using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using aZaaS.KStar.Web.Areas.BusinessTrip.Data;
using aZaaS.KStar.Web.Areas.BusinessTrip.Data.Mapping;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Data
{
    public partial class BusinessTripDBContext : DbContext
    {
        static BusinessTripDBContext()
        {
            Database.SetInitializer<BusinessTripDBContext>(null);
        }

        public BusinessTripDBContext()
            : base("BusinessTripDBContext")
        {
        }

        public DbSet<ScheduleItem> ScheduleItems { get; set; }
        public DbSet<TravelRequest> TravelRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ScheduleItemMap());
            modelBuilder.Configurations.Add(new TravelRequestMap());
        }
    }
}

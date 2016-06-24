using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Data.Mapping
{
    public class ScheduleItemMap : EntityTypeConfiguration<ScheduleItem>
    {
        public ScheduleItemMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Departure)
                .HasMaxLength(50);

            this.Property(t => t.Destination)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ScheduleItem");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RequestId).HasColumnName("RequestId");
            this.Property(t => t.FromDate).HasColumnName("FromDate");
            this.Property(t => t.ToDate).HasColumnName("ToDate");
            this.Property(t => t.Departure).HasColumnName("Departure");
            this.Property(t => t.Destination).HasColumnName("Destination");
            this.Property(t => t.Comment).HasColumnName("Comment");

            // Relationships
            this.HasOptional(t => t.TravelRequest)
                .WithMany(t => t.ScheduleItems)
                .HasForeignKey(d => d.RequestId);

        }
    }
}

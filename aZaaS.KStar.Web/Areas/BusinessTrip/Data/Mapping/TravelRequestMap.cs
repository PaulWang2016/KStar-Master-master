using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace aZaaS.KStar.Web.Areas.BusinessTrip.Data.Mapping
{
    public class TravelRequestMap : EntityTypeConfiguration<TravelRequest>
    {
        public TravelRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Applicant)
                .HasMaxLength(50);

            this.Property(t => t.ApplicantName)
                .HasMaxLength(50);

            this.Property(t => t.DepartmetName)
                .HasMaxLength(50);

            this.Property(t => t.Phone)
                .HasMaxLength(50);

            this.Property(t => t.Entourage)
                .HasMaxLength(50);

            this.Property(t => t.TravelReason);

            // Table & Column Mappings
            this.ToTable("TravelRequest");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.FormId).HasColumnName("FormId");
            this.Property(t => t.Applicant).HasColumnName("Applicant");
            this.Property(t => t.ApplicantName).HasColumnName("ApplicantName");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.BackDate).HasColumnName("BackDate");
            this.Property(t => t.DepartmentId).HasColumnName("DepartmentId");
            this.Property(t => t.DepartmetName).HasColumnName("DepartmetName");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Entourage).HasColumnName("Entourage");
            this.Property(t => t.IsUsingCar).HasColumnName("IsUsingCar");
            this.Property(t => t.TotalDays).HasColumnName("TotalDays");
            this.Property(t => t.TravelReason).HasColumnName("TravelReason");
        }
    }
}

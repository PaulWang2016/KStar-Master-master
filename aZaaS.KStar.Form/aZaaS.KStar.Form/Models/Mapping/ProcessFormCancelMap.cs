using System.Data.Entity.ModelConfiguration;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormCancelMap : EntityTypeConfiguration<ProcessFormCancel>
    {
        public ProcessFormCancelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("ProcessFormCancel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProcInstId).HasColumnName("ProcInstId");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}

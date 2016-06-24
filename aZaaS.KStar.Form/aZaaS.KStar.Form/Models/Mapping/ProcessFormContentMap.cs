using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormContentMap : EntityTypeConfiguration<ProcessFormContent>
    {
        public ProcessFormContentMap()
        {
            // Primary Key
            this.HasKey(t => t.SysId);

            // Properties
            // Table & Column Mappings
            this.ToTable("ProcessFormContent");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.FormID).HasColumnName("FormID");
            this.Property(t => t.JsonData).HasColumnName("JsonData");
        }
    }
}

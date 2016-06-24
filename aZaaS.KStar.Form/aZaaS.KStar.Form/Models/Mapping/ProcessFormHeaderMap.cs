using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormHeaderMap : EntityTypeConfiguration<ProcessFormHeader>
    {
        public ProcessFormHeaderMap()
        {
            // Primary Key
            this.HasKey(t => t.FormID);

            // Table & Column Mappings
            this.ToTable("ProcessFormHeader");
            this.Property(t => t.FormID).HasColumnName("FormID");
            this.Property(t => t.FormSubject).HasColumnName("FormSubject");
            this.Property(t => t.ProcInstID).HasColumnName("ProcInstID");
            this.Property(t => t.ProcessFolio).HasColumnName("ProcessFolio");
            this.Property(t => t.Priority).HasColumnName("Priority");
            this.Property(t => t.SubmitterAccount).HasColumnName("SubmitterAccount");
            this.Property(t => t.SubmitterDisplayName).HasColumnName("SubmitterDisplayName");
            this.Property(t => t.SubmitDate).HasColumnName("SubmitDate");
            this.Property(t => t.ApplicantAccount).HasColumnName("ApplicantAccount");
            this.Property(t => t.ApplicantDisplayName).HasColumnName("ApplicantDisplayName");
            this.Property(t => t.ApplicantTelNo).HasColumnName("ApplicantTelNo");
            this.Property(t => t.ApplicantEmail).HasColumnName("ApplicantEmail");
            this.Property(t => t.ApplicantPositionID).HasColumnName("ApplicantPositionID");
            this.Property(t => t.ApplicantPositionName).HasColumnName("ApplicantPositionName");
            this.Property(t => t.ApplicantOrgNodeID).HasColumnName("ApplicantOrgNodeID");
            this.Property(t => t.ApplicantOrgNodeName).HasColumnName("ApplicantOrgNodeName");
            this.Property(t => t.SubmitComment).HasColumnName("SubmitComment");
            this.Property(t => t.IsDraft).HasColumnName("IsDraft");
            this.Property(t => t.DraftUrl).HasColumnName("DraftUrl");
        }
    }
}

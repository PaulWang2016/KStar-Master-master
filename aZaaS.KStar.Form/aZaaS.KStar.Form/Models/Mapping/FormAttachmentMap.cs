using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class FormAttachmentMap : EntityTypeConfiguration<FormAttachment>
    {
        public FormAttachmentMap()
        {
            ToTable("FormAttachment");
            HasKey(x => x.FileId);

            Property(x => x.FileId).HasColumnName("FileId").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.FileGuid).HasColumnName("FileGuid").IsOptional();
            Property(x => x.FormId).HasColumnName("FormId").IsOptional();
            Property(x => x.ProcInstId).HasColumnName("ProcInstId").IsOptional();
            Property(x => x.ProcessName).HasColumnName("ProcessName").IsOptional();
            Property(x => x.ActivityName).HasColumnName("ActivityName").IsOptional();
            Property(x => x.FileBytes).HasColumnName("FileBytes").IsOptional();
            Property(x => x.FileType).HasColumnName("FileType").IsOptional();
            Property(x => x.FileExtension).HasColumnName("FileExtension").IsOptional();
            Property(x => x.OldFileName).HasColumnName("OldFileName").IsOptional();
            Property(x => x.NewFileName).HasColumnName("NewFileName").IsOptional();
            Property(x => x.StoragePath).HasColumnName("StoragePath").IsOptional();
            Property(x => x.DownloadUrl).HasColumnName("DownloadUrl").IsOptional();
            Property(x => x.Uploader).HasColumnName("Uploader").IsOptional();
            Property(x => x.UploaderName).HasColumnName("UploaderName").IsOptional();
            Property(x => x.UploadedDate).HasColumnName("UploadedDate").IsOptional();
            Property(x => x.FileComment).HasColumnName("FileComment").IsOptional();
        }
    }
}

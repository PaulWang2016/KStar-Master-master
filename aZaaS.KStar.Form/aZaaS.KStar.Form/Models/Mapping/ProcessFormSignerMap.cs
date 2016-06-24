using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormSignerMap : EntityTypeConfiguration<ProcessFormSigner>
    {
        public ProcessFormSignerMap()
        {
            this.HasKey(t => new { t.ProcessInstId, t.SignerName });

            this.ToTable("ProcessFormSigner");
            this.Property(t => t.ProcessInstId).HasColumnName("ProcessInstId");
            this.Property(t => t.ActivityName).HasColumnName("ActivityName");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.SignerName).HasColumnName("SignerName");
            this.Property(t => t.SignerDate).HasColumnName("SignerDate");
            this.Property(t => t.IsApproval).HasColumnName("IsApproval");
            this.Property(t => t.ActionName).HasColumnName("ActionName");
            this.Property(t => t.ApprovalDate).HasColumnName("ApprovalDate");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormApprovalDraftMap : EntityTypeConfiguration<ProcessFormApprovalDraft>
    {
        public ProcessFormApprovalDraftMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Table & Column Mappings
            this.ToTable("ProcessFormApprovalDraft");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SerialNo).HasColumnName("SerialNo");
            this.Property(t => t.ActionName).HasColumnName("ActionName");
            this.Property(t => t.ActionComment).HasColumnName("ActionComment");
        }
    }
}

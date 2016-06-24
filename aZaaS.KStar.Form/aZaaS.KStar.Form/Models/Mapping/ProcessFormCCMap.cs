using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormCCMap : EntityTypeConfiguration<ProcessFormCC>
    {
        public ProcessFormCCMap()
        {
            this.HasKey(t => t.SysId);

            // Table & Column Mappings
            this.ToTable("ProcessFormCC");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.FormId).HasColumnName("FormId");
            this.Property(t => t.Originator).HasColumnName("Originator");
            this.Property(t => t.OriginatorName).HasColumnName("OriginatorName");
            this.Property(t => t.Receiver).HasColumnName("Receiver");
            this.Property(t => t.ReceiverName).HasColumnName("ReceiverName");
            this.Property(t => t.FormViewUrl).HasColumnName("FormViewUrl");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.ReceiverStatus).HasColumnName("ReceiverStatus");
            this.Property(t => t.ReviewComment).HasColumnName("ReviewComment"); 
        }
    }
}

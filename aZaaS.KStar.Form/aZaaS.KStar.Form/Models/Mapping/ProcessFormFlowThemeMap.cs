using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ProcessFormFlowThemeMap : EntityTypeConfiguration<ProcessFormFlowTheme>
    {
        public ProcessFormFlowThemeMap()
        {
            this.HasKey(t => t.ID); 
            this.ToTable("ProcessFormFlowTheme");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ProcessFullName).HasColumnName("ProcessFullName").IsUnicode(true);
            this.Property(t => t.ModelFullName).HasColumnName("ModelFullName");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.RuleString).HasColumnName("RuleString");  
        }
    }
}

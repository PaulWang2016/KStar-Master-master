using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ControlRenderTemplateMap:EntityTypeConfiguration<ControlRenderTemplate>
    {
        public ControlRenderTemplateMap()
        {
            this.HasKey(t => t.SysId);      

            this.ToTable("ControlRenderTemplate");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.HtmlTemplate).HasColumnName("HtmlTemplate");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");            
        }
    }
}

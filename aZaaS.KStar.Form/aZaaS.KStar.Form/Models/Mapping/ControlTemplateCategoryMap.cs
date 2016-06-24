using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ControlTemplateCategoryMap:EntityTypeConfiguration<ControlTemplateCategory>
    {
        public ControlTemplateCategoryMap()
        {
            this.HasKey(t => t.SysId);

            this.ToTable("ControlTemplateCategory");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ParentId).HasColumnName("ParentId");            
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");            
        }
    }
}

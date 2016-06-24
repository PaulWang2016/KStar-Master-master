using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ActivityControlSettingMap:EntityTypeConfiguration<ActivityControlSetting>
    {
        public ActivityControlSettingMap()
        {
            this.HasKey(t =>t.SysId);

            this.ToTable("ActivityControlSetting");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ActivityId).HasColumnName("ActivityId");
            this.Property(t => t.WorkMode).HasColumnName("WorkMode");
            this.Property(t => t.ControlRenderId).HasColumnName("ControlRenderId");
            this.Property(t => t.ControlName).HasColumnName("ControlName");
            this.Property(t => t.ControlType).HasColumnName("ControlType");
            this.Property(t => t.IsHide).HasColumnName("IsHide");
            this.Property(t => t.IsDisable).HasColumnName("IsDisable");
            this.Property(t => t.IsCustom).HasColumnName("IsCustom");
            this.Property(t => t.RenderTemplateId).HasColumnName("RenderTemplateId");
            this.Property(t => t.ProcessFullName).HasColumnName("ProcessFullName");
            
        }
    }
}

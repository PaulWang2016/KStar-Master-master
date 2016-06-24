using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Form.Models.Mapping
{
    public class ActivityFormSettingMap:EntityTypeConfiguration<ActivityFormSetting>
    {
        public ActivityFormSettingMap()
        {
            this.HasKey(t =>t.SysId);

            // Properties      

            this.ToTable("ActivityFormSetting");
            this.Property(t => t.SysId).HasColumnName("SysId");
            this.Property(t => t.ActivityId).HasColumnName("ActivityId");
            this.Property(t => t.WorkMode).HasColumnName("WorkMode");
            this.Property(t => t.IsCustom).HasColumnName("IsCustom");
            this.Property(t => t.IsEditable).HasColumnName("IsEditable");            
            this.Property(t => t.IsSettingEnabled).HasColumnName("IsSettingEnabled");
        }
    }
}

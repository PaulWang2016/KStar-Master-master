using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using aZaaS.KStar.Form.Models.Mapping;
using aZaaS.KStar.Form.Models;
using System;

namespace aZaaS.KStar.Form.Repositories
{
    public partial class aZaaSKStarFormContext : DbContext
    {
        public static Func<string> ConnectionSetter;
        static aZaaSKStarFormContext()
        {
            Database.SetInitializer<aZaaSKStarFormContext>(null);
        }

        public aZaaSKStarFormContext() : this(ConnectionSetter()) { }
        public aZaaSKStarFormContext(string connectionString)
            : base(connectionString) { }

        public DbSet<FormAttachment> FormAttachments { get; set; }
        public DbSet<ProcessFormContent> ProcessFormContents { get; set; }
        public DbSet<ProcessFormHeader> ProcessFormHeaders { get; set; }
        public DbSet<ProcessFormCC> ProcessFormCCs { get; set; }
        public DbSet<ProcessFormSigner> ProcessFormSigners { get; set; }
        public DbSet<ProcessFormCancel> ProcessFormCancels { get; set; }
        public DbSet<ProcessFormApprovalDraft> ProcessFormApprovalDrafts { get; set; }
        public DbSet<ActivityControlSetting> ActivityControlSettings { get; set; }        
        public DbSet<ControlRenderTemplate> ControlRenderTemplates { get; set; }
        public DbSet<ControlTemplateCategory> ControlTemplateCategorys { get; set; }

        public DbSet<ProcessFormFlowTheme> ProcessFormFlowThemes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FormAttachmentMap());
            modelBuilder.Configurations.Add(new ProcessFormContentMap());
            modelBuilder.Configurations.Add(new ProcessFormHeaderMap());
            modelBuilder.Configurations.Add(new ProcessFormCCMap());
            modelBuilder.Configurations.Add(new ProcessFormSignerMap());
            modelBuilder.Configurations.Add(new ProcessFormCancelMap());
            modelBuilder.Configurations.Add(new ProcessFormApprovalDraftMap());

            modelBuilder.Configurations.Add(new ActivityControlSettingMap());            
            modelBuilder.Configurations.Add(new ControlRenderTemplateMap());
            modelBuilder.Configurations.Add(new ControlTemplateCategoryMap());
            modelBuilder.Configurations.Add(new ProcessFormFlowThemeMap());
        }
    }
}

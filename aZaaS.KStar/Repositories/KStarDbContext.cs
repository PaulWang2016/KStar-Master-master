using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using aZaaS.KStar.Widgets;
using aZaaS.KStar.Documents;
using aZaaS.KStar.Menus;
using aZaaS.KStar.Portal;
using aZaaS.KStar.DynamicWidgets;
using aZaaS.KStar.AppRole;
using aZaaS.KStar.UserProfiles;
using aZaaS.KStar.SuperAdmin;
using aZaaS.KStar.AppDelegate;
using System.Web;
using aZaaS.KStar.Report;
using aZaaS.KStar.WorkflowData;
using aZaaS.KStar.Workflow.Configuration.Models;
using aZaaS.KStar.CustomRole.Models;
using aZaaS.KStar.DataDictionary;
using aZaaS.KStar.WorkflowConfiguration.Models;

namespace aZaaS.KStar.Repositories
{
    public class KStarDbContext : DbContext
    {
        public static Func<string> ConnectionSetter;
        static KStarDbContext()
        {
            //WE DON'T NEED ANY INITIALIZER
            Database.SetInitializer<KStarDbContext>(null);       
        }
        public KStarDbContext() : this(ConnectionSetter()) { }
        public KStarDbContext(string connectionString)
            : base(connectionString) { }
        public DbSet<WidgetEntity> Widget { get; set; }
        public DbSet<DocumentLibraryEntity> DocumentLibrary { get; set; }
        public DbSet<DocumentItemEntity> DocumentItem { get; set; }
        public DbSet<MenuEntity> Menu { get; set; }
        public DbSet<MenuItemEntity> MenuItem { get; set; }
        public DbSet<PortalEnvironmentEntity> PortalEnvironment { get; set; }

        public DbSet<DynamicWidgetEntity> DynamicWidget { get; set; }
        public DbSet<AppRoleEntity> AppRole { get; set; }
        public DbSet<AppDelegateEntity> AppDelegate { get; set; }

        public DbSet<UserProfileEntity> UserProfile { get; set; }
        public DbSet<SuperADEntity> SuperAD { get; set; }

        //public DbSet<AttachmentInfoEntity> AttachmentInfo { get; set; }

        public DbSet<ReportInfoEntity> ReportInfo { get; set; }
        public DbSet<ReportPermission> ReportPermissions { get; set; }

        public DbSet<ReportStatistics> ReportStatistics { get; set; }
        public DbSet<ReportFavouriteEntity> ReportFavourite { get; set; }
        public DbSet<Report_FavouriteEntity> Report_Favourite { get; set; }
        public DbSet<ReportCategoryEntity> ReportCategory { get; set; }
        public DbSet<FeedbackEntity> ReportFeedback { get; set; }

        //public DbSet<AuditLogDetailEntity> AuditLogDetail { get; set; }
        //public DbSet<AuditLogMasterEntity> AuditLogMaster { get; set; }

        public DbSet<RequestMainInfoEntity> RequestMainInfo { get; set; }
        public DbSet<ConfigurationEntity> Configurations { get; set; }

        public DbSet<Configuration_Category> Configuration_CategorySet { get; set; }
        public DbSet<Configuration_ProcessSet> Configuration_ProcessSetSet { get; set; }
        public DbSet<Configuration_ProcessVersion> Configuration_ProcessVersionSet { get; set; }
        public DbSet<Configuration_Activity> Configuration_ActivitySet { get; set; }
        public DbSet<Configuration_RefActivity> Configuration_RefActivitySet { get; set; }
        public DbSet<Configuration_User> Configuration_UserSet { get; set; }
        public DbSet<Configuration_ProcCommon> Configuration_ProcCommonSet { get; set; }
        public DbSet<CustomRoleCategory> CustomRoleCategory { get; set; }
        public DbSet<CustomRoleClassify> CustomRoleClassify { get; set; }
        public DbSet<DataDictionaryEntity> DataDictionary { get; set; }
        public DbSet<CandidateEntity> Candidate { get; set; }
        public DbSet<ViewFlowArgsEntity> ViewFlowArgs { get; set; }
        public DbSet<Role_ProcessSet> Role_ProcessSet { get; set; } 
        public DbSet<Configuration_LineRule> Configuration_LineRule { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WidgetEntity>().ToTable("Widget", "dbo");
            modelBuilder.Entity<DocumentLibraryEntity>().ToTable("DocumentLibrary", "dbo");
            modelBuilder.Entity<DocumentItemEntity>().ToTable("DocumentItem", "dbo");
            modelBuilder.Entity<MenuEntity>().ToTable("Menu", "dbo");
            modelBuilder.Entity<MenuItemEntity>().ToTable("MenuItem", "dbo");
            modelBuilder.Entity<PortalEnvironmentEntity>().ToTable("PortalEnvironment", "dbo");

            modelBuilder.Entity<DynamicWidgetEntity>().ToTable("DynamicWidget", "dbo");
            modelBuilder.Entity<AppRoleEntity>().ToTable("AppRole", "dbo");
            modelBuilder.Entity<AppDelegateEntity>().ToTable("AppDelegate", "dbo");
            modelBuilder.Entity<AppRoleEntity>().HasKey(m => new { m.RoleId, m.MenuId });
            modelBuilder.Entity<AppDelegateEntity>().HasKey(m => new { m.Id });

            modelBuilder.Entity<UserProfileEntity>().HasKey(m => new { m.UserId, m.Key });
            modelBuilder.Entity<UserProfileEntity>().ToTable("UserProfile", "dbo");
            modelBuilder.Entity<SuperADEntity>().ToTable("SuperAD", "dbo");

            //modelBuilder.Entity<AttachmentInfoEntity>().ToTable("AttachmentInfo", "dbo");

            //about report
            modelBuilder.Entity<ReportInfoEntity>().ToTable("ReportInfo", "dbo");
            modelBuilder.Entity<ReportPermission>().ToTable("ReportPermission", "dbo");
            modelBuilder.Entity<ReportStatistics>().ToTable("ReportStatistics", "dbo"); 
            modelBuilder.Entity<ReportFavouriteEntity>().ToTable("ReportFavourite", "dbo");
            modelBuilder.Entity<ReportFavouriteEntity>().HasKey(m => new { m.ID, m.UserID });
            modelBuilder.Entity<Report_FavouriteEntity>().ToTable("ReportInfo_Favourite", "dbo");
            modelBuilder.Entity<ReportCategoryEntity>().ToTable("ReportCategory", "dbo");
            modelBuilder.Entity<FeedbackEntity>().ToTable("ReportInfo_Feedback", "dbo");
            //modelBuilder.Entity<AuditLogDetailEntity>().ToTable("AuditLog_Detail", "dbo");
            //modelBuilder.Entity<AuditLogMasterEntity>().ToTable("AuditLog_Master", "dbo");

            modelBuilder.Entity<RequestMainInfoEntity>().ToTable("RequestMainInfo", "dbo");
            modelBuilder.Entity<ConfigurationEntity>().ToTable("Configuration", "dbo");

            modelBuilder.Entity<Configuration_Category>().ToTable("Configuration_Category", "dbo");
            modelBuilder.Entity<Configuration_ProcessSet>().ToTable("Configuration_ProcessSet", "dbo");
            modelBuilder.Entity<Configuration_ProcessVersion>().ToTable("Configuration_ProcessVersion", "dbo");
            modelBuilder.Entity<Configuration_Activity>().ToTable("Configuration_Activity", "dbo");
            modelBuilder.Entity<Configuration_RefActivity>().ToTable("Configuration_RefActivity", "dbo");
            modelBuilder.Entity<Configuration_User>().ToTable("Configuration_User", "dbo");
            modelBuilder.Entity<Configuration_ProcCommon>().ToTable("Configuration_ProcCommon", "dbo");

            modelBuilder.Entity<Role_ProcessSet>().ToTable("Role_ProcessSet", "dbo");

            modelBuilder.Entity<CustomRoleCategory>().ToTable("CustomRoleCategory", "dbo");
            modelBuilder.Entity<CustomRoleClassify>().ToTable("CustomRoleClassify", "dbo");

            modelBuilder.Entity<DataDictionaryEntity>().ToTable("DataDictionary", "dbo");

            modelBuilder.Entity<CandidateEntity>().ToTable("Candidate", "dbo");

            modelBuilder.Entity<ViewFlowArgsEntity>().ToTable("ViewFlowArgs", "dbo");
            modelBuilder.Entity<Configuration_LineRule>().ToTable("Configuration_LineRule", "dbo");

            

            base.OnModelCreating(modelBuilder);
        }

        //public override int SaveChanges()
        //{
        //    Object currentUser = HttpContext.Current.Session["CurrentUser"];
        //    var audit = new AuditTrail(this, currentUser != null ? currentUser.ToString() : HttpContext.Current.User.Identity.Name);

        //    audit.AuditLogMasters();
        //    var retVal = base.SaveChanges();


        //    foreach (var item in audit.GetAuditLogMasters())
        //    {
        //        this.AuditLogMaster.Add(item);
        //        SaveAuditLogDetail(item.AuditLogDetails, item.Id);
        //    }
        //    base.SaveChanges();

        //    return retVal;
        //}

        //internal void SaveAuditLogDetail(IList<AuditLogDetailEntity> AuditLogs, Int64 parentId)
        //{
        //    foreach (var item in AuditLogs)
        //    {
        //        item.ParentID = parentId;
        //        this.AuditLogDetail.Add(item);
        //    }
        //}
    }
}

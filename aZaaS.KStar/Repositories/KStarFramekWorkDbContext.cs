using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.ProcessForm;
using aZaaS.KStar.LogRequestProvider;

namespace aZaaS.KStar.Repositories
{
    public class KStarFramekWorkDbContext : DbContext
    {
        public static Func<string> ConnectionSetter;
        static KStarFramekWorkDbContext()
        {
            //WE DON'T NEED ANY INITIALIZER
            Database.SetInitializer<KStarFramekWorkDbContext>(null);       
        }
        public KStarFramekWorkDbContext() : this(ConnectionSetter()) { }
        public KStarFramekWorkDbContext(string connectionString)
            : base(connectionString) { }

        public DbSet<ProcessFormHeader> ProcessFormHeader { get; set; }

        public DbSet<ProcessFormContent> ProcessFormContent { get; set; }
        public DbSet<ProcessFormCancel> ProcessFormCancel { get; set; }
        public DbSet<ActivityControlSetting> ActivityControlSetting { get; set; }
        public DbSet<LogRequest> LogRequest { get; set; }

        public DbSet<RoleUser> RoleUser { get; set; }

        public DbSet<Position> Positions { get; set; }
        

        public DbSet<PositionUser> PositionUsers { get; set; }

        public DbSet<PositionOrgNodes> PositionOrgNodes { get; set; }
         
        public DbSet<UserOrgNode> UserOrgNodes { get; set; }

        public DbSet<OrgNode> OrgNodes { get; set; }
        
        public DbSet<User> User { get; set; } 

        public DbSet<ProcessActivityParticipantSetEntry> ProcessActivityParticipantSetEntry { get; set; }

        public DbSet<ProcessActivityParticipantSet> ProcessActivityParticipantSet { get; set; }

        public DbSet<ProcessPrognosis> ProcessPrognosis { get; set; }

        public DbSet<ProcessPrognosisDetail> ProcessPrognosisDetail { get; set; }

        public DbSet<ProcessPrognosisTask> ProcessPrognosisTask { get; set; }

        public  DbSet<view_ProcinstList> view_ProcinstList { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessFormHeader>().ToTable("ProcessFormHeader", "dbo");
            modelBuilder.Entity<ProcessFormContent>().ToTable("ProcessFormContent", "dbo");
            modelBuilder.Entity<ProcessFormCancel>().ToTable("ProcessFormCancel", "dbo");
            modelBuilder.Entity<ActivityControlSetting>().ToTable("ActivityControlSetting", "dbo");
            modelBuilder.Entity<LogRequest>().ToTable("LogRequest", "dbo");
            modelBuilder.Entity<RoleUser>().ToTable("RoleUsers", "dbo");
            modelBuilder.Entity<Position>().ToTable("Position", "dbo");
            modelBuilder.Entity<PositionUser>().ToTable("PositionUsers", "dbo");
            modelBuilder.Entity<PositionOrgNodes>().ToTable("PositionOrgNodes", "dbo");
            modelBuilder.Entity<UserOrgNode>().ToTable("UserOrgNodes", "dbo");
            modelBuilder.Entity<OrgNode>().ToTable("OrgNode", "dbo");
            modelBuilder.Entity<ProcessActivityParticipantSet>().ToTable("ProcessActivityParticipantSet", "dbo");
            modelBuilder.Entity<ProcessActivityParticipantSetEntry>().ToTable("ProcessActivityParticipantSetEntry", "dbo");
            modelBuilder.Entity<RoleUser>().HasKey(x => new { x.User_SysId, x.Role_SysId });
            modelBuilder.Entity<PositionUser>().HasKey(x => new { x.User_SysId, x.Position_SysId });
            modelBuilder.Entity<PositionOrgNodes>().HasKey(x => new { x.OrgNode_SysId, x.Position_SysId });
            modelBuilder.Entity<UserOrgNode>().HasKey(x => new { x.User_SysId, x.OrgNode_SysId });
            modelBuilder.Entity<User>().ToTable("User", "dbo");
            modelBuilder.Entity<ProcessPrognosis>().ToTable("ProcessPrognosis", "dbo");
            modelBuilder.Entity<ProcessPrognosisDetail>().ToTable("ProcessPrognosisDetail", "dbo");
            modelBuilder.Entity<ProcessPrognosisTask>().ToTable("ProcessPrognosisTask", "dbo");
            modelBuilder.Entity<view_ProcinstList>().HasKey(x => new { x.ProcInstID, x.Originator });
            base.OnModelCreating(modelBuilder);
        }
    }
}

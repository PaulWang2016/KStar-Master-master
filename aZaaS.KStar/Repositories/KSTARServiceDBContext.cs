using aZaaS.KStar.KstarMobile;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.Repositories
{
    public class KSTARServiceDBContext : DbContext
    {
        public static Func<string> ConnectionSetter;
        static KSTARServiceDBContext()
        {
            //WE DON'T NEED ANY INITIALIZER
            Database.SetInitializer<KSTARServiceDBContext>(null);       
        }
        public KSTARServiceDBContext() : this(ConnectionSetter()) { }
        public KSTARServiceDBContext(string connectionString) : base(connectionString) { }

        public DbSet<GroupDefinitionEntity> GroupDefinition { get; set; }
        public DbSet<ItemDefinitionEntity> ItemDefinition { get; set; }
        public DbSet<LabelContentEntity> LabelContent { get; set; }
        public DbSet<ProcessDefinitionEntity> ProcessDefinition { get; set; } 
        public DbSet<ProcessExtend> ProcessExtendSet { get; set; }
        public DbSet<ProcessPermission> ProcessPermissionSet { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //KStar
            modelBuilder.Entity<GroupDefinitionEntity>().ToTable("GroupDefinition", "dbo");
            modelBuilder.Entity<ItemDefinitionEntity>().ToTable("ItemDefinition", "dbo");
            modelBuilder.Entity<LabelContentEntity>().ToTable("LabelContent", "dbo");
            modelBuilder.Entity<ProcessDefinitionEntity>().ToTable("ProcessDefinition", "dbo");
            modelBuilder.Entity<ProcessExtend>().ToTable("ProcessExtend", "dbo");
            modelBuilder.Entity<ProcessPermission>().ToTable("ProcessPermission", "dbo");
            base.OnModelCreating(modelBuilder);
        }
    }
}

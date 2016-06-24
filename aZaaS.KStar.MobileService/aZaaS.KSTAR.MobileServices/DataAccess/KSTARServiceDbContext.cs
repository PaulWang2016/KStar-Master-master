using aZaaS.KSTAR.MobileServices.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace aZaaS.KSTAR.MobileServices.DataAccess
{
    public class KSTARServiceDbContext : DbContext
    {
        public KSTARServiceDbContext()
            : base("KSTARServiceConnection")
        {
        }

        public KSTARServiceDbContext(string connectionString)
            : base(connectionString) { }

        public DbSet<ProcessDefinition> ProcessDefinitionSet { get; set; }
        public DbSet<GroupDefinition> GroupDefinitionSet { get; set; }
        public DbSet<ItemDefinition> ItemDefinitionSet { get; set; }
        public DbSet<LabelContent> LabelContentSet { get; set; }
        public DbSet<LogEntity> LogEntitySet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessDefinition>().ToTable("ProcessDefinition", "dbo");
            modelBuilder.Entity<GroupDefinition>().ToTable("GroupDefinition", "dbo");
            modelBuilder.Entity<ItemDefinition>().ToTable("ItemDefinition", "dbo");
            modelBuilder.Entity<LabelContent>().ToTable("LabelContent", "dbo");
            modelBuilder.Entity<LogEntity>().ToTable("LogEntity", "dbo");

            base.OnModelCreating(modelBuilder);
        }
    }
}
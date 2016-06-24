namespace SSOLoginWeb.Manage.DbContext
{
    using SSOLoginWeb.Manage.DbContext.EntityObject;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class KStarDbContext : DbContext
    {

        public KStarDbContext()
            : base("aZaaSFramework")
        {
            Database.SetInitializer<KStarDbContext>(null);      
        } 
        public virtual DbSet<User> UserSet { get; set; }

        public virtual DbSet<RoleUser> RoleUserSet { get; set; }

        public virtual DbSet<NeoWay_Business_NetworkRole> NeoWay_Business_NetworkRoleSet { get; set; }
         
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User", "dbo");
            modelBuilder.Entity<RoleUser>().ToTable("RoleUsers", "dbo");
            modelBuilder.Entity<NeoWay_Business_NetworkRole>().ToTable("NeoWay_Business_NetworkRole", "dbo");
            base.OnModelCreating(modelBuilder);
        }
    } 
}
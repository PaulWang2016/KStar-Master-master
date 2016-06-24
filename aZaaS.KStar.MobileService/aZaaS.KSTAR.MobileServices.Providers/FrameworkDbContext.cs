using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KSTAR.MobileServices.Providers
{
    public class FrameworkDbContext : DbContext
    {
        public FrameworkDbContext()
            : base("DBConnectionString")
        {
            Database.SetInitializer<FrameworkDbContext>(null);
        }
        public FrameworkDbContext(string connectionString)
            : base(connectionString) { }


        public DbSet<ProcessFormContent> ProcessFormContentSet { get; set; }

        public DbSet<ProcessFormHeader> ProcessFormHeaderSet { get; set; }

        public DbSet<User> UserSet { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessFormContent>().ToTable("ProcessFormContent", "dbo");
            modelBuilder.Entity<ProcessFormHeader>().ToTable("ProcessFormHeader", "dbo");
            modelBuilder.Entity<User>().ToTable("User", "dbo");
             base.OnModelCreating(modelBuilder);
        }
    }
      
    public  class ProcessFormContent
    {
        [Key]
        public int SysId { get; set; }
        public Nullable<int> FormID { get; set; }
        public string JsonData { get; set; }
        public string XmlData { get; set; }
    }

    public  class ProcessFormHeader
    {
        [Key]
        public int FormID { get; set; }
        public string FormSubject { get; set; }
        public Nullable<int> ProcInstID { get; set; }
        public string ProcessFolio { get; set; }
        public Nullable<int> Priority { get; set; }
        public string SubmitterAccount { get; set; }
        public string SubmitterDisplayName { get; set; }
        public Nullable<System.DateTime> SubmitDate { get; set; }
        public string ApplicantAccount { get; set; }
        public string ApplicantDisplayName { get; set; }
        public string ApplicantTelNo { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicantPositionID { get; set; }
        public string ApplicantPositionName { get; set; }
        public string ApplicantOrgNodeID { get; set; }
        public string ApplicantOrgNodeName { get; set; }
        public string SubmitComment { get; set; }
        public Nullable<bool> IsDraft { get; set; }
        public string DraftUrl { get; set; }
    }

    public  class User
    {
        public System.Guid SysId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }

}

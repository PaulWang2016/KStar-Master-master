using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.Framework.Organization;
using System.Data.SqlClient;
using aZaaS.Framework.Organization.Expressions;
using aZaaS.Framework.Extend;
using aZaaS.Framework.Configuration;
using aZaaS.Framework.Organization.OrgChart;

namespace aZaaS.Framework.Organization.UserManagement
{
    public class UserRepository : EntityFrameworkRepository<User>
    {
        public UserRepository(IRepositoryContext context)
            : base(context)
        {
            aZaaS.Framework.Configuration.Config.Initialize(new aZaaS.Framework.Organization.ExtendDbConfig());
        }

        public User ReadUser(Guid userId)
        {
            return this.EfContext.Context.Set<User>().Include("ExtendItems").Include("ReportTo").Include("Roles").Include("Positions").Include("Nodes").Where(x => x.SysID == userId).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves user according to the specified user name.
        /// </summary>
        /// <param name="userName">The specified username</param>
        /// <returns>The satisfied user instance</returns>
        public User GetUser(string userName)
        {
            return this.EfContext.Context.Set<User>().Include("ExtendItems").FirstOrDefault(u => u.UserName.Equals(userName));
            //return this.Get(new UserNameEqualsSpecification(userName));
        }

        /// <summary>
        /// Checks whether the specified user name exists.
        /// </summary>
        /// <param name="userName">The specified user name</param>
        /// <returns>True or false</returns>
        public bool UserNameExists(string userName)
        {
            return this.Exists(new UserNameEqualsSpecification(userName));
        }

        public bool UserFieldExists(User user, UserExtend field)
        {
            return this.Exists(new UserFieldExistsSpecification(user, field));
        }

        /// <summary>
        /// Checks whether the specified role is already assigned to the user. 
        /// </summary>
        /// <param name="user">The user instance</param>
        /// <param name="role">The role instance</param>
        /// <returns>True or false</returns>
        public bool UserRoleExists(User user, Role role)
        {
            return this.Exists(new UserRoleExistsSpecification(user, role));
        }

        public bool UserOwnerExists(User user, User owner)
        {
            return this.Exists(new UserOwnerExistsSpecification(user, owner));
        }

        public bool UserPositionExists(User user, Position position)
        {
            return this.Exists(new UserPositionExistsSpecification(user, position));
        }

        public IEnumerable<User> GetUsersWithFields(System.Linq.Expressions.Expression<Func<User, bool>> express)
        {          
            return this.EfContext.Context.Set<User>().Include("ExtendItems").Where(express).ToList();
        }

        public IEnumerable<User> GetUsersWithFields(System.Linq.Expressions.Expression<Func<User, bool>> express, int pageNubmer, int pageSize)
        {
            int skip = pageSize * (pageNubmer - 1), take = pageSize;
            return this.EfContext.Context.Set<User>().Include("ExtendItems").Where(express).OrderBy(e => e.SysID).Skip(skip).Take(take);      
        }

        public IEnumerable<User> GetNonReferenceUsers()
        {
            return this.EfContext.Context.Set<User>().Include("Nodes").Where(x => x.Nodes.Count==0).ToList();
        }

        public void UpdateField(UserExtend field)
        {
            User user = this.EfContext.Context.Set<User>().Include("ExtendItems").Where(x => x.SysID == field.SysID).FirstOrDefault();
            UserExtend temp = user.ExtendItems.Where(x => x.Name == field.Name).FirstOrDefault();
            temp.Value = field.Value;
            this.Update(user);
            this.Context.Commit();
        }

        public void RemoveField(Guid sysId, string name)
        {
            User user = this.EfContext.Context.Set<User>().Include("ExtendItems").Where(x => x.SysID == sysId).FirstOrDefault();
            UserExtend temp = user.ExtendItems.Where(x => x.Name == name).FirstOrDefault();
            user.ExtendItems.Remove(temp);
            this.Update(user);
            this.Context.Commit();
        }

        protected override IEnumerable<User> DoQuery(QueryExpression expression)
        {
            string sql = string.Empty;

            sql = string.Format(@"SELECT DISTINCT [USER].[SysId],[UserName],[FirstName],[LastName],[Sex],[Email],[Address],[Phone],[Status],[Remark]
                                FROM (
                                SELECT U.[SysId],[UserName],[FirstName],[LastName],[Sex],[Email],[Address],[Phone],[Status],[Remark],
                                F.SysId AS [ExFieldId],F.Name AS [ExFieldName],F.Value AS [ExFieldValue]
                                FROM [User] AS U LEFT JOIN  [UserExtends] AS F  ON U.[SysId] = F.[SysId] 
                                ) AS [User] WHERE ( {0} )", (expression as LogicalExpression).ToSqlLogicalCondition());

            return this.EfContext.Context.Set<User>().SqlQuery(sql);
        }

        public int DoQueryCount(System.Linq.Expressions.Expression<Func<User, bool>> express)
        {
            return this.EfContext.Context.Set<User>().Where(express).OrderBy(e => e.SysID).Count();              
        }


        protected override Extend.FieldBase[] GetExtendFiledsDefintion()
        {
            IExtendableProvider extendableProvider = new ExtendableProvider(Config.Default);
            var fields = extendableProvider.GetExtensionFields("User");

            return fields;            
        }

        protected override void SaveExtendFiledsDefintion(FieldBase[] fields)
        {
            IExtendableProvider extendableProvider = new ExtendableProvider(Config.Default);
            extendableProvider.SaveExtensionFields("User",fields);
        }

        public IEnumerable<User> GetUsersWithRelationships(System.Linq.Expressions.Expression<Func<User, bool>> express)
        {
            return this.EfContext.Context.Set<User>().Include("Nodes").Include("Positions").Include("ReportTo").Include("ExtendItems").Where(express).ToList();
        }

        public IEnumerable<User> GetUsersWithRelationships(System.Linq.Expressions.Expression<Func<User, bool>> express, int pageNubmer, int pageSize)
        {
            int skip = pageSize * (pageNubmer - 1), take = pageSize;
            return this.EfContext.Context.Set<User>().Include("Nodes").Include("Positions").Include("ReportTo").Include("ExtendItems").Where(express).OrderBy(e => e.SysID).Skip(skip).Take(take);
        }       

        /// <summary>
        /// 获取当前用户的下属用户列表
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<User> GetUsersReportFrom(Guid userId)
        {
            List<User> reportfrom = new List<User>();
            List<User> users = this.EfContext.Context.Set<User>().Include("ReportTo").Where(x => x.SysID != userId).ToList();
            users.ForEach(x =>
            {
                 User user=new User(){ SysID=userId};
                 if (x.ReportTo != null && x.ReportTo.Count > 0 && x.ReportTo.Contains(user))
                 {
                     reportfrom.Add(x);
                 }
            });
            return reportfrom;
        }


        public void DeleteUserExtends(string name)
        {
            var userExtends = this.EfContext.Context.Set<UserExtend>().Where(x => x.Name == name);
            this.EfContext.Context.Set<UserExtend>().RemoveRange(userExtends);
            this.EfContext.Context.SaveChanges();
        }
    }
}

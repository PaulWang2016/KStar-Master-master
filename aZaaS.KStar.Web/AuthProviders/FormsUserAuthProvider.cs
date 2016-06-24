using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

using aZaaS.KStar.Web.Helper;
using aZaaS.KStar.Authentication;

namespace aZaaS.KStar.Web.AuthProviders
{
    public class FormsUserAuthProvider : IUserAuthProvider
    {
        //private readonly string _connectionString;
        private const string PARAM_CONNECTIONSTRING = "ConnectionString";

        public FormsUserAuthProvider()
        {
            //OPTION2:
            //_connectionString = ConfigurationManager.ConnectionStrings["aZaaS.KStar"].ConnectionString;
        }

        public bool Authenticate(string userName, string password)
        {
            //背景设定：
            //KStar运行于非域环境中，用户使用自定义帐号和密码登录

            this.ValidUserExists(userName);//校验KStar User表用户

            //校验用户扩展表的用户名和密码（以后如果合并，即可忽略以上校验）
            using (var cxt = new FxUserContext(this.GetParameter(PARAM_CONNECTIONSTRING)))
            {
                Fx_User user = cxt.Fx_Users.FirstOrDefault(u => u.UserName == userName);
                if (user == null)
                    ExceptionRaiser.UserNotExists(userName);

                if (user.Password != EncryptUtility.GetPasswordHash(password, user.PasswordSalt))
                    ExceptionRaiser.PasswordNotMatch(userName);

                return true;
            }
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            this.ValidUserExists(userName);
            Authenticate(userName, oldPassword);

            using (var cxt = new FxUserContext(this.GetParameter(PARAM_CONNECTIONSTRING)))
            {
                var user = cxt.Fx_Users.FirstOrDefault(u => u.UserName == userName);
                string text = EncryptUtility.CreateSalt();
                user.Password = EncryptUtility.GetPasswordHash(newPassword, text);
                user.PasswordSalt = text;
                cxt.SaveChanges();
            }

            return true;
        }

        public string LoginName(string userName)
        {
            return userName;
        }

        public void ParameterMapValidator(Dictionary<string, string> parameters)
        {
            //TODO: Validating configured parameters here...
        }
    }

    internal class Fx_User
    {
        [Key]
        public string UserName
        {
            get;
            set;
        }
        public string Password
        {
            get;
            set;
        }
        public string PasswordSalt
        {
            get;
            set;
        }
    }

    internal class FxUserContext : DbContext
    {
        public DbSet<Fx_User> Fx_Users
        {
            get;
            set;
        }
        public FxUserContext(string nameOrConnectionstring)
            : base(nameOrConnectionstring)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fx_User>().ToTable("Fx_User");
            base.OnModelCreating(modelBuilder);
        }
    }
}
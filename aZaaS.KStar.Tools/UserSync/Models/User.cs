using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using LinqToLdap.Mapping;

namespace UserSync.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string Email { get; set; }
        public string CommonName { get; set; }
        public Guid Guid { get; set; }
        public SecurityIdentifier Sid { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime WhenCreated { get; set; }
        public DateTime LastChanged { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }

        public string UserFQDN
        {
            get
            {
                Domain =_domainResolver(DistinguishedName);

                if (string.IsNullOrEmpty(Domain))
                    return UserName;

                return string.Format(@"{0}\{1}", Domain, UserName);
            }
        }

        private static Func<string, string> _domainResolver;
        public static void SetDomainResolver(Func<string, string> domainResolver)
        {
            _domainResolver = domainResolver;
        }

        public override int GetHashCode()
        {
            return this.UserFQDN.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}{2}", UserFQDN, FirstName, LastName);
        }
    }
}

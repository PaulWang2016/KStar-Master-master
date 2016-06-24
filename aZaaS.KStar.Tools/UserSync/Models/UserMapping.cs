using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LinqToLdap.Mapping;

namespace UserSync.Models
{
    public class UserMapping : ClassMap<User>
    {

        public UserMapping(string namingContext)
        {
            NamingContext(namingContext);
        }

        public override IClassMap PerformMapping(string namingContext = null,
            string objectCategory = null, bool includeObjectCategory = true,
            IEnumerable<string> objectClasses = null, bool includeObjectClasses = true)
        {
            ObjectCategory("Person");
            ObjectClass("User");

            Map(x => x.UserName).Named("sAMAccountName");
            Map(x => x.Email).Named("userPrincipalName");
            Map(x => x.DisplayName).Named("displayName");
            DistinguishedName(x => x.DistinguishedName);
            Map(x => x.CommonName).Named("cn").ReadOnly();
            Map(x => x.Guid).Named("objectguid").StoreGenerated();
            Map(x => x.Sid).Named("objectsid").StoreGenerated();
            Map(x => x.Title);
            Map(x => x.FirstName).Named("givenname");
            Map(x => x.LastName).Named("sn");
            Map(x => x.WhenCreated).StoreGenerated();
            Map(x => x.LastChanged).StoreGenerated();
            Map(x => x.Description).Named("description");

            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.CustomRole
{
    public abstract class CustomRoleBase : MarshalByRefObject,ICustomRole
    {
        private Guid _key;

        public CustomRoleBase(string key)
        {
            _key = Guid.Parse(key);
        }

        public Guid Key
        {
            get { return _key; }
        }

        public abstract IEnumerable<string> Execute(CustomRoleContext context);

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}

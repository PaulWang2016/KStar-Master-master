using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace aZaaS.KStar.Repositories
{
    internal class K2DBContext : DbContext
    {
        public K2DBContext() : this("K2DB") { }
        public K2DBContext(string connectionString)
            : base(connectionString) { }

    }
}

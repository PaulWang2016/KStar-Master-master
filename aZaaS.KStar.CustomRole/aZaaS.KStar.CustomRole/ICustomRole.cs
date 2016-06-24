using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace aZaaS.KStar.CustomRole
{
    [InheritedExport(typeof(ICustomRole))]
    public interface ICustomRole
    {
        Guid Key { get; }

        IEnumerable<string> Execute(CustomRoleContext context);
    }
}

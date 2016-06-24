using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aZaaS.KStar.KstarMobile
{
    public class ProcessComparer : IEqualityComparer<ProcessDefinitionEntity>
    {
        public bool Equals(ProcessDefinitionEntity x, ProcessDefinitionEntity y)
        {
            if (x == null)
                return y == null;
            return x.ProcessFullName == y.ProcessFullName;
        }

        public int GetHashCode(ProcessDefinitionEntity obj)
        {
            if (obj == null)
                return 0;
            return obj.ProcessFullName.GetHashCode();
        }        
    }
}

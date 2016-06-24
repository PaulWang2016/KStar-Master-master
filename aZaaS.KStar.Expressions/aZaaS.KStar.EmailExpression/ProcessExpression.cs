using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

using aZaaS.Framework;
using aZaaS.Framework.Template;

namespace aZaaS.KStar.EmailExpression
{
    [Export("Process", typeof(ITemplateExpression))]
    public class ProcessExpression : ProcessInstanceBase, ITemplateExpression
    {
        public void Fill(ServiceContext context)
        {
            this.SetProperties(context);
        }
    }
}

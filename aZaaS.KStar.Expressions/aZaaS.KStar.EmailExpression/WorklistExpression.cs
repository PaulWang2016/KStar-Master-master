using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using System.ComponentModel.Composition;
using aZaaS.Framework.Extensions;
using aZaaS.Framework.Template;


namespace aZaaS.KStar.EmailExpression
{
    [Export("Worklist", typeof(ITemplateExpression))]
    public class WorklistExpression : WorklistItemBase, ITemplateExpression
    {
        public void Fill(ServiceContext context)
        {
            this.SetProperties(context);
        }

    }
}

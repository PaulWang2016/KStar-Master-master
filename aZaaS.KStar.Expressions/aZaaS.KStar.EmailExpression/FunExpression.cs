using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aZaaS.Framework.Template;
using System.ComponentModel.Composition;
using System.IO;
using aZaaS.Framework;

namespace aZaaS.KStar.EmailExpression
{
    [Export("Fun", typeof(ITemplateExpression))]
    public class FunExpression : ITemplateExpression
    {
        public void Fill(ServiceContext context)
        {
        }

        public TimeSpan DateDiff(DateTime date1, DateTime date2)
        {
            var span = (date1 - date2);
            return span;
        }

        public string Link(string text, params string[] paths)
        {
            var link = Path.Combine(paths).Replace("\\", "/");
            return string.Format("<a href='{0}'>{1}</a>", link, text);
        }
    }
}

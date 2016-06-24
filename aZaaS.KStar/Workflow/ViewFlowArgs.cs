using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aZaaS.KStar.Repositories;

namespace aZaaS.KStar
{
    public class ViewFlowArgs
    {
        public bool SaveFlowArgs(string allArgs, string procInstId)
        {
            using (KStarDbContext dbContext = new KStarDbContext())
            {
                allArgs = (string.IsNullOrEmpty(allArgs) ? string.Format("ProcInstID={0}", procInstId) : string.Format("{0}&ProcInstID={1}", allArgs, procInstId));
                var item = dbContext.ViewFlowArgs.FirstOrDefault(r => r.ProcInstID == procInstId);

                if (item == null)
                {
                    item = new ViewFlowArgsEntity()
                    {
                        ProcInstID = procInstId,
                        FlowArgs = allArgs
                    };

                    dbContext.ViewFlowArgs.Add(item);
                }
                else
                {
                    item.FlowArgs = allArgs;
                }

                dbContext.SaveChanges();

                return true;
            }
        }

        public string FormatViewUrl(string viewUrl, string procInstId)
        {
            if (string.IsNullOrWhiteSpace(viewUrl))
            {
                viewUrl = string.Format("{0}?ProcInstID={1}", "null", procInstId);
                return viewUrl;
            }

            var list = viewUrl.Split('?');

            if (list.Length < 2)
            {
                viewUrl = string.Format("{0}?ProcInstID={1}", viewUrl, procInstId);
                return viewUrl;
            }

            var serverArgList = GetArgments(procInstId);

            var argments = list.Last();
            var argList = argments.Split('&');

            foreach (var args in argList)
            {
                var arg = args.Split('=');

                var key = arg.Last().TrimStart('{').TrimEnd('}');

                if (key.Length > 0)
                {
                    if (serverArgList.ContainsKey(key))
                    {
                        viewUrl = viewUrl.Replace(arg.Last(), serverArgList[key]);
                    }
                    else
                    {
                        viewUrl = viewUrl.Replace(key, "");
                    }
                }
            }

            return viewUrl;
        }

        private Dictionary<string, string> GetArgments(string procInstId)
        {
            var dicArgs = new Dictionary<string, string>();

            using (KStarDbContext dbContext = new KStarDbContext())
            {
                var item = dbContext.ViewFlowArgs.FirstOrDefault(r => r.ProcInstID == procInstId);

                if (item == null)
                {
                    return dicArgs;
                }

                var argList = item.FlowArgs.Split('&');

                foreach (var arg in argList)
                {
                    var argString = arg.Split('=');

                    dicArgs.Add(argString.First(), argString.Last());
                }

                return dicArgs;
            }
        }
    }

    public class ViewFlowArgsEntity
    {
        [Key]
        public string ProcInstID { get; set; }

        public string FlowArgs { get; set; }
    }

    
}

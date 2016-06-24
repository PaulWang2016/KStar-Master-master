using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using aZaaS.KStar.MgmtDtos;

namespace aZaaS.KStar.Report
{
    public static class ReportUrlResolver
    {
        private readonly static Dictionary<string, Func<UserWithFieldsDto, string>> _queryParameters = new Dictionary<string, Func<UserWithFieldsDto, string>>();

        public static void AddQueryParameter(string name, Func<UserWithFieldsDto, string> valueResolver)
        {
            if (!_queryParameters.ContainsKey(name))
                _queryParameters.Add(name, valueResolver);
            else
                _queryParameters[name] = valueResolver;
        }

        public static string ResolveQueryParameters(UserWithFieldsDto user)
        {
            var paramsList = new List<string>();

            foreach (var key in _queryParameters.Keys)
            {
                paramsList.Add(string.Format("{0}={1}", key, _queryParameters[key](user)));
            }

            return string.Join("&", paramsList.ToArray());
        }

        public static string ApplyQueryParameters(UserWithFieldsDto user,string reportUrl)
        {
            var resolvedParameters = ResolveQueryParameters(user);

            var insertParameters = string.Format("{0}{1}", string.IsNullOrEmpty(resolvedParameters) ? "" : "?", resolvedParameters);
            var appendParameters = string.Format("{0}{1}", string.IsNullOrEmpty(resolvedParameters) ? "" : "&", resolvedParameters);

            return reportUrl.Contains('?') ? string.Format("{0}{1}", reportUrl, appendParameters) : string.Format("{0}{1}", reportUrl, insertParameters);
        }

        public static string ApplyQueryParameters(string resolvedParameters, string reportUrl)
        {
            var insertParameters = string.Format("{0}{1}", string.IsNullOrEmpty(resolvedParameters) ? "" : "?", resolvedParameters);
            var appendParameters = string.Format("{0}{1}", string.IsNullOrEmpty(resolvedParameters) ? "" : "&", resolvedParameters);

            return reportUrl.Contains('?') ? string.Format("{0}{1}", reportUrl, appendParameters) : string.Format("{0}{1}", reportUrl, insertParameters);
        }
    }
}

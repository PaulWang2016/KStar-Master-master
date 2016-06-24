using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using aZaaS.Framework;
using aZaaS.Framework.Template;
using System.ComponentModel.Composition;
using aZaaS.Framework.Workflow;
using System.Data;

namespace aZaaS.KStar.EmailExpression
{
    [Export("SmartObject", typeof(ITemplateExpression))]
    public class SmartObjectExpression : ITemplateExpression
    {
        private ServiceContext _context;

        public void Fill(ServiceContext context)
        {
            this._context = context;
        }

        private Dictionary<string, dynamic> _singleCache = new Dictionary<string, dynamic>();
        private Dictionary<string, List<dynamic>> _listCache = new Dictionary<string, List<dynamic>>();

        public dynamic Single(string smartObjectName, string methodName, string fieldName, string fieldValue)
        {
            var key = string.Format("[{0}]|[{1}]|[{2}]|[{3}]", smartObjectName, methodName, fieldName, fieldName);
            if (!_singleCache.ContainsKey(key))
            {
                var dy = new ExObject();

                var obj = K2SmartObjectHelper.SmartObjectLoad(this._context, new Dictionary<string, object>() { { fieldName, fieldValue } }, smartObjectName, methodName);
                var dic = obj as Dictionary<string, object>;

                if (dic != null)
                {
                    dy.SetItems(dic);
                }

                _singleCache.Add(key, dy);
            }
            return _singleCache[key];
        }


        public List<dynamic> List(string smartObjectName, string methodName)
        {
            return List(smartObjectName, methodName, "", "");
        }

        public List<dynamic> List(string smartObjectName, string methodName, string fieldName, string fieldValue)
        {
            var key = string.Format("[{0}]|[{1}]|[{2}]|[{3}]", smartObjectName, methodName, fieldName, fieldName);
            if (!_listCache.ContainsKey(key))
            {
                var list = new List<dynamic>();

                var dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldValue))
                {
                    dic.Add(fieldName, fieldValue);
                }

                var soList = K2SmartObjectHelper.SmartObjectGetList(this._context, dic, smartObjectName, methodName);

                var cols = soList.Columns;

                foreach (DataRow row in soList.Rows)
                {
                    var data = new Dictionary<string, object>();
                    foreach (DataColumn col in cols)
                    {
                        data.Add(col.ColumnName, row[col]);
                    }
                    var dy = new ExObject();
                    dy.SetItems(data);
                    list.Add(dy);
                }
                _listCache.Add(key, list);
            }
            return _listCache[key];
        }
    }
}

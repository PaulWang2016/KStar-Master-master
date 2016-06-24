using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Dynamic;

namespace aZaaS.KStar.EmailExpression
{
    public class ExObject : DynamicObject, IEnumerable
    {
        private readonly IDictionary<string, object> _dateItems = new Dictionary<string, object>();
        public IDictionary<string, Object> DataItems
        {
            get { return this._dateItems; }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this._dateItems.Keys;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Count() != 1)
            {
                base.TryGetIndex(binder, indexes, out result);
            }

            var indexStr = indexes.Single() as string;
            if (_dateItems.ContainsKey(indexStr))
            {
                result = _dateItems[indexStr];
                return true;
            }

            return base.TryGetIndex(binder, indexes, out result);

        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Count() != 1)
            {
                return base.TrySetIndex(binder, indexes, value);
            }

            var indexStr = indexes.Single() as string;
            _dateItems[indexStr] = value;
            return true;

        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == "DataItems")
            {
                result = this._dateItems;
                return true;
            }

            result = null;
            if (this._dateItems.ContainsKey(binder.Name))
            {
                result = this._dateItems[binder.Name];
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this._dateItems[binder.Name] = value;
            return true;
        }

        internal void SetItems(IDictionary<string, object> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    this._dateItems[item.Key] = item.Value;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return DataItems.GetEnumerator();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace aZaaS.KStar.Helper
{
    public class ListFilterExpress
    {
        public static Expression<Func<T, bool>> GetFilterExpress<T>(List<ListFilter> filter)
        {
            Expression expressfilter = Expression.Constant(true);
            Expression totalExpr = Expression.Constant(true);
            Expression emptyValue = Expression.Constant(null);
            ParameterExpression param = Expression.Parameter(typeof(T), "c");
            if (filter != null)
            {
                //遍历每个property
                foreach (ListFilter p in filter)
                {
                    Expression right = Expression.Constant(p.Value);
                    Expression left = Expression.Property(param, p.Field);
                    Expression emptyExpr = Expression.NotEqual(left, emptyValue);
                    switch (p.Operator)
                    {
                        case ListFilterOperator.eq:
                            expressfilter = Expression.Equal(left, right);
                            break;
                        case ListFilterOperator.neq:
                            expressfilter = Expression.NotEqual(left, right);
                            break;
                        case ListFilterOperator.startswith:
                            expressfilter = Expression.Call(left, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), right);
                            expressfilter = Expression.AndAlso(emptyExpr, expressfilter);
                            break;
                        case ListFilterOperator.notstartswith:
                            expressfilter = Expression.Not(Expression.Call(left, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), right));
                            expressfilter = Expression.AndAlso(emptyExpr, expressfilter);
                            break;
                        case ListFilterOperator.contains:
                            expressfilter = Expression.Call(left, typeof(string).GetMethod("Contains"), right);
                            expressfilter = Expression.AndAlso(emptyExpr, expressfilter);
                            break;
                        case ListFilterOperator.doesnotcontain:
                            expressfilter = Expression.Not(Expression.Call(left, typeof(string).GetMethod("Contains"), right));
                            expressfilter = Expression.AndAlso(emptyExpr, expressfilter);
                            break;
                        case ListFilterOperator.endswith:
                            expressfilter = Expression.Call(left, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), right);
                            expressfilter = Expression.AndAlso(emptyExpr, expressfilter);
                            break;
                        case ListFilterOperator.notendswith:
                            expressfilter = Expression.Not(Expression.Call(left, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), right));
                            expressfilter = Expression.AndAlso(emptyExpr, expressfilter);
                            break;
                    }
                    if (p.logic == LinkLogic.And)
                    {
                        totalExpr = Expression.And(expressfilter, totalExpr);
                    }
                    else
                    {
                        totalExpr = Expression.Or(expressfilter, totalExpr);
                    }
                }
            }
            Expression<Func<T, bool>> express = Expression.Lambda<Func<T, bool>>(totalExpr, param);            
            return express;
        }

    }

    public class ListFilter
    {
        public string Field { get; set; }
        public ListFilterOperator Operator { get; set; }
        public object Value { get; set; }
        public LinkLogic logic { get; set; }
    }

    public enum ListFilterOperator
    { 
        eq,
        neq,
        startswith,
        notstartswith,
        contains,
        doesnotcontain,
        endswith,
        notendswith
    }

    public enum LinkLogic
    {
        And,
        Or
    }
}

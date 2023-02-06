using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public static class ExpressionUtil
    {
        /// <summary>
        /// 动态生成表达式
        /// </summary>
        /// <param name="prostr">属性字段(CreateTime)</param>
        /// <param name="value">属性值(startTime)</param>
        /// <param name="handStr">操作符eg: < <= > >=   </param>
        /// <returns>eg形式: p=>p.prostr 操作符 value 构成(p=>p.CreateTime>startTime形式 此处操作符是<)</returns>
        public static Expression<Func<T, bool>> GetDynamicEx<T>(string prostr, string value, string handStr)
        {
            //var testdate = DateTime.Now;
            ConstantExpression ctscontext = Expression.Constant(typeof(T));
            // 定义Customer p
            ParameterExpression parameterExpr = Expression.Parameter(typeof(T), "p");
            // 定义Customer  p.CreateTime
            MemberExpression time = Expression.Property(parameterExpr, prostr);

            // 定义Customer  p.CreateTime==testdate
            BinaryExpression timequal = null;
            //Expression.Equal(time, Expression.Constant(value));

            switch (handStr)
            {
                case "<":
                    timequal = Expression.LessThan(time, Expression.Constant(value));
                    break;
                case ">":
                    timequal = Expression.GreaterThan(time, Expression.Constant(value));
                    break;
                case "<=":
                    timequal = Expression.LessThanOrEqual(time, Expression.Constant(value));
                    break;
                case ">=":
                    timequal = Expression.GreaterThanOrEqual(time, Expression.Constant(value));
                    break;
                default:
                    timequal = Expression.Equal(time, Expression.Constant(value));
                    break;
            }

            Expression<Func<T, bool>> lambda1 =
                Expression.Lambda<Func<T, bool>>(
                    timequal,
                    new ParameterExpression[] { parameterExpr });

            return lambda1;
        }

        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dynamo.ORM.Constants
{
    internal class ExpressionValues
    {
        internal static readonly IDictionary<string, string> Comparitives = new Dictionary<string, string>
        {
            { "LessThanOrEqual", "<=" },
            { "GreaterThanOrEqual", ">=" },
            { "AndAlso", "AND" },
            { "OrElse", "OR" },
            { "GreaterThan", ">" },
            { "NotEqual", "!=" },
            { "LessThan", "<" },
            { "Equal", "=" }
        };

        internal static readonly IDictionary<Type, Func<Expression, object>> GetExpressionValues = new Dictionary<Type, Func<Expression, object>>
        {
            { typeof(int), (expression) => Expression.Lambda<Func<int>>(expression).Compile()() },
            { typeof(int?), (expression) => Expression.Lambda<Func<int?>>(expression).Compile()() },
            { typeof(bool), (expression) => Expression.Lambda<Func<bool>>(expression).Compile()() },
            { typeof(bool?), (expression) => Expression.Lambda<Func<bool?>>(expression).Compile()() },
            { typeof(object), (expression) => Expression.Lambda<Func<object>>(expression).Compile()() },
            { typeof(DateTime), (expression) => Expression.Lambda<Func<DateTime>>(expression).Compile()() },
            { typeof(DateTime?), (expression) => Expression.Lambda<Func<DateTime?>>(expression).Compile()() }
        };
    }
}

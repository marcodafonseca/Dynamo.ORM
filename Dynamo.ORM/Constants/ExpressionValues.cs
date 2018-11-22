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
            { "NotEqual", "<>" },
            { "LessThan", "<" },
            { "Equal", "=" }
        };

        internal static readonly IDictionary<Type, Func<Expression, object>> GetExpressionValues = new Dictionary<Type, Func<Expression, object>>
        {
            { typeof(bool), (expression) => Expression.Lambda<Func<bool>>(expression).Compile()() },
            { typeof(bool?), (expression) => Expression.Lambda<Func<bool?>>(expression).Compile()() },
            { typeof(byte), (expression) => Expression.Lambda<Func<byte>>(expression).Compile()() },
            { typeof(byte?), (expression) => Expression.Lambda<Func<byte?>>(expression).Compile()() },
            { typeof(byte[]), (expression) => Expression.Lambda<Func<byte[]>>(expression).Compile()() },
            { typeof(char), (expression) => Expression.Lambda<Func<char>>(expression).Compile()() },
            { typeof(char?), (expression) => Expression.Lambda<Func<char?>>(expression).Compile()() },
            { typeof(DateTime), (expression) => Expression.Lambda<Func<DateTime>>(expression).Compile()() },
            { typeof(DateTime?), (expression) => Expression.Lambda<Func<DateTime?>>(expression).Compile()() },
            { typeof(decimal), (expression) => Expression.Lambda<Func<decimal>>(expression).Compile()() },
            { typeof(decimal?), (expression) => Expression.Lambda<Func<decimal?>>(expression).Compile()() },
            { typeof(double), (expression) => Expression.Lambda<Func<double>>(expression).Compile()() },
            { typeof(double?), (expression) => Expression.Lambda<Func<double?>>(expression).Compile()() },
            { typeof(float), (expression) => Expression.Lambda<Func<float>>(expression).Compile()() },
            { typeof(float?), (expression) => Expression.Lambda<Func<float?>>(expression).Compile()() },
            { typeof(short), (expression) => Expression.Lambda<Func<short>>(expression).Compile()() },
            { typeof(short?), (expression) => Expression.Lambda<Func<short?>>(expression).Compile()() },
            { typeof(int), (expression) => Expression.Lambda<Func<int>>(expression).Compile()() },
            { typeof(int?), (expression) => Expression.Lambda<Func<int?>>(expression).Compile()() },
            { typeof(long), (expression) => Expression.Lambda<Func<long>>(expression).Compile()() },
            { typeof(long?), (expression) => Expression.Lambda<Func<long?>>(expression).Compile()() },
            { typeof(ushort), (expression) => Expression.Lambda<Func<ushort>>(expression).Compile()() },
            { typeof(ushort?), (expression) => Expression.Lambda<Func<ushort?>>(expression).Compile()() },
            { typeof(uint), (expression) => Expression.Lambda<Func<uint>>(expression).Compile()() },
            { typeof(uint?), (expression) => Expression.Lambda<Func<uint?>>(expression).Compile()() },
            { typeof(ulong), (expression) => Expression.Lambda<Func<ulong>>(expression).Compile()() },
            { typeof(ulong?), (expression) => Expression.Lambda<Func<ulong?>>(expression).Compile()() },

            { typeof(object), (expression) => Expression.Lambda<Func<object>>(expression).Compile()() },
        };
    }
}

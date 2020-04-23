using System;
using System.Collections.Generic;

namespace Dynamo.ORM.Converters
{
    public static class ValueConverters
    {
        public static IDictionary<Type, Func<object, object>> StringConverter = new Dictionary<Type, Func<object, object>>
        {
            { typeof(string), (object @object) => $"{@object}" },
            { typeof(decimal), (object @object) => decimal.Parse($"{@object}") },
            { typeof(double), (object @object) => double.Parse($"{@object}") },
            { typeof(float), (object @object) => float.Parse($"{@object}") },
            { typeof(short), (object @object) => short.Parse($"{@object}") },
            { typeof(int), (object @object) => int.Parse($"{@object}") },
            { typeof(long), (object @object) => long.Parse($"{@object}") },
            { typeof(ushort), (object @object) => ushort.Parse($"{@object}") },
            { typeof(uint), (object @object) => uint.Parse($"{@object}") },
            { typeof(ulong), (object @object) => ulong.Parse($"{@object}") },
            { typeof(decimal?), (object @object) => GetNullableValue(@object, () => decimal.Parse($"{@object}")) },
            { typeof(double?), (object @object) => GetNullableValue(@object, () => double.Parse($"{@object}")) },
            { typeof(float?), (object @object) => GetNullableValue(@object, () => float.Parse($"{@object}")) },
            { typeof(short?), (object @object) => GetNullableValue(@object, () => short.Parse($"{@object}")) },
            { typeof(int?), (object @object) => GetNullableValue(@object, () => int.Parse($"{@object}")) },
            { typeof(long?), (object @object) => GetNullableValue(@object, () => long.Parse($"{@object}")) },
            { typeof(ushort?), (object @object) => GetNullableValue(@object, () => ushort.Parse($"{@object}")) },
            { typeof(uint?), (object @object) => GetNullableValue(@object, () => uint.Parse($"{@object}")) },
            { typeof(ulong?), (object @object) => GetNullableValue(@object, () => ulong.Parse($"{@object}")) },
        };

        private static object GetNullableValue(object @object, Func<object> nonNullFunction)
        {
            if (@object != null)
                return nonNullFunction();
            return null;
        }
    }
}
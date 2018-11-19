using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.UnitTests
{
    internal static class BaseExtensions
    {
        internal static void PopulateProperties<T>(this T entity) where T : class, new()
        {
            var type = entity.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.SetMethod.IsPublic)
                    property.SetValue(entity, populate[property.PropertyType]);
            }
        }

        internal static void UpdateProperties<T>(this T entity) where T : class, new()
        {
            var type = entity.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.SetMethod.IsPublic)
                    property.SetValue(entity, update[property.PropertyType]);
            }
        }

        internal static bool IsEqual<T>(this T entity, T value) where T : class, new()
        {
            var result = true;

            var type = entity.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (!property.GetValue(entity).Equals(property.GetValue(value)))
                    result = false;
            }

            return result;
        }

        private static readonly IDictionary<Type, object> populate = new Dictionary<Type, object>
        {
            { typeof(string), "TEST" },
            { typeof(int), 100 },
            { typeof(int?), 100 },
            { typeof(decimal), 100.01 },
            { typeof(decimal?), 100.01 },
            { typeof(double), 100.01 },
            { typeof(double?), 100.01 },
            { typeof(bool), true },
            { typeof(bool?), true },
            { typeof(DateTime), new DateTime(2018, 11, 16) },
            { typeof(DateTime?), new DateTime(2018, 11, 16) },
        };

        private static readonly IDictionary<Type, object> update = new Dictionary<Type, object>
        {
            { typeof(string), "UPDATE" },
            { typeof(int), 100 },
            { typeof(int?), 200 },
            { typeof(decimal), 200.01 },
            { typeof(decimal?), 200.01 },
            { typeof(double), 200.01 },
            { typeof(double?), 200.01 },
            { typeof(bool), false },
            { typeof(bool?), false },
            { typeof(DateTime), new DateTime(2018, 11, 20) },
            { typeof(DateTime?), new DateTime(2018, 11, 20) },
        };
    }
}
using Dynamo.ORM.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
                if (property.SetMethod?.IsPublic ?? false)
                {
                    if (populate.ContainsKey(property.PropertyType))
                    {
                        var defaultValue = (DefaultValueAttribute)property.GetCustomAttributes(typeof(DefaultValueAttribute), true).FirstOrDefault();
                        if (defaultValue != null)
                            property.SetValue(entity, defaultValue.Value);
                        else
                            property.SetValue(entity, populate[property.PropertyType]);
                    }
                    else
                    {
                        var value = property.PropertyType.CreateInstance();
                        value.PopulateProperties();
                        property.SetValue(entity, value);
                    }
                }
            }
        }

        internal static void UpdateProperties<T>(this T entity) where T : class, new()
        {
            var type = entity.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.SetMethod.IsPublic)
                {
                    if (populate.ContainsKey(property.PropertyType))
                        property.SetValue(entity, update[property.PropertyType]);
                    else
                    {
                        var value = property.PropertyType.CreateInstance();
                        value.UpdateProperties();
                        property.SetValue(entity, value);
                    }
                }
            }
        }

        internal static bool IsEqual<T>(this T entity, T value) where T : class, new()
        {
            var result = true;

            var type = entity.GetType();

            if (type.IsValueType || type == typeof(string))
                return entity.Equals(value);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var entityProperty = property.GetValue(entity);
                var valueProperty = property.GetValue(value);

                if (!string.IsNullOrWhiteSpace($"{entityProperty}{valueProperty}"))
                    if (property.PropertyType.IsArray ||
                        (property.PropertyType.IsGenericType && property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable))))
                    {
                        if (!IsEqualArray(entityProperty as ICollection, valueProperty as ICollection))
                            result = false;
                    }
                    else if (!populate.ContainsKey(property.PropertyType))
                    {
                        if (!IsEqual(entityProperty, valueProperty))
                            result = false;
                    }
                    else if (!Equals(entityProperty, valueProperty))
                        result = false;
            }

            return result;
        }

        private static bool IsEqualArray<T>(T entityProperty, T valueProperty) where T : ICollection
        {
            var result = true;

            if (entityProperty?.Count != valueProperty?.Count)
                result = false;
            else
            {
                var entityEnumerable = entityProperty.GetEnumerator();
                var valueEnumerable = entityProperty.GetEnumerator();

                for (int i = 0; i < entityProperty.Count; i++)
                {
                    entityEnumerable.MoveNext();
                    valueEnumerable.MoveNext();

                    if (!IsEqual(entityEnumerable.Current, valueEnumerable.Current))
                        result = false;
                }
            }

            return result;
        }

        private static readonly IDictionary<Type, object> populate = new Dictionary<Type, object>
        {
            { typeof(string), "TEST" },
            { typeof(int), 100 },
            { typeof(int?), null },
            { typeof(decimal), 100.01m },
            { typeof(decimal?), null },
            { typeof(double), 100.01 },
            { typeof(double?), null },
            { typeof(bool), true },
            { typeof(bool?), null },
            { typeof(byte), (byte)0x0 },
            { typeof(byte?), null },
            { typeof(byte[]), new byte[]{ 128 } },
            { typeof(char), 'A' },
            { typeof(char?), null },
            { typeof(DateTime), new DateTime(2018, 11, 16) },
            { typeof(DateTime?), null },
            { typeof(float), 100.01f },
            { typeof(float?), null },
            { typeof(short), (short)100 },
            { typeof(short?), null },
            { typeof(long), (long)100 },
            { typeof(long?), null },
            { typeof(ushort), (ushort)100 },
            { typeof(ushort?), null },
            { typeof(uint), (uint)100 },
            { typeof(uint?), null },
            { typeof(ulong), (ulong)100 },
            { typeof(ulong?), null },
            { typeof(Guid), Guid.NewGuid() },
            { typeof(Guid?), null },
            { typeof(string[]), new string[]{ "test" } },
            { typeof(IList<string>), new List<string>{ "string" } },
            { typeof(IList<Services.ChildModel>), new List<Services.ChildModel>
                {
                    new Services.ChildModel{
                        Property1 = "test"
                    }
                }
            },
            { typeof(int[]), new int[]{ 12 } },
            { typeof(IList<int>), new List<int>{ 12 } },
        };

        private static readonly IDictionary<Type, object> update = new Dictionary<Type, object>
        {
            { typeof(string), "UPDATE" },
            { typeof(int), 100 },
            { typeof(int?), 200 },
            { typeof(decimal), 200.01m },
            { typeof(decimal?), 200.01m },
            { typeof(double), 200.01 },
            { typeof(double?), 200.01 },
            { typeof(bool), false },
            { typeof(bool?), false },
            { typeof(byte), (byte)0x1 },
            { typeof(byte?), (byte)0x0 },
            { typeof(char), 'A' },
            { typeof(char?), 'B' },
            { typeof(byte[]), new byte[]{ 0x2, 0x1 } },
            { typeof(DateTime), new DateTime(2018, 11, 20) },
            { typeof(DateTime?), new DateTime(2018, 11, 20) },
            { typeof(float), 100.01f },
            { typeof(float?), 200.02f },
            { typeof(short), (short)100 },
            { typeof(short?), (short)200 },
            { typeof(long), (long)100 },
            { typeof(long?), (long)100 },
            { typeof(ushort), (ushort)100 },
            { typeof(ushort?), (ushort)100 },
            { typeof(uint), (uint)100 },
            { typeof(uint?), (uint?)100 },
            { typeof(ulong), (ulong)100 },
            { typeof(ulong?), (ulong)100 },
            { typeof(Guid), Guid.NewGuid() },
            { typeof(Guid?), null },
            { typeof(string[]), new string[]{ "test", "test2" } },
            { typeof(IList<string>), new List<string>{ "test", "test2" } },
            { typeof(IList<Services.ChildModel>), new List<Services.ChildModel>
                {
                    new Services.ChildModel{
                        Property1 = "test 2"
                    }
                }
            },
            { typeof(int[]), new int[]{ 34 } },
            { typeof(IList<int>), new List<int>{ 34 } },
        };
    }
}
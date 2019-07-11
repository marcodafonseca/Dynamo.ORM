using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Dynamo.ORM.Constants
{
    internal class AttributeValueConverter
    {
        internal static IDictionary<Type, Func<object, AttributeValue>> ConvertToAttributeValue = new Dictionary<Type, Func<object, AttributeValue>>
        {
            { typeof(bool), (object @object) => new AttributeValue { BOOL = bool.Parse($"{@object}")} },
            { typeof(bool?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.BOOL = bool.Parse($"{@object}")) },
            { typeof(byte), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(byte?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(byte[]), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.B = new MemoryStream((byte[])@object), () => @object != null && ((byte[])@object).Length > 0) },
            { typeof(char), (object @object) => new AttributeValue{N = "" + Convert.ToInt32(@object) } },
            { typeof(char?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = "" + Convert.ToInt32(@object)) },
            { typeof(DateTime), (object @object) => new AttributeValue { S = ((DateTime)@object).ToString(Formats.DateFormat) } },
            { typeof(DateTime?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.S = ((DateTime)@object).ToString(Formats.DateFormat)) },
            { typeof(decimal), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(decimal?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(double), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(double?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(float), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(float?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(short), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(short?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(int), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(int?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(long), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(long?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(string), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.S = $"{@object}", () => !string.IsNullOrWhiteSpace($"{@object}")) },
            { typeof(ushort), (object @object) => new AttributeValue{ N = $"{@object}" } },
            { typeof(ushort?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(uint), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(uint?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(ulong), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(ulong?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.N = $"{@object}") },
            { typeof(object), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.M = ToDictionary(@object)) },
            { typeof(Guid), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.S = @object.ToString()) },
            { typeof(Guid?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.S = @object.ToString()) }
        };

        internal static IDictionary<Type, Func<AttributeValue, object>> ConvertToValue = new Dictionary<Type, Func<AttributeValue, object>>
        {
            { typeof(bool), (AttributeValue attributeValue) => attributeValue.BOOL },
            { typeof(bool?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => attributeValue.BOOL) },
            { typeof(byte), (AttributeValue attributeValue) => byte.Parse(attributeValue.N) },
            { typeof(byte?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => (byte?)byte.Parse(attributeValue.N)) },
            { typeof(byte[]), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => attributeValue.B.ToArray()) },
            { typeof(char), (AttributeValue attributeValue) => (char)Convert.ToInt32(attributeValue.N) },
            { typeof(char?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => (char?)(char)Convert.ToInt32(attributeValue.N)) },
            { typeof(DateTime), (AttributeValue attributeValue) => DateTime.ParseExact(attributeValue.S, Formats.DateFormat, CultureInfo.InvariantCulture)},
            { typeof(DateTime?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => DateTime.ParseExact(attributeValue.S, Formats.DateFormat, CultureInfo.InvariantCulture)) },
            { typeof(decimal), (AttributeValue attributeValue) => decimal.Parse(attributeValue.N) },
            { typeof(decimal?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => decimal.Parse(attributeValue.N)) },
            { typeof(double), (AttributeValue attributeValue) => double.Parse(attributeValue.N) },
            { typeof(double?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => double.Parse(attributeValue.N)) },
            { typeof(float), (AttributeValue attributeValue) => float.Parse(attributeValue.N) },
            { typeof(float?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => float.Parse(attributeValue.N)) },
            { typeof(short), (AttributeValue attributeValue) => short.Parse(attributeValue.N) },
            { typeof(short?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => short.Parse(attributeValue.N)) },
            { typeof(int), (AttributeValue attributeValue) => int.Parse(attributeValue.N) },
            { typeof(int?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => (int?)int.Parse(attributeValue.N)) },
            { typeof(long), (AttributeValue attributeValue) => long.Parse(attributeValue.N) },
            { typeof(long?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => (long?)long.Parse(attributeValue.N)) },
            { typeof(string), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => attributeValue.S) },
            { typeof(ushort), (AttributeValue attributeValue) => ushort.Parse(attributeValue.N) },
            { typeof(ushort?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => ushort.Parse(attributeValue.N)) },
            { typeof(uint), (AttributeValue attributeValue) => uint.Parse(attributeValue.N) },
            { typeof(uint?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => (uint?)uint.Parse(attributeValue.N)) },
            { typeof(ulong), (AttributeValue attributeValue) => ulong.Parse(attributeValue.N) },
            { typeof(ulong?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => (ulong?)ulong.Parse(attributeValue.N)) },
            { typeof(object), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => attributeValue.M) },
            { typeof(Guid), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => new Guid(attributeValue.S)) },
            { typeof(Guid?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => new Guid(attributeValue.S)) }
        };

        internal static object FromDictionary(Type type, Dictionary<string, AttributeValue> value)
        {
            var result = Activator.CreateInstance(type);

            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                if (property.SetMethod.IsPublic && value.ContainsKey(property.Name))
                {
                    if (ConvertToValue.ContainsKey(property.PropertyType))
                    {
                        property.SetValue(result, ConvertToValue[property.PropertyType](value[property.Name]));
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        property.SetValue(result, FromDictionary(property.PropertyType, value[property.Name].M));
                    }
                }
            }

            return result;
        }

        internal static Dictionary<string, AttributeValue> ToDictionary(object @object)
        {
            var type = @object.GetType();
            var properties = type.GetProperties();
            var dictionary = new Dictionary<string, AttributeValue>();

            foreach (var property in properties)
            {
                if (property.SetMethod.IsPublic)
                {
                    if (ConvertToAttributeValue.ContainsKey(property.PropertyType))
                    {
                        dictionary.Add(property.Name, ConvertToAttributeValue[property.PropertyType](property.GetValue(@object)));
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        dictionary.Add(property.Name, new AttributeValue { M = ToDictionary(property.GetValue(@object)) });
                    }
                }
            }

            return dictionary;
        }

        private static AttributeValue GetNullableAttributeValue(object @object, Func<AttributeValue, object> nonNullFunction, Func<bool> validator = null)
        {
            var attributeValue = new AttributeValue();

            if (validator == null)
                validator = () => @object != null;

            if (validator())
                nonNullFunction(attributeValue);
            else
                attributeValue.NULL = true;

            return attributeValue;
        }

        private static object GetNullableValue(AttributeValue attributeValue, Func<object> nonNullFunction)
        {
            if (!attributeValue.NULL)
                return nonNullFunction();
            return null;
        }
    }
}
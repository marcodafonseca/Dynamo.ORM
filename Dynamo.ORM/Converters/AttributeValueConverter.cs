using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Constants;
using Dynamo.ORM.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Dynamo.ORM.Converters
{
    public static class AttributeValueConverter
    {
        public static IDictionary<Type, Func<AttributeValue, object>> ConvertToValue = new Dictionary<Type, Func<AttributeValue, object>>
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
            { typeof(Guid?), (AttributeValue attributeValue) => GetNullableValue(attributeValue, () => new Guid(attributeValue.S)) },
        };

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
            { typeof(Guid?), (object @object) => GetNullableAttributeValue(@object, (AttributeValue attributeValue) => attributeValue.S = @object.ToString()) },
        };

        public static object ConvertAttributeValue(AttributeValue value)
        {
            if (value.B != null)
                return ConvertToValue[typeof(byte[])](value);
            else if (value.IsBOOLSet)
                return ConvertToValue[typeof(bool)](value);
            else if (value.IsLSet)
            {
                var list = new List<object>();

                foreach (var item in value.L)
                    list.Add(ConvertAttributeValue(item));

                return list;
            }
            else if (value.IsMSet)
                return CreateDictionaryObject(value.M);
            else if (!string.IsNullOrWhiteSpace(value.N))
                return ConvertToValue[typeof(int)](value);
            else if (value.NS.Count > 0)
            {
                var list = new List<long>();

                foreach (var item in value.NS)
                    list.Add(long.Parse(item));

                return list;
            }
            else if (!string.IsNullOrWhiteSpace(value.S))
                return value.S;
            else if (value.SS.Count > 0)
            {
                var list = new List<string>();

                foreach (var item in value.SS)
                    list.Add(item);

                return list;
            }

            return null;
        }

        internal static IList<string> ConvertToArrayValue(Type type, AttributeValue attributeValue)
        {
            if (attributeValue.NULL)
                return null;
            else if (ListAttributeValueConverter.StringTypes.Contains(type))
                return attributeValue.SS;
            else if (ListAttributeValueConverter.AllNumberTypes.Contains(type))
                return attributeValue.NS;
            else
                throw new InvalidCastException("Type not supported: " + type);
        }

        internal static object ConvertToDictionary(Type propertyType, Dictionary<string, AttributeValue> propertyValues)
        {
            var dictionary = (IDictionary)BaseExtensions.CreateInstance(propertyType);

            foreach (var propertyValue in propertyValues)
                dictionary.Add(propertyValue.Key, ConvertAttributeValue(propertyValue.Value));

            return dictionary;
        }

        internal static object FromDictionary(Type type, Dictionary<string, AttributeValue> value)
        {
            object result;

            if (type.IsArray)
                result = type.CreateInstance(value.Count);
            else if (type == typeof(object))
                result = CreateDictionaryObject(value);
            else
                result = type.CreateInstance();

            var properties = type.GetProperties();

            if (value == null)
                result = null;
            else if (value.Count > 0 && properties.Length > 0)
                foreach (var property in properties)
                {
                    var propertyType = property.PropertyType;

                    if (property.SetMethod.IsPublic && value.ContainsKey(property.Name))
                    {
                        if (ConvertToValue.ContainsKey(propertyType))
                        {
                            property.SetValue(result, ConvertToValue[propertyType](value[property.Name]));
                        }
                        else if (propertyType.IsArray || (propertyType.IsGenericType && propertyType.GetInterfaces().Contains(typeof(IEnumerable))))
                        {
                            var typeArgument = propertyType.GetDeclaringType();

                            if (ListAttributeValueConverter.StringTypes.Contains(typeArgument))
                                property.SetValue(result, FromList(propertyType, value[property.Name].SS));
                            else if (ListAttributeValueConverter.AllNumberTypes.Contains(typeArgument))
                                property.SetValue(result, FromList(propertyType, value[property.Name].NS));
                            else
                                property.SetValue(result, FromList(propertyType, value[property.Name].L));
                        }
                        else if (propertyType.IsClass)
                        {
                            property.SetValue(result, FromDictionary(propertyType, value[property.Name].M));
                        }
                    }
                }

            return result;
        }

        internal static object FromList(Type propertyType, IList<string> values)
        {
            var typeArgument = propertyType.GetDeclaringType();

            if (values != null)
                if (propertyType.IsArray)
                {
                    var array = propertyType.CreateInstance(values.Count);

                    for (var i = 0; i < values.Count; i++)
                        array.SetValue(ValueConverters.StringConverter[typeArgument](values[i]), i);

                    return array;
                }
                else if (propertyType.IsGenericType)
                {
                    var list = (IList)propertyType.CreateInstance();

                    foreach (var value in values)
                        list.Add(ValueConverters.StringConverter[typeArgument](value));

                    return list;
                }

            return null;
        }

        internal static object FromList(Type propertyType, IList<AttributeValue> values)
        {
            var typeArgument = propertyType.GetDeclaringType();

            if (propertyType.IsArray)
            {
                var array = typeArgument.CreateInstance(values.Count);

                for (var i = 0; i < values.Count; i++)
                    array.SetValue(ConvertToValue[typeArgument](values[i]), i);

                return array;
            }
            else if (propertyType.IsGenericType)
            {
                IList list = (IList)propertyType.CreateInstance();

                foreach (var value in values)
                {
                    var attributeValue = (Dictionary<string, AttributeValue>)ConvertToValue[typeof(object)](value);

                    list.Add(FromDictionary(typeArgument, attributeValue));
                }

                return list;
            }

            return null;
        }

        internal static Dictionary<string, AttributeValue> ToDictionary(object @object)
        {
            var type = @object.GetType();
            var properties = type.GetProperties();
            var dictionary = new Dictionary<string, AttributeValue>();

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;

                if (property.SetMethod?.IsPublic == true)
                {
                    if (ConvertToAttributeValue.ContainsKey(propertyType))
                    {
                        dictionary.Add(property.Name, ConvertToAttributeValue[propertyType](property.GetValue(@object)));
                    }
                    else if (propertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        var elementType = propertyType.GetDeclaringType();

                        var propertyValue = ((IEnumerable)property.GetValue(@object));

                        dictionary.Add(property.Name, ListAttributeValueConverter.ConvertToAttributeValue(elementType, propertyValue?.GetEnumerator()));
                    }
                    else if (propertyType.IsClass)
                    {
                        dictionary.Add(property.Name, new AttributeValue { M = ToDictionary(property.GetValue(@object)) });
                    }
                }
            }

            return dictionary;
        }

        private static object CreateDictionaryObject(Dictionary<string, AttributeValue> values)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var value in values)
                dictionary.Add(value.Key, ConvertAttributeValue(value.Value));

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
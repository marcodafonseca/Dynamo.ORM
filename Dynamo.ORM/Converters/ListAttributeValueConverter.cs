using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dynamo.ORM.Converters
{
    internal static class ListAttributeValueConverter
    {
        public static Type[] AllTypes
        {
            get
            {
                return StringTypes
                    .Concat(NumberTypes)
                    .Concat(NullableNumberTypes)
                    .ToArray();
            }
        }

        public static Type[] AllNumberTypes
        {
            get
            {
                return NumberTypes
                    .Concat(NullableNumberTypes)
                    .ToArray();
            }
        }

        public static Type[] StringTypes = {
            typeof(string)
        };

        public static Type[] NumberTypes = {
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(ushort),
            typeof(uint),
            typeof(ulong),
        };

        public static Type[] NullableNumberTypes = {
            typeof(decimal?),
            typeof(double?),
            typeof(float?),
            typeof(short?),
            typeof(int?),
            typeof(long?),
            typeof(ushort?),
            typeof(uint?),
            typeof(ulong?),
        };

        internal static AttributeValue ConvertToAttributeValue(Type type, IEnumerator value)
        {
            var stringValues = new List<string>();
            var attributeValues = new List<AttributeValue>();

            if (value != null)
            {
                if (AllTypes.Contains(type))
                    while (value.MoveNext())
                        stringValues.Add($"{value.Current}");
                else if (type.IsClass)
                    while (value.MoveNext())
                        attributeValues.Add(AttributeValueConverter.ConvertToAttributeValue[typeof(object)](value.Current));
                else
                    throw new InvalidCastException("Type not supported: " + type.Name);

                if (stringValues.Count > 0)
                {
                    if (StringTypes.Contains(type))
                        return new AttributeValue
                        {
                            SS = stringValues
                        };
                    else if (AllNumberTypes.Contains(type))
                        return new AttributeValue
                        {
                            NS = stringValues
                        };
                }
                else if (attributeValues.Count > 0)
                    return new AttributeValue
                    {
                        L = attributeValues
                    };
            }

            return new AttributeValue
            {
                NULL = true
            };
        }
    }
}
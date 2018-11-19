using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Dynamo.ORM.Constants
{
    internal class AttributeValueConverter
    {
        internal static IDictionary<Type, Func<object, AttributeValue>> ConvertToAttributeValue = new Dictionary<Type, Func<object, AttributeValue>>
        {
            { typeof(string), (object @object) => new AttributeValue($"{@object}") },
            { typeof(int), (object @object) => new AttributeValue { N = $"{@object}" } },
            { typeof(int?), (object @object) => new AttributeValue { N = @object == null ? null : $"{@object}" } },
            { typeof(bool), (object @object) => new AttributeValue { BOOL = bool.Parse($"{@object}")} },
            { typeof(bool?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.BOOL = bool.Parse($"{@object}");
                    return attributeValue;
                }
            },
            { typeof(DateTime), (object @object) => new AttributeValue { S = ((DateTime)@object).ToString(Formats.DateFormat) } },
            { typeof(DateTime?), (object @object) =>
                {
                    var attributeValue = new AttributeValue();
                    if (@object != null)
                        attributeValue.S = ((DateTime)@object).ToString(Formats.DateFormat);
                    return attributeValue;
                }
            }
        };

        internal static IDictionary<Type, Func<AttributeValue, object>> ConvertToValue = new Dictionary<Type, Func<AttributeValue, object>>
        {
            { typeof(string), (AttributeValue attributeValue) => attributeValue.S },
            { typeof(int), (AttributeValue attributeValue) => int.Parse(attributeValue.N) },
            { typeof(int?), (AttributeValue attributeValue) =>
                {
                    if (attributeValue.N != null)
                        return int.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(bool), (AttributeValue attributeValue) => attributeValue.BOOL },
            { typeof(bool?), (AttributeValue attributeValue) =>
                {
                    if (attributeValue.IsBOOLSet)
                        return int.Parse(attributeValue.N);
                    return null;
                }
            },
            { typeof(DateTime), (AttributeValue attributeValue) => DateTime.ParseExact(attributeValue.S, Formats.DateFormat, CultureInfo.InvariantCulture)},
            { typeof(DateTime?), (AttributeValue attributeValue) =>
                {
                    if (!string.IsNullOrWhiteSpace(attributeValue.S))
                        return DateTime.ParseExact(attributeValue.S, Formats.DateFormat, CultureInfo.InvariantCulture);
                    return null;
                }
            },
        };
    }
}

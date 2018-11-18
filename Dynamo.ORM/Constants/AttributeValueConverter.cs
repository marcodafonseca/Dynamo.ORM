using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dynamo.ORM.Constants
{
    internal class AttributeValueConverter
    {
        internal static IDictionary<Type, Func<object, AttributeValue>> ConvertToAttributeValue = new Dictionary<Type, Func<object, AttributeValue>>
        {
            { typeof(string), (object obj) => new AttributeValue($"{obj}") },
            { typeof(int), (object obj) => new AttributeValue { N = $"{obj}" } },
            { typeof(bool), (object obj) => new AttributeValue { BOOL = bool.Parse($"{obj}")} },
        };

        internal static IDictionary<Type, Func<AttributeValue, object>> ConvertToValue = new Dictionary<Type, Func<AttributeValue, object>>
        {
            { typeof(string), (AttributeValue attributeValue) => attributeValue.S },
            { typeof(int), (AttributeValue attributeValue) => int.Parse(attributeValue.N) },
            { typeof(bool), (AttributeValue attributeValue) => attributeValue.BOOL }
        };
    }
}

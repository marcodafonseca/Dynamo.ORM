using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;

namespace Dynamo.ORM.Extensions
{
    internal static class KeyValuePairExtensions
    {
        internal static KeyValuePair<string, AttributeValueUpdate> MapToAttributeValueUpdate(this KeyValuePair<string, AttributeValue> keyValuePair) =>
            new KeyValuePair<string, AttributeValueUpdate>(keyValuePair.Key, new AttributeValueUpdate(keyValuePair.Value, AttributeAction.PUT));
    }
}
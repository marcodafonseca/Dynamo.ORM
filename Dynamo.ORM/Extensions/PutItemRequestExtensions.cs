using Amazon.DynamoDBv2.Model;

namespace Dynamo.ORM.Extensions
{
    internal static class PutItemRequestExtensions
    {
        internal static Put Map(this PutItemRequest request)
        {
            if (request == null)
                return new Put();

            return new Put
            {
                ConditionExpression = request.ConditionExpression,
                ExpressionAttributeNames = request.ExpressionAttributeNames,
                ExpressionAttributeValues = request.ExpressionAttributeValues,
                Item = request.Item,
                TableName = request.TableName
            };
        }
    }
}

using Amazon.DynamoDBv2.Model;

namespace Dynamo.ORM.Extensions
{
    internal static class DeleteItemRequestExtensions
    {
        internal static Delete Map(this DeleteItemRequest request)
        {
            if (request == null)
                return new Delete();

            return new Delete
            {
                ConditionExpression = request.ConditionExpression,
                ExpressionAttributeNames = request.ExpressionAttributeNames,
                ExpressionAttributeValues = request.ExpressionAttributeValues,
                Key = request.Key,
                TableName = request.TableName
            };
        }
    }
}

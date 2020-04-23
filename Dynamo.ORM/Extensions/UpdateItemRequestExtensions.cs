using Amazon.DynamoDBv2.Model;

namespace Dynamo.ORM.Extensions
{
    internal static class UpdateItemRequestExtensions
    {
        internal static Update Map(this UpdateItemRequest request)
        {
            if (request == null)
                return new Update();

            return new Update
            {
                ConditionExpression = request.ConditionExpression,
                ExpressionAttributeNames = request.ExpressionAttributeNames,
                ExpressionAttributeValues = request.ExpressionAttributeValues,
                Key = request.Key,
                TableName = request.TableName,
                UpdateExpression = request.UpdateExpression
            };
        }
    }
}
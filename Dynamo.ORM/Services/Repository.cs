using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Models;
using Dynamo.ORM.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using Dynamo.ORM.Exceptions;
using Dynamo.ORM.Constants;

namespace Dynamo.ORM.Services
{
    public class Repository : IRepository
    {
        private readonly IAmazonDynamoDB amazonDynamoDB;

        public Repository(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task Add<T>(T entity) where T : Base, new()
        {
            var request = new PutItemRequest
            {
                TableName = entity.GetTableName(),
                Item = entity.Map(true)
            };

            await amazonDynamoDB.PutItemAsync(request);
        }

        public async Task Delete<T>(T entity) where T : Base, new()
        {
            var request = new DeleteItemRequest
            {
                TableName = entity.GetTableName(),
                Key = entity.GetKey()
            };

            await amazonDynamoDB.DeleteItemAsync(request);
        }
        
        public async Task Delete<T>(object partitionKey, object sortKey = null) where T : Base, new()
        {
            var generic = new T();
            var key = generic.GetKey(partitionKey, sortKey);

            var request = new DeleteItemRequest
            {
                TableName = generic.GetTableName(),
                Key = key
            };

            await amazonDynamoDB.DeleteItemAsync(request);
        }

        public async Task<T> Get<T>(object partitionKey, object sortKey = null) where T : Base, new()
        {
            var generic = new T();
            var expressionAttributeNames = generic.GetExpressionAttributes();
            var key = generic.GetKey(partitionKey, sortKey);

            if (partitionKey == null)
                throw new RepositoryException("No partitionKey provided");

            var request = new GetItemRequest
            {
                TableName = generic.GetTableName(),
                Key = key,
                ProjectionExpression = string.Join(", ", expressionAttributeNames.Keys),
                ExpressionAttributeNames = expressionAttributeNames
            };

            var response = (await amazonDynamoDB.GetItemAsync(request)).Item;

            if (response.Count == 0)
                return null;

            return response.Map<T>();
        }

        public async Task<T> Get<T>(Expression<Func<T, bool>> expression) where T : Base, new()
        {
            var generic = new T();

            var tableName = generic.GetTableName();
            var expressionAttributeNames = generic.GetExpressionAttributes();

            var dynamoDbRequest = new ScanRequest
            {
                TableName = tableName,
                ProjectionExpression = string.Join(", ", expressionAttributeNames.Keys),
                ExpressionAttributeNames = expressionAttributeNames
            };
            
            var expressionString = new StringBuilder();

            dynamoDbRequest.ExpressionAttributeValues = Converters.ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            dynamoDbRequest.FilterExpression = expressionString.ToString();

            var results = (await amazonDynamoDB.ScanAsync(dynamoDbRequest)).Items;

            if (results.Count > 1)
                throw new RepositoryException($"Too many items returned by query: ({results.Count})");
            else if (results.Count == 0)
                return null;

            return results.SingleOrDefault()?.Map<T>();
        }

        public async Task<List<T>> List<T>(Expression<Func<T, bool>> expression = null) where T : Base, new()
        {
            var generic = new T();

            var tableName = generic.GetTableName();
            var expressionAttributeNames = generic.GetExpressionAttributes();

            var dynamoDbRequest = new ScanRequest
            {
                TableName = tableName,
                ProjectionExpression = string.Join(", ", expressionAttributeNames.Keys),
                ExpressionAttributeNames = expressionAttributeNames
            };

            if (expression != null)
            {
                var expressionString = new StringBuilder();

                dynamoDbRequest.ExpressionAttributeValues = Converters.ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

                dynamoDbRequest.FilterExpression = expressionString.ToString();
            }

            return (await amazonDynamoDB.ScanAsync(dynamoDbRequest))
                .Items
                .Select(x => x.Map<T>())
                .ToList();
        }

        public async Task Update<T>(T entity) where T : Base, new()
        {
            var request = new UpdateItemRequest
            {
                TableName = entity.GetTableName(),
                AttributeUpdates = entity
                    .Map()
                    .Select(x => x.MapToAttributeValueUpdate())
                    .ToDictionary(x => x.Key, x => x.Value),
                Key = entity.GetKey()
            };

            await amazonDynamoDB.UpdateItemAsync(request);
        }
    }
}
﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Exceptions;
using Dynamo.ORM.Extensions;
using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dynamo.ORM.Services
{
    public class Repository : IRepository
    {
        private readonly IAmazonDynamoDB amazonDynamoDB;

        private List<TransactWriteItem> writeActions;

        public Repository(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task Add<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var request = new PutItemRequest
            {
                TableName = string.IsNullOrWhiteSpace(tableName) ? entity.GetTableName() : tableName,
                Item = entity.Map(true)
            };

            if (writeActions == null)
                await amazonDynamoDB.PutItemAsync(request, cancellationToken: cancellationToken);
            else
                writeActions.Add(new TransactWriteItem
                {
                    Put = request.Map()
                });
        }

        public void BeginWriteTransaction()
        {
            writeActions = new List<TransactWriteItem>();
        }

        public async Task CommitWriteTransaction(CancellationToken cancellationToken = default)
        {
            var request = new TransactWriteItemsRequest
            {
                TransactItems = writeActions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            var response = await amazonDynamoDB.TransactWriteItemsAsync(request, cancellationToken: cancellationToken);
        }

        public async Task Delete<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var request = new DeleteItemRequest
            {
                TableName = string.IsNullOrWhiteSpace(tableName) ? entity.GetTableName() : tableName,
                Key = entity.GetKey()
            };

            if (writeActions == null)
                await amazonDynamoDB.DeleteItemAsync(request, cancellationToken: cancellationToken);
            else
                writeActions.Add(new TransactWriteItem
                {
                    Delete = request.Map()
                });
        }

        public async Task Delete<T>(object partitionKey, object sortKey = null, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var generic = new T();
            var key = generic.GetKey(partitionKey, sortKey);

            var request = new DeleteItemRequest
            {
                TableName = string.IsNullOrWhiteSpace(tableName) ? generic.GetTableName() : tableName,
                Key = key
            };

            if (writeActions == null)
                await amazonDynamoDB.DeleteItemAsync(request, cancellationToken: cancellationToken);
            else
                writeActions.Add(new TransactWriteItem
                {
                    Delete = request.Map()
                });
        }

        public async Task<T> Get<T>(object partitionKey, object sortKey = null, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var generic = new T();
            var expressionAttributeNames = generic.GetExpressionAttributes();
            var key = generic.GetKey(partitionKey, sortKey);

            if (partitionKey == null)
                throw new RepositoryException("No partitionKey provided");

            var request = new GetItemRequest
            {
                TableName = string.IsNullOrWhiteSpace(tableName) ? generic.GetTableName() : tableName,
                Key = key,
                ProjectionExpression = string.Join(", ", expressionAttributeNames.Keys),
                ExpressionAttributeNames = expressionAttributeNames
            };

            var response = (await amazonDynamoDB.GetItemAsync(request, cancellationToken: cancellationToken)).Item;

            if (response.Count == 0)
                return null;

            return response.Map<T>();
        }

        public async Task<T> Get<T>(Expression<Func<T, bool>> expression, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var generic = new T();

            tableName = string.IsNullOrWhiteSpace(tableName) ? generic.GetTableName() : tableName;
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

        public async Task<long> GetTableSize<T>(string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var generic = new T();

            tableName = string.IsNullOrWhiteSpace(tableName) ? generic.GetTableName() : tableName;

            var itemCount = (await amazonDynamoDB.DescribeTableAsync(tableName)).Table.ItemCount;

            return itemCount;
        }

        public async Task<IList<T>> List<T>(Expression<Func<T, bool>> expression = null, string tableName = null, int page = 1, int pageSize = int.MaxValue, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var generic = new T();

            tableName = string.IsNullOrWhiteSpace(tableName) ? generic.GetTableName() : tableName;
            var expressionAttributeNames = generic.GetExpressionAttributes();

            var dynamoDbRequest = new ScanRequest
            {
                TableName = tableName,
                ProjectionExpression = string.Join(", ", expressionAttributeNames.Keys),
                ExpressionAttributeNames = expressionAttributeNames,
                Limit = pageSize,
            };

            if (expression != null)
            {
                var expressionString = new StringBuilder();

                dynamoDbRequest.ExpressionAttributeValues = Converters.ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

                dynamoDbRequest.FilterExpression = expressionString.ToString();
            }

            var paginators = amazonDynamoDB.Paginators.Scan(dynamoDbRequest);

            var pagingEnumerator = paginators.Responses.GetAsyncEnumerator(cancellationToken);

            var currentPageIndex = 0;

            while (currentPageIndex < page)
            {
                await pagingEnumerator.MoveNextAsync();
                currentPageIndex++;
            }

            if (pagingEnumerator.Current == null)
            {
                throw new PageNotFoundException($"Page {page} doesn't exist");
            }

            return pagingEnumerator.Current
                .Items
                .Select(x => x.Map<T>())
                .ToList();
        }

        public void RollbackWriteTransaction()
        {
            writeActions = null;
        }

        public async Task Update<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var request = new UpdateItemRequest
            {
                TableName = string.IsNullOrWhiteSpace(tableName) ? entity.GetTableName() : tableName,
                AttributeUpdates = entity
                    .Map()
                    .Select(x => x.MapToAttributeValueUpdate())
                    .ToDictionary(x => x.Key, x => x.Value),
                Key = entity.GetKey()
            };

            if (writeActions == null)
                await amazonDynamoDB.UpdateItemAsync(request, cancellationToken: cancellationToken);
            else
                writeActions.Add(new TransactWriteItem
                {
                    Update = request.Map()
                });
        }
    }
}
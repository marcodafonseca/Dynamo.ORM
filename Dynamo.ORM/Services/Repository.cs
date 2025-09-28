using Amazon.DynamoDBv2;
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
    /// <summary>
    /// Provides a repository implementation for interacting with Amazon DynamoDB, supporting CRUD operations,
    /// transactional writes, and query capabilities. This class is designed to work with entities derived from the <see
    /// cref="Base"/> class.
    /// </summary>
    /// <remarks>The <see cref="Repository"/> class facilitates interaction with Amazon DynamoDB by
    /// abstracting common operations such as adding, updating, deleting, and retrieving items. It also supports
    /// transactional write operations through the use of DynamoDB's TransactWriteItems API. This class is intended to
    /// be used with entities that implement the <see cref="Base"/> class, which provides necessary metadata for mapping
    /// to DynamoDB tables.  The repository supports both single operations and batched transactional operations. To
    /// perform transactional writes, use <see cref="BeginWriteTransaction"/> to start a transaction, followed by
    /// multiple write operations (e.g., <see cref="Add{T}"/>, <see cref="Update{T}"/>, <see cref="Delete{T}(T, string,
    /// CancellationToken)"/>), and then call <see cref="CommitWriteTransaction"/> to commit the transaction.
    /// Transactions can be rolled back using <see cref="RollbackWriteTransaction"/>.  This class is thread-safe for
    /// non-transactional operations. However, transactional operations (e.g., <see cref="BeginWriteTransaction"/> and
    /// subsequent writes) are not thread-safe and should be used in a single-threaded context.</remarks>
    public class Repository : IRepository
    {
        private readonly IAmazonDynamoDB amazonDynamoDB;

        private List<TransactWriteItem> writeActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository"/> class using the specified Amazon DynamoDB client.
        /// </summary>
        /// <param name="amazonDynamoDB">The Amazon DynamoDB client used to interact with the DynamoDB service. Cannot be null.</param>
        public Repository(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void BeginWriteTransaction()
        {
            writeActions = new List<TransactWriteItem>();
        }

        /// <inheritdoc />
        public async Task CommitWriteTransaction(CancellationToken cancellationToken = default)
        {
            if (writeActions == null || writeActions.Count == 0)
                return;

            var request = new TransactWriteItemsRequest
            {
                TransactItems = writeActions,
                ReturnConsumedCapacity = ReturnConsumedCapacity.TOTAL
            };

            var response = await amazonDynamoDB.TransactWriteItemsAsync(request, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

            var response = await amazonDynamoDB.GetItemAsync(request, cancellationToken: cancellationToken);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new RepositoryException($"Could not get item with partitionKey {partitionKey} and sortKey {sortKey}");

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK && (response.Item == null || response.Item.Count == 0))
                return null;

            return response.Item.Map<T>();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<long> GetTableSize<T>(string tableName = null, CancellationToken cancellationToken = default) where T : Base, new()
        {
            var generic = new T();

            tableName = string.IsNullOrWhiteSpace(tableName) ? generic.GetTableName() : tableName;

            var response = await amazonDynamoDB.DescribeTableAsync(tableName);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new RepositoryException($"Could not describe table {tableName}");

            if (!response.Table.ItemCount.HasValue)
                throw new RepositoryException($"Could not get item count for table {tableName}");

            return response.Table.ItemCount.Value;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void RollbackWriteTransaction()
        {
            writeActions = null;
        }

        /// <inheritdoc />
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
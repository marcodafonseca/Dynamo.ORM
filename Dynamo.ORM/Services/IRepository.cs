using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dynamo.ORM.Services
{
    /// <summary>
    /// Defines a generic repository interface for performing CRUD operations and managing transactions on entities that
    /// inherit from <see cref="Base"/>.
    /// </summary>
    /// <remarks>This interface provides methods for adding, retrieving, updating, and deleting entities, as
    /// well as managing transactions and querying data. It supports asynchronous operations and allows for
    /// customization through optional parameters such as table names and filter expressions.</remarks>
    public interface IRepository
    {
        /// <summary>
        /// Adds the specified entity to the data store.
        /// </summary>
        /// <remarks>The method adds the entity to the specified table or a default table if no table name
        /// is provided.  Ensure that the entity meets any constraints required by the data store.</remarks>
        /// <typeparam name="T">The type of the entity to add. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
        /// <param name="entity">The entity to add. Cannot be <see langword="null"/>.</param>
        /// <param name="tableName">The name of the table to which the entity will be added. If <see langword="null"/>, a default table name is
        /// used based on the entity type.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task Add<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Begins a new write transaction.
        /// </summary>
        /// <remarks>This method initiates a transaction that allows multiple write operations to be
        /// performed as a single atomic unit.  Ensure that the transaction is committed or rolled back to finalize or
        /// discard changes, respectively.</remarks>
        void BeginWriteTransaction();

        /// <summary>
        /// Commits the current write transaction, ensuring that all changes are persisted.
        /// </summary>
        /// <remarks>This method should be called after making changes within a write transaction to
        /// ensure that the changes are saved. If the operation is canceled via the <paramref
        /// name="cancellationToken"/>, the transaction will not be committed.</remarks>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous operation. The task completes when the transaction is successfully
        /// committed.</returns>
        Task CommitWriteTransaction(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an item from the specified table in the database.
        /// </summary>
        /// <remarks>If the specified item does not exist in the table, the operation completes without
        /// throwing an exception.</remarks>
        /// <typeparam name="T">The type of the item to delete. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
        /// <param name="partitionKey">The partition key of the item to delete. This value is required and cannot be null.</param>
        /// <param name="sortKey">The sort key of the item to delete. This value is optional and can be null if the table does not use a sort
        /// key.</param>
        /// <param name="tableName">The name of the table from which the item will be deleted. If null, the default table name for the type
        /// <typeparamref name="T"/> will be used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task Delete<T>(object partitionKey, object sortKey = null, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Deletes the specified entity from the database.
        /// </summary>
        /// <remarks>This method performs the delete operation asynchronously. Ensure that the entity
        /// exists in the database before calling this method to avoid unexpected behavior.</remarks>
        /// <typeparam name="T">The type of the entity to delete. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
        /// <param name="entity">The entity to delete. Cannot be <see langword="null"/>.</param>
        /// <param name="tableName">The name of the database table from which the entity will be deleted. If <see langword="null"/>, the default
        /// table name for the entity type is used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        Task Delete<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Retrieves an item from the specified table based on the provided partition key and optional sort key.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
        /// <param name="partitionKey">The partition key of the item to retrieve. This key is required to locate the item in the table.</param>
        /// <param name="sortKey">The optional sort key of the item to retrieve. If the table uses a composite key, this parameter specifies
        /// the sort key.</param>
        /// <param name="tableName">The name of the table to query. If null, the default table associated with the type <typeparamref name="T"/>
        /// is used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation can be canceled by passing a cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved item of type
        /// <typeparamref name="T"/>,  or <see langword="null"/> if no item matching the specified keys is found.</returns>
        Task<T> Get<T>(object partitionKey, object sortKey = null, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Retrieves a single entity of type <typeparamref name="T"/> that matches the specified condition.
        /// </summary>
        /// <remarks>This method performs an asynchronous query to retrieve a single entity based on the
        /// provided condition. If multiple entities match the condition, the behavior depends on the underlying data
        /// source.</remarks>
        /// <typeparam name="T">The type of the entity to retrieve. Must inherit from <see cref="Base"/> and have a parameterless
        /// constructor.</typeparam>
        /// <param name="expression">A LINQ expression that defines the condition to match the entity.</param>
        /// <param name="tableName">The name of the table to query. If <see langword="null"/>, the default table for the entity type
        /// <typeparamref name="T"/> is used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
        /// name="T"/> that matches the specified condition, or <see langword="null"/> if no matching entity is found.</returns>
        Task<T> Get<T>(Expression<Func<T, bool>> expression, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Gets the approximated item count for the table
        /// https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_TableDescription.html
        /// </summary>
        /// <typeparam name="T">The type of the table entity, which must derive from <see cref="Base"/> and have a parameterless
        /// constructor.</typeparam>
        /// <param name="tableName">The name of the table to retrieve the size for. If <see langword="null"/>, the default table name for the
        /// entity type <typeparamref name="T"/> is used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The operation will be canceled if the token is triggered.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the approximate item count of the table.</returns>
        Task<long> GetTableSize<T>(string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Retrieves a paginated list of items from the specified table that match the given filter criteria.
        /// </summary>
        /// <remarks>This method supports filtering, pagination, and querying a specific table. If no
        /// filter is provided, all items are retrieved. The operation is asynchronous and can be canceled using the
        /// provided <paramref name="cancellationToken"/>.</remarks>
        /// <typeparam name="T">The type of the items to retrieve. Must inherit from <see cref="Base"/> and have a parameterless
        /// constructor.</typeparam>
        /// <param name="expression">An optional filter expression to apply to the items. If null, all items are retrieved.</param>
        /// <param name="tableName">The name of the table to query. If null, the default table for the type <typeparamref name="T"/> is used.</param>
        /// <param name="page">The page number to retrieve. Must be 1 or greater. Defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Must be 1 or greater. Defaults to <see cref="int.MaxValue"/>.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of items of type
        /// <typeparamref name="T"/> that match the specified criteria.</returns>
        Task<IList<T>> List<T>(Expression<Func<T, bool>> expression = null, string tableName = null, int page = 1, int pageSize = int.MaxValue, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Rolls back the current write transaction, discarding any changes made during the transaction.
        /// </summary>
        /// <remarks>This method should be called to revert a write transaction if an error occurs or if
        /// the changes  made during the transaction are no longer needed. After calling this method, the transaction
        /// is considered complete and cannot be reused.</remarks>
        void RollbackWriteTransaction();

        /// <summary>
        /// Updates the specified entity in the database.
        /// </summary>
        /// <remarks>This method performs an update operation for the specified entity in the database. If
        /// the entity does not exist in the database, the behavior depends on the implementation.</remarks>
        /// <typeparam name="T">The type of the entity to update. Must inherit from <see cref="Base"/> and have a parameterless constructor.</typeparam>
        /// <param name="entity">The entity to update. The entity must not be <see langword="null"/> and must contain valid data for the
        /// update operation.</param>
        /// <param name="tableName">The name of the database table where the entity resides. If <see langword="null"/>, the default table name
        /// for the entity type is used.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        Task Update<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();
    }
}
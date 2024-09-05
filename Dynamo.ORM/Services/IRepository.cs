using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dynamo.ORM.Services
{
    public interface IRepository
    {
        Task Add<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        void BeginWriteTransaction();

        Task CommitWriteTransaction(CancellationToken cancellationToken = default);

        Task Delete<T>(object partitionKey, object sortKey = null, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        Task Delete<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        Task<T> Get<T>(object partitionKey, object sortKey = null, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        Task<T> Get<T>(Expression<Func<T, bool>> expression, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        /// <summary>
        /// Gets the approximated item count for the table
        /// https://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_TableDescription.html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">The table to get item count for</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The item count for the table as a long</returns>
        Task<long> GetTableSize<T>(string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();

        [Obsolete("Method is deprecated in favor of overload containing the ListOptions parameter.")]
        Task<IList<T>> List<T>(Expression<Func<T, bool>> expression = null, string tableName = null, int page = 1, int pageSize = int.MaxValue, CancellationToken cancellationToken = default) where T : Base, new();

        Task<IList<T>> List<T>(Expression<Func<T, bool>> expression = null, ListOptions listOptions = null, CancellationToken cancellationToken = default) where T : Base, new();

        void RollbackWriteTransaction();

        Task Update<T>(T entity, string tableName = null, CancellationToken cancellationToken = default) where T : Base, new();
    }
}
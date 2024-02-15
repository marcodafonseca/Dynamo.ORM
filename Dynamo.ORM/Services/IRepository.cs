using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dynamo.ORM.Services
{
    public interface IRepository
    {
        Task Add<T>(T entity, string tableName = null) where T : Base, new();

        void BeginWriteTransaction();

        Task CommitWriteTransaction();

        Task Delete<T>(object partitionKey, object sortKey = null, string tableName = null) where T : Base, new();

        Task Delete<T>(T entity, string tableName = null) where T : Base, new();

        Task<T> Get<T>(object partitionKey, object sortKey = null, string tableName = null) where T : Base, new();

        Task<T> Get<T>(Expression<Func<T, bool>> expression, string tableName = null) where T : Base, new();

        Task<List<T>> List<T>(Expression<Func<T, bool>> expression = null, string tableName = null) where T : Base, new();

        void RollbackWriteTransaction();

        Task Update<T>(T entity, string tableName = null) where T : Base, new();
    }
}
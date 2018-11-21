﻿using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dynamo.ORM.Services
{
    public interface IRepository
    {
        Task<T> Get<T>(object partitionKey, object sortKey = null) where T : Base, new();
        Task<T> Get<T>(Expression<Func<T, bool>> expression) where T : Base, new();
        Task<List<T>> List<T>(Expression<Func<T, bool>> expression = null) where T : Base, new();
        Task Add<T>(T entity) where T : Base, new();
        Task Update<T>(T entity) where T : Base, new();
        Task Delete<T>(object partitionKey, object sortKey = null) where T : Base, new();
        Task Delete<T>(T entity) where T : Base, new();
    }
}

using Dynamo.ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dynamo.ORM.Services
{
    public interface IRepository
    {
        Task<List<T>> List<T>(Expression<Func<T, bool>> expression = null) where T : Base, new();
    }
}

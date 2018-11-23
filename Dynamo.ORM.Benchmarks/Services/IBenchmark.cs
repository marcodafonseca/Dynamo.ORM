using Dynamo.ORM.Benchmarks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dynamo.ORM.Benchmarks.Services
{
    interface IBenchmark
    {
        Task<Result> AddItems(int iteration);
        Task<Result> GetItemsSimple(int iteration);
        Task<Result> GetItemsComplex(int iteration);
        Task<Result> UpdateItems(int iteration);
        Task<Result> DeleteItemsSimple(int iteration);
        Task<Result> DeleteItemsComplex(int iteration);
        Task<Result> ListItemsSimple(int iteration);
        Task<Result> ListItemsComplex(int iteration);
    }
}

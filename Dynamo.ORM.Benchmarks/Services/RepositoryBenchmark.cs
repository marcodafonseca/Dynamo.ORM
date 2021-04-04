using Amazon.DynamoDBv2;
using Dynamo.ORM.Benchmarks.Models;
using Dynamo.ORM.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dynamo.ORM.Benchmarks.Services
{
    internal class RepositoryBenchmark : IBenchmark
    {
        private readonly Repository repository;
        private readonly string repositoryName = "RepositoryBenchmark";

        public RepositoryBenchmark(IAmazonDynamoDB amazonDynamo)
        {
            repository = new Repository(amazonDynamo);
        }

        public async Task<Result> AddItems(int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.Add(model);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Add {iteration} items to table";
            result.Service = repositoryName;
            result.Method = "Add";

            return result;
        }

        public async Task<Result> DeleteItemsComplex(int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.Delete(model);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Delete {iteration} items in table (Complex)";
            result.Service = repositoryName;
            result.Simple = false;
            result.Method = "Delete";

            return result;
        }

        public async Task<Result> DeleteItemsSimple(int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.Delete<Model>(model.Id);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Delete {iteration} items in table (Simple)";
            result.Service = repositoryName;
            result.Simple = true;
            result.Method = "Delete";

            return result;
        }

        public async Task<Result> GetItemsComplex(int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.Get<Model>(x => x.Id == model.Id);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Get {iteration} items (complex)";
            result.Service = repositoryName;
            result.Simple = false;
            result.Method = "Get";

            return result;
        }

        public async Task<Result> GetItemsSimple(int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.Get<Model>(model.Id);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Get {iteration} items (simple)";
            result.Service = repositoryName;
            result.Simple = true;
            result.Method = "Get";

            return result;
        }

        public async Task<Result> ListItemsComplex(int iteration)
        {
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                await repository.List<Model>(x => x.Property1 != null);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"List {iteration} items in table (Complex)";
            result.Service = repositoryName;
            result.Simple = false;
            result.Method = "List";

            return result;
        }

        public async Task<Result> ListItemsSimple(int iteration)
        {
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                await repository.List<Model>();
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"List {iteration} items in table (Simple)";
            result.Service = repositoryName;
            result.Simple = true;
            result.Method = "List";

            return result;
        }

        public async Task<Result> UpdateItems(int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                model.Property2 = "UPDATED";

                await repository.Update(model);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Update {iteration} items in table";
            result.Service = repositoryName;
            result.Method = "Update";

            return result;
        }
    }
}
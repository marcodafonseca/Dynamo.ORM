using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CsvHelper;
using Dynamo.ORM.Benchmarks.Models;
using Dynamo.ORM.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dynamo.ORM.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var outputPath = "Output";

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            IEnumerable<Result> results = new List<Result>();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"-- Iteration {i + 1} --");
                results = results.Union(RunBenchmarks().Result);
            }

            results = results
                .OrderBy(x => x.Description)
                .ThenBy(x => x.Count);

            var filePath = $"{outputPath}\\results-{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

            using (var streamWriter = File.AppendText(filePath))
            using (var csvHelper = new CsvWriter(streamWriter))
            {
                csvHelper.WriteRecords(results);
            }

            Console.WriteLine();
            Console.WriteLine("Benchmarking completed");
            Console.WriteLine($"Results written to {filePath}");
            Console.WriteLine();
            Console.WriteLine("Press any key to close application");

            Console.ReadKey();
        }

        private static async Task<IList<Result>> RunBenchmarks()
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000/"
            };
            var client = new AmazonDynamoDBClient(config);
            var repository = new Repository(client);
            var results = new List<Result>();

            #region Make sure testing table exists
            var tables = client.ListTablesAsync().Result;

            if (!tables.TableNames.Contains("Benchmarking"))
                await client.CreateTableAsync("Benchmarking", new List<KeySchemaElement> {
                    new KeySchemaElement("Id", KeyType.HASH)
                }, new List<AttributeDefinition> {
                    new AttributeDefinition("Id", ScalarAttributeType.N)
                }, new ProvisionedThroughput(1, 1));
            #endregion

            var iterations = new[] { 10, 100, 1000 };

            foreach (var iteration in iterations)
            {
                Console.WriteLine($"Set {iteration}");
                results.Add(await AddItems(repository, iteration));
                results.Add(await GetItemsSimple(repository, iteration));
                results.Add(await GetItemsComplex(repository, iteration));
                results.Add(await UpdateItems(repository, iteration));
                results.Add(await ListItemsSimple(repository, iteration));
                results.Add(await ListItemsComplex(repository, iteration));
                results.Add(await DeleteItemsSimple(repository, iteration));
                results.Add(await AddItems(repository, iteration));
                results.Add(await DeleteItemsComplex(repository, iteration));
            }

            #region Delete testing table
            await client.DeleteTableAsync("Benchmarking");
            #endregion

            return results;
        }

        private static async Task<Result> AddItems(Repository repository, int iteration)
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

            return result;
        }

        private static async Task<Result> GetItemsSimple(Repository repository, int iteration)
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

            return result;
        }

        private static async Task<Result> GetItemsComplex(Repository repository, int iteration)
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

            return result;
        }

        private static async Task<Result> UpdateItems(Repository repository, int iteration)
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

            return result;
        }

        private static async Task<Result> DeleteItemsSimple(Repository repository, int iteration)
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

            return result;
        }

        private static async Task<Result> DeleteItemsComplex(Repository repository, int iteration)
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

            return result;
        }

        private static async Task<Result> ListItemsSimple(Repository repository, int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.List<Model>();
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Delete {iteration} items in table (Simple)";

            return result;
        }

        private static async Task<Result> ListItemsComplex(Repository repository, int iteration)
        {
            var model = new Model();
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                model.Id = i;

                await repository.List<Model>(x => x.Property1 != null);
            }

            stopwatch.Stop();

            result.Count = iteration;
            result.TimeTaken = stopwatch.Elapsed;
            result.Description = $"Delete {iteration} items in table (Complex)";

            return result;
        }
    }
}

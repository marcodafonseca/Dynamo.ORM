using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using CsvHelper;
using Dynamo.ORM.Benchmarks.Models;
using Dynamo.ORM.Benchmarks.Services;
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

            IBenchmark[] benchmarks = new IBenchmark[] {
                new AwsSdkBenchmark(client),
                new RepositoryBenchmark(client)
            };

            var iterations = new[] { 10, 100, 1000 };

            foreach (var benchmark in benchmarks)
                foreach (var iteration in iterations)
                {
                    Console.WriteLine($"Set {iteration}");
                    results.Add(await benchmark.AddItems(iteration));
                    results.Add(await benchmark.GetItemsSimple(iteration));
                    results.Add(await benchmark.GetItemsComplex(iteration));
                    results.Add(await benchmark.UpdateItems(iteration));
                    results.Add(await benchmark.ListItemsSimple(iteration));
                    results.Add(await benchmark.ListItemsComplex(iteration));
                    results.Add(await benchmark.DeleteItemsSimple(iteration));
                    results.Add(await benchmark.AddItems(iteration));
                    results.Add(await benchmark.DeleteItemsComplex(iteration));
                }

            #region Delete testing table
            await client.DeleteTableAsync("Benchmarking");
            #endregion

            return results;
        }
    }
}

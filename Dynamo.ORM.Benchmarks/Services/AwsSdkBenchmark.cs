using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Benchmarks.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dynamo.ORM.Benchmarks.Services
{
    public class AwsSdkBenchmark : IBenchmark
    {
        private readonly string repositoryName = "AwsSdkBenchmark";
        private readonly IAmazonDynamoDB amazonDynamoDB;

        public AwsSdkBenchmark(IAmazonDynamoDB amazonDynamoDB)
        {
            this.amazonDynamoDB = amazonDynamoDB;
        }

        public async Task<Result> AddItems(int iteration)
        {
            var request = new PutItemRequest
            {
                TableName = "Benchmarking",
                Item = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = "0" } },
                    { "Property1", new AttributeValue("TEST") },
                    { "Property2", new AttributeValue("TEST") },
                    { "Property3", new AttributeValue("TEST") },
                    { "Property4", new AttributeValue("TEST") },
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                request.Item["Id"] = new AttributeValue { N = $"{i}" };

                await amazonDynamoDB.PutItemAsync(request);
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
            var request = new DeleteItemRequest
            {
                TableName = "Benchmarking",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = "0" } }
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                request.Key["Id"] = new AttributeValue { N = $"{i}" };

                await amazonDynamoDB.DeleteItemAsync(request);
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
            var request = new DeleteItemRequest
            {
                TableName = "Benchmarking",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = "0" } }
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                request.Key["Id"] = new AttributeValue { N = $"{i}" };

                await amazonDynamoDB.DeleteItemAsync(request);
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
            var request = new ScanRequest
            {
                TableName = "Benchmarking",
                ProjectionExpression = "#id, #property1, #property2, #property3, #property4",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#id", "Id" },
                    { "#property1", "Property1" },
                    { "#property2", "Property2" },
                    { "#property3", "Property3" },
                    { "#property4", "Property4" },
                },
                FilterExpression = "#id = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":id", new AttributeValue{ N = "0" } }
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                request.ExpressionAttributeValues[":id"] = new AttributeValue { N = $"{i}" };

                var results = (await amazonDynamoDB.ScanAsync(request)).Items[0];
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
            var request = new GetItemRequest
            {
                TableName = "Benchmarking",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue{ N = "0" } }
                },
                ProjectionExpression = "#id, #property1, #property2, #property3, #property4",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#id", "Id" },
                    { "#property1", "Property1" },
                    { "#property2", "Property2" },
                    { "#property3", "Property3" },
                    { "#property4", "Property4" },
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                request.Key["Id"] = new AttributeValue { N = $"{i}" };

                var results = (await amazonDynamoDB.GetItemAsync(request)).Item;
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
            var request = new ScanRequest
            {
                TableName = "Benchmarking",
                ProjectionExpression = "#id, #property1, #property2, #property3, #property4",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#id", "Id" },
                    { "#property1", "Property1" },
                    { "#property2", "Property2" },
                    { "#property3", "Property3" },
                    { "#property4", "Property4" },
                },
                FilterExpression = "#property1 = :property1",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":property1", new AttributeValue{ NULL = true } }
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                var results = (await amazonDynamoDB.ScanAsync(request)).Items;
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
            var request = new ScanRequest
            {
                TableName = "Benchmarking",
                ProjectionExpression = "#id, #property1, #property2, #property3, #property4",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#id", "Id" },
                    { "#property1", "Property1" },
                    { "#property2", "Property2" },
                    { "#property3", "Property3" },
                    { "#property4", "Property4" },
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                var results = (await amazonDynamoDB.ScanAsync(request)).Items;
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
            var request = new UpdateItemRequest
            {
                TableName = "Benchmarking",
                AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    { "Property1", new AttributeValueUpdate(new AttributeValue("TEST"), AttributeAction.PUT) },
                    { "Property2", new AttributeValueUpdate(new AttributeValue("TEST"), AttributeAction.PUT) },
                    { "Property3", new AttributeValueUpdate(new AttributeValue("TEST"), AttributeAction.PUT) },
                    { "Property4", new AttributeValueUpdate(new AttributeValue("TEST"), AttributeAction.PUT) },
                },
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { N = "0" } },
                }
            };
            var result = new Result();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < iteration; ++i)
            {
                request.Key["Id"] = new AttributeValue { N = $"{i}" };

                request.AttributeUpdates["Property2"] = new AttributeValueUpdate(new AttributeValue("UPDATED"), AttributeAction.PUT);

                await amazonDynamoDB.UpdateItemAsync(request);
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
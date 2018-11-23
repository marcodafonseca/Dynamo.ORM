using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryInlineFunctionQueryTests
    {
        private readonly AmazonDynamoDBClient client;

        public RepositoryInlineFunctionQueryTests()
        {
            var config = new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000/"
            };
            client = new AmazonDynamoDBClient(config);

            var tables = client.ListTablesAsync().Result;

            if (!tables.TableNames.Contains("TESTS"))
                client.CreateTableAsync("TESTS", new List<KeySchemaElement> {
                    new KeySchemaElement("Id", KeyType.HASH)
                }, new List<AttributeDefinition> {
                    new AttributeDefinition("Id", ScalarAttributeType.N)
                }, new ProvisionedThroughput(1, 1))
                .Wait();
        }

        /// <summary>
        /// Test filtering with the ToUpper function
        /// </summary>
        [Fact]
        public async void TestToUpperQueryOnStringParameter_ExpectResults()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property1 == "test".ToUpper());

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Null(entity.Property6);
        }

        /// <summary>
        /// Test filtering with the toLower function
        /// </summary>
        [Fact]
        public async void TestToLowerQueryOnStringParameter_ExpectResults()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            value.Id = 2000;
            value.Property1 = "test";

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property1 == "TEST".ToLower());

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Null(entity.Property6);
        }

        /// <summary>
        /// Test filtering with a newly instantiated array while calling the max value
        /// </summary>
        [Fact]
        public async void TestMaxQueryOnIntParameter_ExpectResults()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            value.Property3 = 400;

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property3 == new int[] { 1, 400, 3 }.Max());

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Null(entity.Property6);
        }
    }
}

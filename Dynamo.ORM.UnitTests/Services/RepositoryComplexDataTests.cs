using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Models;
using Dynamo.ORM.Services;
using System.Collections.Generic;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryComplexDataTests
    {
        private readonly AmazonDynamoDBClient client;

        public RepositoryComplexDataTests()
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

        [Fact]
        public async void TestSavingClassProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var model = new ComplexData();

            model.PopulateProperties();

            model.Id = 1000;

            await repository.Add(model);

            var result = await repository.Get<ComplexData>(model.Id);

            Assert.True(model.IsEqual(result));
        }

        [Fact]
        public async void TestUpdatingClassProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var model = new ComplexData();

            model.PopulateProperties();

            model.Id = 1000;

            await repository.Add(model);

            var newModel = await repository.Get<ComplexData>(model.Id);

            newModel.UpdateProperties();

            newModel.Id = 1000;

            await repository.Update(newModel);

            var result = await repository.Get<ComplexData>(newModel.Id);

            Assert.True(!model.IsEqual(result));
            Assert.True(!model.IsEqual(newModel));
            Assert.True(newModel.IsEqual(result));
        }
    }

    [DynamoDBTable("TESTS")]
    internal class ComplexData : Base
    {
        [DynamoDBHashKey]
        public int Id { get; set; }

        public TestModel TestModel { get; set; }
        public ComplexChildData ChildModel { get; set; }
    }

    internal class ComplexChildData
    {
        public TestModel TestModel { get; set; }
    }
}
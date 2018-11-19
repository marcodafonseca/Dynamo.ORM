using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Dynamo.ORM.Models;
using Dynamo.ORM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryTests
    {
        private readonly AmazonDynamoDBClient client;

        public RepositoryTests()
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
        /// Test repository's "Add" function
        /// Make sure the data coming back from the database has the same values
        /// </summary>
        [Fact]
        public async void TestAdd_ExpectItemInTable()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();

            value.PopulateProperties();

            repository.Add(value);

            var entity = await repository.Get<MockTestTable>(value.Id);

            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Delete" function
        /// Make sure item is no longer in the database
        /// </summary>
        [Fact]
        public async void TestDeleteEntity_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();

            value.PopulateProperties();

            repository.Add(value);

            var entity1 = await repository.Get<MockTestTable>(value.Id);

            repository.Delete(value);

            var entity2 = await repository.Get<MockTestTable>(value.Id);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
            Assert.Null(entity2);
        }

        /// <summary>
        /// Test repository's "Delete" function
        /// Make sure item is no longer in the database
        /// </summary>
        [Fact]
        public async void TestDeleteByPartitionKey_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();

            value.PopulateProperties();

            repository.Add(value);

            var entity1 = await repository.Get<MockTestTable>(value.Id);

            repository.Delete<MockTestTable>(value.Id);

            var entity2 = await repository.Get<MockTestTable>(value.Id);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
            Assert.Null(entity2);
        }

        /// <summary>
        /// Test repository's "Get" function by way of a constant expression
        /// </summary>
        [Fact]
        public async void TestGetByConstantExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();

            value.PopulateProperties();

            repository.Add(value);

            var entity = await repository.Get<MockTestTable>(x => x.Id == 100);

            Assert.NotNull(entity);
            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression
        /// </summary>
        [Fact]
        public async void TestGetByMemberExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();

            value.PopulateProperties();

            repository.Add(value);

            var entity = await repository.Get<MockTestTable>(x => x.Id == value.Id);

            Assert.NotNull(entity);
            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression
        /// </summary>
        [Fact]
        public async void TestGetByComplexExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();
            var dateFilter = new DateTime(2018, 11, 16).ToUniversalTime();

            value.PopulateProperties();

            repository.Add(value);

            var entity1 = await repository.Get<MockTestTable>(x => x.Property1 == "TEST" && x.Property2 == dateFilter && x.Id == 100);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Update" function
        /// Expect item added, updated and returned
        /// </summary>
        [Fact]
        public async void TestUpdateEntity_ExpectUpdatedResultsReturned()
        {
            var repository = new Repository(client);

            var value = new MockTestTable();

            value.PopulateProperties();

            repository.Add(value);

            var entity1 = await repository.Get<MockTestTable>(value.Id);

            value.UpdateProperties();

            repository.Update(value);

            var entity2 = await repository.Get<MockTestTable>(value.Id);

            Assert.False(entity1.IsEqual(value));
            Assert.False(entity1.IsEqual(entity2));
            Assert.True(entity2.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "List" function
        /// Expect same results returned
        /// </summary>
        [Fact]
        public async void TestListEntities_ExpectMultipleResults()
        {
            var repository = new Repository(client);

            var values = new List<MockTestTable>();

            for (int i = 10; i < 20; i++)
            {
                var value = new MockTestTable();

                value.PopulateProperties();

                value.Id = i;

                values.Add(value);
            }

            foreach (var value in values)
                repository.Add(value);

            var results = (await repository.List<MockTestTable>(x => x.Id < 20 && x.Id >= 10))
                .OrderBy(x => x.Id)
                .ToList();

            Assert.Equal(values.Count, results.Count);

            for (int i = 0; i < results.Count; i++)
                Assert.True(values[i].IsEqual(results[i]));
        }
    }

    [DynamoDBTable("TESTS")]
    class MockTestTable : Base
    {
        private DateTime property2;

        [DynamoDBHashKey]
        public int Id { get; set; }
        public string Property1 { get; set; }
        public DateTime Property2
        {
            get
            {
                return property2.ToUniversalTime();
            }
            set
            {
                property2 = value;
            }
        }
        public int Property3 { get; set; }
        public bool Property4 { get; set; }
    }
}

using Amazon.DynamoDBv2;
using Dynamo.ORM.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryTests : IClassFixture<SharedFixture>
    {
        private const string primaryTableName = "TESTS";
        private const string secondaryTableName = "SECOND_TABLE_TESTS";

        private readonly AmazonDynamoDBClient client;
        private readonly SharedFixture sharedFixture;

        public RepositoryTests(SharedFixture sharedFixture)
        {
            client = AmazonDynamoDBClientTestExtensions.InitializeTestDynamoDbClient();
            client.CreateTestTableIfNotExists(primaryTableName).Wait();
            client.CreateTestTableIfNotExists(secondaryTableName).Wait();

            this.sharedFixture = sharedFixture;
        }

        /// <summary>
        /// Test repository's "Add" function
        /// Make sure the data coming back from the database has the same values
        /// </summary>
        [Fact]
        public async Task TestAdd_ExpectItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity = await repository.Get<TestModel>(value.Id);

            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Add" function
        /// Make sure the data coming back from the database has the same values
        /// </summary>
        [Fact]
        public async Task TestAddEntryWithNullProperties_ExpectItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel
            {
                Id = sharedFixture.RandomInt()
            };

            await repository.Add(value);

            var entity = await repository.Get<TestModel>(value.Id);

            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Add"  function with tablename parameter
        /// Make sure the data coming back from the database has the same values
        /// </summary>
        [Fact]
        public async Task TestAddInRunTimeTable_ExpectItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            value.PopulateProperties();

            await repository.Add(value, tableName: runtimeTableName);

            var entity = await repository.Get<TestModel>(value.Id, tableName: runtimeTableName);

            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Delete" function
        /// Make sure item is no longer in the database
        /// </summary>
        [Fact]
        public async Task TestDeleteByPartitionKey_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity1 = await repository.Get<TestModel>(value.Id);

            await repository.Delete<TestModel>(value.Id);

            var entity2 = await repository.Get<TestModel>(value.Id);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
            Assert.Null(entity2);
        }

        /// <summary>
        /// Test repository's "Delete" function
        /// Make sure item is no longer in the database
        /// </summary>
        [Fact]
        public async Task TestDeleteEntity_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity1 = await repository.Get<TestModel>(value.Id);

            await repository.Delete(value);

            var entity2 = await repository.Get<TestModel>(value.Id);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
            Assert.Null(entity2);
        }

        /// <summary>
        /// Test repository's "Delete" function with tablename parameter
        /// Make sure item is no longer in the database
        /// </summary>
        [Fact]
        public async Task TestDeleteEntityWithRunTimeTable_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            value.PopulateProperties();

            await repository.Add(value, tableName: runtimeTableName);

            var entity1 = await repository.Get<TestModel>(value.Id, tableName: runtimeTableName);

            await repository.Delete(value, tableName: runtimeTableName);

            var entity2 = await repository.Get<TestModel>(value.Id, tableName: runtimeTableName);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
            Assert.Null(entity2);
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression
        /// </summary>
        [Fact]
        public async Task TestGetByComplexExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var dateFilter = new DateTime(2018, 11, 16).ToUniversalTime();

            value.PopulateProperties();

            var id = value.Id;

            await repository.Add(value);

            var entity1 = await repository.Get<TestModel>(x => x.Property1 == "TEST" && x.Property2 == dateFilter && x.Id == id);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression wtih tablename parameter
        /// </summary>
        [Fact]
        public async Task TestGetByComplexExpressionWithRunTimeTableName_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            var dateFilter = new DateTime(2018, 11, 16).ToUniversalTime();

            value.PopulateProperties();

            var id = value.Id;

            await repository.Add(value, tableName: runtimeTableName);

            var entity1 = await repository.Get<TestModel>(x => x.Property1 == "TEST" && x.Property2 == dateFilter && x.Id == id, tableName: runtimeTableName);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a constant expression
        /// </summary>
        [Fact]
        public async Task TestGetByConstantExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            var id = value.Id;

            await repository.Add(value);

            var entity = await repository.Get<TestModel>(x => x.Id == id);

            Assert.NotNull(entity);
            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression
        /// </summary>
        [Fact]
        public async Task TestGetByMemberExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity = await repository.Get<TestModel>(x => x.Id == value.Id);

            Assert.NotNull(entity);
            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "List" function
        /// Expect same results returned
        /// </summary>
        [Fact]
        public async Task TestListEntities_ExpectMultipleResults()
        {
            var repository = new Repository(client);

            var values = new List<TestModel>();
            var ids = new List<int>();

            for (int i = 10; i < 20; i++)
            {
                var value = new TestModel();

                value.PopulateProperties();

                value.Id = sharedFixture.RandomInt() + 1000000;

                ids.Add(value.Id);

                values.Add(value);
            }

            foreach (var value in values)
                await repository.Add(value);

            var minId = ids.Min();
            var maxId = ids.Max();

            var results = (await repository.List<TestModel>(x => x.Id <= maxId && x.Id >= minId))
                .OrderBy(x => x.Id)
                .ToList();

            Assert.True(results.Count >= values.Count);
        }

        /// <summary>
        /// Test repository's "List" function with tablename parameter
        /// Expect same results returned
        /// </summary>
        [Fact]
        public async Task TestListEntitiesWithTableName_ExpectMultipleResults()
        {
            var repository = new Repository(client);

            var values = new List<TestModel>();
            var ids = new List<int>();

            for (int i = 20; i < 30; i++)
            {
                var value = new TestModel();

                value.PopulateProperties();

                value.Id = sharedFixture.RandomInt() + 2000000;

                ids.Add(value.Id);

                values.Add(value);
            }

            var runtimeTableName = secondaryTableName;
            foreach (var value in values)
                await repository.Add(value, tableName: runtimeTableName);

            var minId = ids.Min();
            var maxId = ids.Max();

            var results = (await repository.List<TestModel>(x => x.Id <= maxId && x.Id >= minId, tableName: runtimeTableName))
                .OrderBy(x => x.Id)
                .ToList();

            Assert.True(results.Count >= values.Count);
        }

        /// <summary>
        /// Test repository's "Update" function
        /// Expect item added, updated and returned
        /// </summary>
        [Fact]
        public async Task TestUpdateEntity_ExpectUpdatedResultsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity1 = await repository.Get<TestModel>(value.Id);

            value.UpdateProperties();

            await repository.Update(value);

            var entity2 = await repository.Get<TestModel>(value.Id);

            Assert.False(entity1.IsEqual(value));
            Assert.False(entity1.IsEqual(entity2));
            Assert.True(entity2.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Update" function with tablename parmater
        /// Expect item added, updated and returned
        /// </summary>
        [Fact]
        public async Task TestUpdateEntityWithTableName_ExpectUpdatedResultsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            value.PopulateProperties();

            await repository.Add(value, tableName: runtimeTableName);

            var entity1 = await repository.Get<TestModel>(value.Id, tableName: runtimeTableName);

            value.UpdateProperties();

            await repository.Update(value, tableName: runtimeTableName);

            var entity2 = await repository.Get<TestModel>(value.Id, tableName: runtimeTableName);

            Assert.False(entity1.IsEqual(value));
            Assert.False(entity1.IsEqual(entity2));
            Assert.True(entity2.IsEqual(value));
        }
    }
}
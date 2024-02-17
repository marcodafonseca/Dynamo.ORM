using Amazon.DynamoDBv2;
using Dynamo.ORM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryTests
    {
        private const string primaryTableName = "TESTS";
        private const string secondaryTableName = "SECOND_TABLE_TESTS";
        private readonly AmazonDynamoDBClient client;

        public RepositoryTests()
        {
            client = AmazonDynamoDBClientTestExtensions.InitializeTestDynamoDbClient();
            client.CreateTestTableIfNotExists(primaryTableName).Wait();
            client.CreateTestTableIfNotExists(secondaryTableName).Wait();
        }

        /// <summary>
        /// Test repository's "Add" function
        /// Make sure the data coming back from the database has the same values
        /// </summary>
        [Fact]
        public async void TestAdd_ExpectItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity = await repository.Get<TestModel>(value.Id);

            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Add"  function with tablename parameter
        /// Make sure the data coming back from the database has the same values
        /// </summary>
        [Fact]
        public async void TestAddInRunTimeTable_ExpectItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            value.PopulateProperties();

            await repository.Add(value, runtimeTableName);

            var entity = await repository.Get<TestModel>(value.Id, runtimeTableName);

            Assert.True(entity.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Delete" function
        /// Make sure item is no longer in the database
        /// </summary>
        [Fact]
        public async void TestDeleteByPartitionKey_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            // Just added this line so other tests don't interfere with it when running them all together
            value.Id = 3001;

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
        public async void TestDeleteEntity_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            // Just added this line so other tests don't interfere with it when running them all together
            value.Id = 3000;

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
        public async void TestDeleteEntityWithRunTimeTable_ExpectNoItemInTable()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            value.PopulateProperties();

            // Just added this line so other tests don't interfere with it when running them all together
            value.Id = 3000;

            await repository.Add(value, runtimeTableName);

            var entity1 = await repository.Get<TestModel>(value.Id, runtimeTableName);

            await repository.Delete(value, runtimeTableName);

            var entity2 = await repository.Get<TestModel>(value.Id, runtimeTableName);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
            Assert.Null(entity2);
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression
        /// </summary>
        [Fact]
        public async void TestGetByComplexExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var dateFilter = new DateTime(2018, 11, 16).ToUniversalTime();

            value.PopulateProperties();

            await repository.Add(value);

            var entity1 = await repository.Get<TestModel>(x => x.Property1 == "TEST" && x.Property2 == dateFilter && x.Id == 100);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a member expression wtih tablename parameter
        /// </summary>
        [Fact]
        public async void TestGetByComplexExpressionWithRunTimeTableName_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            var dateFilter = new DateTime(2018, 11, 16).ToUniversalTime();

            value.PopulateProperties();

            await repository.Add(value, runtimeTableName);

            var entity1 = await repository.Get<TestModel>(x => x.Property1 == "TEST" && x.Property2 == dateFilter && x.Id == 100, runtimeTableName);

            Assert.NotNull(entity1);
            Assert.True(entity1.IsEqual(value));
        }

        /// <summary>
        /// Test repository's "Get" function by way of a constant expression
        /// </summary>
        [Fact]
        public async void TestGetByConstantExpression_ExpectSameValuesReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entity = await repository.Get<TestModel>(x => x.Id == 100);

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
        public async void TestListEntities_ExpectMultipleResults()
        {
            var repository = new Repository(client);

            var values = new List<TestModel>();

            for (int i = 10; i < 20; i++)
            {
                var value = new TestModel();

                value.PopulateProperties();

                value.Id = i;

                values.Add(value);
            }

            foreach (var value in values)
                await repository.Add(value);

            var results = (await repository.List<TestModel>(x => x.Id < 20 && x.Id >= 10))
                .OrderBy(x => x.Id)
                .ToList();

            Assert.Equal(values.Count, results.Count);

            for (int i = 0; i < results.Count; i++)
                Assert.True(values[i].IsEqual(results[i]));
        }

        /// <summary>
        /// Test repository's "List" function with tablename parameter
        /// Expect same results returned
        /// </summary>
        [Fact]
        public async void TestListEntitiesWithTableName_ExpectMultipleResults()
        {
            var repository = new Repository(client);

            var values = new List<TestModel>();

            for (int i = 10; i < 20; i++)
            {
                var value = new TestModel();

                value.PopulateProperties();

                value.Id = i;

                values.Add(value);
            }

            var runtimeTableName = secondaryTableName;
            foreach (var value in values)
                await repository.Add(value, runtimeTableName);

            var results = (await repository.List<TestModel>(x => x.Id < 20 && x.Id >= 10, runtimeTableName))
                .OrderBy(x => x.Id)
                .ToList();

            Assert.Equal(values.Count, results.Count);

            for (int i = 0; i < results.Count; i++)
                Assert.True(values[i].IsEqual(results[i]));
        }

        /// <summary>
        /// Test repository's "Update" function
        /// Expect item added, updated and returned
        /// </summary>
        [Fact]
        public async void TestUpdateEntity_ExpectUpdatedResultsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            // Just added this line so other tests don't interfere with it when running them all together
            value.Id = 3003;

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
        public async void TestUpdateEntityWithTableName_ExpectUpdatedResultsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();
            var runtimeTableName = secondaryTableName;
            value.PopulateProperties();

            // Just added this line so other tests don't interfere with it when running them all together
            value.Id = 3003;

            await repository.Add(value, runtimeTableName);

            var entity1 = await repository.Get<TestModel>(value.Id, runtimeTableName);

            value.UpdateProperties();

            await repository.Update(value, runtimeTableName);

            var entity2 = await repository.Get<TestModel>(value.Id, runtimeTableName);

            Assert.False(entity1.IsEqual(value));
            Assert.False(entity1.IsEqual(entity2));
            Assert.True(entity2.IsEqual(value));
        }
    }
}
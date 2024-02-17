using Amazon.DynamoDBv2;
using Dynamo.ORM.Services;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryTypeQueriesTests
    {
        private readonly AmazonDynamoDBClient client;

        public RepositoryTypeQueriesTests()
        {
            client = AmazonDynamoDBClientTestExtensions.InitializeTestDynamoDbClient();
            client.CreateTestTableIfNotExists("TESTS").Wait();
        }

        /// <summary>
        /// Test Repository's "List" function using a boolean property
        /// </summary>
        [Fact]
        public async void TestBoolQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property4 == value.Property4);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property4, entity.Property4);
        }

        /// <summary>
        /// Test Repository's "List" function using a byte[] property
        /// </summary>
        [Fact]
        public async void TestByteArrayQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property7 == value.Property7);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property7, entity.Property7);
        }

        /// <summary>
        /// Test Repository's "List" function using a byte property
        /// </summary>
        [Fact]
        public async void TestByteQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property5 == value.Property5);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property5, entity.Property5);
        }

        /// <summary>
        /// Test Repository's "List" function using a char property
        /// </summary>
        [Fact]
        public async void TestCharQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property9 == value.Property9);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property9, entity.Property9);
        }

        /// <summary>
        /// Test Repository's "List" function using an DateTimeproperty
        /// </summary>
        [Fact]
        public async void TestDateTimeQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property2 == value.Property2);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property2, entity.Property2);
        }

        /// <summary>
        /// Test Repository's "List" function using a decimal property
        /// </summary>
        [Fact]
        public async void TestDecimalQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property12 == value.Property12);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property12, entity.Property12);
        }

        /// <summary>
        /// Test Repository's "List" function using a double property
        /// </summary>
        [Fact]
        public async void TestDoubleQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property14 == value.Property14);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property14, entity.Property14);
        }

        /// <summary>
        /// Test Repository's "List" function using a float property
        /// </summary>
        [Fact]
        public async void TestFloatQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property16 == value.Property16);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property16, entity.Property16);
        }

        /// <summary>
        /// Test Repository's "List" function using an Integer property
        /// </summary>
        [Fact]
        public async void TestIntegerQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property3 == value.Property3);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property3, entity.Property3);
        }

        /// <summary>
        /// Test Repository's "List" function using a long property
        /// </summary>
        [Fact]
        public async void TestLongQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property20 == value.Property20);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property20, entity.Property20);
        }

        /// <summary>
        /// Test Repository's "List" function using a bool? property
        /// </summary>
        [Fact]
        public async void TestNullableBoolQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property8 == value.Property8);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property8, entity.Property8);
        }

        /// <summary>
        /// Test Repository's "List" function using a byte? property
        /// </summary>
        [Fact]
        public async void TestNullableByteQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property6 == value.Property6);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property6, entity.Property6);
        }

        /// <summary>
        /// Test Repository's "List" function using a char? property
        /// </summary>
        [Fact]
        public async void TestNullableCharQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property10 == value.Property10);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property10, entity.Property10);
        }

        /// <summary>
        /// Test Repository's "List" function using a DateTime? property
        /// </summary>
        [Fact]
        public async void TestNullableDateTimeQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property11 == value.Property11);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property11, entity.Property11);
        }

        /// <summary>
        /// Test Repository's "List" function using a decimal? property
        /// </summary>
        [Fact]
        public async void TestNullableDecimalQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property13 == value.Property13);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property13, entity.Property13);
        }

        /// <summary>
        /// Test Repository's "List" function using a double? property
        /// </summary>
        [Fact]
        public async void TestNullableDoubleQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property15 == value.Property15);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property15, entity.Property15);
        }

        /// <summary>
        /// Test Repository's "List" function using a float? property
        /// </summary>
        [Fact]
        public async void TestNullableFloatQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property17 == value.Property17);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property15, entity.Property15);
        }

        /// <summary>
        /// Test Repository's "List" function using a long? property
        /// </summary>
        [Fact]
        public async void TestNullableLongQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property21 == value.Property21);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property21, entity.Property21);
        }

        /// <summary>
        /// Test Repository's "List" function using a short? property
        /// </summary>
        [Fact]
        public async void TestNullableShortQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property19 == value.Property19);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property19, entity.Property19);
        }

        /// <summary>
        /// Test Repository's "List" function using a uint? property
        /// </summary>
        [Fact]
        public async void TestNullableUIntQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property27 == value.Property27);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property27, entity.Property27);
        }

        /// <summary>
        /// Test Repository's "List" function using a ulong? property
        /// </summary>
        [Fact]
        public async void TestNullableULongQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property25 == value.Property25);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property25, entity.Property25);
        }

        /// <summary>
        /// Test Repository's "List" function using a ushort? property
        /// </summary>
        [Fact]
        public async void TestNullableUShortQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property23 == value.Property23);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property23, entity.Property23);
        }

        /// <summary>
        /// Test filtering by a nullable field
        /// </summary>
        [Fact]
        public async void TestQueryOnNullValue_ExpectItemReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property6 == null);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Null(entity.Property6);
        }

        /// <summary>
        /// Test Repository's "List" function using a short property
        /// </summary>
        [Fact]
        public async void TestShortQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property18 == value.Property18);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property18, entity.Property18);
        }

        /// <summary>
        /// Test Repository's "List" function using an String property
        /// </summary>
        [Fact]
        public async void TestStringQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property1 == value.Property1);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property1, entity.Property1);
        }

        /// <summary>
        /// Test Repository's "List" function using a uint property
        /// </summary>
        [Fact]
        public async void TestUIntQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property26 == value.Property26);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property26, entity.Property26);
        }

        /// <summary>
        /// Test Repository's "List" function using a ulong property
        /// </summary>
        [Fact]
        public async void TestULongQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property24 == value.Property24);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property24, entity.Property24);
        }

        /// <summary>
        /// Test Repository's "List" function using a ushort property
        /// </summary>
        [Fact]
        public async void TestUShortQuery_ExpectItemsReturned()
        {
            var repository = new Repository(client);

            var value = new TestModel();

            value.PopulateProperties();

            await repository.Add(value);

            var entities = await repository.List<TestModel>(x => x.Property22 == value.Property22);

            var size = entities.Count;

            Assert.NotEqual(0, size);

            foreach (var entity in entities)
                Assert.Equal(value.Property22, entity.Property22);
        }
    }
}
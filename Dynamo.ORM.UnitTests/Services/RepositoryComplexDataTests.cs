using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Dynamo.ORM.Models;
using Dynamo.ORM.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dynamo.ORM.UnitTests.Services
{
    public class RepositoryComplexDataTests : IClassFixture<SharedFixture>
    {
        private readonly AmazonDynamoDBClient client;
        private readonly SharedFixture sharedFixture;

        public RepositoryComplexDataTests(SharedFixture sharedFixture)
        {
            client = AmazonDynamoDBClientTestExtensions.InitializeTestDynamoDbClient();
            client.CreateTestTableIfNotExists<ComplexData>().Wait();
            client.CreateTestTableIfNotExists<DictionaryData>().Wait();

            this.sharedFixture = sharedFixture;
        }

        [Fact]
        public async Task TestDictionaryProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var model = new DictionaryData
            {
                Dictionary = new Dictionary<string, object>() {
                    { "Property1", 1 },
                    { "Property2", "2" },
                    { "Property3", "3" },
                },
                List = new List<int>() {
                    1,
                    2,
                    3,
                },
                Id = sharedFixture.RandomInt()
            };

            await repository.Add(model);

            var newModel = await repository.Get<DictionaryData>(model.Id);

            Assert.True(model.IsEqual(newModel));
        }

        [Fact]
        public async Task TestSavingClassProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var model = new ComplexData();

            model.PopulateProperties();

            model.Id = sharedFixture.RandomInt();

            await repository.Add(model);

            var result = await repository.Get<ComplexData>(model.Id);

            Assert.True(model.IsEqual(result));
        }

        [Fact]
        public async Task TestSavingNullClassProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var model = new ComplexData();

            model.PopulateProperties();

            model.Id = sharedFixture.RandomInt();

            model.ChildModel = null;

            await repository.Add(model);

            var newModel = await repository.Get<ComplexData>(model.Id);

            newModel.UpdateProperties();

            newModel.ChildModel = null;

            newModel.Id = sharedFixture.RandomInt();

            await repository.Update(newModel);

            var result = await repository.Get<ComplexData>(newModel.Id);

            Assert.True(!model.IsEqual(result));
            Assert.True(!model.IsEqual(newModel));
            Assert.True(newModel.IsEqual(result));

            Assert.Equal(model.ChildModel, result.ChildModel);
            Assert.Equal(newModel.ChildModel, result.ChildModel);
        }

        [Fact]
        public async Task TestUpdatingClassProperty_ExpectPropertyReturned()
        {
            var repository = new Repository(client);

            var model = new ComplexData();

            model.PopulateProperties();

            model.Id = sharedFixture.RandomInt();

            await repository.Add(model);

            var newModel = await repository.Get<ComplexData>(model.Id);

            newModel.UpdateProperties();

            newModel.Id = sharedFixture.RandomInt();

            await repository.Update(newModel);

            var result = await repository.Get<ComplexData>(newModel.Id);

            Assert.True(!model.IsEqual(result));
            Assert.True(!model.IsEqual(newModel));
            Assert.True(newModel.IsEqual(result));
        }
    }

    internal class ComplexChildData
    {
        public TestModel TestModel { get; set; }
    }

    [DynamoDBTable("ComplexDataTests")]
    internal class ComplexData : Base
    {
        public ComplexChildData ChildModel { get; set; }

        [DynamoDBHashKey]
        public int Id { get; set; }

        public TestModel TestModel { get; set; }
    }

    [DynamoDBTable("DictionaryDataTests")]
    internal class DictionaryData : Base
    {
        public IDictionary<string, object> Dictionary { get; set; }

        [DynamoDBHashKey]
        public int Id { get; set; }

        public IList<int> List { get; set; }
    }
}